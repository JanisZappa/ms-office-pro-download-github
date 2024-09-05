Shader "Unlit/CamLast"
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
float4 _MainTex_TexelSize;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 boxSize  = clamp(fwidth(i.uv) * _MainTex_TexelSize.zw, 1e-5, 1.0);
                float2 tx       = i.uv * _MainTex_TexelSize.zw - .5 * boxSize; 
                float2 txOffset = saturate((frac(tx) - (1.0 - boxSize)) / boxSize);
                float2 uv       = (floor(tx) + .5 + txOffset) * _MainTex_TexelSize.xy;
                float4 col      = tex2Dgrad(_MainTex, uv, ddx(i.uv), ddy(i.uv));
                //col = tex2D(_MainTex, i.uv);
                //col.xyz *= 1 - .085 * (1 - pow(saturate(1.0 - max(txOffset.x, txOffset.y)), 100));
                //col.xyz *= 1 + .05 * (1 - pow(saturate(1.0 - max(txOffset.x, txOffset.y)), 100));
                return col;
            }
            ENDCG
        }
    }
}
