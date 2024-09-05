Shader "Unlit/Die"
{
    Properties
    {
        _MainTex ("T", 2D) = "white" {}
         _Color("C", Color) = (1,1,1,1)
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 wPos     : TEXCOORD1;
            };

            sampler2D _MainTex;
            samplerCUBE _Reflection,  _Light;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float dist = length(camDir);
                camDir /= dist;
                
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 baseN = normalize(col.xyz * 2.0 - 1.0).xyz;
                float3 n = mul(unity_ObjectToWorld, baseN);
                //return float4(n * .5 + .5, 1);
                float4 l = texCUBE(_Light, n) * 1.25 + .35 + saturate(n.y) * .75 + .25;
                float4 r = texCUBElod(_Reflection,float4(reflect(camDir, n), 6)) * .75 + .25 * texCUBElod(_Reflection,float4(reflect(camDir, n), 2));
               
                float3 mixN = (baseN * .5 + .5) * .15 + (n * .5 + .5) * .85;
                return float4((pow((mixN * .5 + .5) * .75 + col.a * .95, 5) * 3 + (r.xyz - .4) * .01) * l.xyz, 1);
                return pow((r * .25 + l) * lerp(_Color, 1, col.a), 1.25) * 2;
            }
            ENDCG
        }
    }
}
