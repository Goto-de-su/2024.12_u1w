Shader "Custom/SoftMaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radius", Float) = 0.3
        _Softness ("Softness", Float) = 0.1
        _MaskColor ("Mask Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Center;
            float _Radius;
            float _Softness;
            float4 _MaskColor;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t vertex)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(vertex.vertex);
                output.uv = vertex.uv;
                return output;
            }

            fixed4 frag (v2f input) : SV_Target
            {
                // UV座標を取得
                float2 uv = input.uv;

                // 中心からの距離を計算
                float dist = distance(uv, _Center.xy);

                // 円形マスクを作成
                float alpha = smoothstep(_Radius + _Softness, _Radius, dist);

                // マスク部分の色を設定
                fixed4 color = lerp(_MaskColor, tex2D(_MainTex, uv), alpha);

                // アルファ値を適用
                color.a = 1.0 - alpha;

                return color;
            }
            ENDCG
        }
    }
}
