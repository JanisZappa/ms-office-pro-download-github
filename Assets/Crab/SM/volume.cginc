fixed3 _Min;
fixed _Size;

fixed4 _Fog;
sampler3D _VolumeTex;
float4x4 _ToBox;

fixed4 SampleTex(fixed3 pos, fixed3 result)
{
    //pos.y = -16 + abs(pos.y - -16);
    pos = mul(_ToBox, float4(pos, 1));

    fixed3 lP = (pos - _Min) / _Size;
    if(lP.x < 0 || lP.x > 1 || lP.y < 0 || lP.y > 1 || lP.z < 0 || lP.z > 1)
       return fixed4(result, 1);
    
    fixed4 src = tex3D(_VolumeTex, lP);
        
    result = lerp(result, _Fog.xyz, _Fog.a);
    return fixed4(pow(lerp(lerp(result, src.xyz, src.a), _Fog.xyz, _Fog.a), 1.5) * 1.7, 1);
}