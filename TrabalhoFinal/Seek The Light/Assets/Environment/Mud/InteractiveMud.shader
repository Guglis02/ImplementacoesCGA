Shader "Custom/Mud Interactive" {
    Properties{
        [Header(Main)]
        _Noise("Mud Noise", 2D) = "gray" {}
        _NoiseScale("Noise Scale", Range(0,2)) = 0.1
        _NoiseWeight("Noise Weight", Range(0,2)) = 0.1
        [HDR]_ShadowColor("Shadow Color", Color) = (0.5,0.5,0.5,1)

        [Space]
        [Header(Tesselation)]
        _MaxTessDistance("Max Tessellation Distance", Range(10,100)) = 50
        _Tess("Tessellation", Range(1,32)) = 20

        [Space]
        [Header(Mud)]
        [HDR]_Color("Mud Color", Color) = (0.5,0.5,0.5,1)
        [HDR]_PathColorIn("Mud Path Color In", Color) = (0.5,0.5,0.7,1)
        [HDR]_PathColorOut("Mud Path Color Out", Color) = (0.5,0.5,0.7,1)
        _PathBlending("Mud Path Blending", Range(0,3)) = 0.3
        _MainTex("Mud Texture", 2D) = "white" {}
        _MudHeight("Mud Height", Range(0,2)) = 0.3
        _MudDepth("Mud Path Depth", Range(0,100)) = 0.3
        _MudTextureOpacity("Mud Texture Opacity", Range(0,1)) = 1
        _MudTextureScale("Mud Texture Scale", Range(0,2)) = 0.3
    }
    HLSLINCLUDE

    // Includes
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
    #include "MudTessellation.hlsl"
    #pragma require tessellation tessHW
    #pragma vertex TessellationVertexProgram
    #pragma hull hull
    #pragma domain domain
    // Keywords
    
    #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
    #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
    #pragma multi_compile _ _SHADOWS_SOFT
    #pragma multi_compile_fog


    ControlPoint TessellationVertexProgram(Attributes v)
    {
        ControlPoint p;
        p.vertex = v.vertex;
        p.uv = v.uv;
        p.normal = v.normal;
        return p;
    }
    ENDHLSL

    SubShader{
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}

        Pass{
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            // Programa de vertica eh feito no mudtessellation.hlsl
            #pragma fragment frag
            #pragma target 4.0
            

            sampler2D _MainTex;
            float4 _Color;
            float4 _PathColorIn, _PathColorOut;
            float _PathBlending;
            float _MudTextureOpacity, _MudTextureScale;
            float4 _ShadowColor;
            

            half4 frag(Varyings IN) : SV_Target
            {
                float3 worldPosition = mul(unity_ObjectToWorld, IN.vertex).xyz;
                float2 uv = IN.worldPos.xz - _Position.xz;
                uv /= (_OrthographicCamSize * 2);
                uv += 0.5;

                // Textura com a informacao do path		
                float4 effect = tex2D(_MudEffectRT, uv);

                // Mascara para evitar bleeding
                effect *= smoothstep(0.99, 0.9, uv.x) * smoothstep(0.99, 0.9,1- uv.x);
                effect *= smoothstep(0.99, 0.9, uv.y) * smoothstep(0.99, 0.9,1- uv.y);

                // Textura de noise amostrada em worldspace
                float3 topdownNoise = tex2D(_Noise, IN.worldPos.xz * _NoiseScale).rgb;

                // Textura da lama em worldspace
                float3 mudtexture = tex2D(_MainTex, IN.worldPos.xz * _MudTextureScale).rgb;
                
                // Lerp entre a textura da lama e a cor da lama
                float3 mudTex = lerp(_Color.rgb,mudtexture * _Color.rgb, _MudTextureOpacity);
                 
                // Lerp das cores usando a textura do path
                float3 path = lerp(_PathColorOut.rgb * effect.g, _PathColorIn.rgb, saturate(effect.g * _PathBlending));
                float3 mainColors = lerp(mudTex,path, saturate(effect.g));

                // Informacao de luz e sombra
                float shadow = 0;
                half4 shadowCoord = TransformWorldToShadowCoord(IN.worldPos);
                
                #if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
                    Light mainLight = GetMainLight(shadowCoord);
                    shadow = mainLight.shadowAttenuation;
                #else
                    Light mainLight = GetMainLight();
                #endif

                // Suporte para outros pontos de luz
                float3 extraLights;
                int pixelLightCount = GetAdditionalLightsCount();
                for (int j = 0; j < pixelLightCount; ++j) {
                    Light light = GetAdditionalLight(j, IN.worldPos, half4(1, 1, 1, 1));
                    float3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
                    extraLights += attenuatedLightColor;			
                }

                float4 litMainColors = float4(mainColors,1) ;
                extraLights *= litMainColors.rgb;

                // Adiciona a luz principal e ambiente
                half4 extraColors;
                extraColors.rgb = litMainColors.rgb * mainLight.color.rgb * (shadow + unity_AmbientSky.rgb);
                extraColors.a = 1;
                
                // Sombras coloridas
                float3 coloredShadows = (shadow + (_ShadowColor.rgb * (1-shadow)));
                litMainColors.rgb = litMainColors.rgb * mainLight.color * (coloredShadows);
                // Junta tudo
                float4 final = litMainColors+ extraColors + float4(extraLights,0);
                // Adiciona fog
                final.rgb = MixFog(final.rgb, IN.fogFactor);
                return final;
            }
            ENDHLSL
        }
    }
}