Shader "Unlit/PixelVis"
{
    Properties
    {
        _Frame("Frame", int) = 0
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

            int _Frame;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.normal = mul(unity_ObjectToWorld, v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 worldPos = i.screenPos.xy / i.screenPos.w * _ScreenParams.xy;//‚ * .125;

                float x = step(fmod(worldPos.x, 2), 1);
                float y = step(fmod(worldPos.y, 2), 1);
                float v = y * 2 + lerp(x, 1 - x, y);
                float t = floor(fmod(_Frame, 4));
                float a = 1 - step(1, floor(abs(v - t)));
                clip(a - .5);


                float3 normal = normalize(i.normal);
                float l = dot(normal, normalize(float3(2, 1, 1)));
                return float4((.5 + .5 * l).xxx, 1);
            }
            ENDCG
        }
    }
}
