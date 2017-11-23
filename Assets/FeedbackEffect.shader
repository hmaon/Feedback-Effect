Shader "Hidden/FeedbackEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		BlendOp Add

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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float _UVMultiplier;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = float2(.5,.5) + (v.uv - float2(.5,.5)) * _UVMultiplier;
				return o;
			}
			
			sampler2D _MainTex;
			float _BlendAlpha;
			float _BoostThreshold;
			float4 _BoostAmount;

			fixed4 frag (v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);

				//half4 sizeMultipliers = half4(1.1, 1.2, 1.3, .2);

				//for (int j = 0; j < 4; ++j)
				//{
				//	col += tex2D(_MainTex, i.uv * (sizeMultipliers[j])) * 0.2;
				//}

				col.rgb = dot(col.rgb, col.rgb) > _BoostThreshold ? col.rgb * _BoostAmount.rgb : col.rgb;

				col.a = _BlendAlpha;

				return col;
			}
			ENDCG
		}
	}
}
