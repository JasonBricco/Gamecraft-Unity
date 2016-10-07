using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public struct HitInfo
{
	public bool hit;
	public Vector3i hitPos;
	public Vector3i adjPos;
	public Vector3i normal;

	public void CalculateNormal()
	{
		normal = hitPos - adjPos;
	}
}

public class PlayerInteraction : MonoBehaviour 
{
	[SerializeField] private int editRange = 10;

	private static BlockInstance currentBlock;
	
	private GameObject reticle;
	private Renderer reticleRenderer;

	private Image selectedPanel;
	private Text selectedText;
	
	private int structureID = 0;

	private StructureGenerator[] structures =
	{
		new MassBreak(),
		new TreeGenerator(),
		new WallGenerator()
	};
	
	private delegate void Add();
	private Add currentAdd;
	
	private Block selectedBlock = new Block(BlockID.Grass);

	private static bool reticleEnabled = true;

	private string[] buttonNames;

	public static BlockInstance CurrentBlock
	{
		get { return currentBlock; }
	}

	private void Awake()
	{
		selectedPanel = UIStore.GetUI<Image>("SelectedBlockPanel");
		selectedText = UIStore.GetUI<Text>("SelectedBlockText");

		reticle = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/Reticle"), Vector3.zero, Quaternion.identity);
		reticleRenderer = reticle.GetComponent<Renderer>();
	
		currentAdd = AddBlock;

		EventManager.OnCommand += (command, args) => 
		{ 
			if (command == CommandType.ToggleReticle)
				reticleEnabled = !reticleEnabled;
		};
	}

	private void Update()
	{
		if (Engine.CurrentState != GameState.Playing) return;

		if (Input.GetMouseButtonUp(0))
			currentAdd();

		if (Input.GetMouseButtonUp(1))
			DeleteBlock();

		if (Input.GetKeyDown(KeyCode.Q))
			PickBlock();

		if (Input.GetKeyDown(KeyCode.Z))
			UndoManager.Undo();

		if (Input.GetKeyDown(KeyCode.X))
			UndoManager.Redo();

		if (!reticleEnabled) 
		{
			DisableReticle();
			return;
		}
		
		HitInfo info = GetHit();
		
		if (info.hit)
		{
			Vector3i pos = info.hitPos;
			currentBlock = new BlockInstance(Map.GetBlock(pos.x, pos.y, pos.z), pos.x, pos.y, pos.z);
			EnableReticle();
			reticle.transform.position = info.hitPos;
		}
		else
			DisableReticle();
	}
	
	public void BlockSelected(int ID)
	{
		Block newBlock = new Block((BlockID)ID);
		currentAdd = AddBlock;
		selectedBlock = newBlock;
		Engine.ChangeState(GameState.Playing);
		ShowSelectedBlock(newBlock);
	}
	
	public void StructureSelected(int ID)
	{
		currentAdd = AddStructure;
		structureID = ID;
		Engine.ChangeState(GameState.Playing);
	}
	
	private void ShowSelectedBlock(Block block)
	{
		string name = block.Name();
		selectedText.text = name;

		UIFader.CancelCurrent();

		UIFader.FadeIn(selectedPanel, 0.1f);
		UIFader.FadeIn(selectedText, 0.1f);

		UIFader.FadeOut(selectedPanel, 2.0f, 0.5f);
		UIFader.FadeOut(selectedText, 2.0f, 0.5f);
	}

	public static void AllowReticle()
	{
		reticleEnabled = true;
	}

	public static void DisallowReticle()
	{
		reticleEnabled = false;
	}
	
	private void EnableReticle()
	{
		if (!reticleRenderer.enabled)
			reticleRenderer.enabled = true;
	}

	private void DisableReticle()
	{
		if (reticleRenderer.enabled)
			reticleRenderer.enabled = false;
	}
	
	private void PickBlock()
	{
		HitInfo info = GetHit();

		if (info.hit)
		{
			Vector3i setPos = info.hitPos;
			selectedBlock = new Block(Map.GetBlock(setPos.x, setPos.y, setPos.z).ID);
			ShowSelectedBlock(selectedBlock);

			currentAdd = AddBlock;
		}
	}
	
	private void AddBlock()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;

		HitInfo info = GetHit();

		if (info.hit)
		{
			bool overwrite = Map.GetBlock(info.hitPos.x, info.hitPos.y, info.hitPos.z).AllowOverwrite();
			Vector3i setPos = overwrite ? info.hitPos : info.adjPos;

			bool empty = Utils.ValidatePlacement(setPos);

			if (empty)
				Map.SetBlockAdvanced(setPos.x, setPos.y, setPos.z, selectedBlock, info.normal, true);
		}
	}
	
	private void AddStructure()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;

		HitInfo info = GetHit();

		if (info.hit)
			structures[structureID].Generate(info);
	}
	
	private void DeleteBlock()
	{
		HitInfo info = GetHit();
		
		if (info.hit)
		{
			Vector3i setPos = info.hitPos;
			Map.SetBlockAdvanced(setPos.x, setPos.y, setPos.z, new Block(BlockID.Air), Vector3i.zero, true);
		}
	}

	public HitInfo GetHit()
	{
		HitInfo info = new HitInfo();
		info.hit = false;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3? point = Utils.Raycast(ray, editRange);
		
		if (point.HasValue) 
		{
			info.hit = true;
			Vector3 pos = point.Value;
			Vector3i hitPos = Utils.GetBlockPos(pos  + ray.direction * 0.01f);
			info.hitPos = hitPos;

			pos -= ray.direction * 0.01f;
			info.adjPos = Utils.GetBlockPos(pos);

			info.CalculateNormal();
		}
		
		return info;
	}
}
