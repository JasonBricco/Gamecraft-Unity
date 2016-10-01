Shader "Voxel/Outer Fluid" 
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
		Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		Cull off
		
		CGPROGRAM
      	#pragma surface surf Lambert noambient vertex:vert keepalpha exclude_path:deferred exclude_path:prepass noshadow nolightmap nodynlightmap nodirlightmap nometa noforwardadd
      	
      	struct Input 
      	{
        	float2 uv_MainTex;
      	};
      	
      	sampler2D _MainTex;
      	float4 _Sun;
      	float _Alpha;
      	float _Speed;
      	
      	void vert(inout appdata_full v) 
      	{
        	v.texcoord.x += _SinTime.y * _Speed;
      	}
		
      	void surf(Input IN, inout SurfaceOutput o) 
      	{
      		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
      		o.Alpha = _Alpha;
      	
      		float3 light = _Sun.rgb;
      		float sun = _Sun.a;
      		float3 ambient = UNITY_LIGHTMODEL_AMBIENT * 2 * sun;
      		ambient = max(ambient, 0.0666);
      		ambient = max(ambient, light);
        	o.Emission = o.Albedo * ambient;
     	}
		
		ENDCG
	}
	
	FallBack "Standard"
}
