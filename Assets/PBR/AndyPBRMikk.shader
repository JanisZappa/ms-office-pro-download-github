Shader "Unlit/AndyPBRMikk"
{
    Properties
    {
        roughness("Roughness", range(0, 1)) = 0
        metal("Metal", range(0, 1)) = 0
        [NoScaleOffset] _Reflection ("R", Cube) = "grey" {}
        _Normal ("N", 2D) = "white" {}
        _Spec ("S", 2D) = "white" {}
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
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 wPos : TEXCOORD1;
            };

            float roughness, metal;
            samplerCUBE _Reflection;
            sampler2D  _Normal, _Spec;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {   
                float4 ntex = tex2D(_Normal, i.uv);
                float4 stex = tex2D(_Spec, i.uv * 1) * .25 + tex2D(_Spec, i.uv * 4) * .75 ;
                float3 normal = UnityObjectToWorldNormal(normalize(ntex.xyz * 2 -1));
                       //normal = normalize(normal +stex.xyz * .25 );
                float3 view   = normalize(_WorldSpaceCameraPos - i.wPos);
                float3 light  = normalize(float3(1, .5, .5));
             
                float d = dot(normal, light);
                      d = (d * .5 + .5) * .05 + .85 * max(d, 0) + max(normal.y, 0) * .1;
                      d *= 1.0 - saturate(metal + pow(ntex.a, 6) * .25);
                      
                float front = saturate(dot(normal, view));
                  
                float rough = roughness;
                      rough = saturate(rough - pow(ntex.a, 10) * .1);
                      rough = rough - saturate(pow(stex.x, 2) * 2) * (1.0 - rough * .95);
                      
                float shinyness = 1.0 - (rough * front + rough * rough * (1.0 - front));
                   //   shinyness *= ntex.a;
                float3 camRef = reflect(-view, normal);
                float s = dot(light, camRef);
                      s = saturate(abs(s) * (step(0.0, s) * .15 + .85) + .25 * (.25 + .75 * pow(shinyness, 2)));
                
                
                      
                float ss = pow(s, 1 + shinyness * 100) * shinyness;
                
                
                float4 ref = texCUBElod(_Reflection, float4(camRef, 10 * (1.0 - shinyness))) * shinyness * 2.2;
                      // ref += ss;
                float4 result = .5 * (d + .15 + ref * .25) * (1 + ref);
                return pow(saturate(result), 2.2) * 2.2;
            }
            ENDCG
        }
    }
}
