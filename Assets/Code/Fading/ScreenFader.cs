using UnityEngine;

public sealed class ScreenFader : MonoBehaviour 
{
	[SerializeField] private Material fadeMaterial;
	private static Color fadeColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

	private static float prevAlpha = 0.0f;

	private void Awake()
	{
		Events.OnStateChange += (state) => 
		{
			if (state == GameState.Paused)
				SetPause();

			if (state == GameState.Playing)
				RemovePause();
		};
	}

	public static void SetFade(float r, float g, float b, float a)
	{
		fadeColor = new Color(r, g, b, a);
	}

	public static void SetPause()
	{
		prevAlpha = fadeColor.a;
		fadeColor.a = 0.7f;
	}

	public static void RemovePause()
	{
		fadeColor.a = prevAlpha;
	}

	private void OnPostRender()
	{
		if (fadeColor.a == 0.0f) return;

		fadeMaterial.SetColor("_Color", fadeColor);

		GL.PushMatrix();
		GL.LoadOrtho();

		fadeMaterial.SetPass(0);
		GL.Begin(GL.QUADS);

		GL.Vertex3(0.0f, 0.0f, 0.1f);
		GL.Vertex3(1.0f, 0.0f, 0.1f);
		GL.Vertex3(1.0f, 1.0f, 0.1f);
		GL.Vertex3(0.0f, 1.0f, 0.1f);

		GL.End();
		GL.PopMatrix(); 
	}
}
