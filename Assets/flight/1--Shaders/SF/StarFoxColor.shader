Shader "Unlit/SF"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float2 uv : TEXCOORD0;
                float2 uv2: TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color  : TEXCOORD0;
                uint frame : TEXCOORD1;
            };
            
            StructuredBuffer<PosRot> _Buffer;

            v2f vert (appdata v, uint instanceID: SV_InstanceID)
            {
                v2f o;
                uint Frame = fmod(instanceID, 30);
                PosRot pR  = _Buffer[instanceID];
                o.vertex = ToFrame(GetVertex(v.vertex.xyz, pR, Frame), Frame);
                o.color  = lerp(v.color, 1, pR.anim.x);
                o.frame  = Frame;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                uint _Frame =  i.frame;
                ClipFrame(i.vertex, _Frame);
            
                float3 c = i.color + (1 - i.color.a) * (sin((_Time.y - _Frame * (1.0 / 24 / 30)) * 30) * .5 + .25) * .2;
                
                return Grade(float4(c, 1));
              
            }
            ENDCG
        }
    }
}
