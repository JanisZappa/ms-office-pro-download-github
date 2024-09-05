using UnityEngine;
using GridData;


public class DynamicTest : MonoBehaviour
{
    private SpriteInfo[] items;
    private const int maxCount = 32;
    
    private void Start()
    {
        items = new[]
        {
            ItemInfo.GetSpriteInfo("character", "None"),
            ItemInfo.GetSpriteInfo("character", "None"),
            ItemInfo.GetSpriteInfo("character", "None")
        };
    }

    
    private void OnEnable()
    {
        DynamicSprites.OnSpriteCollect += OnSpriteCollect;
    }


    private void OnDisable()
    {
        DynamicSprites.OnSpriteCollect -= OnSpriteCollect;
    }
    

    private void OnSpriteCollect()
    {
        float t = GridTime.Now * .5f;
        float t2 = GridTime.Now;

        for (int i = 0; i < maxCount; i++)
        {
            SpriteInfo info = items[Mathf.FloorToInt(i * 32.153f * i * 6.324f) % 3];
            int f = Mathf.FloorToInt(GridTime.Now * 24 + i * 12.31f) % info.frames;
            DynamicSprites.RenderThis(
                new ScatterObject(
                    new Vector3(Mathf.Sin(t * .25f + i * .05f) * (11 + Mathf.Sin(t * .3615f + i * .45f) * .75f), 
                        0,  
                        (6 - (i * 1f / maxCount) * 12 + Mathf.Sin(t * .9615f + i * 3.45f) * .4f) * 1.3f), 
                    ItemInfo.SpriteIndex(info.item, info.anim, f, Mathf.FloorToInt((i * 8.42f + t2) * 16) % info.directions), 
                    .75f + Mathf.Repeat(i * 131.412887f, 1) * .45f, 
                    Mathf.Repeat(i * 12.12887f, 1) > .5f? 1 : 0));
        }
    }
}
