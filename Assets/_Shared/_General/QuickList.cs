using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickList<T>
{
    public int Count;
    
    private readonly T[] things;
    private int capacity;

    public T this[int index]
    {
        get { return things[index]; }
        set { things[index] = value; }
    }
    
    
    public QuickList(int capacity)
    {
        things = new T[capacity];
        
        this.capacity = capacity;
    }

    public void Clear()
    {
        Count = 0;
    }

    public void RemoveAt(int index)
    {
        if(index >= Count)
            return;
        
        Count--;
        things[index] = things[Count];
    }

    public void Add(T thing)
    {
        if (Count == capacity)
        {
            Debug.LogFormat("Can't add {0} to QuickList", thing);
            return;
        }
        
        things[Count++] = thing;
    }
}

