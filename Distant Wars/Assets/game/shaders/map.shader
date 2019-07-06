Shader "DW/Map"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TopLandColor ("Top Land Color", Color) = (1,1,1,1)
        _BottomLandColor ("Bottom Land Color", Color) = (1,1,1,1)
        _SeaColor ("Sea Color", Color) = (1,1,1,1)
        _DeepSeaColor ("Deep Sea Color", Color) = (1,1,1,1)
        _ColorGamma ("Color Gamma", Range(0, 2)) = 1
        _HeightRange ("Height Range", Range(0, 1)) = 1
        _SeaLevel ("Sea Level", Range(0, 1)) = 0.01
        _SeaBlend ("Sea Blend", Range(0, 0.01)) = 0.005
        _LinesIntensity ("Lines Intensity", Range (0,1)) = 0.5
        _LinesSecondaryIntensity ("Lines Secondary Intensity", Range (0,1)) = 0.2
        _LinesBands ("Lines Bands", Range (0,100)) = 10
        _LinesSecondaryBands ("Lines Secondary Bands", Range (0,10)) = 5
        _LineWidth ("Line Width", Range (0,0.05)) = 1
        _LineStrength ("Line Strength", Range (0,2)) = 1
        _ShadowRange ("Shadow Range", Range(0, 1)) = 0.001
        _ShadowIntensity ("Shadow Intensity", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _TopLandColor;
            float4 _BottomLandColor;
            float4 _SeaColor;
            float4 _DeepSeaColor;
            float _ColorGamma;
            float _HeightRange;
            float _SeaLevel;
            float _SeaBlend;
            float _LinesIntensity;
            float _LinesSecondaryIntensity;
            float _LinesBands;
            float _LinesSecondaryBands;
            float _LineWidth;
            float _LineStrength;
            float _ShadowRange;
            float _ShadowIntensity;
            
            float height_at(float2 uv)
            {
                return saturate(tex2D(_MainTex, uv) / _HeightRange);
            }
            
            float line_color
            (
                float /* height */ h, 
                float /* line bands */ lb, 
                float /* line intensity*/ li, 
                float /* line width */ lw
            )
            {
                float /* sea level */       sl = _SeaLevel;
                float /* height fraction */ hf = (frac((h - sl) * lb + 0.5) - 0.5) / lb;
                float /* line color */      lc = 1 - li * smoothstep(0, abs(hf), lw);
                
                return lc;
            }
            
            float line_color(float2 uv)
            {
                float /* texture width */         tw = _MainTex_TexelSize.z;
                float /* lines strength */        ls = _LineStrength;
                float /* lines width */           lw = _LineWidth;
                float /* height */                 h = height_at(uv); // NOTE: assuming this would be optimized
                float /* uv derivative */        duv = fwidth(uv) * tw * 10; // magic number
                float /* height derivative */     dh = fwidth(h) * tw;
                float /* adjusted line width */  alw = lw * dh / pow(duv, ls);
                
                float /* main lines bands */     mlb = _LinesBands;
                float /* main lines intensity */ mli = _LinesIntensity;
                float /* main line color */      mlc = line_color(h, mlb, mli, alw);
                
                float /* sec lines bands */      slb = _LinesBands * _LinesSecondaryBands;
                float /* sec lines intensity */  sli = _LinesIntensity * _LinesSecondaryIntensity;
                float /* sec line color */       slc = line_color(h, slb, sli, alw);
                
                return mlc * slc;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float tx = _MainTex_TexelSize.x;
                float ty = _MainTex_TexelSize.y;
                
                
                float /* line color */ lc = line_color(i.uv); 
                
                float  /* height */         h = height_at(i.uv);
                float  /* is land */       il = smoothstep(_SeaLevel - _SeaBlend, _SeaLevel + _SeaBlend, h);
                float4 /* land color */   ldc = lerp(_BottomLandColor, _TopLandColor, pow(saturate((h - _SeaLevel) / (1 - _SeaLevel)), _ColorGamma));
                float4 /* sea color */     sc = lerp(_DeepSeaColor, _SeaColor, saturate(h / _SeaLevel));
                float4 /* terrain color */ tc = lerp(sc, ldc, il);
                
                float /* top left height */     htl = height_at(i.uv + float2(-tx, +ty));
                float /* bottom right height */ hbr = height_at(i.uv + float2(+tx, -ty));
                float /* shadow */               sh = 1 - _ShadowIntensity * step(hbr, htl);
                
                lc = lerp(0.5 * (lc + 1), lc, il); // brighter lines under the sea 
                
                return tc * lc * sh;
            }
            ENDCG
        }
    }
}
