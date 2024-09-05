Shader "Unlit/CrappyFisheye3"
{
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
                float4 color  : COLOR;
            };

            struct v2f
            {
                float3 normal : TEXCOORD0;
                float2 dir    : TEXCOORD1;
                float4 color  : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };
            

            v2f vert (appdata v)
            {
                v2f o;
                
                float4 viewPos = mul(unity_CameraProjection,  mul(unity_ObjectToWorld, v.vertex));
                
                float2 dir = float2(viewPos.x, viewPos.y);
                       //viewPos.x += dir.x;
                     //  viewPos.y += dir.y;
                       
				//vert = mul(UNITY_MATRIX_P, viewPos);
				float4 vert = mul(unity_WorldToObject, mul(unity_CameraInvProjection, viewPos));
               
                o.vertex = UnityObjectToClipPos(vert);
                o.normal = v.normal;
                o.color = length(dir);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float d = dot(normalize(i.normal), normalize(float3(1, .2, .5))) * .4 + .6;
                return  i.color;
            }
            ENDCG
        }
    }
}
