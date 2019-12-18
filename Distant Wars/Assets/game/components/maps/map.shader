Shader "DW/Map"
{
    Properties
    {
        _TopLandColor            ("Top Land Color", Color) = (1,1,1,1)
        _BottomLandColor         ("Bottom Land Color", Color) = (1,1,1,1)
        _ShallowSeaColor         ("Sea Color", Color) = (1,1,1,1)
        _DeepSeaColor            ("Deep Sea Color", Color) = (1,1,1,1)
        _ShoreHighColor          ("Shore High Color", Color) = (1,1,1,1)
        _ShoreLowColor           ("Shore Low Color", Color) = (1,1,1,1)
        _SeaColorGamma           ("Sea Color Gamma", Range(0, 8)) = 1
        _LandColorGamma          ("Land Color Gamma", Range(0, 2)) = 1
        _ShoreColorGamma         ("Shore Color Gamma", Range(0, 2)) = 1
        _HeightRange             ("Height Range", Range(0, 1)) = 1
        _SeaLevel                ("Sea Level", Range(0, 1)) = 0.01
        _ShoreWidth              ("Sea Level", Range(0, 1)) = 1
        _ShoreBlend              ("Shore Blend", Range(0, 0.01)) = 0.005
        _SeaBlend                ("Sea Blend", Range(0, 0.01)) = 0.005
        _LinesIntensity          ("Lines Intensity", Range (0,1)) = 0.5
        _LinesSecondaryIntensity ("Lines Secondary Intensity", Range (0,1)) = 0.2
        _LinesBands              ("Lines Bands", Range (0,100)) = 10
        _LinesSecondaryBands     ("Lines Secondary Bands", Range (0,10)) = 5
        _LineWidth               ("Line Width", Range (0,10)) = 1
        _LineStrength            ("Line Strength", Range (0,2)) = 1
        _ShadowIntensity         ("Shadow Intensity", Range(0, 1)) = 0.5
        _VisionBrightness        ("Vision Brightness", Range(0, 1)) = 0.5
        _VisionSaturation        ("Vision Saturation", Range(0, 1)) = 0.1
        _VisionSharpness         ("Vision Sharpness", Range(0, 16)) = 8
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZWrite Off
        ZTest Off
        ZClip False

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature HIDE_MAP_VISION
            #pragma multi_compile __ SHOW_SPACE_GRID
            #pragma multi_compile __ SHOW_MAP_GRID

            #include "UnityCG.cginc"
            #include "Assets/Plugins/shader_common/shader_common.cginc"

            float4 _TopLandColor;
            float4 _BottomLandColor;
            float4 _ShallowSeaColor;
            float4 _DeepSeaColor;
            float4 _ShoreLowColor;
            float4 _ShoreHighColor;
            float _SeaColorGamma;
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
            float _VisionBrightness;
            float _VisionSaturation;
            float _VisionSharpness;

            sampler2D _MapTex;
            float4 _MapTex_ST;
            float4 _MapTex_TexelSize;
            sampler2D _VisionTex;
            float4 _VisionTex_ST;
            float4 _VisionTex_TexelSize;
            sampler2D _DiscoveryTex;
            float4 _DiscoveryTex_ST;
            float4 _DiscoveryTex_TexelSize;

            float2 _GridCellSize;
            
            float border(float2 uv)
            {
                float2 ts = _MapTex_TexelSize.xy;
                return step(ts.x, uv.x) * step(ts.x, 1 - uv.x) * step(ts.y, uv.y) * step(ts.y, 1 - uv.y);  
            }
            
            float filtered_texture_fetch(float2 uv, sampler2D tex, float2 res)
            {
                //Note: from Inigo Quilez at https://www.iquilezles.org/www/articles/hwinterpolation/hwinterpolation.htm
                float2 st = uv*res - 0.5;
            
                float2 iuv = floor(st);
                float2 fuv = frac(st);
            
                float a = tex2D(tex, (iuv+float2(0.5,0.5))/res);
                float b = tex2D(tex, (iuv+float2(1.5,0.5))/res);
                float c = tex2D(tex, (iuv+float2(0.5,1.5))/res);
                float d = tex2D(tex, (iuv+float2(1.5,1.5))/res);
            
                return lerp(lerp(a, b, fuv.x), lerp(c, d, fuv.x), fuv.y);
            }

            float filtered_main_texture_fetch(float2 uv)
            {
                return filtered_texture_fetch(uv, _MapTex, _MapTex_TexelSize.zw);
            }

            float filtered_vision_texture_fetch(float2 uv)
            {
                return filtered_texture_fetch(uv, _VisionTex, _VisionTex_TexelSize.zw);
            }

            float filtered_discovery_texture_fetch(float2 uv)
            {
                return filtered_texture_fetch(uv, _DiscoveryTex, _DiscoveryTex_TexelSize.zw);
            }
            
            float height_at(float2 uv)
            {
                float h = filtered_main_texture_fetch(uv);
                
                return /* height */ saturate(h / _HeightRange);
            }
            
            float shore_level()
            {
                float  /* sea level */   sl = _SeaLevel;
                float  /* lines bands */ bs = _LinesBands;
                
                return /* shore level */ sl + _ShoreWidth / bs;
            }
            
            float is_shore(float2 uv)
            {
                float /* sea level */    sl = _SeaLevel;
                float /* sea blend */    sb = _SeaBlend;
                float /* shore blend */ shb = _ShoreBlend;
                float /* height */        h = height_at(uv);
                float /* shore level */ shl = shore_level();
                
                return /* is shore */ sstep(sl, h, sb) - sstep(shl, h, shb);
            }
            
            float is_land(float2 uv)
            {
                float /* height */ h = height_at(uv);
                float /* shore level */ shl = shore_level();
                
                return /* is land */ sstep(shl, h, _ShoreBlend);
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
                
                return /* topo line color */ 1 - li * smoothstep(0, abs(hf), lw);
            }
            
            float topo_line(float2 uv)
            {
                float /* texel width */       tx = _MapTex_TexelSize.x;
                float /* texel heigth */      ty = _MapTex_TexelSize.y;
                float /* texture width */     tw = _MapTex_TexelSize.z;
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
                
                // dimmer lines under the sea
                return /* topo line color */ lerp(0.5 * (lc + 1), lc, il);
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
                float  /* sea level */         scg = _SeaColorGamma;
                float  /* land gamma */         lg = _LandColorGamma;
                float  /* shore gamma */       shg = _ShoreColorGamma;
                float  /* lines bands */        bs = _LinesBands;
                
                float  /* shore level */  shl = shore_level();
                float  /* height */         h = height_at(uv);
                float3 /* sea color */     sc = lerp( dsc,  ssc, pow(sat(remap01(h,   0,  sl)), scg));
                float3 /* shore color */  shc = lerp(shlc, shhc, pow(sat(remap01(h,  sl, shl)), shg));   
                float3 /* land color */   ldc = lerp( blc,  tlc, pow(sat(remap01(h, shl,   1)),  lg));
                
                float  /* is shore */     ish = is_shore(uv);
                float  /* is land */       il = is_land(uv);
                
                return /* terrain color */ lerp(lerp(sc, shc, ish), ldc, il);
            }
            
            float shadow(float2 uv)
            {
                float2 ts = _MapTex_TexelSize.xy;
                float  tx = ts.x;
                float  ty = ts.y;
                
                float /* shadow's intensity */   si = _ShadowIntensity;
                float /* top left height */     htl = height_at(uv + float2(-tx, +ty));
                float /* bottom right height */ hbr = height_at(uv + float2(+tx, -ty));
                
                return /* shadow color */ 1 - si * sstep(hbr, htl, 0.001);
            }

            #if SHOW_SPACE_GRID
            float space_grid(/* world position */ float2 wp)
            {
                float2 gp =  wp / _GridCellSize;
                float2 fr = frac(gp);
                float  w  = 0.005;
                //return sstep(w, fr.x, w / 2) * sstep(w, fr.y, w / 2);
                return sstep(w, fr.x, w / 2) 
                     * sstep(w, fr.y, w / 2)
                     * sstep(fr.x, 1 - w, w / 2)
                     * sstep(fr.y, 1 - w, w / 2)
                ;
            }
            #endif
            
            #if SHOW_MAP_GRID
            float map_grid(float2 uv)
            {
                float2  ts = _MapTex_TexelSize.xy;
                float   tx = ts.x;
                float   ty = ts.y;
                float    w = 0.05;
                float2 suv = frac(uv / ts);
                return /* grid color */ 
                1 - 
                (
                      step(0.5 - w, suv.x) 
                    * step(suv.x, 0.5 + w)
                    * step(0.5 - w, suv.y) 
                    * step(suv.y, 0.5 + w)
                );
            }
            #endif

            float3 vision(float /* discovery */ d, float /* vision */ v, float3 /* map color */ mc)
            {
                float  brt = _VisionBrightness;
                float  sat = _VisionSaturation;
                float shrp = 8;//_VisionSharpness;

                float /* color intensity */ ci = dot(float3(0.222, 0.707, 0.071), mc);
                float3 fgc = brt * lerp(float3(ci, ci, ci), mc, sat);
                float3 bgd = float3(0,0,0);

                return lerp(bgd, lerp(fgc,  mc, pow(v, shrp)), pow(max(d, v), shrp));
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0; // texture coordinates
                float2 wp : TEXCOORD1; // world position
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MapTex);
                o.wp = mul(unity_ObjectToWorld, v.vertex).xy;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                float3 /* map color */ mc = 1
                    * border(uv)
                    * topo_line(uv)
                    * terrain(uv)
                    * shadow(uv)
                    #if SHOW_SPACE_GRID
                    * space_grid(i.wp)
                    #endif
                    #if SHOW_MAP_GRID
                    * map_grid(uv)
                    #endif
                ;

                #if !HIDE_MAP_VISION
                    /* vision    */ float v = filtered_vision_texture_fetch(uv).x;
                    /* discovery */ float d = filtered_discovery_texture_fetch(uv).x;
                    
                    if (d == 0.0) discard;
                    
                    mc = vision(d, v, mc);
                #endif
                
                return float4(mc, 1);
            }
            ENDCG
        }
    }
}
