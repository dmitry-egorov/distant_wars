Shader "DW/Unit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 0, 0, 1)
        [Toggle(WHITE_ICON)] _WhiteIcon ("White Icon", Float) = 0
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
            
            #pragma shader_feature WHITE_ICON

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
                if (col > 0.9)
                    discard;
                #ifdef WHITE_ICON
                    float4 icon_color = float4(1,1,1,1);
                #else
                    float4 icon_color = float4(0,0,0,1);
                #endif
                return step(col, 0.4) * icon_color + step(0.4, col) * _Color;
            }
            ENDCG
        }
    }
}
