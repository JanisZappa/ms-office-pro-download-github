Shader "Unlit/ATestInst"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormalMap ("Normal", 2D) = "white" {}
        _DetailMap ("Details", 2D) = "white" {}
        _LightMap ("LightMap", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderQueue"="AlphaTest" "RenderType"="TransparentCutout" }
        Cull Off
        
        Pass
        {
            AlphaToMask On
            
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
                float4 pos   : SV_POSITION;
                float2 uv    : TEXCOORD0;
                float3 wPos  : TEXCOORD1;
                float4 range : TEXCOORD2;
                float selected : TEXCOORD3;
            };
            
            sampler2D _MainTex, _NormalMap, _DetailMap, _LightMap;
            uint Selection;
            
            
            v2f vert (appdata v, uint instanceID: SV_InstanceID)
            {
                v2f o;
                
                ScatterObject sO = RenderBuffer[instanceID];
                float2 pos = sO.pos.xz;
                    
                ItemInfo i = ItemInfoBuffer[sO.id];
                
                float4 vert = v.vertex;
                float u = vert.x;
                
                o.uv = float2(lerp(i.uvMin.x, i.uvMax.x, lerp(1.0 - vert.x, vert.x, sO.flip)), 
                              lerp(i.uvMin.y, i.uvMax.y, vert.y));
                
                vert = float4(lerp(i.qMax.x, i.qMin.x, vert.x) * (1 - sO.flip * 2), lerp(i.qMin.y, i.qMax.y, vert.y), 0, 1);
                vert.xy *= sO.scale;
                
                o.wPos = float3(vert.x + pos.x, 0, pos.y);
                vert += float4(pos.xy, -vert.y, 0);
                
                o.pos = UnityObjectToClipPos(vert);
                o.range = float4(i.yRange, i.zRange) * sO.scale;
                o.selected = step(abs(instanceID - Selection), .5);
                
                return o;
            }
            
            
            fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float alpha = col.a;
                
                clip(alpha - .25);
                
                alpha = (alpha - .5) / max(fwidth(alpha), .0001) + .5;
                
                
                fixed4 nor = tex2D(_NormalMap, i.uv);
                fixed4 dtl = tex2D(_DetailMap, i.uv);
                fixed4 lgt = tex2D(_LightMap, i.uv);
                
                float3 n = normalize(nor.xyz * 2.0 - 1.0);
                
                float3 wPos = i.wPos;
                wPos.y += lerp(i.range.x, i.range.y, dtl.a);
                wPos.z += lerp(i.range.z, i.range.w, nor.a);
                
                //alpha = min(alpha, 1 - ClipIt(wPos));
                
                float select = 0;//min(pow(1 - dtl.x, 1.5), .65) * i.selected * (sin(_Time.y * 8 + wPos.y * 25) * .35 + .65);
                return float4((Result(col.xyz, n, float4(dtl.xyz, lgt.x), wPos, lgt, 1) + select * .175) * (1 +  select * 2.5), alpha);
            }
            ENDCG
        }
    }
}