
Shader "Sango/outline_urp" {
	Properties{
		[MainTexture] _BaseMap("MainTex", 2D) = "white" {}
		_OutlineWidth("width", float) = 0.004//定义一个变量
	}
		SubShader{
			Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" "RenderType" = "TransparentCutout" }

			Pass
			{
				Name "OUTLINEPASS"
				Tags {
					"LightMode" = "SRPDefaultUnlit"
				}
				Fog { Mode Off }
				ZWrite Off
				Cull Front
				Blend SrcAlpha OneMinusSrcAlpha
				HLSLPROGRAM
				#include "sango_outlineLib.hlsl"
				//#pragma multi_compile_fwdbase
				#pragma multi_compile_fog
				#pragma skip_variants DIRLIGHTMAP_COMBINED
				//#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
				#pragma multi_compile _ SANGO_EDITOR
				#pragma skip_variants FOG_EXP FOG_EXP2
				#pragma exclude_renderers xbox360 ps3 
				#pragma target 3.0
				#pragma vertex outline_vert
				#pragma fragment outline_frag
				ENDHLSL
			}
		}
}

