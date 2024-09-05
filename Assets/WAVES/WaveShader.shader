Shader "Unlit/WaveShader"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = ""{}
        _EnvTex ("Environment", Cube) = "gray" {}
        _Refraction ("Refration Index", float) = 0.9
        _Fresnel ("Fresnel Coefficient", float) = 5.0
        _Reflectance ("Reflectance", float) = 1.0
        _Bump ("_MainTex", 2D) = ""{}
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 wP : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };
            
            StructuredBuffer<float4> WaveData;
            sampler2D _MainTex;
            
            samplerCUBE _EnvTex;
            half _Refraction;
            half _Fresnel;
            half _Reflectance;
            
            sampler2D _Bump;
            
            v2f vert (appdata v)
            {
                v2f o;
                
                float4 data = WaveData[v.uv.x];
                
                float4 vert = v.vertex + float4(0, data.x, 0, 0);
                o.vertex = UnityObjectToClipPos(vert);
                o.normal = data.yzw;
                o.wP = mul(unity_ObjectToWorld, vert);
                o.viewDir = ObjSpaceViewDir(v.vertex);
                
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half3 n = i.normal;
                      n.y = pow(n.y, 1.4);
                      n = normalize(n);
                      
                float amount = 1.2;//abs(i.wP.y) * 5;      
                float tm = _Time.x * 2;
                 n = normalize(n + tex2D(_Bump, i.wP.xz * .8 * .5 + float2(tm, tm)).xyz * .05 * amount);
                 n = normalize(n + tex2D(_Bump, i.wP.zx * 1.5* .5 + float2(tm, tm)).xyz * .10 * amount);   
                 n = normalize(n + tex2D(_Bump, i.wP.zx * -3.222 * .5 + float2(tm, tm)).xyz * .15 * amount);     
                //n.y = pow(n.y, 6);
                  //    n = normalize(n);   
                //return n.z * .5 + .5; 
                half3 lightDir = normalize(half3(-1, .331, 1));
            
                float d = dot(n, lightDir);
                // return d;
                //return max(0, d) * .75 +.25;
 
                half3 viewDir      = normalize( _WorldSpaceCameraPos - i.wP);
                half3 halfVector   = normalize(lightDir + viewDir);
                half3 specularTerm = pow( saturate( dot( n, halfVector)), 1200);
 
                half d2 = d;
                d =  lerp(max(0, d), d * .5 + .5, .7);
                
                half3 t = tex2D(_MainTex, half2(clamp(d + i.wP.y * .2 * 0, .01, .99), .5)).xyz;
                
                //return float4(n.x * .5 + .5, n.z * .5 + .5, n.y * .5 + .5, 1);
                
            half3 v = normalize (i.viewDir);
            half fr = pow(1.0f - dot(v, n), _Fresnel) * _Reflectance;
            
            half3 reflectDir = reflect(-v, n);
            half3 refractDir = refract(-v, n, _Refraction);

            half3 reflectColor = texCUBE (_EnvTex, reflectDir).rgb;
            half3 refractColor = texCUBE (_EnvTex, refractDir).rgb;
                
             
                
                return half4(pow(t + specularTerm * .5 + (reflectColor * fr + refractColor) * .8 - .45, 1.4) * (.95 + .05 * d2), 1);
            }
            ENDCG
        }
    }
}
