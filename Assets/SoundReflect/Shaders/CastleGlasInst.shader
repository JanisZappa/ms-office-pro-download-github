Shader "Castle/Glass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Noise ("Noise", 2D) = "black" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Blend One One
        LOD 100
        ZWRITE ON

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "CastleGlasFrag.cginc"
            
            #pragma multi_compile GRID_OFF      GRID_ON
            #pragma multi_compile GRAY_OFF      GRAY_ON
            #pragma multi_compile FOG_OFF      FOG_ON
            #pragma shader_feature SCENE_VIEW
            
            #pragma multi_compile_instancing 
            #pragma target 4.5
            
            struct ScatterObject
            {
                float3 pos;
                float3 rot;
            };
            
            StructuredBuffer<ScatterObject> RenderBuffer;
            uint _InstOffset;
            
            
            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            

            v2f vert (appdata v, uint instanceID: SV_InstanceID)
            {
                v2f o;
                
                ScatterObject sO = RenderBuffer[instanceID + _InstOffset];
                
                float3x3 rot = Euler3x3(sO.rot);
                o.wPos = sO.pos + mul(rot, v.vertex.xyz);
                
                o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, float4(o.wPos, 1)));
                o.color  = v.color * 0;
                o.uv     = v.uv;
                o.normal = mul(rot, v.normal);
                return o;
            }
            
            ENDCG
        }
    }
}
