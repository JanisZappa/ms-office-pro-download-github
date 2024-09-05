Shader "Unlit/Walker"
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
                float3 wPos : TEXCOORD1;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 view   = _WorldSpaceCameraPos - i.wPos;
                float dist = length(view);
                view /= dist;
                float fog = 1 - saturate(dist * .1);
                      fog = pow(fog, 4) * .95 + .05 * pow(fog, 2);
                return i.color * fog;
            }
            ENDCG
        }
    }
}
