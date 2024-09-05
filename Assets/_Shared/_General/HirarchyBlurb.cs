using System.Collections.Generic;
using UnityEngine;


public class HirarchyBlurb : MonoBehaviour
{
	[TextArea(10,20)]
	public string blurb;
	[HideInInspector] public List<GameObject> subObjects;
	
	private Transform oldParent;
	private string oldBlurb;
	private int oldSiblingIndex;
	
	private const int max = 26;
	
	
	public void UpdateNames()
	{
		int childIndex = transform.GetSiblingIndex();
		if (oldBlurb == blurb && oldParent == transform.parent && oldSiblingIndex == childIndex)
			return;

		oldBlurb = blurb;
		oldParent = transform.parent;
		oldSiblingIndex = childIndex;
			
		while (subObjects.Count > 0)
		{
			DestroyImmediate(subObjects[0]);
			subObjects.RemoveAt(0);
		}
		
		List<string> parts = new List<string>();
		string[] split = blurb.Split(' ');

		string partString = "";
		for (int i = 0; i < split.Length; i++)
		{
			if (partString.Length + split[i].Length > max)
			{
				parts.Add(partString);
				partString = "";
			}
			
			partString += split[i];
			
			if (partString.Length < max && i < split.Length - 1)
				partString += " ";
		}
		
		if(!string.IsNullOrEmpty(partString))
			parts.Add(partString);

		if (parts.Count == 0)
		{
			name = "⁃ Blurb";
			return;
		}

		name = "⁃ " + parts[0];

		for (int i = 1; i < parts.Count; i++)
		{
			GameObject sub = new GameObject("⁃ " + parts[i]) {tag = "EditorOnly"};
			sub.transform.SetParent(oldParent);
			sub.transform.SetSiblingIndex(childIndex + i);
			
			subObjects.Add(sub);
		}
	}
}
