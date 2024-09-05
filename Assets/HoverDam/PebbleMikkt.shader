Shader "Unlit/PebbleMikkt"
{
    Properties
    {
        _Normal ("N", 2D) = "white" {}
        [NoScaleOffset] _Reflection ("R", Cube) = "grey" {}
        _Sub ("S", 2D) = "blue" {}
        
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
                float4 vertex  : POSITION;
                float2 uv      : TEXCOORD0;
                float3 normal  : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float3 wPos     : TEXCOORD1;
                float3 normal   : TEXCOORD2;
                float3 tangent  : TEXCOORD3;
                float3 binormal : TEXCOORD4;
            };

            sampler2D _Normal, _Sub;
            samplerCUBE _Reflection;
            float4x4 NormalLight;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldDir(v.normal);
                o.tangent = UnityObjectToWorldDir(v.tangent);
                o.binormal = cross(o.tangent, o.normal) * v.tangent.w;
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 ntex = tex2D(_Normal, i.uv);
                float shiny = .5;
                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float dist = length(camDir);
                
                float fade = dist - 2;
                      fade = 1.0 - saturate(abs(fade) * (.0065 + step(fade, 0.0) * .25));
                //return fade;
                camDir /= dist;
                
                float tint = (1.0 - (1.0 - ntex.a) * fade) * .75 + .25;
                
               
                
                float3 tn = ntex.xyz * 2.0 - 1.0;
                     //  tn += (stex.xyz * 2.0 - 1.0) * .35;
                tn = normalize(tn);
                
                float3 n = ((i.tangent) * tn.x + (i.binormal) * tn.y + (i.normal) * tn.z);
                n = normalize(lerp(i.normal, n, fade));
                //return float4(n * .5 + .5, 1);    
                  
                float rim = saturate(1.0 - dot(n, -camDir));
                      rim = pow(rim, 3) * .4 * tint + (pow(1.0 - rim, 15)) * .2 * tint;
                // return rim;
                float4 r = texCUBElod(_Reflection,float4(reflect(camDir, n), (1.0 - shiny * .5) * 16));
                       r = pow(r, 2) ;
                       
                       n = mul(NormalLight, n);
                   
                       
                float4 result = 
                    float4(n * .5 + .5, 1) * tint + rim + r * .125 * shiny;
           
                //result = lerp(result, float4(0, 0, 0, 0), 1.0 - pow(1.0 - saturate(abs(i.wPos.y) * .025), 5));
                return pow(result, 1.125);
            }
            ENDCG
        }
    }
}
