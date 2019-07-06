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
        _LinesBands ("Lines Bands", Range (0,100)) = 10
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
            float _LinesBands;
            float _LineWidth;
            float _LineStrength;
            float _ShadowRange;
            float _ShadowIntensity;
            
            float adjust_height(float h)
            {
                return saturate(h / _HeightRange);
            }
            
            float height_at(float2 uv)
            {
                return adjust_height(tex2D(_MainTex, uv));
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
                
                float duv = fwidth(i.uv) * 10000;
                float /* height */ h = height_at(i.uv);
                float /* top left height */ htl = height_at(i.uv + float2(-tx, +ty));
                float /* bottom right height */ hbr = height_at(i.uv + float2(+tx, -ty));
                 
                float  /* height derivative */ dh = fwidth(h) * _MainTex_TexelSize.z;
                float  /* height fraction */   hf = frac((h - _SeaLevel) * _LinesBands);
                float  /* line width */        lw = _LineWidth * dh / pow(duv, _LineStrength);
                float  /* line color */        lc = 1 - _LinesIntensity * (smoothstep(0, hf, lw) + smoothstep(0, 1-hf, lw));
                
                float4 /* land color */    ldc = lerp(_BottomLandColor, _TopLandColor, pow(saturate((h - _SeaLevel) / (1 - _SeaLevel)), _ColorGamma));
                float4 /* sea color */     sc = lerp(_DeepSeaColor, _SeaColor, saturate(h / _SeaLevel));
                float4 /* terrain color */ tc = lerp(sc, ldc, smoothstep(_SeaLevel - _SeaBlend, _SeaLevel + _SeaBlend, h));
                
                float  /* shadow */ sh = 1 - _ShadowIntensity * step(hbr, htl); 
                
                return tc * lc * sh;
            }
            ENDCG
        }
    }
}
