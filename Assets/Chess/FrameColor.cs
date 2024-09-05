using UnityEngine;


public class FrameColor : MonoBehaviour
{
    private static readonly int _Color = Shader.PropertyToID("_Color");
    
    
    private void Start()
    {
        GetComponent<MeshRenderer>().material.SetColor(_Color, Color.white);
    }
}
