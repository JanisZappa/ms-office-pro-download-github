using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HashSetInit 
{
    public static HashSet<int> Get(int capacity)
    {
        HashSet<int> hashy = new HashSet<int>(Enumerable.Range(0, capacity).ToList());
        hashy.Clear();

        return hashy;
    }
}

public static class LinkedListInit
{
    public static LinkedList<int> Get(int capacity)
    {
        LinkedList<int> linky = new LinkedList<int>();
        for (int i = 0; i < capacity; i++)
            linky.AddLast(i);
        
        linky.Clear();

        return linky;
    }
}
