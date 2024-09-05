Shader "Unlit/AndyPBR"
{
    Properties
    {
        roughness("Roughness", range(0, 1)) = 0
        metal("Metal", range(0, 1)) = 0
        [NoScaleOffset] _Reflection ("R", Cube) = "grey" {}
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
                float3 normal : NORMAL;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 wPos   : TEXCOORD1;
                float4 color : TEXCOORD2;
                float2 uv : TEXCOORD3;
            };

            float roughness, metal;
            samplerCUBE _Reflection;
            sampler2D  _Spec;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.color = v.color;
                 o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {   
                float4 stex = tex2D(_Spec, i.uv * 5) * .5 + .5 * tex2D(_Spec, i.uv * 7);            
               
                float3 normal = normalize(i.normal);
                float3 view   = normalize(_WorldSpaceCameraPos - i.wPos);
                float3 light  = normalize(float3(1, .5, .5));
             
                float d = dot(normal, light);
                      d = (d * .5 + .5) * .25 + .65 * max(d, 0) + max(normal.y, 0) * .1;
                      d *= 1.0 - metal;
                float front = saturate(dot(normal, view));
                  
                float rough = i.color.a;
                rough *= .85;
                rough = 1.0 - (1.0 - rough) * (.25 + .75 * saturate(pow(stex.x, 4) * 4));
                float shinyness = 1.0 - (rough * front + rough * rough * (1.0 - front));
                      
                float3 camRef = reflect(-view, normal);
                float s = dot(light, camRef);
                      s = saturate(abs(s) * (step(0.0, s) * 11.515 + .85) + .25 * (.25 + .75 * pow(shinyness, 2)));
                
                
                      
                float ss = pow(s, 1 + shinyness * 100) * shinyness;
                
                
                float4 ref = texCUBElod(_Reflection, float4(camRef, 10 * (1.0 - shinyness))) * shinyness * 2.2;
                      // ref += ss;
                float4 result = i.color * (d * .65 + .35 + ref * .1) * (1 + ref);
                return pow(saturate(result), 2.2) * 3.3;
            }
            ENDCG
        }
    }
}
