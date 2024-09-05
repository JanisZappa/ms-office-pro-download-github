Shader "Labyrinth/Entity"
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

            #include "Lab.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f2
            {
                float4 color : TEXCOORD0;
                float3 wPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f2 vert (appdata v)
            {
                v2f2 o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.wPos   = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f2 i) : SV_Target
            {
                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float dist = length(camDir);
                camDir /= dist;
                return fog(i.color * 3, camDir, dist, i.wPos.y);
            }
            ENDCG
        }
    }
}
