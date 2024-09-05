Shader "Unlit/StarFoxDefault"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [PerRendererData] _Frame("Frame", int) = 0
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
                fixed4 posW   : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            

            v2f vert (appdata v, uint instanceID: SV_InstanceID)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
             
                o.color = v.color;
                o.posW  = float4(mul(unity_ObjectToWorld, float4(-v.uv.x, v.uv.y, v.uv2.x, 1)).xyz, 0);
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float2 pS = i.vertex.xy; //.5 * (i.vertex.xy + 1);
                float chessboard = frac((floor(pS.x + pS.y)) * .5) * 2;
            
                float3 dir = i.posW.xyz - _WorldSpaceCameraPos;
                float m = saturate(max(0, length(dir) - 2000) / 20000);
                
                float3 c = i.color + (1 - i.color.a) * (sin((_Time.y) * 20) * .5 + .5) * .15;
                       //c = lerp(c, float3(0.5, .5, .95),  m);
                       
                       //c += (max(v.x, max(v.y, v.z))).xxx * .001;
                
                const int steps = 12, steps2 = 13;
                                
                float r = round(c.r * steps);
                float g = round(c.g * steps);
                float b = round(c.b * steps);
                
                float chessOffset = saturate(step(.5, c.r) - step(.5, c.g) + step(.5, c.b)) * chessboard;
                
                float offset = (r + g * steps2 + b * steps2 * steps2 + chessOffset) / 2198.0 + (1.0 / 2197.0 * .5);
                
                return 0;//tex2D(_MainTex, float2(offset, .5));
            }
            ENDCG
        }
    }
}
