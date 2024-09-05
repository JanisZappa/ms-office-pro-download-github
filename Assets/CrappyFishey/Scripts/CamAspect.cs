using UnityEngine;

public class CamAspect : MonoBehaviour
{
    private readonly PrefBool box = new PrefBool("CamAspect");
    
    private Camera cam;
    
    private void Start()
    {
        cam = GetComponent<Camera>();
        UpdateCam();
    }

    
    private void Update()
    {
        if (Input.GetKeyDown(Keys.ü))
        {
            box.Toggle();
            UpdateCam();
        }
    }


    private void UpdateCam()
    {
        float x = Screen.width;
        float y = Screen.height;
        if (box)
        {
            float desire = y / 3 * 4;
            float multi = desire / x;
            cam.rect = new Rect((1f - multi) * .5f, 0, multi, 1);
        }
        else
            cam.rect = new Rect(0, 0, 1, 1);
    }
}
