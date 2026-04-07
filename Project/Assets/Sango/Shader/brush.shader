Shader "Sango/brush"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		//[HideInInspector] _BrushTex("Brush Texture", 2D) = "white" {}
		//[HideInInspector] _BrushColor("Brush Color", Color) = (1,1,1,1)
		//[HideInInspector] _BrushSize("Brush Size", Range(1, 1000)) = 1
		//[HideInInspector] _BrushUV("Brush UV", Vector) = (0,0,0,0)
		//[HideInInspector] _BlendMode("Blend Mode", Int) = 0 // 0: Normal, 1: Multiply
		//[HideInInspector] _Pressure("Pressure", Range(0, 1)) = 1.0
		} 

	SubShader{
		Tags { "RenderType"="UniversalPipeline" }
		LOD 100
		ZTest Always Cull Off ZWrite Off Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			CBUFFER_END
			float4 _Brush;
			half4 _BrushUV;
			float _BrushSize;
			half4 _BrushColor;
			int _BlendMode;
			float _Pressure;
			TEXTURE2D(_MainTex);
			TEXTURE2D(_BrushTex);

			#define smp SamplerState_Linear_Clamp
			SAMPLER(smp);

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			half4 frag (v2f i) : SV_Target
			{
				// 应用压感到画笔大小
				float pressure = _Pressure;
				float size = _BrushSize * pressure;
				
				// 采样画笔纹理
				float2 uv = i.uv + (0.5f/size);
				uv = uv - _BrushUV.xy;
				uv *= size;
				half4 brushCol = SAMPLE_TEXTURE2D(_BrushTex, smp, uv);
				
				// 采样目标纹理（BaseMap）
				half4 baseCol = SAMPLE_TEXTURE2D(_MainTex, smp, i.uv);
				
				// 计算画笔颜色，应用压感到透明度
				brushCol.rgb = _BrushColor.rgb;
				brushCol.a *= _BrushColor.a * pressure;
				
				// 根据混合模式计算最终颜色
				half4 finalCol = brushCol;
				if (_BlendMode == 1) // Multiply
				{
					// 正片叠底混合模式
					finalCol.rgb = baseCol.rgb * brushCol.rgb;
					finalCol.a = brushCol.a;
				}
				
				// 透明度阈值
				if (finalCol.a < 0.1f)
				{
					finalCol.a = 0.0;
				}
				
				return finalCol;
			}
						
			ENDHLSL
		}


	}
}