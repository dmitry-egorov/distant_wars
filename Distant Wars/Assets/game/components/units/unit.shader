Shader "DW/Unit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 0, 0, 1)
        [KeywordEnum(Default, Highlighted, Selected)] _Mode ("Mode", Float) = 0
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
            
            #pragma multi_compile_local DEFAULT HIGHLIGHTED SELECTED

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
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed col = tex2D(_MainTex, i.uv);
                if (col > 0.75)
                    discard;
                    
                #ifdef DEFAULT
                    float4 icon_color = float4(0,0,0,1);
                    float4 border_color = float4(0,0,0,1);
                #endif
                #ifdef HIGHLIGHTED
                    float4 icon_color = float4(1,1,1,1);
                    float4 border_color = float4(0,0,0,1);
                #endif
                #ifdef SELECTED
                    float4 icon_color = float4(1,1,1,1);
                    float4 border_color = float4(1,1,1,1);
                #endif
                
                return step(col, 0.25) * icon_color + step(col, 0.5) * border_color + step(0.5, col) * _Color;
            }
            ENDCG
        }
    }
}
