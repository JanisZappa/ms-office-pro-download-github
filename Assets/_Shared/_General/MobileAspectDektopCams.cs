using UnityEngine;
using UnityEngine.SceneManagement;


public class MobileAspectDektopCams : MonoBehaviour
{
	private const float width = 561, height = 315;
	
	private enum CamMode
	{
		Default, Landscape, Portrait, Square
	}

	private static CamMode Mode = CamMode.Default;
	private static Camera BlackCam;
	
	
	private static bool toggleActive = true;
	
	
	private void Update ()
	{
		if (toggleActive && Input.GetKeyDown(KeyCode.C))
		{
			if(BlackCam != null)
				Destroy(BlackCam.gameObject);
			
			Mode = (CamMode) (((int) Mode + 1) % 4);
			
			SetMode();
		}
	}


	private void SetMode()
	{
		Camera[] allCams = FindObjectsOfType<Camera>();

		const float scale = .8f;
			
		Rect rect = new Rect(Vector2.zero, Vector2.one);
		switch (Mode)
		{
			case CamMode.Default:
				rect = new Rect(Vector2.zero, Vector2.one);
				break;

			case CamMode.Landscape:
			{
				const float desiredAspect = width / height;
				float w = Screen.height * scale / Screen.width;
				float h = (Screen.height * scale / desiredAspect) / Screen.height;
					
				rect = new Rect(new Vector2((1 - w) * .5f, (1 - h) * .5f), new Vector2(w, h));
				goto CreateBlackCam;
			}

			case CamMode.Portrait:
			{
				const float desiredAspect = height / width;
				float shortSide = Screen.height * scale * desiredAspect;
				float w = shortSide / Screen.width;
				float h = scale;
					
				rect = new Rect(new Vector2((1 - w) * .5f, (1 - h) * .5f), new Vector2(w, h));
				goto CreateBlackCam;
			}
				
			case CamMode.Square:
			{
				const float desiredAspect = height / width;
				float w = (Screen.width * desiredAspect * scale) / Screen.width;
				float h = (Screen.width * desiredAspect * scale) / Screen.height;

				rect = new Rect(new Vector2((1 - w) * .5f, (1 - h) * .5f), new Vector2(w, h));
				goto CreateBlackCam;
			}
		}
			
			
		CreateBlackCam:
		{
			BlackCam = new GameObject("BlackCam").AddComponent<Camera>();
				
			BlackCam.depth           = -100;
			BlackCam.backgroundColor = Color.black;
			BlackCam.clearFlags      = CameraClearFlags.Color;
			BlackCam.cullingMask     = 0;
		}


		for (int i = 0; i < allCams.Length; i++)
		{
			Camera cam = allCams[i];
			
			if(cam.gameObject.CompareTag("MainCamera") && 
			   cam.targetTexture == null)
				cam.rect = rect;
		}
			


		if (BlackCam != null)
		{
			BlackCam.depth = -100;
			BlackCam.backgroundColor = Color.black;
			BlackCam.clearFlags = CameraClearFlags.Color;
			BlackCam.cullingMask = 0;
		}	
	}


	public static void SetDefault()
	{
		MobileAspectDektopCams m = FindObjectOfType<MobileAspectDektopCams>();

		if (m != null)
		{
			Mode = CamMode.Default;
			m.SetMode();
		}
	}


	public static void SetToggle(bool active)
	{
		toggleActive = active;
	}
	
	
	/*[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Register()
	{
		if (!Application.isMobilePlatform)// && !Application.isEditor)
			SceneManager.sceneLoaded += PutInScene;
	}
    
	
	private static void PutInScene(Scene scene, LoadSceneMode mode)
	{
		new GameObject().AddComponent<MobileAspectDektopCams>();
	}*/
}
