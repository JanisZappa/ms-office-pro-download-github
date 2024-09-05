Shader "Unlit/Atmosphere"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f
            {
                float  z : TEXCOORD0;
                float4 color : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.z = mul(unity_ObjectToWorld, v.vertex).z;
                
                //float4 vert = v.vertex;
                //float y = pow(abs((_WorldSpaceCameraPos.z -40) -o.z) * .005, 100);
                //vert.y += y * -20;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                o.color = v.color;
                return o;
            }
            
            float3 fog(float3 col, float t, float t2)
            {
                float3 ext  = saturate(exp2(-t *0.00015* float3(1,2,4))); 
                float3 ext2 = saturate(exp2(-t2*0.00015* float3(4,1,2))); 
                ext = lerp(ext2, ext, .75 + .25 * col.x);
                return col*ext + (1.0-ext); // 0.55
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float t  = pow(max(0, i.z - _WorldSpaceCameraPos.z - 40) * .07, 4);
                float t2 = pow(max(0, i.z - _WorldSpaceCameraPos.z - 40) * .1, 4);
                return float4(fog(i.color.xyz, t, t2), 1);
            }
            ENDCG
        }
    }
}
