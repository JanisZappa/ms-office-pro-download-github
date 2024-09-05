Shader "Labyrinth/Glow"
{
    SubShader
    {
    Tags { "RenderType"="Transparent" "Queue"="Transparent+5"}
        LOD 100
        Blend OneMinusDstColor  One
        ZWRITE OFF
        OFFSET 0, 1
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Lab.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float2 uv2    : TEXCOORD1;
                float2 uv3    : TEXCOORD2;
                float3 normal : NORMAL;
                float4 color  : COLOR;
            };

            struct v2f
            {
                float3 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 wPos   : TEXCOORD1;
                float2 amount : TEXCOORD2;
                float3 normal : TEXCOORD3;
                float4 color  : TEXCOORD4;
            };
            
            
            v2f vert (appdata v)
            {
                v2f o;
                
                float4 center = float4(float3(v.uv, v.uv2.x), 1);
                float4 vert   = float4(v.vertex.xyz - center.xyz, 1);
                
                float3 forward =  -normalize(center - _WorldSpaceCameraPos);
                float3 up      =  normalize(UNITY_MATRIX_V._m10_m11_m12);
                float3 right   =  normalize(cross(forward, up));
                
                vert = mul(vert, BillboardMatrix(forward, up, right));

                // undo object to world transform surface shader will apply
                o.wPos   = vert.xyz + mul(unity_ObjectToWorld, center).xyz + float4(forward * abs(forward.y) * 1.5, 0);
                o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, float4(o.wPos, 1)));
                o.uv     = float3(v.uv, v.uv2.x);
                o.amount = v.uv3;
                o.normal = v.normal;
                o.color  = v.color;
                
                return o;
            }
            
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv     = (i.amount - .5) * 2;
                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float  dist   = length(camDir);
                
                float d = 1.0 - saturate(dist * .004);
                return (i.color + .25) * pow((1.0 - saturate(length(uv) * 1.25 - .035)) * .99 + .005, 3) * d * .95;
            }
            ENDCG
        }}
}
