using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public sealed class CompilePlayExit 
{
	private static CompilePlayExit instance;

	static CompilePlayExit()
	{
		Unused(instance);
		instance = new CompilePlayExit();
	}

	private CompilePlayExit()
	{
		EditorApplication.update += OnEditorUpdate;
	}

	~CompilePlayExit()
	{
		EditorApplication.update -= OnEditorUpdate;
		instance = null;
	}

	private static void OnEditorUpdate() 
	{
		if (EditorApplication.isPlaying && EditorApplication.isCompiling) 
			EditorApplication.isPlaying = false;
	}

	private static void Unused<T>(T unusedVariable) {}
}
