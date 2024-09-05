#include "GridPBR.cginc"
#define PLightCount 8
#define DLightCount 4
#define Gray .35
   
struct PLight
{
   float3 pos;
   float3 color;
   float rangemulti;
   float banana;
};
struct DLight
{
   float3 dir;
   float3 color;
   float2 banana;
};

struct ScatterObject
{
    float3 pos;
    int id;
    float scale, flip;
};
            
struct ItemInfo
{
    float2 qMin,   qMax, 
          uvMin,  uvMax, 
         yRange, zRange;
};
   
StructuredBuffer<ScatterObject> RenderBuffer;
StructuredBuffer<ItemInfo>      ItemInfoBuffer;
StructuredBuffer<PLight>         PLightBuffer;
StructuredBuffer<DLight>         DLightBuffer;
float Now, LightsOn;
float3 CamPos;

sampler2D SkyTex;
float SkyScale;
   
float3 GetPBR(float3 color, float3 wPos, float3 normal, float4 dtl, float4 lgt, float sprite)
{
   float sss = lgt.y * (1.0 - pow(1.0 - saturate(wPos.y), 2));
   float3 V = normalize(float3(0, 1, -1));
   float h = (1.0 - pow(1.0 - saturate(wPos.y * .2), 2)) * .65 + .9;
   float3 result = 0;
   
//  Point  //
   [unroll]
   for(int x = 0; x < PLightCount; x++)
   {
       PLight l = PLightBuffer[x];
       
       float3 L = l.pos - wPos;
       float dist = length(L);
     
       float spec = 1.0 - saturate(dist * l.rangemulti);
       if(spec < .00001)
            continue;
           
       L /= dist; 
       spec = pow(spec, 2);
            
       float t = pow(1.0 - saturate(dist * l.rangemulti * 1.6666666666), 2);
       
       float m = (lerp(1, lgt.a, pow(saturate(L.y), 2) * sprite) * lerp(1, lgt.z, pow(saturate(-L.y), 2) * sprite));  
             //m = 1;   
       float dP = dot(L, normal);
       float dP2 = max(0, dP) +  max(0, -dP) * sss * 2;
             dP = dP2 * .25 * m + .75 * (dP * .5 + .5) * (m * .25 + .75) + (-dP * .5 + .5) * sss * 2;
       
       result += PBR(color, wPos, normal, L, V, dtl.xyz, dP, dP * t, dP2 * spec) * l.color * LightsOn;
   }
   
//  Dir  //
   [unroll]
   for(int x = 0; x < 1; x++)
   {
        DLight l = DLightBuffer[x];
        
       float3 L = l.dir;
       float m = (lerp(1, lgt.a, pow(saturate(L.y), 2) * sprite) * lerp(1, lgt.z, pow(saturate(-L.y), 2) * sprite));     
       float dP = dot(L, normal);
             dP = max(0, dP) * .25 * m + .75 * (dP * .5 + .5) * (m * .25 + .75) + (-dP * .5 + .5) * sss * .25;
       
       result += PBR(color, wPos, normal, L, V, dtl.xyz, dP, dP, 1) * l.color * h;
   }
   
//  Emission  //
   result += color * lgt.x * 2.5 * (.05 + .95 * pow(dtl.x, 2));
   return result;
}


float3 Depth(float3 wPos)
{
    return float3(saturate((wPos.x + 30) / 60.0), saturate((wPos.y) * .2), saturate((wPos.z + 20) * .025));
    return ((sin(wPos.y * 10 + _Time.y * -.5) * .5 + .5) * .95 + .05);
}


float Fog(float3 wPos)
{
    return saturate(pow(saturate((length(CamPos - wPos) - 121.5) * .035), 3) - pow(saturate(wPos.y  * .2), 3));
}


float ClipIt(float3 wPos)
{
    float3 dir = wPos - CamPos;
    dir.x *= .5;
    return step(0, (dir.x * dir.x + (dir.z * dir.z) * 2) -22100);
}


float FrontShade(float3 wPos)
{
    float3 dir = wPos - CamPos;
    dir.z -= 198.65;
    dir.x *= .5;
    //return saturate((dir.x * dir.x + (dir.z * dir.z) * 2) -22020);
    return saturate(sqrt((dir.x * dir.x + (dir.z * dir.z) * 2)) -149.75);
}


float3 Sky(float3 wPos, float3 normal)
{
    float d = wPos.y - 60;
    normal.z *= -.05;
    normal.x *= -.05;
    
    wPos += normal * d;
    
    //normal = normalize(normal);
    return 0;//pow(tex2D(SkyTex, wPos.xz * SkyScale + float2(.5, _Time.y * .025)).x, 3) * (pow(normal.y * .5 + .5, 2) * .2 + .2) * 1;
}


float3 Result(float3 color, float3 normal, float4 dtl, float3 wPos, float4 lgt, float sprite)
{
    //return Sky(wPos, normal);
    //return dtl.x;
    //return lgt.y;
    //return lgt.a;
    float fog = Fog(wPos);
    //return fog;
    //return reflect * .5 + .5;
    //return saturate(normal.z * 4);
    //return normal.x * .5 + .5;
    //return normal * .5 + .5;
    //return Depth(wPos);
    //return sss;
    float3 r = lerp(pow(GetPBR(color, wPos, normal, dtl, lgt, sprite) * (1.5 + wPos.y * .1)  + Sky(wPos, normal), 1.75), float3(.015, .0275, .0425) * 1.3, fog * .25 + .015);
    return pow(lerp(r, 0, FrontShade(wPos) * .35), 1.25) * 2;//HClip(r, wPos);
}