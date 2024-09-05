Shader "Unlit/Sand"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTex2 ("Texture", 2D) = "white" {}
        _Sky("Sky", Color) = (1,1,1,1)
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 wPos   : TEXCOORD2;
            };

            sampler2D _MainTex, _MainTex2;
            float4 _Sky;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            
            
            float3 GetNormal(sampler2D tex, float2 uv, float3 f, float3 r, float3 n)
            {
                float4 col = tex2D(tex, uv) * 2 - 1;
                return r * col.x + f * col.y + n * col.z;
            } 
              

            fixed4 frag (v2f i) : SV_Target
            {
                float3 light = normalize(float3(1, .4, .3));
            
                float3 n = normalize(i.normal);
                float3 r = normalize(cross(float3(0, 0, 1), n));
                float3 f = normalize(cross(r, n));
                
                n = normalize(lerp(GetNormal(_MainTex2, i.uv * 5, f, r, n), n, 1.0 - (1.0 - pow(1.0 - abs(n.y), 10)) * .1));
                r = normalize(cross(float3(0, 0, 1), n));
                f = normalize(cross(r, n));
                
                float3 sandN = normalize(lerp(GetNormal(_MainTex2, i.uv * 11.32 + float2(_Time.y * -.04, 0), f, r, n), n, 1.0 - (1.0 - pow(1.0 - abs(n.y), 2)) * .035));
                r = normalize(cross(float3(0, 0, 1), sandN));
                f = normalize(cross(r, sandN));
            
                sandN = normalize(GetNormal(_MainTex, i.uv * 300, f, r, sandN));
                n = normalize(n * .7 + .3 * sandN);
                float d = saturate(dot(n, light));
                n = sandN;
                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float dist = length(camDir);
                camDir /= dist;
                
                float l = saturate(dot(reflect(camDir, n), light));
                float s = (pow(l, 40) * .4) * .95;
                float b = pow(1 - saturate(dot(-camDir, n)), 8)* .15;
                
                //return s + b;
                
                float4 result = (d * .7 + .15) + s + b  + (1.0 - pow(1.0 - saturate(dist * .000275), 2)) * .35;
                result.xyz *= lerp(float3(.27, .35, .45), float3(.9, .65, .5), pow(n.y, 2)) * 1.35;
                result.xyz = pow(result.xyz + .075, 1.5) * 1.15;
                result = lerp(result, _Sky, pow(saturate(dist * .00085), 2) * .8);
                return result;
            }
            ENDCG
        }
    }
}
