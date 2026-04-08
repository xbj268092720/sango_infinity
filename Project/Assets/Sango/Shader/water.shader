
Shader "Sango/water_urp" {
	Properties{
		_BaseMap("MainTex", 2D) = "white" {}
		_Alpha("alpha", float) = 0.9
		_HorizontalAmount ("Horizontal Amount", Float) = 8
		_VerticalAmount("Vertical Amount", Float) = 8
		_Color("Water Color", Color) = (0.45,0.5,0.44,1)
    	_Speed ("Speed", Float) = 4
		}
		SubShader{
			Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" "RenderType" = "TransparentCutout" }
			LOD 300
			
			Lighting Off 
			Pass {
				Name "FORWARD"
				Tags {
					"LightMode" = "UniversalForward"
				}
				Blend SrcAlpha OneMinusSrcAlpha
				HLSLPROGRAM
				//#define SANGO_BASE_COLOR 1
				//#define SANGO_GRID_COLOR 1
				#define SANGO_FOG 1
				#define SANGO_WATER 1
				#define SANGO_COLOR 1

				//#define SANGO_BRUSH 1
				//#define SANGO_TERRAIN_TYPE 1
				#define SANGO_TERRAIN_TYPE 0
				#include "sango_shaderLib.hlsl"

				#pragma skip_variants DIRLIGHTMAP_COMBINED
				//#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
				#pragma multi_compile _ SANGO_EDITOR
				#pragma multi_compile _ _MAIN_LIGHT_SHADOWS //主光源阴影
				#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE //级联阴影(多张阴影贴图, 近处分辨率高, 远处分辨率低)
				#pragma multi_compile _ _SHADOWS_SOFT //阴影抗锯齿
				#pragma multi_compile_fog


				#pragma skip_variants FOG_EXP FOG_EXP2
				#pragma exclude_renderers xbox360 ps3 
				#pragma target 3.0
				#pragma vertex sango_vert
				#pragma fragment sango_frag

				ENDHLSL
			}
			

			}
}
