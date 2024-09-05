using UnityEngine;


public class CropInit : MonoBehaviour
{
    private void Start()
    {
        Resolution[] r = Screen.resolutions;
        Resolution select = r[r.Length - 1];
        Screen.SetResolution(select.width, select.height, true);
        gameObject.SetActive(false);
    }
}
