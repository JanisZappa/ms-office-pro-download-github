#include "VertRot.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float3 wPos : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float3 normal : TEXCOORD1;
    float4 shading : TEXCOORD2;
    float4 color : TEXCOORD3;
};


sampler2D _Noise, _Stamp;
float4 _Color, _Color2;
float CanvasSize;
            

v2f verttwosided (appdata v, float side)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    float3 root = float3(v.uv.x, 0, v.uv.y);
    float3 wRoot = mul(unity_ObjectToWorld, float4(root, 1)).xyz;
    float3 wind = normalize(float3(1, 0, .65));
    
    float col  = tex2Dlod(_Noise, float4(wRoot.xz * .0135  + wind.xz * _Time.y * .015, 0, 0)).x * .75 +
                 tex2Dlod(_Noise, float4(wRoot.xz * .0125  + wind.xz * _Time.y * .0298, 0, 0)).x * .25;
    float col2 = tex2Dlod(_Noise, float4(wRoot.xz * .235  + wind.xz * _Time.y * .035, 0, 0)).x;
    
    float3 vert    = v.vertex.xyz - root;
           vert.y *= 1.55 + (1 - pow(1 - (sin(wRoot.x * 3) * .5 + .5), 4)) * .2;
    
    float amount = (sin(_Time.y * .5 + col * 6) * .5 + .5) * .35 + (sin(_Time.y * 2 + col2 * 4) * .5) * .2;
    
           wind = normalize(float3(wind.x * amount, .5, wind.z * amount));
           
    float2 canvasUV = (wRoot.xz + float2(CanvasSize, CanvasSize) * .5) / CanvasSize;
    float4 s = pow(tex2Dlod(_Stamp, float4(canvasUV, 0, 0)), 1.0 / 2.2);
    wind = lerp(wind, float3(-(s.x * 2 - 1), 0, -(s.y * 2 - 1)), s.z);
    wind = mul(unity_WorldToObject, wind);
    
    float4 quaternion = rotation(wind);
           vert = rotateVector(quaternion, vert);
           vert += root;
           vert.y *= 1 - s.z * .75;
    
    o.normal = rotateVector(quaternion, v.normal * side);
    o.normal.y *= 1 + s.z * 1.333333333;
    o.normal = normalize(o.normal);
    
    float shadex = s.z;
    float shadey = saturate(o.normal.y) * .5 + .5;
    float shadez = (saturate(vert.y * .75) * .95 + .05);
    
    o.shading = float4(shadex, shadey, shadez, length(wRoot - _WorldSpaceCameraPos));
    o.color   = lerp(_Color2, _Color, pow(o.normal.y * .5 + .5, 2));
    
    //float hill = tex2Dlod(_Noise, float4(wRoot.xz * .00035 + .1, 0, 0)).x * 40;
    //vert += float3(0, hill, 0);
    o.wPos     = mul(unity_ObjectToWorld, float4(vert, 1)).xyz;
    o.vertex = UnityObjectToClipPos(float4(vert, 1));
    
    return o;
}


v2f vertblock (appdata v)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    float3 wRoot = mul(unity_ObjectToWorld, v.vertex).xyz;
    float3 wind = normalize(float3(1, 0, .65));
    
    float col  = tex2Dlod(_Noise, float4(wRoot.xz * .0135  + wind.xz * _Time.y * .015, 0, 0)).x * .75 +
                 tex2Dlod(_Noise, float4(wRoot.xz * .0125  + wind.xz * _Time.y * .0298, 0, 0)).x * .25;
    float col2 = tex2Dlod(_Noise, float4(wRoot.xz * .235  + wind.xz * _Time.y * .035, 0, 0)).x;
    
    float3 vert    = v.vertex.xyz;
           vert.y *= 1.55 + (1 - pow(1 - (sin(wRoot.x * 3) * .5 + .5), 4)) * .2;
    
    float amount = (sin(_Time.y * .5 + col * 6) * .5 + .5) * .35 + (sin(_Time.y * 2 + col2 * 4) * .5) * .2;
    
           wind = normalize(float3(wind.x * amount, .5, wind.z * amount));
           
    float2 canvasUV = (wRoot.xz + float2(CanvasSize, CanvasSize) * .5) / CanvasSize;
    float4 s = pow(tex2Dlod(_Stamp, float4(canvasUV, 0, 0)), 1.0 / 2.2);
    wind = lerp(wind, float3(-(s.x * 2 - 1), 0, -(s.y * 2 - 1)), s.z);
    wind = mul(unity_WorldToObject, wind);
    
    float4 quaternion = rotation(wind);
    float3 n = rotateVector(quaternion, float3(0, 1, 0));
    float3 vert2 = n * vert.y;
           vert2.y *= 1 - s.z * .75;
           
    vert.y = vert2.y;
    
    o.normal = n;
    float ny = 1 - n.y;
    
    float shadex = s.z;
    float shadey = saturate(ny) * .5 + .5;
    float shadez = (saturate(vert.y * .75) * .95 + .05);
    
    o.shading = float4(shadex, shadey, shadez, length(wRoot - _WorldSpaceCameraPos));
    o.color   = lerp(_Color2, _Color, pow(ny * .5 + .5, 2));
    
    //float hill = tex2Dlod(_Noise, float4(wRoot.xz * .00035 + .1, 0, 0)).x * 40;
    //vert += float3(0, hill, 0);
    o.wPos     = mul(unity_ObjectToWorld, float4(vert, 1)).xyz;
    o.vertex = UnityObjectToClipPos(float4(vert, 1));
    
    return o;
}


fixed4 Result(v2f i)
{
    float2 canvasUV = (i.wPos.xz + float2(CanvasSize, CanvasSize) * .5) / CanvasSize;
    /*float shadow = tex2D(_Stamp, canvasUV + float2(.003, .003)).z * .25 + 
                   tex2D(_Stamp, canvasUV + float2(.0025, .00275)).z * .25 +
                   tex2D(_Stamp, canvasUV + float2(.0025, .00225)).z * .25 +
                   tex2D(_Stamp, canvasUV + float2(.0025, .00225)).z * .25;
          shadow = 1 - pow(1 - shadow, 4);*/
    float shadow = 0;
    
    
    float4 shade   = i.shading;
    float4 result  = i.color;
           result *= 1 - pow(saturate(shade.a * .0325), 2) * .835f;
           result *= shade.z * shade.y + shade.x * (.075 + shadow * .075);
           
    return pow(result, 1.5) * 4;
}


fixed4 ResultBlock(v2f i)
{
    float2 canvasUV = (i.wPos.xz + float2(CanvasSize, CanvasSize) * .5) / CanvasSize;
    /*float shadow = tex2D(_Stamp, canvasUV + float2(.003, .003)).z * .25 + 
                   tex2D(_Stamp, canvasUV + float2(.0025, .00275)).z * .25 +
                   tex2D(_Stamp, canvasUV + float2(.0025, .00225)).z * .25 +
                   tex2D(_Stamp, canvasUV + float2(.0025, .00225)).z * .25;
          shadow = 1 - pow(1 - shadow, 4);*/
    float shadow = 0;
    
    
    float4 shade   = i.shading;
    float4 result  = i.color;
           result *= 1 - pow(saturate(shade.a * .0325), 2) * .835f;
           result *= shade.x * (.075 + shadow * .075);
           
    return pow(result, 1.5) * 4;
}