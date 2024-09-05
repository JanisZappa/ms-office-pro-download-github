using System.IO;
using UnityEngine;


public static class StreamWriterExt 
{
    public static void Space(this StreamWriter outfile, int emptyLines)
    {
        for (int i = 0; i < emptyLines; i++)
            outfile.WriteLine("");
    }
}
