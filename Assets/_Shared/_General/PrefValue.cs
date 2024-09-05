using UnityEngine;


public class prefBool
{
	private readonly string id;
	
	private  bool value
	{
		get { return PlayerPrefs.GetInt(id) == 1; }
		set { PlayerPrefs.SetInt(id, value? 1 : 0);}
	}

	public prefBool(string id)
	{
		this.id = id;
	}
	
	public void KeySwitch(KeyCode key, bool down = true)
	{
		if (down ? Input.GetKeyDown(key) : Input.GetKeyUp(key))
			value = !value;
	}


	public bool Toggle()
	{
		value = !value;
		return value;
	}
	
	public static implicit operator bool(prefBool d)
	{
		return d.value;
	}

	public void Set(bool value)
	{
		this.value = value;
	}
}


public class prefInt
{
	private readonly string id;
	
	private int value
	{
		get { return PlayerPrefs.GetInt(id); }
		set { PlayerPrefs.SetInt(id, value);}
	}

	public prefInt(string id)
	{
		this.id = id;
	}
	
	public static implicit operator int(prefInt d)
	{
		return d.value;
	}
	
	public void Set(int value)
	{
		this.value = value;
	}
}


public class prefFloat
{
	private readonly string id;
	
	private float value
	{
		get { return PlayerPrefs.GetFloat(id); }
		set { PlayerPrefs.SetFloat(id, value);}
	}

	public prefFloat(string id)
	{
		this.id = id;
	}
	
	public static implicit operator float(prefFloat d)
	{
		return d.value;
	}
	
	public void Set(float value)
	{
		this.value = value;
	}
}