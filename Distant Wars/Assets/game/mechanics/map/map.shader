Shader "DW/Map"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TopLandColor ("Top Land Color", Color) = (1,1,1,1)
        _BottomLandColor ("Bottom Land Color", Color) = (1,1,1,1)
        _ShallowSeaColor ("Sea Color", Color) = (1,1,1,1)
        _DeepSeaColor ("Deep Sea Color", Color) = (1,1,1,1)
        _ColorGamma ("Color Gamma", Range(0, 2)) = 1
        _HeightRange ("Height Range", Range(0, 1)) = 1
        _SeaLevel ("Sea Level", Range(0, 1)) = 0.01
        _SeaBlend ("Sea Blend", Range(0, 0.01)) = 0.005
        _LinesIntensity ("Lines Intensity", Range (0,1)) = 0.5
        _LinesSecondaryIntensity ("Lines Secondary Intensity", Range (0,1)) = 0.2
        _LinesBands ("Lines Bands", Range (0,100)) = 10
        _LinesSecondaryBands ("Lines Secondary Bands", Range (0,10)) = 5
        _LineWidth ("Line Width", Range (0,10)) = 1
        _LineStrength ("Line Strength", Range (0,2)) = 1
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
            float4 _ShallowSeaColor;
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
            float _ShadowIntensity;
            
            float sqr(float v) { return v * v; }
            float sstep(float min, float max, float width) { return smoothstep(min - width, min + width, max); }
            
            float simple_texture_fetch(float2 uv) { return tex2D(_MainTex, uv); }
            
            float precise_texture_fetch(float2 uv)
            {
                //Note: from Inigo Quilez at https://www.iquilezles.org/www/articles/hwinterpolation/hwinterpolation.htm
                float2 res = _MainTex_TexelSize.zw;
            
                float2 st = uv*res - 0.5;
            
                float2 iuv = floor(st);
                float2 fuv = frac(st);
            
                float a = tex2D(_MainTex, (iuv+float2(0.5,0.5))/res);
                float b = tex2D(_MainTex, (iuv+float2(1.5,0.5))/res);
                float c = tex2D(_MainTex, (iuv+float2(0.5,1.5))/res);
                float d = tex2D(_MainTex, (iuv+float2(1.5,1.5))/res);
            
                return lerp(lerp(a, b, fuv.x), lerp(c, d, fuv.x), fuv.y);
            }
            
            float height_at(float2 uv)
            {
                //float v = simple_texture_fetch(uv);
                float h = precise_texture_fetch(uv);
                return saturate(h / _HeightRange);
            }
            
            float is_land(float2 uv)
            {
                float /* height */ h = height_at(uv);
                return sstep(_SeaLevel, h, _SeaBlend);
            }
            
            float line_color
            (
                float /* height */ h, 
                float /* line bands */ lb, 
                float /* line intensity*/ li, 
                float /* line width */ lw
            )
            {
                float /* height fraction */ hf = (frac(h * lb + 0.5) - 0.5) / lb;
                float /* line color */ lc = 1 - li * smoothstep(0, abs(hf), lw);
                
                return lc;
            }
            
            float line_color(float2 uv)
            {
                float /* texel width */       tx = _MainTex_TexelSize.x;
                float /* texel heigth */      ty = _MainTex_TexelSize.y;
                float /* texture width */     tw = _MainTex_TexelSize.z;
                float /* lines strength */    ls = _LineStrength;
                float /* lines width */       lw = _LineWidth;
                float /* height */             h = height_at(uv);
                float /* uv derivative */    duv = fwidth(uv);
                float /* height derivative */ dh = fwidth(h);
                
                float /* adjusted line width */ alw = 0.3 * tx * lw * dh * tw / pow(duv * tw, ls);
                
                float /* sea level */ sl = _SeaLevel;
                float /* height above sea level */ ah = h - sl; 
                
                float /* main lines bands */     mlb = _LinesBands;
                float /* main lines intensity */ mli = _LinesIntensity;
                float /* main line color */      mlc = line_color(ah, mlb, mli, alw);
                
                float /* sec lines bands */      slb = _LinesBands * _LinesSecondaryBands;
                float /* sec lines intensity */  sli = _LinesIntensity * _LinesSecondaryIntensity;
                float /* sec line color */       slc = line_color(ah, slb, sli, alw);
                
                float /* line color */ lc = mlc * slc;
                
                float /* is land */ il = is_land(uv);
                lc = lerp(0.5 * (lc + 1), lc, il); // dimmer lines under the sea
                
                return lc;
            }
            
            float3 terrain_color(float2 uv)
            {
                float3 /* top land color */    tlc = _TopLandColor;
                float3 /* bottom land color */ blc = _BottomLandColor;
                float3 /* deep sea color */    dsc = _DeepSeaColor;
                float3 /* shallow sea color */ ssc = _ShallowSeaColor;
                float  /* sea level */          sl = _SeaLevel;
                float  /* color gamma */        cg = _ColorGamma;
                
                float  /* height */         h = height_at(uv);
                float  /* is land */       il = is_land(uv);
                float3 /* land color */   ldc = lerp(blc, tlc, pow(saturate((h - sl) / (1 - sl)), cg));
                float3 /* sea color */     sc = lerp(dsc, ssc, saturate(h / sl));
                float3 /* terrain color */ tc = lerp(sc, ldc, il);
                
                return tc;
            }
            
            float shadow(float2 uv)
            {
                float tx = _MainTex_TexelSize.x;
                float ty = _MainTex_TexelSize.y;
                
                float /* shadow's intensity */   si = _ShadowIntensity;
                float /* top left height */     htl = height_at(uv + float2(-tx, +ty));
                float /* bottom right height */ hbr = height_at(uv + float2(+tx, -ty));
                float /* shadow */               sh = 1 - si * sstep(hbr, htl, 0.001);
                
                return sh;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                float  /* line color */    lc = line_color(uv);
                float3 /* terrain color */ tc = terrain_color(uv);
                float  /* shadow */        sh = shadow(uv); 
                
                return float4(tc * lc * sh, 1);
            }
            ENDCG
        }
    }
}
