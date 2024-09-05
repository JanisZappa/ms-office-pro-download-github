Shader "SF/Geom"
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
            #pragma geometry geom
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
            
            struct v2g
            {
                float4 vertex : SV_POSITION;
                float4 color  : TEXCOORD0;
                uint instanceID : TEXCOORD1;
            };


            struct g2f
            {
                float4 vertex : SV_POSITION;
                float4 color  : TEXCOORD0;
                uint frame : TEXCOORD1;
            };
            
            StructuredBuffer<PosRot> _Buffer;
            
            [maxvertexcount(90)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
            {
                g2f o;
  
                uint instanceID = IN[0].instanceID;
                float4 color = IN[0].color;
                
                for(uint f = 0; f < 30; f++)
                {
                    for(uint i = 0; i < 3; i++)
                    {
                        o.vertex = ToFrame(GetVertex(IN[i].vertex.xyz, _Buffer[instanceID + f], f), f);
                        o.color = color;
                        o.frame = f;
                        triStream.Append(o);
                    }
                    
                    triStream.RestartStrip();
                }
            }
            

            v2g vert (appdata v, uint instanceID: SV_InstanceID)
            {
                v2g o;
                o.vertex = v.vertex;
                o.color  = v.color;
                o.instanceID = instanceID * 30;
                return o;
            }

            fixed4 frag (g2f i) : SV_Target
            {
                uint _Frame =  i.frame;
                ClipFrame(i.vertex, _Frame);
            
                float3 c = i.color + (1 - i.color.a) * (sin((_Time.y - _Frame * (1.0 / 24 / 30)) * 30) * .5 + .25) * .2;
                
                return float4(c, 1);
              
            }
            ENDCG
        }
    }
}
