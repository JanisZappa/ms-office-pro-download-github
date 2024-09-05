Shader "Labyrinth/Strokes"
{
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+2"}
        LOD 100
        Blend DstColor Zero
        //Offset -1, 1
        //ZWRITE OFF
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag2
            #include "Lab.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color  : COLOR;
            };


            v2fone vert (appdata v)
            {
                v2fone o;
                
                float4 vert = v.vertex - float4(v.normal * .02, 1);
                o.wPos   = mul(unity_ObjectToWorld, vert);
                
                float3 camDir = o.wPos - _WorldSpaceCameraPos;
                float dist    = length(camDir);
                vert += float4(v.normal * .001 * dist, 1);
                
                o.vertex = UnityObjectToClipPos(vert);
                o.normal = 0;
                o.color  = v.color;
                o.uv = 0;
                return o;
            }
            
            
            fixed4 frag2 (v2fone i) : SV_Target
            {
                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float yDiff = max(0, abs(camDir.y) - 20);
                float dist = length(camDir);
                camDir /= dist;
                
                float4 col = fog(i.color, camDir, dist, i.wPos.y);
                
                //return col * 2 + .75;// * .75;
                float fade = 1.0 - (1.0 - pow(saturate(dist * .0035), 20)) * (1.0 - pow(saturate(yDiff * .0125), 6));
                return lerp(col * 2 + .5, 1, fade);// * .75;
            }
            
            
            ENDCG
        }
    }
}
