Shader "Unlit/ShadowInst"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWRITE OFF
        
        Pass
        {
            Blend Zero SrcColor
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "GridInc.cginc"
            #pragma multi_compile_instancing 
            #pragma target 4.5
            
            
            struct appdata
            {
                float4 vertex : POSITION;
            };
            
            struct v2f
            {
                float4 pos  : SV_POSITION;
                float2 uv   : TEXCOORD0;
                float3 wPos : TEXCOORD1;
            };
            
            sampler2D _MainTex, _NormalMap, _DetailMap;
            
            v2f vert (appdata v, uint instanceID: SV_InstanceID)
            {
                v2f o;
                
                ScatterObject sO = RenderBuffer[instanceID];
                float2 pos = sO.pos.xz;
                    
                ItemInfo i = ItemInfoBuffer[sO.id];
                
                float4 vert = v.vertex;
                float u = vert.x;
                
                o.uv = float2(lerp(i.uvMin.x, i.uvMax.x, lerp(1.0 - vert.x, vert.x, sO.flip)), 
                              lerp(i.uvMin.y, i.uvMax.y, 1.0 - vert.y));
                
                vert = float4(lerp(i.qMax.x, i.qMin.x, vert.x) * (1 - sO.flip * 2), lerp(i.qMin.y, i.qMax.y, 1.0 - vert.y) * .625, 10, 1);
                vert.xy *= sO.scale;
                
                o.wPos = float3(vert.x + pos.x, 0, pos.y);
                
                vert += float4(pos.xy, 0, 0);
                
                o.pos = UnityObjectToClipPos(vert);
                
                return o;
            }
            
            
            fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                return max(lerp(col.x, 1, saturate(Fog(i.wPos) * .85)), ClipIt(i.wPos));// lerp(col.x, 1, step(.5, _Time.y % 1));
            }
            ENDCG
        }
    }
}