using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(ItemInfo))]
public class ItemInfoEditor : Editor
{
    private static string Path => Application.dataPath + "/Grid/TexData/";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Parse Data"))
            Parse();
        if (GUILayout.Button("Merge Tex"))
            Merge();
        GUILayout.EndHorizontal();
    }


    private void Parse()
    {
        
    }


    private void Merge()
    {
        ItemInfo inf = target as ItemInfo;
            
        string[] folders = Directory.GetDirectories(Path);
        int count = folders.Length;
        
        List<Texture2D> texCollect = new List<Texture2D>();

        string[] toMerge = {"GridTex", "GridTexN", "GridTexX", "GridTexY", "ShadowTex"};
        int mergeCount = toMerge.Length;
        for (int e = 0; e < mergeCount; e++)
        {
            string merge = toMerge[e] + ".png";
            
            for (int i = 0; i < count; i++)
            {
                string folder = folders[i].Replace(Application.dataPath, "Assets");

                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(folder + "/" + merge);
                if(texCollect.Count == 0)
                    texCollect.Add(tex);
                else
                {
                    int tCount = texCollect.Count;
                    for (int j = 0; j < tCount; j++)
                    {
                        Texture2D texOld = texCollect[j];
                        if (texOld.height < tex.height)
                        {
                            texCollect.Insert(j, tex);
                            goto alreadyinserted;
                        }
                    }
                    
                    texCollect.Add(tex);
                    
                    alreadyinserted: ;
                }
            }

        
            
            int xRes = 0;
            int yRes = 0;
            for (int i = 0; i < count; i++)
            {
                Texture2D t = texCollect[i];
                xRes += t.width;
                yRes = Mathf.Max(yRes, t.height);
            }
        
            Texture2D mergeTex = new Texture2D(xRes, yRes);
            int total = xRes * yRes;
            Color[] c = new Color[total];
            Texture2D first = texCollect[0];
            Color fill = first.GetPixel(first.width - 1, first.height -1);
            for (int i = 0; i < total; i++)
                c[i] = fill;
            mergeTex.SetPixels(c);
        
            int offset = 0;
            for (int i = 0; i < count; i++)
            {
                Texture2D t = texCollect[i];
                mergeTex.SetPixels(offset, 0, t.width, t.height, t.GetPixels());
                offset += t.width;
            }
        
            File.WriteAllBytes(Path + merge, mergeTex.EncodeToPNG());
            AssetDatabase.Refresh();
            inf.SetTexture(AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Replace(Application.dataPath, "Assets/") + merge), e);
        }
       
        EditorUtility.SetDirty(inf);
    }
}
