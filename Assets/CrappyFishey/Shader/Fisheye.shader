// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "Crappy/Fisheye"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = ""{}
        _MatCap  ("_MatCap",  2D) = ""{}
        _NormalMap("NormalMap", 2D) = "white" {}
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
            #pragma editor_sync_compilation
            #include "Crappy.cginc"
            #include "CrappyFrag.cginc"
            
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
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex  = Bulge(v.vertex); 
                o.normal  = mul(unity_ObjectToWorld, v.normal);
                o.wP      = mul(unity_ObjectToWorld, v.vertex);
                o.tangent = mul(unity_ObjectToWorld, v.tangent);
                
                o.color  = v.color;
                o.uv     = v.uv;
                return o;
            }
            
            ENDCG
        }
    }
}
