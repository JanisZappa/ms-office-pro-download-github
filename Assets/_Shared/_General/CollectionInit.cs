using System.Collections.Generic;


public static class CollectionInit 
{
    public static T[] Array<T>(int count) where T : new()
    {
        T[] array = new T[count];
        
        for (int i = 0; i < count; i++)
            array[i] = new T();
            
        return array;
    }
    
    
    public static List<T> List<T>(int count) where T : new()
    {
        List<T> list = new  List<T>(count);
        
        for (int i = 0; i < count; i++)
            list.Add(new T());
            
        return list;
    }
}
