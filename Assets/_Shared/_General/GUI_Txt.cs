using System.Text;
using UnityEngine;

public class GUI_Txt : MonoBehaviour
{
    private int size;
    private Color color;
    private TextAnchor anchor;
    private readonly StringBuilder builder = new StringBuilder(10000);


    public GUI_Txt Begin(Color color, int size, TextAnchor anchor = TextAnchor.UpperLeft)
    {
        this.color  = color;
        this.size   = size;
        this.anchor = anchor;
        
        builder.Length = 0;
        return this;
    }    
    
    
    public GUI_Txt Add(string value, bool lineBreak = false)
    {
        builder.Append(value);
        
        if(lineBreak)
            builder.Append("\n");
        
        return this;
    }    
    
    
    public GUI_Txt Clear()
    {
        builder.Length = 0;
        return this;
    }  
    
    
    
    private void OnGUI()
    {
        if(builder.Length == 0)
            return;
        
        GUI.skin.label.alignment = anchor;
        GUI.color = color;
        GUI.skin.label.fontSize = size;
        int margin = Mathf.FloorToInt(Mathf.Min(Screen.width, Screen.height) / 30);
        GUI.Label(new Rect(margin, margin / 2, Screen.width - margin * 2, Screen.height - margin), builder.ToString());
    }
}
