Shader "Hidden/CritterProcedural"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Size ("Size", Float) = 0.5
		_HealthyColor ("Healthy Color", Color) = (1,1,1,1)
	    _UnhealthyColor("Unhealthy Color", Color) = (1,1,1,1)
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
			
		struct CritterData {
			float health;
			float consumption;
			float timeDrain;
			float isAlive;
			float timeSinceDecision;
			float2 movementDirection;
			float speed;
		};

		uniform StructuredBuffer<float2> _CritterPoints;
		uniform StructuredBuffer<CritterData> _CritterData;
		uniform float4 Sim_EnergyCaps;
		float4 _HealthyColor;
		float4 _UnhealthyColor;

		struct vs_out {
			float4 pos : SV_POSITION;
			float4 color : COLOR;
		};

		vs_out vert(uint id : SV_VertexID)
		{
			vs_out o;
			o.pos = float4(_CritterPoints[id], 0, 1);
			o.pos = mul(UNITY_MATRIX_VP, o.pos);
			o.color = lerp(_UnhealthyColor, _HealthyColor, _CritterData[id].health / Sim_EnergyCaps.y);
			o.color.rgb *= _CritterData[id].isAlive;
			return o;
		}

		struct gs_out {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float4 color : COLOR;
		};

		float _Size;

		[maxvertexcount(4)]
		void geom(point vs_out input[1], inout TriangleStream<gs_out> outStream)
		{
			float dx = _Size / unity_OrthoParams.x;
			float dy = _Size / unity_OrthoParams.y;

			//Min size = 1 pixel
			dx = max(dx, 1 / _ScreenParams.x);
			dy = max(dy, 1 / _ScreenParams.y);

			gs_out output;

			output.pos = input[0].pos + float4(-dx, dy, 0, 0); 
			output.uv = float2(0, 0);
			output.color = input[0].color;
			outStream.Append(output);

			output.pos = input[0].pos + float4(dx, dy, 0, 0); 
			output.uv = float2(1, 0);
			output.color = input[0].color;
			outStream.Append(output);

			output.pos = input[0].pos + float4(-dx, -dy, 0, 0); 
			output.uv = float2(0, 1);
			output.color = input[0].color;
			outStream.Append(output);

			output.pos = input[0].pos + float4(dx, -dy, 0, 0); 
			output.uv = float2(1, 1);
			output.color = input[0].color;
			outStream.Append(output);

			outStream.RestartStrip();
		}

		sampler2D _Sprite;

		fixed4 frag(gs_out i) : COLOR0
		{
			fixed4 col = tex2D(_Sprite, i.uv);
			col *= i.color;
			col.rgb *= col.a;
			return col;
		}
		ENDCG
		}
	}
}
