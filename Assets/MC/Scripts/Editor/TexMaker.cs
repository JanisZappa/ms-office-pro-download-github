using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;


public static class TexMaker
{
    [MenuItem("Tex/Make")]
    private static void Make()
    {
        Texture2D[] all = Assets.FindAllTextures();
        
        List<Texture2D> blocks = new List<Texture2D>();
        
        for (int i = 0; i < all.Length; i++)
        {
            Texture2D tex = all[i];
            string path = AssetDatabase.GetAssetPath(tex);
            if(path.Contains("BlockTex"))
                blocks.Add(tex);
        }
        
        int count = blocks.Count;

        List<string> names = new List<string>();
        for (int i = 0; i < count; i++)
            names.Add(blocks[i].name);
        
        
    //  Check Which Ones Have Tops  //
        List<bool> sides = new List<bool>();
        for (int i = 0; i < count; i++)
        {
            bool top = false;
            string n = names[i];
            if (!n.Contains("top"))
            {
                for (int e = 0; e < count; e++)
                    if(e != i && names[e].Replace(n, "") == "top")
                    {
                        top = true;
                        break;
                    }
            }
            
            sides.Add(top);
        }
            
        
        const int margin = 64;
        const int block = margin * 2 + 128;
        const int maxRes = 2048;
        const int maxBlocks = maxRes / block;
        
        
        
        int yBlocks = Mathf.FloorToInt(count * 1f / maxBlocks);
        int xBlocks = Mathf.Min(count, maxBlocks);

        if (xBlocks == 0)
        {
            xBlocks = maxBlocks;
            yBlocks -=1;
        }
        yBlocks += 1;
        
        int xRes = xBlocks * block, yRes = yBlocks * block;
        int resCheck = 16;
        while(resCheck < xRes)
            resCheck *= 2;
        xRes = resCheck;
            resCheck = 16;
        while(resCheck < yRes)
            resCheck *= 2;
        yRes = resCheck;
        
        Debug.Log(blocks.Count + "  ->  " + xBlocks + "|" + yBlocks + " --- " + xRes + " " + yRes);
       
        Texture2D result = new Texture2D(xRes, yRes);

        for (int i = 0; i < count; i++)
        {
            Texture2D blockTex = blocks[i];
            
            bool hasAlpha = HasAlpha(blockTex, out Color mixColor);
            bool side     = !hasAlpha && sides[i];
            
            int xB = i % maxBlocks;
            int yB = Mathf.FloorToInt(i * 1f / maxBlocks);
                
            for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
            {
                Color[] read;
                if (side)
                {
                    int width  = x == 1? 128 : margin;
                    int height = y == 1? 128 : margin;
                    
                    int posX = x < 2? 0 : 128 - margin;
                    int posY = y < 2? 0 : 128 - margin;
                    
                    read = Flip(blockTex.GetPixels(posX, posY, width, height), width, height, x != 1, y != 1);
                }
                else
                {
                    read = blockTex.GetPixels(
                        x > 0? 0 : 128 - margin, 
                        y > 0? 0 : 128 - margin, 
                        x == 1? 128 : margin, 
                        y == 1? 128 : margin);
                }
                
                MakeAlpha(read, mixColor, hasAlpha && (x == 0 || x == 2 || y == 0 || y == 2));
                
                result.SetPixels(
                    xB * block + (x == 0? 0 : x == 1? margin : margin + 128), 
                    yB * block + (256 - (margin * 2 + 128)) + (y == 0? 0 : y == 1? margin : margin + 128), 
                    x == 1? 128 : margin, 
                    y == 1? 128 : margin, 
                    read);
            }
        }
        
        result.Apply();
        
        File.WriteAllBytes(Application.dataPath + "/MC/Tex/AllBlocks.png", result.EncodeToPNG());
        AssetDatabase.Refresh();
        
        ProjectTxt.Write("MC/Blocks",  names.OrderBy(name => name).ToArray());
    }


    private static bool HasAlpha(Texture2D tex, out Color mixColor)
    {
        Color[] allColors = tex.GetPixels();
        int count = allColors.Length;
        
        int solid = 0;
        Vector3 mix = Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            Color c = allColors[i];
            if (!(c.r >= .9999f && c.g <= 0.0001f && c.b >= .9999f))
            {
                mix += new Vector3(c.r, c.g, c.b);
                solid++;
            }
        }
        
        mix /= solid;
        mixColor = new Color(mix.x, mix.y, mix.z, 0);
        
        for (int i = 0; i < count; i++)
        {
            Color32 c = allColors[i];
            if(c.r == 255 && c.g == 0 && c.b == 255)
                return true;
        }
        
        return false;
    }


    private static void MakeAlpha(Color[] colors, Color mixColor, bool all = false)
    {
        int count = colors.Length;
        
        for (int i = 0; i < count; i++)
        {
            Color c = colors[i];
            if (all || c.r >= .9999f && c.g <= 0.0001f && c.b >= .9999f)
                colors[i] = mixColor;
        }
    }


    private static Color[] Flip(Color[] colors, int width, int height, bool flipX, bool flipY)
    {
        Color[] result = new Color[colors.Length];

        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            result[y * width + x] = colors[(flipY? height - 1 - y : y) * width + (flipX? width - 1 - x : x)];
        }
        
        return result;
    }
}
