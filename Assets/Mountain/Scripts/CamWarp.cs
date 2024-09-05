using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CamWarp : MonoBehaviour
{
    public Material material;
    public void OnRenderImage(RenderTexture source,RenderTexture destination)
    {
        source.filterMode = FilterMode.Point;
        Graphics.Blit (source, destination, material);
    }
}
