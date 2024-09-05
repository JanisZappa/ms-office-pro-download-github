#include "Castle.cginc"

struct v2f
{
    float4 vertex : SV_POSITION;
    float4 color  : TEXCOORD0;
    float3 wPos   : TEXCOORD1;
    float3 normal : TEXCOORD2;
    float2 uv     : TEXCOORD3;
};

sampler2D _MainTex, _Noise;
            
float4 frag (v2f i) : SV_Target
{
    float3 dir = i.wPos - _WorldSpaceCameraPos;
    float dist = length(dir);
    dir /= dist;
    
    float3 normal = normalize(i.normal);
    float fD = saturate(dot(-dir, normal));
    float fDm = pow(1 - fD, 10);
  
    float faceD = 1.0 - pow(1.0 - fD, 2);
    float faceAdd = tex2D(_Noise, i.uv * .035).x * .4 + tex2D(_Noise, i.uv * 6.75).x * .125 + tex2D(_Noise, i.uv * 11.5).x * .3;
   
    float face = ((pow(faceD, 4 + faceAdd * 4) * .2 + pow(faceD, 10 + faceAdd * 290) * (.1 + .1 * faceAdd) * 2)) * (1 + fDm * 20) * .8;//(1 - saturate(dist * .02)));
    
    face -= .125;
    
    float g1 = (1.0 - pow(saturate(dist * .01825), 1.5));
    float g2 = (1.0 - pow(saturate(dist * .1825), 1.5));
    
    #if GRID_ON
    float grid = 1 + lerp(Grid(i.uv) * g1, Grid(i.uv * 4) * g1, g2 * .35);
    #else
    float grid = 1;
    #endif
       
    //return (1.0 - i.color.a);
    float fog = lerp(pow(1.0 - saturate(dist * .007 * (.5 + .5 * i.color.a)), 10), 1.0, (1.0 - i.color.a));
  
    
    float3 c = i.color.xyz;
    #if GRAY_ON
    c = .6;
    #endif      
    c = (c + (face * (c * .9 + .1))) * grid;
           
           
    #if SCENE_VIEW || FOG_OFF
    c *= .6;
    #else
    c = FogHue(c, fog, dir);
    #endif  
    
    return float4(lerp(pow(c * 1.45, 2) * 1.75, float3(0.01, .025, .035) * 1.75, (1.0 - i.color.a) * .8), 1);
}