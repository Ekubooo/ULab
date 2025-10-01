Shader "Custom/Fractal Surface GPU"
{
	Properties
	{
		_BaseColor ("Base Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
	}
	
	SubShader
	{
		CGPROGRAM
		#pragma surface ConfigureSurface Standard fullforwardshadows addshadow
		#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
		#pragma editor_sync_compilation
		#pragma target 4.5
		// #include "FractalGPU.hlsl"

		struct Input
		{
			float3 worldPos;
		};

		float _Smoothness;
		float4 _BaseColor;

		#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			StructuredBuffer<float4x4> _Matrices;
		#endif

		void ConfigureProcedural ()
		{
		#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		    unity_ObjectToWorld = _Matrices[unity_InstanceID];
		#endif
		}

		void ShaderGraphFunction_float (float3 In, out float3 Out)
		{
		    Out = In;
		}

		void ShaderGraphFunction_half (half3 In, out half3 Out)
		{
		    Out = In;
		}

		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface)
		{
			surface.Albedo = _BaseColor.rgb;
			surface.Smoothness = _Smoothness;
		}
		ENDCG
	}

	FallBack "Diffuse"
}