Shader "Unlit/UV2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Light ("Light", 2D) = "white" {}
        _Normal ("Normal", 2D) = "white" {}
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
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 wPos : TEXCOORD2;
                float3 tangent  : TEXCOORD3;
                float3 binormal : TEXCOORD4;
                
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _Light, _Normal;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = float4(v.uv, v.uv2);
                o.normal = UnityObjectToWorldDir(v.normal);
                
                o.tangent = UnityObjectToWorldDir(v.tangent);
                o.binormal = cross(o.tangent, o.normal) * v.tangent.w;
                
                
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            
            float StepValue(float value, float steps)
            {
                float x = value * steps;
                
                float y = floor(x);
                float z = x - y;
                      z = saturate(z * 4 - 1.5);
                      z = lerp(pow(z, 2), 1.0 - pow(1.0 - z, 2), z);
                      //z = lerp(pow(z, 2), 1.0 - pow(1.0 - z, 2), z);
                return (y + z) / steps;
            }
            
            float3 StepValue3(float3 value, float steps)
            {
                return float3(StepValue(value.x, steps), StepValue(value.y, steps), StepValue(value.z, steps));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float light = tex2D(_Light, i.uv.zw).x;
                float4 tex = tex2D(_MainTex, i.uv.xy * 4);
                
                float4 ntex = tex2D(_Normal, i.uv);
                light *= ntex.a;
                // return light;
                float3 tn = ntex.xyz * 2.0 - 1.0;
               
                float3 n = i.tangent * tn.x + i.binormal * tn.y + i.normal * tn.z;
                // return float4(n * .5 + .5, 1);
                
                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float dist = length(camDir);
                camDir /= dist;
                
                float camDot = saturate(-dot(camDir, n));
                //return camDot;
                
                float rim = saturate(pow(1 - camDot, 4 + tex.x * 4) * 50) * (.0175 + tex.x * .0175) * light * 8;
                 
                float fogtint = pow(1.0 - saturate(dist * .035), 3) * .935 + .065;
                float camtint = (.5 + .5 * camDot) * .6;
                //return camtint;
                
                float4 color = tex;
                float4 result = (color + n.y * .1) * light + light * .2 + rim;
                float  tint   = fogtint * camtint;
                
                       result *= tint;
                
                return result;  
            }
            ENDCG
        }
    }
}
