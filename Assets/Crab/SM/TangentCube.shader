Shader "Unlit/TangentCube"
{
    Properties
    {
        _A		("A",   Color) = (1,1,1,1)
        _B		("B",   Color) = (1,1,1,1)
        _Ref    ("Ref", Cube)  = "grey" {}
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
            #pragma target 4.5

            #include "UnityCG.cginc"
            #include "normals.cginc"
            #include "volume.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float2 uv2    : TEXCOORD1;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float3 normal : TEXCOORD0;
                float3 right  : TEXCOORD1;
                float  angle  : TEXCOORD2;
                float3 pos    : TEXCOORD3;
                float3 posW   : TEXCOORD4;
                
                float4 vertex : SV_POSITION;
            };
            
            float3 _A, _B;
            float _MaxDist;
            samplerCUBE _Ref;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.right  = mul(unity_ObjectToWorld, float3(v.uv, v.uv2.x));
                o.angle  = v.uv2.y;
                o.pos    = v.vertex.xyz;
                o.posW   = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            

            fixed4 frag (v2f i) : SV_Target
            {
                float3 n    = normalize(i.normal);
                float3 lDir = normalize(float3(1, 2, 1));
                return SampleTex(i.posW, (dot(lDir, n) * .5 + .5).xxx);
                
                
                float3 r = normalize(i.right);
                
                float b = .5;
                float s = sin(i.angle * 26);
                      s = pow(abs(s), 1) * sign(s);
                      
                n = rotateVector(setAxisAngle(r, s * .1 + i.angle * -.25), n);
                //n = rotateVector(setAxisAngle(r, i.angle * -.15), n);
                
                n = normalize(n + noiseDir(i.pos * 60) * .1 * b + noiseDir(i.pos * 30) * .075 * b + noiseDir(i.pos * .5) * .025 * b + noiseDir(i.pos * .1) * .1 * b);
                
                
                float3 pDir = i.posW - _WorldSpaceCameraPos;
                float dist  = length(pDir);
                float3 vDir = pDir / dist;
                float3 rDir = reflect(-vDir, n);
                
                float3 refData = texCUBE (_Ref, rDir).xyz;
                float specLighting = ((0.2125 * refData.x + 0.7154 * refData.y +  0.0721 * refData.z) * .5 - .25) * .1;
                
                float v = dot(n, lDir) * .5 + .5;
                float3 result = lerp(_B, _A, v) + specLighting;
                       result = SampleTex(i.posW, result);
                return float4(result, 1);
            }
            ENDCG
        }
    }
}
