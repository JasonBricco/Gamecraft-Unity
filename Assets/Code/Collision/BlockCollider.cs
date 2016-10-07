using UnityEngine;

public sealed class BlockCollider : MonoBehaviour 
{
	[SerializeField] private BoxCollider box;
	[SerializeField] private MeshCollider meshCol;
	private CollisionType type = CollisionType.Cube;

	public CollisionType Type
	{
		get { return type; }
	}

	public void EnableBox(float x, float y, float z, float sizeX, float sizeY, float sizeZ)
	{
		type = CollisionType.Cube;
		transform.position = new Vector3(x, y, z);
		box.size = new Vector3(sizeX, sizeY, sizeZ);

		if (!box.enabled) box.enabled = true;
		if (meshCol.enabled) meshCol.enabled = false;
	}

	public void EnableMesh(float x, float y, float z)
	{
		transform.position = new Vector3(x, y, z);
		
		if (box.enabled) box.enabled = false;
		if (!meshCol.enabled) meshCol.enabled = true;
	}

	public void SetMesh(Mesh mesh, CollisionType type, float x, float y, float z)
	{
		this.type = type;

		meshCol.sharedMesh = null;
		meshCol.sharedMesh = mesh;

		EnableMesh(x, y, z);
	}

	public void Disable()
	{
		if (box.enabled) box.enabled = false;
		if (meshCol.enabled) meshCol.enabled = false;
	}
}
