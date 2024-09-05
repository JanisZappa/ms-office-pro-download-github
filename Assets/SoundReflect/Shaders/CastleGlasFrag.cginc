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
    float faceAdd = tex2D(_Noise, i.uv * .035 * .225 * .075).x * .4 + tex2D(_Noise, i.uv * 6.75 * .225 * .075).x * .025 + tex2D(_Noise, i.uv * 11.5 * .225 * .075).x * .3;
   
    float face = ((pow(faceD, (4 + faceAdd * 4) * 30) * .2 + pow(faceD, (10 + faceAdd * 290) * 30) * (.1 + .1 * faceAdd) * 2) * .05) * (1 + fDm * 20) * .8;//(1 - saturate(dist * .02)));
    face *= 40;
    face = pow(face, 1.5);
    face -= .225;
    
    float g1 = (1.0 - pow(saturate(dist * .01825), 1.5));
    float g2 = (1.0 - pow(saturate(dist * .1825), 1.5));
    
    #if GRID_ON
    float grid = 1 + lerp(Grid(i.uv) * g1, Grid(i.uv * 4) * g1, g2 * .35);
    #else
    float grid = 1;
    #endif
         
    float fog = pow(1.0 - saturate(dist * .007), 10);
  
    
    float3 c = float3(.2, .22, .25);
    #if GRAY_ON
    c = .25;
    #endif      
    c = (c + (face * (c * .9 + .1))) * grid;
           
           
    #if SCENE_VIEW || FOG_OFF
    c *= .6;
    #else
    c = FogHue(c, fog, dir);
    #endif  
    
    //return float4(pow(c * 1.2, 2), 1);
    return float4(1.0 - pow(1.0 - saturate((pow(c * 1.2, 1.5) * 2 - .0045) * fog * 1.012 * 1.5 - .012 * 2), 3), 1);
}