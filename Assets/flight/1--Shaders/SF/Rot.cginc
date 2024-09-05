#include "UnityCG.cginc"

int XRes, YRes;
StructuredBuffer<float4x4> CamBuffer;

struct PosRot
{
    float3 pos;
    float3 euler;
    float2 anim;
};

float3x3 Euler3x3(float3 v)
{
    float sx, cx;
    float sy, cy;
    float sz, cz;

    sincos(v.x, sx, cx);
    sincos(v.y, sy, cy);
    sincos(v.z, sz, cz);

    float3 row1 = float3(sx*sy*sz + cy*cz, sx*sy*cz - cy*sz, cx*sy);
    float3 row3 = float3(sx*cy*sz - sy*cz, sx*cy*cz + sy*sz, cx*cy);
    float3 row2 = float3(cx*sz, cx*cz, -sx);

    return float3x3(row1, row2, row3);
}


float4x4 Euler4x4(PosRot sO)
{
    float sx, cx;
    float sy, cy;
    float sz, cz;

    sincos(sO.euler.x, sx, cx);
    sincos(sO.euler.y, sy, cy);
    sincos(sO.euler.z, sz, cz);

    float4 row1 = float4(sx*sy*sz + cy*cz, sx*sy*cz - cy*sz, cx*sy, sO.pos.x);
    float4 row3 = float4(sx*cy*sz - sy*cz, sx*cy*cz + sy*sz, cx*cy, sO.pos.y);
    float4 row2 = float4(cx*sz, cx*cz, -sx, sO.pos.z);
    float4 row4 = float4(0, 0, 0, 1);
    return float4x4(row1, row2, row3, row4);
}


float4 GetVertex(float3 vertex, PosRot sO, uint Frame)
{
    float3 worldPosition = sO.pos + mul(Euler3x3(sO.euler), vertex);
    
    return mul(UNITY_MATRIX_VP, mul(CamBuffer[Frame], float4(worldPosition, 1.0)));
}


float4 GetVertexFromWP(float3 worldPosition, uint Frame)
{
    return mul(UNITY_MATRIX_VP, mul(CamBuffer[Frame], float4(worldPosition, 1.0)));
}


float3 RotRad(float3 v, float radians)
{
    float s = sin(radians);
    float c = cos(radians);
     
    float tx = v.x;
    float ty = v.y;

    return float3(c * tx - s * ty, s * tx + c * ty, v.z);
}


float4 ToFrame(float4 vertex, uint Frame)
{   
    const float xDiv = 1.0 / 6;
    const float yDiv = 1.0 / 5;
    float xOffset = fmod(Frame, 6)   * xDiv * 2;
    float yOffset = floor(Frame / 6) * yDiv * 2;
    
    vertex.x = ((vertex.x / vertex.w) * xDiv - (1.0 - xDiv) + xOffset) * vertex.w;
    vertex.y = ((vertex.y / vertex.w) * yDiv + (1.0 - yDiv) - yOffset) * vertex.w;
    
    return vertex;
}


void ClipFrame(float4 vertex, uint Frame)
{    
    clip(min(-(abs(XRes * (fmod(Frame, 6) + .5)   - vertex.x) - XRes * .5), 
             -(abs(YRes * (floor(Frame / 6) + .5) - vertex.y) - YRes * .5)));
}
  

float4 GetBillboardVertexFromWP(float3 worldPosition, float3 center, uint Frame)
{
    float3 cnt = mul(CamBuffer[Frame], float4(center, 1.0)).xyz;
    
    float3 r = mul(CamBuffer[Frame], float4(1, 0, 0, 0)).xyz;
           r.z = 0;
           r = normalize(r);
            
    float3 dir = worldPosition - center;
    float3 pos = cnt + RotRad(dir, atan2(r.y, r.x));
    
    return mul(UNITY_MATRIX_VP, float4(pos, 1));
}


sampler2D PaletteTex;

float4 Grade(float4 color)
{
    int r = round(color.x * 255);
    int g = round(color.y * 255);
    int b = round(color.z * 255);
                
    uint offset = (uint)(r + g * 256 + b * 256 * 256);
    uint x = offset % 4096;
    uint y = floor(offset / 4096);
    float2 uv = float2(x + .5, y + .5) / 4096;
    return float4(tex2D(PaletteTex, uv).xyz, color.a);
}