Shader "Custom/SpriteHitFlash"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _Color ("Tint", Color) = (1,1,1,1)

        [HDR] _FlashColor ("Flash HDR Color", Color) = (3,3,3,1)

        _FlashAmount ("Flash Amount", Range(0,1)) = 0

        [MaterialToggle] PixelSnap ("Pixel Snap", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile _ PIXELSNAP_ON

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            half4 _FlashColor;
            half _FlashAmount;

            v2f vert(appdata input)
            {
                v2f output;

                output.vertex = UnityObjectToClipPos(input.vertex);
                output.uv = input.uv;
                output.color = input.color * _Color;

                #ifdef PIXELSNAP_ON
                    output.vertex = UnityPixelSnap(output.vertex);
                #endif

                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                fixed4 textureColor = tex2D(_MainTex, input.uv);
                fixed4 originalColor = textureColor * input.color;

                // 只修改有像素的部分，保留 Sprite 原始透明度
                half3 flashRGB = _FlashColor.rgb;

                half3 finalRGB = lerp(
                    originalColor.rgb,
                    flashRGB,
                    saturate(_FlashAmount)
                );

                return fixed4(finalRGB, originalColor.a);
            }

            ENDCG
        }
    }

    FallBack "Sprites/Default"
}