Shader "Custom/TerrainURP"
{
    Properties
    {
        testTexture("Texture", 2D) = "white"{}
        testScale("Scale", Float) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #define MAX_LAYER_COUNT 8
            #define EPSILON 1E-4

            TEXTURE2D_ARRAY(baseTextures);
            SAMPLER(sampler_baseTextures);

            CBUFFER_START(UnityPerMaterial)
                int layerCount;
                float3 baseColours[MAX_LAYER_COUNT];
                float baseStartHeights[MAX_LAYER_COUNT];
                float baseBlends[MAX_LAYER_COUNT];
                float baseColourStrength[MAX_LAYER_COUNT];
                float baseTextureScales[MAX_LAYER_COUNT];
                float minHeight;
                float maxHeight;
                float testScale;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            float inverseLerp(float a, float b, float value)
            {
                return saturate((value - a) / (b - a));
            }

            float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex)
            {
                float3 scaledWorldPos = worldPos / scale;
                float3 xProjection = SAMPLE_TEXTURE2D_ARRAY(baseTextures, sampler_baseTextures, 
                    float2(scaledWorldPos.y, scaledWorldPos.z), textureIndex).rgb * blendAxes.x;
                float3 yProjection = SAMPLE_TEXTURE2D_ARRAY(baseTextures, sampler_baseTextures, 
                    float2(scaledWorldPos.x, scaledWorldPos.z), textureIndex).rgb * blendAxes.y;
                float3 zProjection = SAMPLE_TEXTURE2D_ARRAY(baseTextures, sampler_baseTextures, 
                    float2(scaledWorldPos.x, scaledWorldPos.y), textureIndex).rgb * blendAxes.z;
                return xProjection + yProjection + zProjection;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float heightPercent = inverseLerp(minHeight, maxHeight, input.positionWS.y);
                float3 blendAxes = abs(input.normalWS);
                blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

                float3 albedo = float3(0, 0, 0);

                for (int i = 0; i < layerCount; i++)
                {
                    float drawStrength = inverseLerp(-baseBlends[i] / 2 - EPSILON, 
                        baseBlends[i] / 2, heightPercent - baseStartHeights[i]);

                    float3 baseColour = baseColours[i] * baseColourStrength[i];
                    float3 textureColour = triplanar(input.positionWS, baseTextureScales[i], 
                        blendAxes, i) * (1 - baseColourStrength[i]);

                    albedo = albedo * (1 - drawStrength) + (baseColour + textureColour) * drawStrength;
                }

                // Simple lighting
                Light mainLight = GetMainLight();
                float3 lighting = mainLight.color * mainLight.distanceAttenuation;
                float NdotL = saturate(dot(input.normalWS, mainLight.direction));
                
                float3 finalColor = albedo * lighting * NdotL + albedo * 0.2; // Add ambient

                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
        
        // Shadow caster pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float3 _LightDirection;

            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Lit"
}