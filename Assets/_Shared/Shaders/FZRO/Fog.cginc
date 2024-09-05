float4 FZero_Fog;
float  FZero_FogDist;


inline float GetDist(float3 pos)
{
    float3 dir = (_WorldSpaceCameraPos - pos);
    return dot(dir, dir);
}
			

inline float FogLerp(float dist)
{
    return 1 - pow(1 - saturate(dist / FZero_FogDist), 5);
}

inline float4 FogResult(float3 colors, float dist)
{
    float fogLerp = FogLerp(dist);
    return float4(lerp(colors, FZero_Fog.xyz, fogLerp), 1);
}