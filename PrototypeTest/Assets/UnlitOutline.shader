Shader "Custom/Sprite-Unlit-Outline"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}

    // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
    [HideInInspector] PixelSnap("Pixel snap", Float) = 0
    [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
    [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
    [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
    [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0

        _Color("MainColor", Color) = (1,1,1,1)
        _AlphaClip("AlphaClip", Range(0,1)) = 0.01
        _OutlineColor("Outline Base Color", Color) = (1,1,1,1)
        _OutlineAlpha("Outline Base Alpha",  Range(0,1)) = 1
        _OutlineGlow("Outline Base Glow", Range(1,100)) = 1.5
        _OutlineWidth("Outline Base Width", Range(0,0.2)) = 0.004
    }

        SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #if defined(DEBUG_DISPLAY)
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/InputData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"
            #endif

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            #pragma multi_compile _ DEBUG_DISPLAY

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                half4   color       : COLOR;
                float2  uv          : TEXCOORD0;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            half4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            half4 _RendererColor;

            half4 _Color;
            half _Alpha;

            half4 _OutlineColor;
            half _OutlineAlpha, _OutlineGlow, _OutlineWidth;

            Varyings UnlitVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(v.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
                #endif
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color * _RendererColor;
                return o;
            }

            half4 UnlitFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                
                half originalAlpha = mainTex.a;

               // half2 destUv = half2(_OutlineWidth * _MainTex_TexelSize.x * 200, _OutlineWidth * _MainTex_TexelSize.y * 200);
                half2 destUv = half2(_OutlineWidth * _MainTex_TexelSize.x * 200, _OutlineWidth * _MainTex_TexelSize.y * 200);


                half spriteLeft = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(destUv.x, 0)).a;
                half spriteRight = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv - half2(destUv.x, 0)).a;
                half spriteBottom = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(0, destUv.y)).a;
                half spriteTop = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv - half2(0, destUv.y)).a;
                half result = spriteLeft + spriteRight + spriteBottom + spriteTop;

                half spriteTopLeft =    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(destUv.x, destUv.y)).a;
                half spriteTopRight =   SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(-destUv.x, destUv.y)).a;
                half spriteBotLeft =    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(destUv.x, -destUv.y)).a;
                half spriteBotRight =   SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(-destUv.x, -destUv.y)).a;
                result = result + spriteTopLeft + spriteTopRight + spriteBotLeft + spriteBotRight;
                result = step(0.05, saturate(result));

                result *= (1 - originalAlpha) * _OutlineAlpha;
                half4 outline = _OutlineColor * i.color.a;
                outline.rgb *= _OutlineGlow;
                outline.a = result;

                mainTex = lerp(mainTex, outline, result);

                #if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;

                InitializeSurfaceData(mainTex.rgb, mainTex.a, surfaceData);
                InitializeInputData(i.uv, inputData);
                SETUP_DEBUG_DATA_2D(inputData, i.positionWS);

                if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
                #endif

                return mainTex;
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue" = "Transparent" "RenderType" = "Transparent"}

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #if defined(DEBUG_DISPLAY)
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/InputData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"
            #endif

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            #pragma multi_compile_fragment _ DEBUG_DISPLAY

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                float4  color           : COLOR;
                float2  uv              : TEXCOORD0;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS      : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _Color;
            half4 _RendererColor;

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(attributes.positionOS);
                #endif
                o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
                o.color = attributes.color * _Color * _RendererColor;
                return o;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                #if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;

                InitializeSurfaceData(mainTex.rgb, mainTex.a, surfaceData);
                InitializeInputData(i.uv, inputData);
                SETUP_DEBUG_DATA_2D(inputData, i.positionWS);

                if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
                #endif

                return mainTex;
            }
            ENDHLSL
        }
    }

        Fallback "Sprites/Default"
}
