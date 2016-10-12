using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public sealed class MainMenu : MonoBehaviour 
{
	private Text percentText;

	private void Start()
	{
		MapData.Load();

		percentText = UIStore.GetUI<Text>("GenerationPercent");
		Engine.ChangeState(GameState.MainMenu);
	}

	public void TryPlay(GameObject worldTypesWindow)
	{
		if (MapData.LoadedData != null) SetTypeAndBuild(MapData.LoadedData.genID);
		else worldTypesWindow.SetActive(true);
	}

	public void SetTypeAndBuild(int type)
	{
		Map.SetWorldType(type);
		Events.SendGameEvent(GameEventType.GeneratingIsland);
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
