Shader "Labyrinth/LightAdd"
{
    SubShader
    {
         Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100
        Blend One One
        ZWRITE OFF
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
                float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 wPos : TEXCOORD1;
                float amount : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = float3(v.uv, v.uv2.x);
                o.wPos = v.vertex;
                o.amount = v.uv2.y;
                return o;
            }
            
            
            float length2(float3 v)
            {
                return v.x * v.x + v.y * v.y + v.z * v.z;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float d = pow(1.0 - saturate(length2(i.uv * 10 - i.wPos) * .006), 3);
                return float4(.2, .3, .35, 0) * .2 * d * i.amount;
                //return float4(.1, .25, .35, 0) * 1 * d * i.amount * (.965 + .06 * sin(_Time.y * 17));
            }
            ENDCG
        }
    }
}
