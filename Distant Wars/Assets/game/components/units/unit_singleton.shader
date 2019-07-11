Shader "DW/Unit Singleton"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 0, 0, 1)
        _Size ("Size", Float) = 1
        
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 flags: TEXCOORD1;// x - is highlighted, y - is selected
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Size;

            v2f vert (appdata d)
            {
                v2f o;
                float2 v = d.vertex.xy;
                int flags = d.vertex.z;
                bool /* is highlighted */ ih = fmod(flags, 2) == 1;
                bool    /* is selected */ is = fmod(flags, 4) >= 2;
                
                o.flags.x = ih ? 1.0 : 0.0;
                o.flags.y = is ? 1.0 : 0.0;
                
                float /* offset x */ ox = fmod(flags,  8) >= 4 ? 0.5 : -0.5;
                float /* offset y */ oy = fmod(flags, 16) >= 8 ? 0.5 : -0.5;
                float /* offset z */ oz = (ih ? -1.0 : 0.0) + (is ? -1.0 : 0.0);
                float4 ov = UnityObjectToClipPos(float3(v.x, v.y, oz));
                ov.x = (floor(ov.x * _ScreenParams.x) + _Size * ox) / _ScreenParams.x;  
                ov.y = (floor(ov.y * _ScreenParams.y) + _Size * oy) / _ScreenParams.y;  
                o.vertex = ov;
                
                float2 uv = float2(0.5 + ox, 0.5 - oy);
                o.uv = TRANSFORM_TEX(uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed col = tex2Dlod(_MainTex, float4(i.uv, 0, 0));
                
                if (col > 0.75)
                    discard;
                    
                float ih = i.flags.x;
                float is = i.flags.y;
                float4 ic = lerp(float4(0,0,0,1), float4(1,1,1,1), is);
                float4 bc = lerp(float4(0,0,0,1), float4(1,1,1,1), saturate(ih + is));
                
                return step(col, 0.25) * ic 
                     + (step(col, 0.5 ) - step(col, 0.25)) * bc
                     + (step(col, 0.75) - step(col, 0.5 )) * _Color
                 ;
            }
            ENDCG
        }
    }
    CustomEditor "UnitShaderEditor"
}
