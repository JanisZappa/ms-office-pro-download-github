sampler2D _MainTex;
sampler2D _LowTex;


inline float4 TexLerp(sampler2D a, sampler2D b, float2 uv, float dist)
{
    float mipMapLerp = saturate(dist / 20000);

	return lerp(tex2D(a, uv), tex2D(b, uv), mipMapLerp);
}

		
inline float3 MainTexLerp(float2 uv, float dist)
{
    return TexLerp(_MainTex, _LowTex, uv, dist).xyz;
}