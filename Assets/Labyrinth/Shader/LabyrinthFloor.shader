Shader "Labyrinth/Floor"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
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
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.color  = v.color;
                o.wPos   = mul(unity_ObjectToWorld, v.vertex);
                o.uv     = 0;
                return o;
            }
            
            
            fixed4 frag2 (v2fone i) : SV_Target
            {
                float3 camDir  = i.wPos - _WorldSpaceCameraPos;
                float  dist    = length(camDir);
                       camDir /= dist;
                
                float4 col = fog(i.color, camDir, dist, i.wPos.y);
                
                float l = (1.0 - pow(saturate(dist * .075), 2)) * .00225;
                return (col * 2 + .5) * .0085 + float4(1, .5, .05, 0) * l;// * .75;
            }
            
            
            ENDCG
        }
    }
}
