Shader "Hidden/CritterProcedural"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Size ("Size", Float) = 0.5
		_Color ("Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
		#pragma target 5.0

		#pragma vertex vert
		#pragma geometry geom
		#pragma fragment frag
		#include "UnityCG.cginc"
			
		StructuredBuffer<float2> positionBuffer;

		struct vs_out {
			float4 pos : SV_POSITION;
		};

		vs_out vert(uint id : SV_VertexID)
		{
			vs_out o;
			o.pos = float4(positionBuffer[id], 0, 1);
			o.pos = mul(UNITY_MATRIX_VP, o.pos);
			return o;
		}

		struct gs_out {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		float _Size;

		[maxvertexcount(4)]
		void geom(point vs_out input[1], inout TriangleStream<gs_out> outStream)
		{
			float dx = _Size / unity_OrthoParams.x;
			float dy = _Size / unity_OrthoParams.y;
			gs_out output;
			output.pos = input[0].pos + float4(-dx, dy, 0, 0); output.uv = float2(0, 0); outStream.Append(output);
			output.pos = input[0].pos + float4(dx, dy, 0, 0); output.uv = float2(1, 0); outStream.Append(output);
			output.pos = input[0].pos + float4(-dx, -dy, 0, 0); output.uv = float2(0, 1); outStream.Append(output);
			output.pos = input[0].pos + float4(dx, -dy, 0, 0); output.uv = float2(1, 1); outStream.Append(output);
			outStream.RestartStrip();
		}

		sampler2D _Sprite;
		fixed4 _Color;

		fixed4 frag(gs_out i) : COLOR0
		{
			fixed4 col = tex2D(_Sprite, i.uv);
			col *= _Color;
			col.rgb *= col.a;
			return col;
		}
		ENDCG
		}
	}
}
