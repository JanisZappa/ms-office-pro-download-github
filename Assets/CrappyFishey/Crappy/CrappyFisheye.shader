Shader "Unlit/CrappyFisheye"
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
                
                float4 vert = UnityObjectToClipPos(v.vertex);
                float2 dir = float2(vert.x, vert.y);// * 2 / sqrt(2);
                o.dir = vert.xy;
                float amount = (pow( saturate(1.0 - length(dir) * .15) , 3))  * 4;
                
                //vert.x = vert.x * (1 + amount);
                //vert.y = vert.y * (1 + amount);
                //dir = normalize(dir);
                //vert = float4((.5 + dir.x * amount) * 420, vert.y, vert.zw) ;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
	            float2 center = float2(0, 0);
	            float2 rel = o.vertex.xy - center;
	            float l = length(rel) * 1;
	            o.vertex.xy = center + rel * .5 + (rel / sqrt(l)) * .15; //TODO: Adjust to actual aspect ratio.
	            
	            
	            // float mul = (1 - pow(1 - cos(l * 3.1415926535897932384626433 * 1), 2)) * .2 + 1;
	                  //mul = lerp(mul, 1, step(5, l));
	            //o.vertex.xy = center + rel * mul; //TODO: Adjust to actual aspect ratio.
                       
                //o.vertex = vert;
                o.normal = v.normal;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //return length(i.dir.xy);
                float2 dir = float2(i.vertex.x  / 420 - .5, i.vertex.y  / 420 - .5) * 2 / sqrt(2);
                //return length(dir);
                float d = dot(normalize(i.normal), normalize(float3(1, .2, .5))) * .4 + .6;
                return  d * i.color;
            }
            ENDCG
        }
    }
}
