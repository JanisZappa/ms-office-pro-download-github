Shader "Labyrinth/Main"
{

    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
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
            #include "Lab.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };


            v2fone vert (appdata v)
            {
                v2fone o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.color  = v.color;
                o.wPos   = mul(unity_ObjectToWorld, v.vertex);
                o.uv     = v.uv;
                return o;
            }
            
            
            ENDCG
        }
    }
}
