Shader "Unlit/PixelDisplay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Pixel ("Pixel", 2D) = "white" {}
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

            sampler2D _MainTex, _Pixel;
            fixed _Amount;
            int XRes, YRes;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                //return col;
                //return pow(col * 1.5, 1.3);
                //col *= 1 - length(fixed2(.5, .5) - i.uv) * .12 * _Amount;
                
                fixed b = .6 + .4 * pow(1.0 - ((col.x + col.y + col.z) / 3), 2);
                
                fixed p = tex2D(_Pixel, float2(i.uv.x * XRes, i.uv.y * YRes)) * 2 - 1;
                return pow(col * (1 + p * .205 * _Amount * b), 1.1) * 1.2;//* (1 + .0165 * _Amount) * 1.2;
            }
            ENDCG
        }
    }
}
