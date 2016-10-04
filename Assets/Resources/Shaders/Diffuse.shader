Shader "Voxel/Diffuse" 
{
	Properties 
	{
		_TexArray ("Array", 2DArray) = "" {}
	}
	
	SubShader 
	{
		Pass
		{
			Tags  { "Queue" = "Geometry" "RenderType" = "Opaque" }
			Lighting Off
			LOD 200
			
			CGPROGRAM
			#pragma vertex vert
		    #pragma fragment frag
		    #pragma multi_compile_fog
		    #pragma target 3.5

		    #include "UnityCG.cginc"

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
				UNITY_FOG_COORDS(1)
			};

			UNITY_DECLARE_TEX2DARRAY(_TexArray);
	      	
	      	VertexOut vert(VertexIn v)
			{
				VertexOut o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord; 
				o.col = v.col;

				UNITY_TRANSFER_FOG(o, o.pos);

				return o;
			}

			float4 frag(VertexOut i) : COLOR
			{
				half4 col = UNITY_SAMPLE_TEX2DARRAY(_TexArray, i.uv.xyz);

				float3 light = i.col.rgb;
				float sun = i.col.a;

				float3 amb = UNITY_LIGHTMODEL_AMBIENT * 2 * sun;
				amb = max(amb, 0.0666);
	      		amb = max(amb, light);

	      		col.xyz *= amb;
	      		UNITY_APPLY_FOG(i.fogCoord, col);

	      		return float4(col.xyz, 1.0);
			}

			ENDCG
		}
	}
	
	FallBack "Standard"
}
