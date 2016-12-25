Shader "Unlit/Environment"
{
	Properties
	{
		[HideInInspector]	_MainTex ("Texture", 2D) = "white" {}
		_FullColor ("Full Energy Color", Color) = (1,1,1,1)
		_EmptyColor ("Empty Energy Color", Color) = (0,0,0,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

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

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _FullColor;
			fixed4 _EmptyColor;

			uniform float _MaxEnergy;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float energy = tex2D(_MainTex, i.uv).r;
				energy /= _MaxEnergy;

				fixed4 col = lerp(_EmptyColor, _FullColor, energy);
				return col;
			}
			ENDCG
		}
	}
}
