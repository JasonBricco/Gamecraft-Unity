Shader "Voxel/Outer Liquid" 
{
	Properties 
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Sun("Sunlight", Color) = (1.0, 1.0, 1.0, 1.0)
		_Alpha("Alpha", float) = 0.5
		_Speed("Speed", float) = 2.0
	}
	
	SubShader 
	{
		Pass
		{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
		    #pragma fragment frag
		    #pragma target 3.5

	      	struct VertexIn
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				float4 col : COLOR;
			};

			struct VertexOut
			{
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				float4 col : COLOR;
			};
	      	
			sampler2D _MainTex;
      		float4 _Sun;
	      	float _Alpha;
	      	float _Speed;

	      	VertexOut vert(VertexIn v)
			{
				VertexOut o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord + _SinTime.y * _Speed; 
				o.col = v.col;

				return o;
			}

			float4 frag(VertexOut i) : COLOR
			{
				float3 col = tex2D(_MainTex, i.uv).rgb;

				float3 light = _Sun.rgb;
				float sun = _Sun.a;

				float3 amb = UNITY_LIGHTMODEL_AMBIENT * 2 * sun;
				amb = max(amb, 0.0666);
	      		amb = max(amb, light);

	      		return float4(col * amb, _Alpha);
			}
			
			ENDCG
		}
	}
	
	FallBack "Standard"
}

