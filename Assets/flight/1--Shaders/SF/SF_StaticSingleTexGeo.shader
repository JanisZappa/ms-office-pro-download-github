Shader "SF/StaticSingleTexGeo"
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
            #pragma geometry geom
            #pragma target 4.5
            #include "Rot.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv : TEXCOORD0;
                float2 uv2: TEXCOORD1;
            };

            
            struct v2g
            {
                float4 vertex : SV_POSITION;
                float4 color  : TEXCOORD0;
                float3 center : TEXCOORD1;
            };


            struct g2f
            {
                float4 vertex : SV_POSITION;
                float4 color  : TEXCOORD0;
                uint   frame  : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            
            
            [maxvertexcount(90)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
            {
                g2f o;
  
                float4 c1  = IN[0].color;
                float4 c2  = IN[1].color;
                float4 c3  = IN[2].color;
                float3 wP0 = IN[0].vertex;
                float3 wP1 = IN[1].vertex;
                float3 wP2 = IN[2].vertex;
                float3 cP0 = IN[0].center;
                float3 cP1 = IN[1].center;
                float3 cP2 = IN[2].center;
                
                for(uint f = 0; f < 30; f++)
                {
                    o.vertex = ToFrame(GetBillboardVertexFromWP(wP0, cP0, f), f);
                    o.color = c1;
                    o.frame = f;
                    triStream.Append(o);
                    
                    o.vertex = ToFrame(GetBillboardVertexFromWP(wP1, cP1, f), f);
                    o.color = c2;
                    o.frame = f;
                    triStream.Append(o);
                    
                    o.vertex = ToFrame(GetBillboardVertexFromWP(wP2, cP2, f), f);
                    o.color = c3;
                    o.frame = f;
                    triStream.Append(o);
                    
                    triStream.RestartStrip();
                }
            }
            

            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = v.vertex;
                o.color  = v.color;
                o.center = float3(v.uv, v.uv2.x);
                return o;
            }

            fixed4 frag (g2f i) : SV_Target
            {
                ClipFrame(i.vertex, i.frame);
            
                return tex2D(_MainTex, i.color.xy);
            }
            ENDCG
        }
    }
}
