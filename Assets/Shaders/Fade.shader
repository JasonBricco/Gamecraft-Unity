Shader "Voxel/Camera Fade"
{
	Properties 
	{ 
		_Color ("Main Color", Color) = (1, 1, 1, 0) 
	}
	
	SubShader 
	{
		Pass 
		{
			ZTest Always Cull Off ZWrite Off
		    Blend SrcAlpha OneMinusSrcAlpha
		    Color [_Color]
		}
	}
}
