using System.Collections.Generic;
using System.IO;
using GridData;
using UnityEngine;


public class ItemInfo : MonoBehaviour
{
    public TextAsset data;
    public TextAsset[] spriteData;
    public Texture2D tex;

    [Space] public Texture[] mergeTex;

    private static readonly Dictionary<string, int> itemmap = new Dictionary<string, int>();
    private static readonly Dictionary<string, int> animmap = new Dictionary<string, int>();
    
    private static readonly Dictionary<Vector4Int, int> spriteMap = new Dictionary<Vector4Int, int>();
    private static readonly Dictionary<Vector2Int, SpriteInfo> infoMap = new Dictionary<Vector2Int, SpriteInfo>();
    private static readonly Dictionary<int, string> nameMap = new Dictionary<int, string>();
    
    private static Texture2D AlphaTex;

    public static ItemData[] itemData;
    private static ItemData[] shadowData;


    private void Start()
    {
        string[] lines = data.text.Split('\n');
        
        int count = lines.Length - 1;
        int itemId = 0;
        int animID = 0;
        for (int i = 0; i < count; i++)
        {
            string[] parts = lines[i].Split('_');
            string item = parts[0];
            string anim = parts[1];
            
            if(!itemmap.ContainsKey(item))
                itemmap.Add(item, itemId++);
                
            if(!animmap.ContainsKey(anim))
                animmap.Add(anim, animID++);
        }
        
        for (int i = 0; i < count; i++)
        {
            string[] parts = lines[i].Split('_');
            int item  = itemmap[parts[0]];
            int anim  = animmap[parts[1]];
            int frame = int.Parse(parts[2]);
            int dir   = int.Parse(parts[3]);
            
            spriteMap.Add(new Vector4Int(item, anim, frame, dir), i);

            Vector2Int infoKey = new Vector2Int(item, anim);
            if (infoMap.TryGetValue(infoKey, out SpriteInfo value))
            {
                value.frames     = Mathf.Max(value.frames, frame + 1);
                value.directions = Mathf.Max(value.directions, dir + 1);
                infoMap[infoKey] = value;
            }
            else
                infoMap.Add(infoKey, new SpriteInfo(item, anim, 1, 1));
            
            nameMap.Add(i, parts[0]);
        }

        AlphaTex = tex;

        itemData   = GetData(spriteData[0]);
        shadowData = GetData(spriteData[1]);
    }


    private static ItemData[] GetData(TextAsset data)
    {
        using (MemoryStream m = new MemoryStream(data.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            int count = r.ReadInt32();
            ItemData[] inf = new ItemData[count];

            for (int e = 0; e < count; e++)
            {
                inf[e] = new ItemData(
                    new Vector2(r.ReadSingle(), r.ReadSingle()), 
                    new Vector2(r.ReadSingle(), r.ReadSingle()),
                    new Vector2(r.ReadSingle(), r.ReadSingle()),
                    new Vector2(r.ReadSingle(), r.ReadSingle()),
                    new Vector2(r.ReadSingle(), r.ReadSingle()),
                    new Vector2(r.ReadSingle(), r.ReadSingle()));
            }

            return inf;
        }

        return null;
    }


    private static int ItemIndex(string item)
    {
        return itemmap.TryGetValue(item, out int value)? value : 0;
    }
    
    
    private static int AnimIndex(string item)
    {
        return animmap.TryGetValue(item, out int value)? value : 0;
    }


    public static int SpriteIndex(int item, int anim, int frame, int dir)
    {
        return spriteMap.TryGetValue(new Vector4Int(item, anim, frame, dir), out int value)? value : 0;
    }


    public static SpriteInfo GetSpriteInfo(string item, string anim)
    {
        return infoMap.TryGetValue(new Vector2Int(ItemIndex(item), AnimIndex(anim)), out SpriteInfo value)? value :new SpriteInfo(0, 0, 0, 0);
    }


    public static string GetSpriteItemName(int id)
    {
        return nameMap.TryGetValue(id, out string value)? value : "None";
    }


    public static float GetAlpha(Vector2 uv)
    {
        return AlphaTex.GetPixelBilinear(uv.x, uv.y).a;
    }


    public static ItemData[] GetData(int id)
    {
        switch (id)
        {
            default: return itemData;
            case 1 : return shadowData;
        }
    }


    public void SetTexture(Texture2D tex, int index)
    {
        if(mergeTex.Length < 5)
            mergeTex = new Texture[5];

        mergeTex[index] = tex;
    }
}


public struct SpriteInfo
{
    public int item, anim, frames, directions;

    public SpriteInfo(int item, int anim, int frames, int directions)
    {
        this.item = item;
        this.anim = anim;
        this.frames = frames;
        this.directions = directions;
    }
}
