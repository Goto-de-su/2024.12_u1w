Shader "Custom/SoftMaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // 表示するステージのテクスチャ
        _Center ("Center Position (UV)", Vector) = (0.5, 0.5, 0, 0) // マスク中心
        _Radius ("Radius", Float) = 0.25 // マスク半径
        _Softness ("Softness", Float) = 0.1 // グラデーションの範囲
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // アルファブレンド設定
            ZWrite Off                     // 深度書き込み無効化
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float2 _Center;
            float _Radius;
            float _Softness;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // テクスチャのカラーを取得
                fixed4 col = tex2D(_MainTex, i.uv);

                // ピクセル位置と中心点の距離を計算
                float dist = distance(i.uv, _Center);

                // グラデーションを計算
                float edgeStart = _Radius;
                float edgeEnd = _Radius + _Softness;

                // 距離に応じて透明から黒へ変化
                float alpha = smoothstep(edgeStart, edgeEnd, dist);

                // 外側は完全に黒く
                if (dist > edgeEnd)
                {
                    return fixed4(0, 0, 0, 1); // 黒
                }

                // マスク内側から外側にかけて透明から黒へのグラデーション
                return fixed4(0, 0, 0, alpha);
            }
            ENDCG
        }
    }
}
