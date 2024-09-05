// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Crappy/FisheyeInst"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = ""{}
        _MatCap  ("_MatCap",  2D) = ""{}
        _NormalMap("NormalMap", 2D) = "white" {}
        _Offset("Offset", int) = 0
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

            #include "Crappy.cginc"
            #include "CrappyFrag.cginc"
            #pragma multi_compile_instancing 
            #pragma instancing_options assumeuniformscaling
		    #pragma editor_sync_compilation
            #pragma target 4.5
            
            #pragma multi_compile BULGE_OFF     BULGE_ON
            #pragma multi_compile FOG_OFF       FOG_ON
            #pragma multi_compile SHADOW_OFF    SHADOW_ON
            #pragma multi_compile CHECKER_OFF   CHECKER_ON
            #pragma multi_compile NORMAL_OFF    NORMAL_ON
            #pragma multi_compile SPEC_OFF      SPEC_ON
            
            
            struct appdata
            {
                float4 vertex : POSITION;
                fixed3 normal : NORMAL;
                fixed3 tangent : TANGENT;
                fixed4 color  : COLOR;
                fixed4 uv     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            StructuredBuffer<uint>Map;
            StructuredBuffer<float4x4>Render;
            uint _Offset;

            v2f vert (appdata v, uint instanceID: SV_InstanceID) 
            {
                UNITY_SETUP_INSTANCE_ID(v);
            
                v2f o;
                
                float4x4 worldM = Render[Map[instanceID + _Offset]];
                float4 wP = mul(worldM, v.vertex);
                o.vertex  = Bulge(mul(unity_WorldToObject, wP)); 
                o.normal  = mul(worldM, v.normal);
                o.wP      = wP;
                o.tangent = mul(worldM, v.tangent);
                
                o.color  = v.color;
                o.uv     = v.uv;
                return o;
            }
            
            ENDCG
        }
    }
}
