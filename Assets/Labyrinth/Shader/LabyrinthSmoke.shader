Shader "Labyrinth/Smoke"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+2"}
        LOD 100
        ZWRITE OFF
//Blend One OneMinusSrcAlpha
Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Lab.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float4 color : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 wPos : TEXCOORD1;
                float bend : TEXCOORD2;
            };


            v2f vert (appdata v)
            {
                v2f o;
                float4 center = float4(float3(v.uv, v.uv2.x), 1);
                float4 vert   = float4(v.vertex.xyz - center.xyz, 1);
                
                float3 forward =  -(center - _WorldSpaceCameraPos);
                       forward.y = 0;
                       forward = normalize(forward);
                float3 up      =  float3(0, 1, 0);
                float3 right   =  normalize(cross(forward, up));
                
                vert = mul(vert, BillboardMatrix(forward, up, right));

                // undo object to world transform surface shader will apply
                o.wPos   = vert.xyz + mul(unity_ObjectToWorld, center).xyz;
                o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, float4(o.wPos, 1)));
                o.color  = v.color;
                o.bend   = dot(right, float3(1, 0, 0));
                return o;
            }
            
            float save(float v)
            {
                return saturate(v * .99 + .005);
            }
            
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float dist = length(camDir) - .75;
                 
                float s = (sin(_Time.y * 1.2 * (.5 + .5 * i.color.y) + i.color.x * 30 + i.color.z * 18 + i.color.y * 100) * .5 + .5) * (1.0 - i.color.x);
                float s2 = (sin(_Time.y * .6 * (.5 + .5 * i.color.y) + pow(i.color.x, 4) * 21.6 + i.color.y * 100) * .5 + .5) * pow(1.0 - i.color.x, 2);
                float u = abs(i.color.z + s2 * (.3 + i.color.y * .4) * .5 - .5  + pow(1.0 - i.color.x, 8) * i.bend * 22 * (.65 + .35 * s)) * 2;
                      u = pow( saturate(u - (1.0 - pow(save(.5 + .5 * i.color.x), 2)) * .9 + .915 - s * .2 ), 2);
                
                
                float a  = pow(save(1.0 - pow(u, 2)), 4) * pow(save(i.color.x), 6);
                      a  = 1.0 - pow(1.0 - a, 90);
                      a *= 1.0 - saturate(dist * .0025);
                      
                float t = sin(_Time.y * 1.2 + i.color.x * 30) * .115 + 1;
                return float4((pow(i.color.x * .95, 2) + .15).xxx  * .075 * t, a * .75);
            }
            ENDCG
        }
    }
}
