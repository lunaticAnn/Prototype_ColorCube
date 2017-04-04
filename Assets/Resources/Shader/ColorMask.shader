Shader "Custom/ColorMask" {
	Properties {
		
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200
		Pass{
		ZWrite On
		ColorMask RG
		}
		
	}
	FallBack Off
}
