using UnityEngine;


public class SetTimeScale : MonoBehaviour
{
    public float timeScale;
    
    private void Start()
    {
        Time.timeScale = timeScale;
        Destroy(this);
    }
}
