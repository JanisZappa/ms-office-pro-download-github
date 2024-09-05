Shader "SF/StaticSingleTex"
{
    Properties
    {
        _MainTex ("Tex", 2D)   = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        AlphaToMask On
        ZWrite On
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma target 4.5
            #include "Rot.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv  : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color  : TEXCOORD0;
                uint   frame  : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            

            v2f vert (appdata v, uint instanceID: SV_InstanceID)
            {
                v2f o;
                uint Frame = fmod(instanceID, 30);
                o.vertex = ToFrame(GetBillboardVertexFromWP(v.vertex.xyz, float3(v.uv, v.uv2.x), Frame), Frame);
                o.color  = v.color;
                o.frame  = Frame;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                ClipFrame(i.vertex, i.frame);
            
                return Grade(tex2D(_MainTex, i.color.xy));
              
            }
            ENDCG
        }
    }
}
