Shader "Unlit/MC2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormalTex ("Normal", 2D) = "white" {}
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
            #pragma geometry geom

            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float3 normal : NORMAL;
                float4 color  : COLOR;
            };

            struct v2g
            {
                float3 normal : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color  : TEXCOORD1;
                float2 uv     : TEXCOORD2;
            };
            
            struct g2f
            {
                float4 vertex : SV_POSITION;
                float4 color  : TEXCOORD0;
                float2 uv     : TEXCOORD1;
                
                float3 normal : TEXCOORD2;
                float3 up     : TEXCOORD3;
                float3 right  : TEXCOORD4;
                float3 wPos   : TEXCOORD5;
            };

            sampler2D _MainTex, _NormalTex;

            v2g vert (appdata v)
            {
                v2g o;
                
                float3 dir = v.vertex.xyz - _WorldSpaceCameraPos;
                float d = length(dir);
                float dist = pow(1.0 - saturate(d * .001), 3);
                
                float4 color = v.color * dist;
                     
                o.vertex = v.vertex;
                o.normal = v.normal;
                o.color  = color;
                o.uv     = v.uv;
                
                return o;
            }
            
            
            [maxvertexcount(4)]
            void geom(point v2g input[1], inout TriangleStream<g2f> triStream)
            {
                v2g vin = input[0];
                
                g2f o;
                float4 vert = vin.vertex;
                float3 norm = vin.normal;
                o.color = vin.color;
                float2 uv = vin.uv;
                
                float s = step(.5, abs(norm.y));
                float4 up = float4(0, 1 - s, s, 0);
                
                float4 right = float4(float3(-norm.z, 0, norm.x) * (1.0 - s) + 
                                      float3(  -up.z, 0,   up.x) * s * -sign(norm.y), 0);
                                      
                o.normal = norm;
                o.up     = up;
                o.right  = right;
                
                o.wPos = vert + right * .5 + up * -.5;
                o.vertex = UnityObjectToClipPos(o.wPos);
                o.uv = uv + float2(.03125, -.03125);
                triStream.Append(o);
                
                o.wPos = vert + right * -.5 + up * -.5;
                o.vertex = UnityObjectToClipPos(o.wPos);
                o.uv = uv + float2(-.03125, -.03125);
                triStream.Append(o);
                
                o.wPos = vert + right * .5 + up * .5;
                o.vertex = UnityObjectToClipPos(o.wPos);
                o.uv = uv + float2(.03125, .03125);
                triStream.Append(o);
                
                o.wPos = vert + right * -.5 + up * .5;
                o.vertex = UnityObjectToClipPos(o.wPos);
                o.uv = uv + float2(-.03125, .03125);
                triStream.Append(o);
                
                triStream.RestartStrip();
            }


            fixed4 frag (g2f i) : SV_Target
            {
                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float dist = length(camDir);
                camDir /= dist;
                      dist = 1.0 - saturate(dist * .005);
                
                float4 nt = tex2D(_NormalTex, i.uv.xy) * 2 - 1;
                float4 ct = tex2D(_MainTex, i.uv.xy);
                float3 norm = i.right * nt.x + i.up * nt.y + i.normal * nt.z;
                       norm = normalize(lerp(i.normal, norm, dist));
                
                float d = dot(norm, 0.5773503) * .5 + .5;
               
                float4 result = i.color * (.55 + .25 * d);
                
                float a = 1;//.15 + ct.a * .3;
                float l = saturate(-dot(camDir, norm));
                float r = (pow(l, 50 * (.1 + .9 * a)) * .25 + pow(l, 1000) * .75) * .05 * (.45 + .55 * nt.a);
                
                result += r * (i.color * .9 + .1) * 6 * dist * (.25 + .75 * a);
                result *= lerp(1, (.95 + .05 * nt.a), dist);
                result = pow(saturate(result * .99  * (.9 + l * (.85 + .15 * a) * .15)  + .005), 1.5) * 1.25;
                
                result *= ct;
                
                result = pow(result, 1.5) * 8;
                result *= float4(.75, 1.1, 1.3, 1);
                return result;
            }
            ENDCG
        }
    }
}
