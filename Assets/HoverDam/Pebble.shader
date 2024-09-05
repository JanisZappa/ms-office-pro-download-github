Shader "Unlit/Pebble"
{
    Properties
    {
         _Color ("C", 2D) = "white" {}
        _Normal ("N", 2D) = "white" {}
        [NoScaleOffset] _Light ("L", Cube) = "grey" {}
        [NoScaleOffset] _Reflection ("R", Cube) = "grey" {}
        
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 wPos : TEXCOORD1;
            };

            sampler2D _Color, _Normal;
            samplerCUBE _Light, _Reflection;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 ctex =  tex2D(_Color, i.uv);
                float4 ntex = tex2D(_Normal, i.uv);
                float tint = ctex.a;
                
                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float dist = length(camDir);
                camDir /= dist;
                
                float3 n = UnityObjectToWorldNormal(normalize(ntex * 2 -1));
                return float4(n * .5 + .5, 1)* ctex.a;
                       
                float rim = saturate(1.0 - dot(n, -camDir));
                //return 1.0 - pow(1.0 - pow(rim, 6), 6);
                
                
                
                float3 d = (texCUBE(_Light, n) * .95 + .05) * 1.5;
                       d *= pow(tint, 2.5);
                       //return float4(d, 1);
               
                float4 r = texCUBElod(_Reflection,float4(reflect(camDir, n), (1.0 - .65) * 16));
                       r = pow(r, 2) ;
                       
                return ctex * (1.0 - pow(1.0 - pow(rim, 6), 6));
                float4 result = ctex * float4(d, 1) + r  * ntex.a * (1 + pow(rim, 2) * 4) * .045 * pow(tint, 3);
                return pow(result, 1.75) * 7.5;
            }
            ENDCG
        }
    }
}
