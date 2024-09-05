using UnityEngine;


public class EnableDRAW : MonoBehaviour
{
	public Camera drawCam;

	[Space(10)] 
	public bool enable = true;
	public bool drawInSceneView;

	
	private void OnEnable()
	{
		if (drawCam != null && drawCam.gameObject.activeInHierarchy && drawCam.enabled)
			DRAW.DrawCam = drawCam;
		else
			if (Camera.main != null)
				DRAW.DrawCam = Camera.main;
		
		DRAW.Enabled    = enable;
		DRAW.EditorDraw = drawInSceneView;
	}


	private void OnValidate()
	{
		if (drawInSceneView && !enable)
			enable = true;
	}
}
