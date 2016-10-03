using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	private Text percentText;

	private void Start()
	{
		percentText = UIStore.GetUI<Text>("GenerationPercent");
		Engine.ChangeState(GameState.MainMenu);
	}

	public void TryPlay(GameObject worldTypesWindow)
	{
		bool dataLoaded = MapData.Load();

		if (dataLoaded)
			Engine.BeginPlay();
		else
			worldTypesWindow.SetActive(true);
	}

	public void SetTypeAndBuild(int type)
	{
		Map.SetWorldType(type);
		EventManager.SendGameEvent(GameEventType.GeneratingIsland);
		StartCoroutine(FillPercentage());
		Map.GenerateAllTerrain();
	}

	private IEnumerator FillPercentage()
	{
		percentText.text = "0%";

		while (true)
		{
			percentText.text = (Map.GetProgress(TerrainFinished) * 100.0f).ToString("F0") + "%";
			yield return null;
		}
	}

	private void TerrainFinished()
	{
		StopAllCoroutines();
		GC.Collect();
		Engine.BeginPlay();
	}
}
