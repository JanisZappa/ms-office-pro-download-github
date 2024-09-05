Shader "Unlit/CamWarp"
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float aspect = unity_OrthoParams.x / unity_OrthoParams.y;

                float2 uv = i.uv - .5;
                uv.x *= aspect;
                float amount = 1 - saturate(cos(length(uv) * (90.0 / 180.0) * 3.14159 * .5));
                uv *= (amount * .5 + .5) * 1.3333 * 1.05;
                //uv *= (amount * .5 + .5) * 3.14159 * .5;
                uv.x *= 1.0 / aspect;
                uv += .5;
                
            
                float4 col = tex2D(_MainTex, uv);
                
                float lum = col.r*.3 + col.g*.59 + col.b*.11;
                float a = col.a;
                
                float4 ca = lerp(col * float4(1.25, 1.05, .95, 1), col * float4(1.05, .95, .95, 1), step(.5, fmod(lum * 10, 1)));
                float4 cb = lerp(col * float4(.25, .4, .55, 1), col * float4(.35, .45, .55, 1), step(.5, fmod(lum * 10, 1)));
                
                //col = lerp(ca, cb, (sin(_Time.y * .25) * .5 + .5));
                col = ca;
                //return col.a;
                col = lerp(col, 1, a * .5);
                return pow(col, 1.25);
            }
            ENDCG
        }
    }
}
