Shader "Castle/Post"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "Crappy.cginc"
            #include "HSV.cginc"
            
            
            uniform sampler2D _MainTex;
            float2 NoiseOffset;
            float SceneFade;
            
            
            float4 frag(v2f_img i) : COLOR 
            {
                
                float3 screentex  = ScreenTex(i.uv);
                
                
                
                float na = animnoise(screentex.x,  5);
                float nb = animnoise(screentex.y, -2.13);
                
                float4 c  = tex2D(_MainTex, i.uv);
                
                float2 uv2 = i.uv - .5;
                float uvMag = uv2.x * uv2.x + uv2.y * uv2.y;
                float shift = (.2 + uvMag * 1) * .00125 * 2.5;
                float4 c2 = tex2D(_MainTex, uv2 * (1 - shift) + (.5 - shift));
                float4 c3 = tex2D(_MainTex, uv2 * (1 - shift) + (.5 + shift));
                
                c = lerp(c, float4(c2.x, c.y, c3.z, c.w), .75);// * .5;
                
                float noise = (na + nb) * .0325 * .9;
                return float4(c.xyz, 1) + noise;
                
                float l = SceneFade;
                float3 hsv = rgb2hsv(c.xyz);
                       hsv.y *= (1.0 - pow(1.0 - l, 6) * .6) * .8;
                       hsv.x += l * pow(1 - l, 4) * .125 * (-1 + hsv.z * 2);
                       hsv.z *= pow(l, 1 + (1 - l) * (1 - hsv.z) * .25);
                       hsv = hsv2rgb(hsv);
                 
                float3 col = hsv;
                       
                return float4(col, 1) + noise;;
            }
            ENDCG
        }
    }
}
