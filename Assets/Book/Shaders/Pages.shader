Shader "Unlit/Pages"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Paper ("Paper", 2D) = "white" {}
        _Turn("Turn", float) = 0
        _Bend("Bend", float) = 0
        _Marking("Marking", Color) = (1, 0, 0, 0)
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
            #define PI 3.1415926535

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float2 uv2    : TEXCOORD1;
                float2 uv3    : TEXCOORD2;
                float3 normal : NORMAL;
                float4 rgb    : COLOR;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 wPos   : TEXCOORD2;
                float2 uv2    : TEXCOORD3;
                float  lrp    : TEXCOORD4;
                float2 uv3    : TEXCOORD5;
                float4 rgb    : TEXCOORD6;
            };

            sampler2D _MainTex, _Paper;
            float _Turn, _Bend;
            float4 _Marking;
            
            float _BulgeL, _BulgeR;
            
            
            
            float3 RotRad(float3 v, float s, float c)
            {
                float tx = v.x;
                float ty = v.y;
         
                return float3(c * tx - s * ty, s * tx + c * ty, v.z);
            }
    

            v2f vert (appdata v)
            {
                v2f o;
                o.uv  = v.uv;
                o.uv2 = v.uv2;
                o.uv3 = v.uv3;
                o.rgb = v.rgb;
                
                float3 vert = v.vertex.xyz;
                
                float bend = (_Bend + lerp(_BulgeR, -_BulgeL, _Turn)) * v.rgb.b + _BulgeL * v.rgb.r + _BulgeR * v.rgb.g;
                float turn = (_Turn + lerp(_BulgeR, -_BulgeL, _Turn) * 1.1) * v.rgb.b + -_BulgeL * v.rgb.r * 1.1 + _BulgeR * v.rgb.g * 1.1;
                
                float s    = step(0.0001, abs(bend));
                      bend = bend + (1.0 - s);
                float lrp  = abs(vert.x) * 0.00952380952380952;
                float rad    = lrp * PI * 2.0 * bend * (v.rgb.r * -2.0 + 1); 
		        float radius = 1.0 / bend / 2.0 / PI * 105;
		     
		        float a = sin(rad);
		        float b = cos(rad);
		       
		        vert = vert.xyz * (1.0 - s) + s * float3(a * radius, b * radius - radius, vert.z);
		        vert.y += (v.rgb.r * _Turn + (1 - _Turn) *  v.rgb.g) * -.001;
		         
		        
		        float3 n = v.normal * (1.0 - s) + s * RotRad(v.normal, a, b);
		        
		        float tr   = turn * PI;
		        float ts   = sin(tr);
		        float tc   = cos(tr);
		        
		        vert = RotRad(vert, ts, tc);
                n    = RotRad(n, ts, tc);
                 
                o.vertex = UnityObjectToClipPos(float4(vert, 1));
                o.normal = mul(unity_ObjectToWorld, n);
                o.wPos   = mul(unity_ObjectToWorld, float4(vert, 1)).xyz;
                o.lrp = lrp;
                
                return o;
            }
            
            
            

            float4 frag (v2f i) : SV_Target
            {
                float4 inktex  = tex2D(_MainTex, i.uv);
                float4 inktex2 = tex2D(_MainTex, i.uv3);
                float ink  = saturate(pow((1 - inktex.x) * 1.22, 2.2)) * .99 + .01;
                float ink2 = saturate(pow((1 - inktex2.x) * 1.22, 2.2)) * .99 + .01;
                float ink3  = saturate(pow((1 - inktex.z) * 1.22, 2.2)) * .99 + .01;
                
                float2 uv = i.uv2;
                float4 paper = tex2D(_Paper, i.uv2 * 3) * .2 + .5 * tex2D(_Paper, i.uv2 * 7.77) + .3 + .1 * tex2D(_Paper, i.uv2 * 12.3212);
                ink2 = saturate(ink2 + paper.z * .35);
                paper = pow(paper, 2);
                
                
                float3 p  = i.wPos;
                float3 l  = float3(222, 229, 136);
                float3 lp = normalize(l - p);
                float3 n  = normalize(i.normal);
                
                float d = (pow(abs(dot(n, lp)), 2)) * .5 + .5;
                float t = (1.0 - abs(_Turn - .5) * 2);
                //return float4(n * .5 + .5, 1);
                ink *= .5 + .5 * ink3;
                ink *= lerp(1, ink2, i.rgb.b * (1.0 - pow(1.0 - t, 1.5)) * 1.4 * ((1.0 - d) * .75 + .25) * (i.lrp * .55 + .45));
                float tint = lerp(1, i.rgb.b, pow(abs(i.uv3.x - _Turn), 4)) * .5 + .5;
               
                     // d *= tint;
                
                d = lerp(d, pow(d, 2), paper.y * ink);
                
                float c = (1.0 - pow(1.0 - saturate(i.lrp * 3), 5));
                d = lerp(.475, d, c) * ((1.0 - pow(1.0 - c, 3)) * .045 + .955);
                
                float3 camDir = p - _WorldSpaceCameraPos;
                float dist = length(camDir);
                camDir /= dist;
                float s = abs(dot(reflect(camDir, n), lp));
                
                float4 result = ink * (d * .925 + .075) * paper + s * paper * .075;
                return (pow(result * 1.2, 1.1) * 1.1  + lerp(0, _Marking, inktex.y) * .05) * (lerp(1, _Marking, inktex.y) * .3 + .7);
            }
            ENDCG
        }
    }
}
