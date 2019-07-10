Shader "DW/Map"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TopLandColor ("Top Land Color", Color) = (1,1,1,1)
        _BottomLandColor ("Bottom Land Color", Color) = (1,1,1,1)
        _ShallowSeaColor ("Sea Color", Color) = (1,1,1,1)
        _DeepSeaColor ("Deep Sea Color", Color) = (1,1,1,1)
        _ShoreHighColor ("Shore High Color", Color) = (1,1,1,1)
        _ShoreLowColor ("Shore Low Color", Color) = (1,1,1,1)
        _LandColorGamma ("Land Color Gamma", Range(0, 2)) = 1
        _ShoreColorGamma ("Shore Color Gamma", Range(0, 2)) = 1
        _HeightRange ("Height Range", Range(0, 1)) = 1
        _SeaLevel ("Sea Level", Range(0, 1)) = 0.01
        _ShoreWidth ("Sea Level", Range(0, 1)) = 1
        _ShoreBlend ("Shore Blend", Range(0, 0.01)) = 0.005
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
            float4 _ShoreLowColor;
            float4 _ShoreHighColor;
            float _LandColorGamma;
            float _ShoreColorGamma;
            float _HeightRange;
            float _SeaLevel;
            float _ShoreWidth;
            float _ShoreBlend;
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
            float remap01(float v, float min, float max) { return (v - min) / (max - min); }
            
            float border(float2 uv)
            {
                float2 ts = _MainTex_TexelSize.xy;
                return step(ts.x, uv.x) * step(ts.x, 1 - uv.x) * step(ts.y, uv.y) * step(ts.y, 1 - uv.y);  
            }
            
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
            
            float shore_level()
            {
                float  /* sea level */   sl = _SeaLevel;
                float  /* lines bands */ bs = _LinesBands;
                return sl + _ShoreWidth / bs;
            }
            
            float is_shore(float2 uv)
            {
                float /* sea level */    sl = _SeaLevel;
                float /* sea blend */    sb = _SeaBlend;
                float /* shore blend */ shb = _ShoreBlend;
                float /* height */        h = height_at(uv);
                float /* shore level */ shl = shore_level();
                
                return sstep(sl, h, sb) - sstep(shl, h, shb);
            }
            
            float is_land(float2 uv)
            {
                float /* height */ h = height_at(uv);
                float /* shore level */ shl = shore_level();
                
                return sstep(shl, h, _ShoreBlend);
            }
            
            float topo_line
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
            
            float topo_line(float2 uv)
            {
                float /* texel width */       tx = _MainTex_TexelSize.x;
                float /* texel heigth */      ty = _MainTex_TexelSize.y;
                float /* texture width */     tw = _MainTex_TexelSize.z;
                float /* lines strength */    ls = _LineStrength;
                float /* lines width */       lw = 0.3 * _LineWidth;
                float /* height */             h = height_at(uv);
                float /* uv derivative */    duv = fwidth(uv);
                float /* height derivative */ dh = fwidth(h);
                
                float /* adjusted line width */ alw = tx * lw * dh * tw / pow(duv * tw, ls);
                
                float /* sea level */              sl = _SeaLevel;
                float /* height above sea level */ ah = h - sl; 
                
                float /* main lines bands */     mlb = _LinesBands;
                float /* main lines intensity */ mli = _LinesIntensity;
                float /* main line color */      mlc = topo_line(ah, mlb, mli, alw);
                
                float /* sec lines bands */      slb = _LinesBands * _LinesSecondaryBands;
                float /* sec lines intensity */  sli = _LinesIntensity * _LinesSecondaryIntensity;
                float /* sec line color */       slc = topo_line(ah, slb, sli, alw);
                
                float /* line color */ lc = mlc * slc;
                
                float /* is land */ il = is_land(uv);
                lc = lerp(0.5 * (lc + 1), lc, il); // dimmer lines under the sea
                
                return lc;
            }
            
            float3 terrain(float2 uv)
            {
                float3 /* top land color */    tlc = _TopLandColor;
                float3 /* bottom land color */ blc = _BottomLandColor;
                float3 /* deep sea color */    dsc = _DeepSeaColor;
                float3 /* shallow sea color */ ssc = _ShallowSeaColor;
                float3 /* shore high color */ shhc = _ShoreHighColor;
                float3 /* shore low color */  shlc = _ShoreLowColor;
                float  /* sea level */          sl = _SeaLevel;
                float  /* land gamma */         lg = _LandColorGamma;
                float  /* shore gamma */       shg = _ShoreColorGamma;
                float  /* lines bands */        bs = _LinesBands;
                
                
                float  /* shore level */  shl = shore_level();
                float  /* height */         h = height_at(uv);
                float3 /* sea color */     sc = lerp(dsc, ssc, saturate(remap01(h, 0, sl)));
                float3 /* shore color */  shc = lerp(shlc, shhc, pow(saturate(remap01(h, sl, shl)), shg));   
                float3 /* land color */   ldc = lerp(blc, tlc, pow(saturate(remap01(h, shl, 1)), lg));
                
                float  /* is shore */     ish = is_shore(uv);
                float  /* is land */       il = is_land(uv);
                float3 /* terrain color */ tc = lerp(lerp(sc, shc, ish), ldc, il);
                
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
                float  /* border */        bc = border(uv);
                float  /* line color */    lc = topo_line(uv);
                float3 /* terrain color */ tc = terrain(uv);
                float  /* shadow */        sh = shadow(uv); 
                
                return float4(bc * tc * lc * sh, 1);
            }
            ENDCG
        }
    }
}
