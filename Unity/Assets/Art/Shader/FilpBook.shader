// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FilpBook"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[ASEBegin]_Tex("Tex", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
		_HV("HV", Vector) = (0,0,0,0)
		[ASEEnd]_speed("speed", Float) = 0

	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

		Cull Off
		HLSLINCLUDE
		#pragma target 2.0
		ENDHLSL

		
		Pass
		{
			Name "Unlit"
			

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			#define ASE_SRP_VERSION 70301

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS_SPRITEUNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			

			sampler2D _Tex;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color;
			float2 _HV;
			float _speed;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D( _AlphaTex ); SAMPLER( sampler_AlphaTex );
				float _EnableAlphaTexture;
			#endif

			float4 _RendererColor;

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.vertex.xyz );

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.clipPos = vertexInput.positionCS;

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float temp_output_4_0_g1 = _HV.x;
				float temp_output_5_0_g1 = _HV.y;
				float2 appendResult7_g1 = (float2(temp_output_4_0_g1 , temp_output_5_0_g1));
				float totalFrames39_g1 = ( temp_output_4_0_g1 * temp_output_5_0_g1 );
				float2 appendResult8_g1 = (float2(totalFrames39_g1 , temp_output_5_0_g1));
				float mulTime8 = _TimeParameters.x * _speed;
				float clampResult42_g1 = clamp( 0.0 , 0.0001 , ( totalFrames39_g1 - 1.0 ) );
				float temp_output_35_0_g1 = frac( ( ( mulTime8 + clampResult42_g1 ) / totalFrames39_g1 ) );
				float2 appendResult29_g1 = (float2(temp_output_35_0_g1 , ( 1.0 - temp_output_35_0_g1 )));
				float2 temp_output_15_0_g1 = ( ( IN.texCoord0.xy / appendResult7_g1 ) + ( floor( ( appendResult8_g1 * appendResult29_g1 ) ) / appendResult7_g1 ) );
				float4 tex2DNode6 = tex2D( _Tex, temp_output_15_0_g1 );
				
				float4 Color = ( tex2DNode6 * _Color );

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D( _AlphaTex, sampler_AlphaTex, IN.texCoord0.xy );
					Color.a = lerp( Color.a, alpha.r, _EnableAlphaTexture );
				#endif

				Color *= IN.color;

				return Color;
			}

			ENDHLSL
		}
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18800
252.8;162.4;1488;747;953.6483;642.2471;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;9;-1044,-91.5;Inherit;False;Property;_speed;speed;3;0;Create;True;0;0;0;False;0;False;0;15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;8;-869,-101.5;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;7;-923,-241.5;Inherit;False;Property;_HV;HV;2;0;Create;True;0;0;0;False;0;False;0,0;6,10;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;5;-594,-290.5;Inherit;False;Flipbook;-1;;1;53c2488c220f6564ca6c90721ee16673;2,71,0,68,0;8;51;SAMPLER2D;0.0;False;13;FLOAT2;0,0;False;4;FLOAT;3;False;5;FLOAT;3;False;24;FLOAT;0;False;2;FLOAT;0;False;55;FLOAT;0;False;70;FLOAT;0;False;5;COLOR;53;FLOAT2;0;FLOAT;47;FLOAT;48;FLOAT;62
Node;AmplifyShaderEditor.ColorNode;12;-283,-47;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-301,-287.5;Inherit;True;Property;_Tex;Tex;0;0;Create;True;0;0;0;False;0;False;-1;None;2883ee17e0c44a240aaed9005c735645;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;61,-283;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;62,-184;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;25;297,-250;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;13;FilpBook;cf964e524c8e69742b1d21fbe2ebcc4a;True;Unlit;0;0;Unlit;3;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;0;True;2;5;False;-1;10;False;-1;3;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;1;Vertex Position;1;0;1;True;False;;False;0
WireConnection;8;0;9;0
WireConnection;5;4;7;1
WireConnection;5;5;7;2
WireConnection;5;2;8;0
WireConnection;6;1;5;0
WireConnection;11;0;6;0
WireConnection;11;1;12;0
WireConnection;14;0;6;4
WireConnection;14;1;12;4
WireConnection;25;1;11;0
ASEEND*/
//CHKSM=1A82744EFAD687EBDE63EC51C9C35CFD697EBD5D