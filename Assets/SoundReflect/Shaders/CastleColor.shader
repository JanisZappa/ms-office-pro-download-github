Shader "Castle/Color"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Noise ("Noise", 2D) = "black" {}
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

            #include "UnityCG.cginc"
            #include "CastleColorFrag.cginc"
            
            #pragma multi_compile GRID_OFF      GRID_ON
            #pragma multi_compile GRAY_OFF      GRAY_ON
            #pragma multi_compile FOG_OFF      FOG_ON
            #pragma shader_feature SCENE_VIEW
            
            
            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color  = v.color;
                o.wPos   = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv     = v.uv;
                o.normal = mul(unity_ObjectToWorld, v.normal);
                return o;
            }


            
            ENDCG
        }
    }
}
