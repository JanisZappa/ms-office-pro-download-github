Shader "Unlit/SDFPlanet"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white"
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Front

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "noiseSimplex.cginc"
            #include "HSV.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float3 wCenter : TEXCOORD0;
                float3 wPos    : TEXCOORD1;
                float radius : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler3D _ColorTex;
            StructuredBuffer<float4>LightBuffer;
            int LightCount;
            StructuredBuffer<float4>PlanetBuffer;
            int PlanetCount;
            float NoiseShift;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex  = UnityObjectToClipPos(v.vertex);
                o.wCenter = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
                o.radius  = length(mul(unity_ObjectToWorld, float4(1, 0, 0, 1)).xyz - o.wCenter);
                o.wPos    = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            
            float4 SphereHit(float3 dir, float3 center, float r)
            {
                float3 p2 = dir;

                float a = dot(p2, p2);
                float b = 2.0 * dot(p2, -center);
                float c = dot(center, center) - r*r;
                
                float test = b * b - 4.0 * a * c;
                
                float u = (-b -sqrt(test)) / (2.0 * a);
                
                return float4(u * p2, test);
            } 
            
            
            float TexNoise(float3 pos)
            {
                return tex2D(_MainTex, pos.x) * .33333 + 
                       tex2D(_MainTex, pos.y) * .33333 +
                       tex2D(_MainTex, pos.z) * .33333;
            }
            
            
            float3 GetShadow(float4 lightPos, float3 wCenter, float3 wPos)
            {
                return 1;
                float m = 1;
                
                float3 rayDir = lightPos.xyz - wPos;
                
                for(int i = 0; i < PlanetCount; i++)
                {
                    float4 p = PlanetBuffer[i];
                    
                    float3 cToP = wCenter - p.xyz;
                    float dist  = cToP.x * cToP.x + cToP.y * cToP.y + cToP.z * cToP.z;
                    float skip = step(dist, .01);
                    
                    m *= lerp(1 - saturate(SphereHit(rayDir, p.xyz - wPos, p.w).w * lightPos.w), 1, skip);
                }
                
                return m;
            }
            
            
            float3 GetLightValues(float4 lightPos, float3 wPos, float3 wN, float3 wCenter, float3 dir, float2 perlin, float fresnel)
            {
                float fPerlin2 = lerp(perlin.y, 1, fresnel);
                
                float3 light = normalize(lightPos.xyz - wPos);
                float3 lToC     = lightPos.xyz - wCenter;
                
                float lightDist   = lToC.x * lToC.x + lToC.y * lToC.y + lToC.z * lToC.z;      
                float source      = step(lightDist, .01);        
                float lightD      = lerp(dot(wN, light), .5 + (1 - fresnel) * 2, source);
                float lightAmount = pow(saturate(1 - length(lightPos.xyz - wPos) * .0133333 * lightPos.w), 2);
                      lightAmount *= GetShadow(lightPos, wCenter, wPos);
                
                float lP = perlin.x * .2 + .8;
                
                float3 lightValues;
                lightValues.x = (saturate(lightD) * .7 * lP + .275 * pow(lightD * .5 + .5, 2) * lP + .025 * lP + saturate(1 - pow(1 - (lightD * .5 + .5), 1.5)) * fresnel * .25) * lightAmount;
                lightValues.y = pow((dot(reflect(dir, wN), light) * .5 + .5), 10 + fPerlin2 * 70) * (.05 + (1 - fPerlin2) * .15) * (.5 + 3.5 * fresnel) * lightAmount * 1.25;
                lightValues.z = source;
                
                return lightValues;
           }
            
           float3 GetNoisePos(float3 nPos, float radius)
           {
                nPos   *= .135 * 1.8 * radius;
                nPos.y *= 4.25;
                
                float t = NoiseShift + radius * 10000;
                 
                nPos += float3(snoise(float4(nPos * 1.75 + 1000, t)) , 
                               snoise(float4(nPos * .75  - 10000, t)), 
                               snoise(float4(nPos * 1.75 + 22, t))) * .25;
                nPos += float3(snoise(float4(nPos * .75  - 1000, t)), 
                               snoise(float4(nPos * .075 + 10000, t)), 
                               snoise(float4(nPos * .75  - 242, t))) * .1;
                nPos += float3(snoise(float4(nPos * 10.75  - 1000, t)), 
                               snoise(float4(nPos * 10.075 + 10000, t)), 
                               snoise(float4(nPos * 10.75  - 242, t))) * .05;
                         
                return nPos;
           }
           
           
            float2 GetPerlin(float3 nPos)
            {             
                float perlin = TexNoise(nPos * .75);
                                
                float perlin2 = abs(fmod(perlin * 12, 2) - 1) * .475 + 
                                abs(fmod(perlin * 24, 2) - 1) * .475 +
                                abs(fmod(perlin * 36, 2) - 1) * .05;
                       perlin = abs(fmod(perlin * 6, 2) - 1);
                       
                return float2(perlin, perlin2);
            }
            
            
            float3 GetColor(float3 n)
            {
                return saturate(pow(tex3D(_ColorTex, n).xyz, 3) * 6);
            }
            

            float4 frag (v2f i) : SV_Target
            {
                float3 dir  = normalize(i.wPos -_WorldSpaceCameraPos);
                float3 toCenter = i.wCenter - _WorldSpaceCameraPos;
                float4 hit  = SphereHit(dir * 1000, toCenter, i.radius);
                float3 wPos = lerp(i.wPos, hit.xyz  + _WorldSpaceCameraPos, saturate(hit.w));
                float dist = length(wPos -_WorldSpaceCameraPos);
                float3 wN   = normalize(wPos - i.wCenter);
                float fresnel = 1 - saturate(dot(wN, -dir));
                
                float3 nPos = mul(unity_WorldToObject, float4(wPos, 1)).xyz;
                float3 lN = normalize(nPos);
                float3 noisePos = GetNoisePos(nPos, i.radius);
                float2 perlin = GetPerlin(noisePos);
                    
                float nOffset = noisePos * .3 + perlin.x * .05 * (1 - fresnel) + perlin.y * .0125 * (1 - fresnel) + i.radius * 10000;// + wPos * .01;
                float3 result  = lerp(GetColor((-lN * .5 + .5) * .05 + nOffset), GetColor((lN * .5 + .5) * .05 - nOffset), perlin.x * .125);
                       result  = lerp(result, .5, .5);
                       //result *= lerp(lerp(perlin.x, 1, saturate((dist - i.radius) * .1)), 1, fresnel) * .35 + .65;
                       result *= lerp(perlin.x, 1, fresnel) * .25 + .75;
                       
                //return float4(result, 1);
                    //   result = .5;
                float3 light = float3(0, 0, 0);
                
                for(int e = 0; e < LightCount; e++)
                {
                    float3 lightValues = GetLightValues(LightBuffer[e], wPos, wN, i.wCenter, dir, perlin, fresnel);
                    light.x += lightValues.x;
                    light.y += lightValues.y;
                    light.z  = max(light.z, lightValues.z);
                }
                       
                //light = float3(1, 0, 0);
                float3 litResult = result;
                       litResult *= light.x;
                       litResult += light.y;
                result = lerp(litResult, lerp(result, 1, .5), light.z);
                result = pow(result, 1.65) * 3 * 1.5;
                
                float a = saturate(hit.w * .0000085 / (i.radius * dist));
                      a = 1.0 - pow(1.0 - a, 20);
                      a = lerp(a,  pow(1 - fresnel, 8) * (.875 + perlin.x * .125) * 2, light.z);
                      a *= pow(saturate((length(toCenter)-i.radius) * 5), 10);
                return float4(saturate(result), a);
            }
            ENDCG
        }
    }
}
