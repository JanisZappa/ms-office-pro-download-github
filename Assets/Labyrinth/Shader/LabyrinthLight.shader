Shader "Labyrinth/Light"
{
    SubShader
    {
         Tags { "RenderType"="Transparent" "Queue"="Transparent+1"}
        LOD 100
        Blend DstColor Zero
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
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 wPos   : TEXCOORD1;
                float  amount : TEXCOORD2;
                float3 normal : TEXCOORD3;
                float4 color : TEXCOORD4;
            };

            v2f vert (appdata v)
            {
                v2f o;
               
               float4 vert = v.vertex;
               
                float dist = length(vert.xyz - _WorldSpaceCameraPos);
                vert += float4(v.normal * dist * .000025, 0);
                o.vertex = UnityObjectToClipPos(vert);
                o.uv = float3(v.uv, v.uv2.x);
                o.wPos = vert;
                o.amount = v.uv2.y;
                o.normal = v.normal;
                o.color = v.color;
                return o;
            }
            
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3 dir = i.uv - i.wPos;
                float dist = length(dir);
                dir /= dist;
            
                float d = pow(1.0 - saturate(dist * .0575), 3);
                      d *= saturate(dot(dir, i.normal));
                      
                float3 camDir = _WorldSpaceCameraPos - i.wPos;
                float camDist = length(camDir);
                        camDir /= camDist;      
                    
                      d *= 1.0 - pow(1.0 - saturate(dot(camDir, i.normal)), 6);
                     //d *= (.965 + .05 * sin(_Time.y * 17));
                      d *= 1.0 - (pow(saturate(camDist * .002), 20));
                return 1 + (i.color + .15) * 9.5 * d;// * i.amount;
                //return float4(.1, .25, .35, 0) * 1 * d * i.amount * (.965 + .06 * sin(_Time.y * 17));
            }
            ENDCG
        }
    }
}
