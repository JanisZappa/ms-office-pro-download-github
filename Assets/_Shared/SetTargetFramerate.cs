using UnityEngine;

public class SetTargetFramerate : MonoBehaviour
{
    public int fps;
    
    private void Start()
    {
        Application.targetFrameRate = fps;
    }
}
