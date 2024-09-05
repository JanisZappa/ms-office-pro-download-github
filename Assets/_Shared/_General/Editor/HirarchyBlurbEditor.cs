using UnityEditor;


[CustomEditor(typeof(HirarchyBlurb))]
public class HirarchyBlurbEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		HirarchyBlurb blurb = target as HirarchyBlurb;
		
		EditorGUI.BeginChangeCheck();
		base.OnInspectorGUI();
		if(EditorGUI.EndChangeCheck() || blurb.transform.hasChanged)
			(target as HirarchyBlurb).UpdateNames();
	}
}
