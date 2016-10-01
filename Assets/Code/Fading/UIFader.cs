using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIFader : MonoBehaviour
{
	private Queue<Graphic> graphicsToFade = new Queue<Graphic>();
	private Queue<Graphic> graphicsToDisable = new Queue<Graphic>();
	private float fadeDuration;

	private static UIFader self;

	private void Start()
	{
		self = this;
	}

	public static void CancelCurrent()
	{
		self.CancelInvoke();
		self.graphicsToFade.Clear();
		self.graphicsToDisable.Clear();
	}

	public static void FadeOut(Graphic graphic, float beginTime, float fadeDuration)
	{
		self.graphicsToFade.Enqueue(graphic);
		self.fadeDuration = fadeDuration;

		self.Invoke("FadeOut", beginTime);
	}

	public static void FadeIn(Graphic graphic, float fadeDuration)
	{
		graphic.enabled = true;
		graphic.CrossFadeAlpha(1.0f, fadeDuration, true);
	}

	private void FadeOut()
	{
		graphicsToDisable.Enqueue(graphicsToFade.Peek());
		graphicsToFade.Dequeue().CrossFadeAlpha(0.0f, fadeDuration, true);
		Invoke("DisableGraphic", fadeDuration);
	}

	private void DisableGraphic()
	{
		graphicsToDisable.Dequeue().enabled = false;
	}
}
