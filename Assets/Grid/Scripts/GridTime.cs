using UnityEngine;

public class GridTime : MonoBehaviour
{
    public static float Now;
    private static bool stop;
    private static readonly int shaderNow = Shader.PropertyToID("Now");

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            stop = !stop;
    }


    public static void AddTime(float dt)
    {
        Now += stop? 0 : dt;
        
        Shader.SetGlobalFloat(shaderNow, Now);
    }
}
