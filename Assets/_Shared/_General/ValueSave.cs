using System.IO;


public static class ValueSave
{
    private static readonly MemoryStream m = new MemoryStream(10000);
    private static readonly BinaryWriter w;
    private static readonly BinaryReader r;

    public static BinaryWriter Writer
    {
        get
        {
            if (check != 0)
            {
                m.Position = 0;
                check = 0;
            }
            
            return w;
        }
    }
    
    public static BinaryReader Reader
    {
        get
        {
            if (check != 1)
            {
                m.Position = 0;
                check = 1;
            }
            
            return r;
        }
    }
    
    private static int check = -1;
    
    
    static ValueSave()
    {
        w = new BinaryWriter(m);
        r = new BinaryReader(m);
    }
}
