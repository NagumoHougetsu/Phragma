Shader "Debug/ShadowCasterTest_Visible"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        // カメラ描画用のパス（Editorで見える）
        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            ZWrite On
            Cull Off
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(1, 0, 0, 1); // 赤
            }
            ENDCG
        }

        // 影描画用のパス（ライトシャドウマップ用）
        Pass
        {
            Tags { "LightMode"="ShadowCaster" }

            ZWrite On
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(positionWS);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                return 0; // unused
            }
            ENDHLSL
        }

    }
}
