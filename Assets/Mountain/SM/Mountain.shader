Shader "Unlit/Mountain"
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
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color  : TEXCOORD3;
                float4 normal : TEXCOORD1;
                float3 wPos   : TEXCOORD2;
                float2 uv : TEXCOORD0;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color  = v.color;
                o.normal = v.normal;
                o.wPos = v.vertex.xyz;
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 n = normalize(i.normal);
                float3 dir = i.wPos - _WorldSpaceCameraPos;
                float dist = length(dir);
                dir /= dist;
                float f = pow(1- saturate(dot(-n, dir)), 2);
                //return f;
                float d = dot(n, normalize(float3(-1, .75, 1))) * .5 + .5;
                      d = pow(d, 6);
                      d = 1.0 - pow(1.0 - d, 15);
                      d = d * .85 + .2;
                      
                float4 result = i.color;// * d;
                       //result += f * .1;
                       //result = .5;
                       //return fmod(i.uv.y * 10, 1);
                       result *= (step(.5, fmod(i.uv.y * 8 + sin(i.uv.x * 16) * .25, 1)) * .035 - .0175) * i.color.a + 1;
                
                float fog = pow(saturate(length(i.wPos - _WorldSpaceCameraPos) * .0001), 1.5) * .5 * .4;
                      fog += f * .04;
                       //result = lerp(result, 1, fog);
                  
                //return i.color.a;
                return float4(pow(pow(result.xyz, 1.0 / 2.2), 3.75), fog);
            }
            ENDCG
        }
    }
}
