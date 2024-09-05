using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneRestart : MonoBehaviour
{
	public KeyCode keycode = KeyCode.Alpha0;
	
	private void Update () 
	{
		if(Input.GetKeyDown(keycode))
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
