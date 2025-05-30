Shader "Custom/PixelatedImage200WithColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelSize ("Pixel Size", Range(1, 500)) = 10
        _Color1 ("Color 1", Color) = (1, 1, 1, 1)
        _Color2 ("Color 2", Color) = (0.75, 0.75, 0.75, 1)
        _Color3 ("Color 3", Color) = (0.5, 0.5, 0.5, 1)
        _Color4 ("Color 4", Color) = (0.25, 0.25, 0.25, 1)
        _Color5 ("Color 5", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        Cull Off Lighting Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PixelSize;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float4 _Color4;
            float4 _Color5;
            
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // UV座標をピクセルグリッドにスナップ
                float2 pixelatedUV = floor(i.uv * _PixelSize) / _PixelSize;
                fixed4 col = tex2D(_MainTex, pixelatedUV);
                col *= i.color; // UIの色を適用
                
                // グレースケール変換
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                
                // 5色の中で最も近い色を選択
                float dist1 = abs(gray - _Color1.r);
                float dist2 = abs(gray - _Color2.r);
                float dist3 = abs(gray - _Color3.r);
                float dist4 = abs(gray - _Color4.r);
                float dist5 = abs(gray - _Color5.r);
                
                if (dist1 < dist2 && dist1 < dist3 && dist1 < dist4 && dist1 < dist5)
                    col.rgb = _Color1.rgb;
                else if (dist2 < dist3 && dist2 < dist4 && dist2 < dist5)
                    col.rgb = _Color2.rgb;
                else if (dist3 < dist4 && dist3 < dist5)
                    col.rgb = _Color3.rgb;
                else if (dist4 < dist5)
                    col.rgb = _Color4.rgb;
                else
                    col.rgb = _Color5.rgb;
                
                return col;
            }
            
            ENDCG
        }
    }
}
