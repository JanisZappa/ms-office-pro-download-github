using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class Txt : MonoBehaviour
{
    public TMP_FontAsset asset;
    public Vector2Int size;
    
    [Space]
    public string[] chars;
    
    private float t;
    
    private TextMeshProUGUI txt;
    private string last;
    private StringBuilder builder;
    
    
    private void Start()
    {
        List<string> collection = new List<string>();
        for (int i = 0; i < asset.characterTable.Count; i++)
            collection.Add(char.ConvertFromUtf32((int)asset.characterTable[i].unicode));
        
        chars = collection.ToArray();
        
        txt = GetComponent<TextMeshProUGUI>();
        
        builder = new StringBuilder(size.x * size.y * 10);
        UpdateTxt();
    }


    private string Next
    {
        get
        {
            while (true)
            {
                string v = chars[Random.Range(0, chars.Length)];
                if(!string.IsNullOrEmpty(v) && !string.IsNullOrWhiteSpace(v))
                    return v;
            }
        }
    }


    private void UpdateTxt()
    {
        builder.Length = 0;

        last = Next;
        
        for (int y = 0; y < size.y; y++)
        {
            int chance = Random.Range(2, 12);
            
            for (int x = 0; x < size.x; x++)
            {
                if (Random.Range(0, chance) != 0)
                    last = Next;
                   
                builder.Append(last);
            }
                    
                
            if(y < size.y - 1)
                builder.Append("\n");
        }
            
        txt.text = builder.ToString();
    }

    
    private void Update()
    {
        UpdateTxt();
        /*t += Time.deltaTime;

        if (t > 1f / 1.6f)
        {
            t -= 1f / 1.6f;
            
            UpdateTxt();
        }*/
    }
}
