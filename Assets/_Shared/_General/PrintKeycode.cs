using System.Collections.Generic;
using UnityEngine;


public class PrintKeycode : MonoBehaviour
{
	private KeyCode[] keyArray;
	private int keyCount;

	
	private void Awake()
	{
		List<KeyCode> getKeys = new List<KeyCode>();
		foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
			getKeys.Add(vKey);

		keyArray = getKeys.ToArray();
		keyCount = keyArray.Length;
	}

	
	private void Update()
	{
		if(Input.anyKey)
			for (int i = 0; i < keyCount; i++)
			{
				KeyCode key = keyArray[i];
				if (Input.GetKeyDown(key))
					Debug.Log(key);
			}
	}
}
