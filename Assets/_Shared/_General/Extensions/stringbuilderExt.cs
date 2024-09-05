using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class stringbuilderExt 
{
    public static StringBuilder NewLine(this StringBuilder builder, int count = 1)
    {
        for (int i = 0; i < count; i++)
            builder.Append("\n");
        
        return builder;
    }
    
    public static StringBuilder Size(this StringBuilder builder, float multi = 1)
    {
        int amount = Mathf.RoundToInt(100 * multi);
        builder.Append("<size=" + amount + "%>");
        return builder;
    }
}
