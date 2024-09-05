using UnityEngine;

public class SetMobileMaxFramerate : MonoBehaviour
{
    private void Start()
    {
        if (Application.isMobilePlatform)
            Application.targetFrameRate = 60;
        
        Destroy(gameObject);
    }
}
