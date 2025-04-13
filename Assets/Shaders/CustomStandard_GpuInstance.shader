// Fixed: SRP Batcher 無効化 + 明示的な Uniform による影対応 + PBR 手動構築
Shader "Custom/CustomStandard_GpuInstance"
{
    Properties
    {
        _BaseMap("Albedo", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        _SpecColor("Specular", Color) = (0.2, 0.2, 0.2)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "Lit"
            "IgnoreProjector" = "True"
        }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend One Zero
            ZWrite On
            Cull Back
            
            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing
            #pragma shader_feature _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            float4 _BaseColor;
            float _Metallic;
            float _Smoothness;
            float4 _SpecColor;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float fogFactor: TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float3 viewDirWS : TEXCOORD3;
                float4 shadowCoord : TEXCOORD4;
                float3 normalWS : TEXCOORD5;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.uv = IN.uv;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionWS = worldPos;
                OUT.fogFactor = ComputeFogFactor(worldPos); 
                OUT.viewDirWS = _WorldSpaceCameraPos - worldPos;
                OUT.shadowCoord = TransformWorldToShadowCoord(worldPos);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                float4 baseColor = texColor * _BaseColor;

                float3 N = normalize(IN.normalWS);
                float3 V = normalize(IN.viewDirWS);
                Light mainLight = GetMainLight();
                float3 L = normalize(mainLight.direction);
                float3 H = normalize(L + V);
                float shadowAtten = MainLightRealtimeShadow(IN.shadowCoord);

                float NdotL = max(0, dot(N, L));
                float3 lightColor = mainLight.color * mainLight.distanceAttenuation * shadowAtten;

                float3 diffuse = baseColor.rgb * NdotL * lightColor;
                float NdotH = max(0, dot(N, H));
                float3 specColor = _SpecColor.rgb;
                float specPower = pow(NdotH, _Smoothness * 128);
                float3 specular = specColor * specPower * lightColor;

                float3 gi = SampleSH(N);
                float3 finalColor = diffuse + specular + gi * baseColor.rgb;
                finalColor.rgb = MixFog(finalColor.rgb, IN.fogFactor);
                return float4(finalColor, baseColor.a);
                //return float4(shadowAtten.xxx, 1.0); // 白なら落ちてない、グレー〜黒なら落ちてる
            }
            ENDHLSL
        }

        Pass {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
        
            ZWrite On
            ZTest LEqual
            ColorMask 0

        
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #pragma multi_compile_instancing
            #pragma shader_feature _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE   
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
        
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    //CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.LitShader"
}