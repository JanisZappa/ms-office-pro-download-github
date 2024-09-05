#include "UnityCG.cginc"
   
            
struct v2fone
{
    float4 vertex : SV_POSITION;
    float3 normal : TEXCOORD0;
    float4 color  : TEXCOORD1;
    float3 wPos   : TEXCOORD2;
    float2 uv     : TEXCOORD3;
};

sampler2D _MainTex;


float4x4 BillboardMatrix(float3 forward, float3 up, float3 right)
{
    return float4x4(right, 0, 
                       up, 0,
                  forward, 0,
                  0, 0, 0, 1);
}


float gradient(float v, uint p)
{
    return ((1.0 - pow(1.0 - abs(v), p)) * sign(v)) * .5 + .5;
}


float4 fog(float4 col, float3 camDir, float dist, float y)
{
    //.002
    //.001425
    //0.0016666
    float a = 1.0 - (pow((1.0 - saturate(dist * .0013)),2));// * (1.0 - saturate(y * .0115)) * (1.0 - saturate(y * -.0225 + .75)));

    return lerp(col, gradient(camDir.y, 4) * .055 + .0075, a);
}


fixed4 frag (v2fone i) : SV_Target
{
   float3 camDir = i.wPos - _WorldSpaceCameraPos;
   float dist = length(camDir);
   camDir /= dist;
   
   float3 normal = normalize(i.normal);
   //return gradient(reflect(camDir, normal).y, 6);
   
   float4 col = i.color;
          //col = col * .25 * .75 * (.2 + .7 * tex2D(_MainTex, i.uv * .075));
   
   col *= abs(dot(normal, normalize(float3(1, 1, 1)))) * .75 + .25 * (normal.y * .5 + .5) + .05;
   col += gradient(reflect(camDir, normal).y, 4) * .175;
   col  = fog(col, camDir, dist, i.wPos.y);
   //col *= .75;
   
   col *= 1.0 + .05 * tex2D(_MainTex, i.uv).x * (1.0 - pow(saturate(dist * .0275), 2));
   
   float l = (1.0 - pow(saturate(dist * .075), 2)) * .025;
   
   
  //camDir = i.wPos - _WorldSpaceCameraPos;
  //camDir.y += 150;
  //return (sin(length(camDir) * 1.1 + _Time.y) * .5 + .5) * .5;
  //return step(.5, saturate(length(camDir) * 0.004 * (1.0 - pow(1.0 - ( sin(_Time.y * .4) * .5 + .5), 60)))) * .2;
  
   
   return pow(col, 1.5) * 1.5 + float4(1, .5, .05, 0) * l;
}