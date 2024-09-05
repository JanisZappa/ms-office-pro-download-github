Shader "Unlit/linkbro"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Focus("Focus", Range(0, 1)) = 0
        _FocusMax("FocusMax", Float) = 0
    }
    SubShader
    {
        Tags { "RenderQueue"="AlphaTest" "RenderType"="10000" }
        AlphaToMask On
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile GRID_OFF GRID_ON
            #pragma multi_compile FOG_OFF  FOG_ON
            #pragma multi_compile BLUR_OFF BLUR_ON
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 wPos   : TEXCOORD1;
                float scale : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Focus, _FocusMax;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = v.uv;
                o.wPos   = mul(unity_ObjectToWorld, v.vertex);
                o.scale = 1.0 / (1.0 + v.uv2.x * .2);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float  dist   = length(camDir);
                
                //float f = .5 + sin(_Time.y) * (_Focus - .5);
                float focusPlane = _FocusMax * max( 1e-5, _Focus);
                float focus = lerp(1.0 - saturate(dist / focusPlane), (1.0 - saturate((_FocusMax - dist) / max(1e-5, _FocusMax - focusPlane))) * 2,  step(focusPlane, dist));
                      focus = focus * 30 * i.scale;
                      
                #if BLUR_OFF
                focus = 0;
                #endif
                
                float2 boxSize  = clamp(fwidth(i.uv) * _MainTex_TexelSize.zw * (1 + focus), 1e-5, 1.0);
                float2 tx       = i.uv * _MainTex_TexelSize.zw - .5 * boxSize; 
                float2 txOffset = saturate((frac(tx) - (1.0 - boxSize)) / boxSize);
                float2 uv       = (floor(tx) + .5 + txOffset) * _MainTex_TexelSize.xy;
                float4 col      = tex2Dgrad(_MainTex, uv, ddx(i.uv), ddy(i.uv));
                //col.xyz = .35;
                
            //  GridLines  //
                #if GRID_ON
                col.xyz *= .915 + .085 * pow(saturate(1.0 - max(txOffset.x, txOffset.y)), 100);
                #endif
                
            //  Proximity  //   
                col.xyz *= .5 + .5 * saturate(dist * 2);
                
                //col.xyz *= pow(1.0 - saturate(dist * .0125), 2);
                col.xyz *= .35 + .65 * (1.0 - pow(1.0 - tex2D(_MainTex, i.uv).a, 2));
                
            //  Fog  //
                #if FOG_ON
                col.xyz = lerp(col.xyz, 1.0, min(.25, 1.0 - pow(1.0 - pow(saturate(dist * .005), 2), 2)));
                #endif
                
            //  Final Adjust  //
                col.xyz = pow(col.xyz, 1.25) * 1.5;
                
                return col;
            }
            ENDCG
        }
    }
}
