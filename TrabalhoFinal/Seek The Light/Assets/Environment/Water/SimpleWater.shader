Shader "Unlit/Simple Water"
{
	Properties
	{
		_Color("Tint", Color) = (1, 1, 1, .5) 
		_FoamC("Foam", Color) = (1, 1, 1, .5) 
		_MainTex ("Main Texture", 2D) = "white" {}
		_TextureDistort("Texture Wobble", range(0,1)) = 0.1
		_NoiseTex("Extra Wave Noise", 2D) = "white" {}
		_Speed("Wave Speed", Range(0,1)) = 0.5
		_Amount("Wave Amount", Range(0,1)) = 0.6
		_Scale("Scale", Range(0,1)) = 0.5
		_Height("Wave Height", Range(0,1)) = 0.1
		_Foam("Foamline Thickness", Range(0,10)) = 8
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"  "Queue" = "Transparent" }
		LOD 100
		Blend OneMinusDstColor One
		Cull Off
		
		GrabPass{
			Name "BASE"
			Tags{ "LightMode" = "Always" }
				}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD3;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 scrPos : TEXCOORD2;//
				float4 worldPos : TEXCOORD4;//
			};
			float _TextureDistort;
			float4 _Color;
			sampler2D _CameraDepthTexture; // Textura de profundidade da camera
			sampler2D _MainTex, _NoiseTex;//
			float4 _MainTex_ST;
			float _Speed, _Amount, _Height, _Foam, _Scale;// 
			float4 _FoamC;
            sampler2D _MaskInt;

			uniform float3 _Position;
			uniform sampler2D _GlobalEffectRT;
			uniform float _OrthographicCamSize;

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

				// Aplica distorcao da textura e ondulacao
				float4 tex = tex2Dlod(_NoiseTex, float4(v.uv.xy, 0, 0));
				v.vertex.y += sin(_Time.z * _Speed + (v.vertex.x * v.vertex.z * _Amount * tex)) * _Height;

				o.vertex = UnityObjectToClipPos(v.vertex);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.scrPos = ComputeScreenPos(o.vertex); 
				UNITY_TRANSFER_FOG(o, o.vertex);		

				// o.vertex -> Posicao do vertice em clip space
				// o.worldPos -> Posicao do vertice em world space
				// o.uv -> UV da textura
				// o.scrPos -> Posicao do vertice em screen space	
				return o;
			}
			
            fixed4 frag (v2f i) : SV_Target
            {
                // Ajustes no UV da textura para compensar a posicao do interactor
                float2 uv = i.worldPos.xz - _Position.xz;
                uv = uv / (_OrthographicCamSize *2);
                uv += 0.5;

                // Ripples
                float ripples = tex2D(_GlobalEffectRT, uv ).b;
 
                // Mascara para evitar bleeding
                float4 mask = tex2D(_MaskInt, uv);              
                ripples *= mask.a;
 
                fixed distortx = tex2D(_NoiseTex, (i.worldPos.xz * _Scale)  + (_Time.x * 2)).r ;// distortion 
                distortx +=  (ripples *2);
           
				// Multiplica cores da textura pelas cores escolhidas
                half4 col = tex2D(_MainTex, (i.worldPos.xz * _Scale) - (distortx * _TextureDistort));   
				
				// Profundidade do pixel
                half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos )));
                
				// Compara profundidade com a posicao em tela para fazer a linha de espuma
				half4 foamLine =1 - saturate(_Foam* (depth - i.scrPos.w ));

                col *= _Color;
                col += (step(0.4 * distortx,foamLine) * _FoamC);
                col = saturate(col) * col.a ;
               
               ripples = step(0.99, ripples * 3);
               float4 ripplesColored = ripples * _FoamC;
               
               return   saturate(col + ripplesColored);
            }
			ENDCG
		}
	}
}