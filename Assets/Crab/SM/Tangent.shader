Shader "Unlit/Tangent"
{
    Properties
    {
        _A		("A", Color) = (1,1,1,1)
        _B		("B", Color) = (1,1,1,1)
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
            #include "noise.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
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


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.right  = mul(unity_ObjectToWorld, float3(v.uv, v.uv2.x));
                o.angle  = v.uv2.y;
                o.pos    = v.vertex.xyz;
                o.posW   = mul(unity_ObjectToWorld, v.vertex.xyz);
                return o;
            }
            
            
             float4 setAxisAngle (float3 axis, float rad) {
              rad = rad * 0.5;
              float s = sin(rad);
              return float4(s * axis[0], s * axis[1], s * axis[2], cos(rad));
            }
            
            
            float4 multQuat(float4 q1, float4 q2) {
              return float4(
                q1.w * q2.x + q1.x * q2.w + q1.z * q2.y - q1.y * q2.z,
                q1.w * q2.y + q1.y * q2.w + q1.x * q2.z - q1.z * q2.x,
                q1.w * q2.z + q1.z * q2.w + q1.y * q2.x - q1.x * q2.y,
                q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z
              );
            }
            
            
            float3 rotateVector( float4 quat, float3 vec ) {
              // https://twistedpairdevelopment.wordpress.com/2013/02/11/rotating-a-vector-by-a-quaternion-in-glsl/
              float4 qv = multQuat( quat, float4(vec, 0.0) );
              return multQuat( qv, float4(-quat.x, -quat.y, -quat.z, quat.w) ).xyz;
            }
            
            
            float3 noiseDir(float3 p)
            {
                return float3(snoise(p), snoise(p + float3(-100, 10, 10000)), snoise(p + float3(100, 4532, -543)));
            }
            

            fixed4 frag (v2f i) : SV_Target
            {
                float3 n = normalize(i.normal);
                float3 r = normalize(i.right);
                
                //n = rotateVector(setAxisAngle(r, sin(i.angle * 4) * .025 + sin(i.angle * 6) * .0125 + sin(i.angle * 3) * .0125 + i.angle * -.25), n);
                n = rotateVector(setAxisAngle(r, i.angle * -.15), n);
                n = normalize(n + noiseDir(i.pos * 60) * .1 * .4 + noiseDir(i.pos * 30) * .075 * .4 + noiseDir(i.pos * .5) * .025 * .4 + noiseDir(i.pos * .1) * .1 * .4);
                
                float3 lDir = normalize(float3(1, 2, 1));
                
                // Using Blinn half angle modification for perofrmance over correctness
                float3 h = normalize(normalize(_WorldSpaceCameraPos - i.posW) - lDir);
                float specLighting = pow(saturate(dot(h, n)), 20.0f) * .2;
                specLighting += pow(saturate(dot(h, n)), 10.0f) * .2;
                float v = dot(n, lDir) * .5 + .5;
                
                return float4(lerp(_B, _A, v) + specLighting, 1);
            }
            ENDCG
        }
    }
}
