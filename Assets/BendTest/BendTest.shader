Shader "Unlit/BendTest"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = ""{}
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
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            
            #pragma multi_compile FLAT_OFF       FLAT_ON
            #pragma multi_compile NOISE_OFF      NOISE_ON

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 color : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float4 values : TEXCOORD2;
                fixed3 posW   : TEXCOORD3;
                float2 uv : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            float2 TwistForces;
            float3 Center;
            float4 Velocity;
            sampler2D BendTex;
            sampler2D _MainTex;
            
            
            float3x3 GetMat(float3 Axis, float Rotation)
            {
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;
            
                float3x3 rot_mat = 
                {   one_minus_c * Axis.x * Axis.x + c, one_minus_c * Axis.x * Axis.y - Axis.z * s, one_minus_c * Axis.z * Axis.x + Axis.y * s,
                    one_minus_c * Axis.x * Axis.y + Axis.z * s, one_minus_c * Axis.y * Axis.y + c, one_minus_c * Axis.y * Axis.z - Axis.x * s,
                    one_minus_c * Axis.z * Axis.x - Axis.y * s, one_minus_c * Axis.y * Axis.z + Axis.x * s, one_minus_c * Axis.z * Axis.z + c
                };
                
                return rot_mat;
            }
            

            v2f vert (appdata v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                
                o.color = v.color;
                float3 wP = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv +  mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xz;
                
                float  cP  = length(wP.xz - _WorldSpaceCameraPos.xz);
                
                
                float height = wP.y;
                float3 dir = float3(wP.x, 0, wP.z);
                float  l   = length(dir);
                       wP  = wP + 
                             float3(Velocity.x, 0, Velocity.y) * pow(wP.y * .8, 2) * .002 * 1+
                             float3(Velocity.z, 0, Velocity.w) * saturate(1.0 - pow(wP.y * .075, 2)) * .01 * 4 * pow(l * .01, 2) +
                             - Center;
                
                dir = float3(wP.x, 0, wP.z);
                l   = length(dir);
                
                #if FLAT_ON
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal =  mul(unity_ObjectToWorld, v.normal);
                o.values = float4(0, l, 0, cP);
                o.posW   = mul(unity_ObjectToWorld, v.vertex);
                return o;
                #endif
                
                float lean = 1 + wP.y * .1 * .5;
                float twist = lerp(TwistForces.y, TwistForces.x, pow(wP.y * .8, 2) * .01);
                float3x3 testrot = GetMat(float3(0, 1, 0), lean * -.0005 * 2 * l * twist);
                wP  = float4(mul(testrot, wP.xyz), 1);
                dir = float3(wP.x, 0, wP.z);
                float3 dirN = normalize(dir);
                float3 axis = normalize(cross(dirN, float3(0, 1, 0)));
                
                float angle = (atan2(dirN.x, dirN.z) / 3.1415926535897 * .5 + .5);
                float sn = (sin(angle * 3.1415926535897 * 8 + _Time.y * 4) * .5 + .5) * .6 + .4;
                
                float4 bendColor = tex2Dlod(BendTex, float4(angle, 0, 0, 0));
                float value = bendColor.x + bendColor.y * 256 + bendColor.z * 256 * 256;
                      value /= 256 * 256 * 256;
                sn = 1 + (bendColor.x - .5) * 1.75;// * .8 + .2;
                sn = 1;
                float radius   = 65 * (1.0 + dirN.x * .75);
                      radius   = 65;// * 1.15 * sn;//(1.0 + dirN.x * .55);
                float diameter = radius * 2 * 3.1415926535897;
                float rad      = l / diameter * 2 * 3.1415926535897;
                float3x3 rot = GetMat(axis, -rad);
                
                float3 p = mul(rot, float3(0, radius + height, 0)) + float3(0, -radius, 0);
                float3 n = mul(rot, mul(testrot, mul(unity_ObjectToWorld, v.normal)));
                
                o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, float4(p + Center, 1)));
                o.normal = n;
                o.values = float4(abs(rad) - height * .0625, l, sn, cP);
                return o;
            }
            
            

            float noise(float2 coords) {
                return (frac(sin(dot(coords.xy + _Time.xy * 100, float2(12.9898,78.233))) * 43758.5453) * 2 - 1) * .005;
            }
            
            
            float3 gamma(float3 v)
            {
                return pow(v, .75);
            }
            

            fixed4 frag (v2f i) : SV_Target
            {
                #if FLAT_OFF
                clip(-i.values.x + 0.45);   
                #endif
                
                
                float d = (dot(normalize(i.normal), normalize(float3(.23, .4, .1))) * .5 + .5);
                float u = saturate(1.0 - pow(1.0 - pow(i.color.w, 2), 3));
               
                float far  = saturate(1.0 - pow(i.values.y * .04, 6)) * .65 + .35;
                float near = (1.0 - pow(1.0 - saturate(pow(i.values.w * .275, 40)), 4)) * .35 + .65;
                
                float4 tex = tex2D(_MainTex, i.uv * .175);// + float2(0, _Time.x * -.5));
                float t = saturate(d + (1.0 - (.65 + .35 * tex.r)) * .35) * (.6 + .4 * tex.r);
                float light = (.3 + .7 * t) * far * near * (.25 + .75 * u);
                
                return float4(gamma(i.color.xyz * light) + noise(i.vertex.xy) * 2, 1);
            }
            ENDCG
        }
    }
}
