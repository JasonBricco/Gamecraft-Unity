using UnityEngine;

public static class Utils
{
	public static Vector3i GetBlockPos(Vector3 pos)
	{
		return new Vector3i(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
	}

	public static Vector3i GetLegsBlock()
	{
		Vector3 pos = Camera.main.transform.position;
		return new Vector3i(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y) - 1, Mathf.RoundToInt(pos.z));
	}

	public static Vector3i GetHeadBlock()
	{
		Vector3 pos = Camera.main.transform.position;
		return new Vector3i(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
	}

	public static Vector3? Raycast(Ray ray, float distance) 
	{
		Vector3 startPoint = ray.origin;
		Vector3 endPoint = ray.origin + ray.direction * distance;
		
		Vector3i start = GetBlockPos(startPoint);
		Vector3i end = GetBlockPos(endPoint);
		
		if (start.x > end.x)
		{
			int tmp = start.x;
			start.x = end.x;
			end.x = tmp;
		}
		
		if (start.y > end.y) 
		{
			int tmp = start.y;
			start.y = end.y;
			end.y = tmp;
		}
		
		if (start.z > end.z) 
		{
			int tmp = start.z;
			start.z = end.z;
			end.z = tmp;
		}
		
		float minDistance = distance;
		
		for (int z = start.z; z <= end.z; z++) 
		{
			for (int y = start.y; y <= end.y; y++) 
			{
				for (int x = start.x; x <= end.x; x++) 
				{
					Block block = Map.GetBlockSafe(x, y, z);

					if (block.IgnoreRaycast())
						continue;
					
					float dist = BlockRayIntersection(new Vector3(x, y, z), ray);
					minDistance = Mathf.Min(minDistance, dist);
				}
			}
		}
		
		if (minDistance != distance)
			return ray.origin + ray.direction * minDistance;
		else return null;
	}
	
	private static float BlockRayIntersection(Vector3 blockPos, Ray ray) 
	{
		float near = float.MinValue;
		float far = float.MaxValue;
		
		for (int i = 0; i < 3; i++) 
		{
			float min = blockPos[i] - 0.5f;
			float max = blockPos[i] + 0.5f;
			
			float pos = ray.origin[i];
			float dir = ray.direction[i];
			
			if (Mathf.Abs(dir) <= float.Epsilon) 
			{
				if ((pos < min) || (pos > max)) 
					return float.MaxValue;
			}
			
			float t0 = (min - pos) / dir;
			float t1 = (max - pos) / dir;
			
			if (t0 > t1) 
			{
				float tmp = t0;
				t0 = t1;
				t1 = tmp;
			}
			
			near = Mathf.Max(t0, near);
			far = Mathf.Min(t1, far);
			
			if (near > far) 
				return float.MaxValue;
			
			if (far < 0.0f) 
				return float.MaxValue;
		}
		
		return near > 0.0f ? near : far;
	}

	public static bool ValidatePlacement(Vector3i pos)
	{
		if (pos.y >= Map.Height)
			return false;
		
		if (pos == GetLegsBlock() || pos == GetHeadBlock())
			return false;	
		
		return true;
	}

	public static int Square(int value)
	{
		return value * value;
	}

	public static float Square(float value)
	{
		return value * value;
	}

	public static int Sign(float val)
	{
		return val < 0 ? -1 : 1;
	}

	public static float Signf(float val)
	{
		return val < 0.0f ? -1.0f : 1.0f;
	}
}
