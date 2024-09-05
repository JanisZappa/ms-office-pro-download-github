using UnityEngine;


public static class Vector2IntExt 
{
	public static Vector2Int OnlyLongest(this Vector2Int value)
	{
		int absX = Mathf.Abs(value.x);
		int absY = Mathf.Abs(value.y);

		int x = absX > absY ? (int)Mathf.Sign(value.x) : 0;
		int y = absX < absY ? (int)Mathf.Sign(value.y) : 0;
		return new Vector2Int(x, y);
	}
	
	
	public static Vector2Int Clamp(this Vector2Int value, int max)
	{
		return new Vector2Int(Mathf.Min(value.x, max), Mathf.Min(value.y, max));
	}


	public static Vector2Int Add(this Vector2Int value, int x, int y)
	{
		return new Vector2Int(value.x + x, value.y + y);
	}

	
	public static Vector2Int Min(this Vector2Int value, Vector2Int other)
	{
		return new Vector2Int(Mathf.Min(value.x, other.x), Mathf.Min(value.y, other.y));
	}
	
	
	public static Vector2Int Max(this Vector2Int value, Vector2Int other)
	{
		return new Vector2Int(Mathf.Max(value.x, other.x), Mathf.Max(value.y, other.y));
	}

	
	public static Vector2Int Vector2IntFloor(this Vector2 value)
	{
		return new Vector2Int(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y));
	}

	
	public static Vector2Int Normalized(this Vector2Int value)
	{
		return new Vector2Int(Mathf.Clamp(value.x, -1, 1), Mathf.Clamp(value.y, -1, 1));
	}


	public static Vector2Int Reverse(this Vector2Int value)
	{
		return new Vector2Int(-value.x, -value.y);
	}
}
