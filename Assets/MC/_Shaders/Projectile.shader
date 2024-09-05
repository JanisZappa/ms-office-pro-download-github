Shader "MC/Projectile"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" }
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed3 posW   : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            

            v2f vert (appdata v)
            {
                v2f o;
                float3 vpos = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz * 1.35);
				float4 worldCoord = float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23, 1);
				float4 viewPos = mul(UNITY_MATRIX_V, worldCoord) + float4(vpos, 0);
				float4 outPos  = mul(UNITY_MATRIX_P, viewPos);

				o.vertex = outPos;
				o.posW = worldCoord;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col.a - .9);
                
                fixed3 caveDist = Dist(i.posW);
                fixed d = caveDist.x;
                
                fixed dist        = pow(1.0 - saturate(d * CaveFog), 6);
                fixed maxShadow   = pow(saturate((CaveFogMax - d) / 50.0), 4);
                
                LightSample sample = Sample(i.posW);
                fixed4 c  = sample.bake * .45;
                fixed4 c2 = sample.dyn  * .25;
                
                //return (col * 2) * (dist * maxShadow);
                return fixed4(((col * 1.3 + col * c + c2) * (dist * maxShadow)).xyz,  pow(saturate(d * .18), 4) * .55);
            }
            ENDCG
        }
    }
}
