Shader "Unlit/Grid"
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD0;
                float depth : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.depth = v.vertex.z * -.5;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float tint = (1.0 - pow(1.0 - saturate(i.depth), 3)) * .5 + .6;
                      tint *= .35 + 1.05 * (1.0 - pow(1.0 - saturate(i.depth * .5), 2));
                      
                float fog = pow(1 - saturate(i.depth * 2), 2) * .01;
                return i.color * tint + fog;
                
            }
            ENDCG
        }
    }
}
