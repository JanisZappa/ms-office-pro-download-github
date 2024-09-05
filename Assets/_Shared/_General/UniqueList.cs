public class UniqueList
{
	public int Length;
	private readonly int Capacity;

	private readonly int[] values;
	private readonly int[] map;
	private readonly bool[] has;
	
	public int this[int index]
	{
		get { return values[index]; }
	}
	

	public UniqueList(int capacity)
	{
		map    = new int[capacity];
		values = new int[capacity];
		has    = new bool[capacity];

		Capacity = capacity;
	}

	public void Clear()
	{
		Length = 0;
		has.Clear();
	}

	public bool Add(int value)
	{
		if (Length == Capacity)
		{
			return false;
		}

		if (has[value])
		{
			return false;
		}

		has[value]     = true;
		map[value]     = Length;
		values[Length] = value;
		Length++;
		
		return true;
	}
	
	public int RemoveAt(int whereToLook)
	{
		if (whereToLook >= Length)
		{
			return 0;
		}
		
		int value = values[whereToLook];
		has[value] = false;
		
		int whatToReplaceItWith = values[Length - 1];
		map[whatToReplaceItWith] = whereToLook;
		values[whereToLook] = whatToReplaceItWith;
		Length--;
		
		return value;
	}
	
	public void Remove(int value)
	{
		int whereToLook = map[value];

		if (whereToLook >= Length)
		{
			return;
		}
			
		if (!has[value])
		{
			return;
		}
		
		has[value] = false;
		
		int whatToReplaceItWith = values[Length - 1];
		map[whatToReplaceItWith] = whereToLook;
		values[whereToLook] = whatToReplaceItWith;
		Length--;
	}
}
