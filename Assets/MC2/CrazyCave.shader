Shader "Unlit/CrazyCave"
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 color : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 wPos     : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                o.normal = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float dist = length(camDir);
                camDir /= dist;
                
                float3 n = i.normal;
                float r = pow(saturate(-dot(camDir, n)), 50) * .4;
                float4 v = (i.color * (.85 + .15 * n.y) + r * i.color) * pow(1.0 - saturate(dist * .0015), 10);
                v = pow(v, 2) * 2;
                return v;
            }
            ENDCG
        }
    }
}
