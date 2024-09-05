Shader "Unlit/LabyrinthWindow"
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
                float4 color : COLOR;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD1;
                float3 wPos   : TEXCOORD2;
                float3 normal : TEXCOORD3;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float4 vert = v.vertex;
               
                float dist = length(vert.xyz - _WorldSpaceCameraPos);
                vert += float4(v.normal * dist * .00005, 0);
                
                o.vertex = UnityObjectToClipPos(vert);
                o.uv     = v.uv;
                o.color  = v.color;
                o.wPos = vert;
                o.normal = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 camDir = _WorldSpaceCameraPos - i.wPos;
                float camDist = length(camDir);
                        camDir /= camDist;      
                        
                        
                float2 v = abs(i.uv);
                       v.y = max(0, v.y - .06);
                        v.x = max(0, v.x - .02);
                float d = 1.0 - saturate(length(v) * 50 - .9);
                
                float t = fmod(_Time.y * .0006 * (.3 + .7 * i.color.a) + i.color.a, 1);
                      t = saturate(t * 400 - 49.5);
                float l = .1 + .9 * t;
                float4 c = lerp(i.color * 35 * l + .2, 0, .65 - pow( d, 2) * .65);
                      d *= saturate(dot(camDir, i.normal));
                      d *= 1.0 - saturate(camDist * .0015);
                      
                return lerp(1, c, d);
            }
            ENDCG
        }
    }
}
