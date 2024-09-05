Shader "Redistribution/VertTube"
{
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
                float4 color  : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color  : TEXCOORD0;
            };

            uniform float Height;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float tint = 1.0 - (v.color.r *.3 + v.color.g *.59 + v.color.b *.11);
                float l    = saturate((v.vertex.y + Height * .5) / Height) * .999;
                o.color  = fixed4(v.color.xyz + (v.color.a * 2 - 1) * .15 * tint * (1.0 - pow(1.0 - l, 5)) + .075, l);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float tint = 1.0 - (i.color.r *.3 + i.color.g *.59 + i.color.b *.11);
                float a = pow(i.color.a, 3);
                      a = -1 + a * 2;
                return fixed4(i.color.xyz + a * .15 * tint, 1);
            }
            ENDCG
        }
    }
}
