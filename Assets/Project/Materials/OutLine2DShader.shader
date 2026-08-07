Shader "Custom/Outline2DShader"
{
    Properties
    {
        [MainTexture] _MainTex("Sprite Texture", 2D) = "white" {}
        [HDR] _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineThickness("Outline Thickness", Range(0, 10)) = 2
    }

    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent"
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize; // Unity 6 автоматически передает сюда размер пикселя текстуры

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _OutlineThickness;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.color = IN.color;
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Считываем оригинальный цвет пикселя спрайта юнита
                half4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;
                
                // Если пиксель непрозрачный, выводим сам спрайт юнита
                if (mainColor.a > 0.1)
                {
                    return mainColor;
                }

                // Вычисляем шаг смещения для поиска краев на основе толщины
                float2 thickness = _MainTex_TexelSize.xy * _OutlineThickness;

                // Сканируем 4 соседних пикселя вокруг текущего прозрачного места
                float alphaUp    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(0, thickness.y)).a;
                float alphaDown  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv - float2(0, thickness.y)).a;
                float alphaRight = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(thickness.x, 0)).a;
                float alphaLeft  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv - float2(thickness.x, 0)).a;

                // Если сам пиксель прозрачный, но рядом есть непрозрачный — это внешняя граница!
                if (alphaUp + alphaDown + alphaRight + alphaLeft > 0.1)
                {
                    return _OutlineColor;
                }

                return mainColor;
            }
            ENDHLSL
        }
    }
}
