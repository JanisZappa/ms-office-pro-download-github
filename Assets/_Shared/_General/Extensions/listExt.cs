using System.Collections.Generic;
using UnityEngine;


public static class listExt 
{
    public static T GetAdd<T>(this List<T> list, T thing)
    {
        list.Add(thing);
        return thing;
    }

    public static void AddRange<T>(this List<T> list, List<T> other)
    {
        int count = other.Count;

        for (int i = 0; i < count; i++)
            list.Add(other[i]);
    }

    public static T GetRemoveAt<T>(this List<T> list, int index)
    {
        T thing = list[index];
        list.RemoveAt(index);
        return thing;
    }
    
    public static T GetRemoveLast<T>(this List<T> list)
    {
        int index = list.Count - 1;
        T thing = list[index];
        list.RemoveAt(index);
        return thing;
    }

    public static int MaxStringLength(this List<string> list)
    {
        int longest = 0;
        for (int i = 0; i < list.Count; i++)
            longest = Mathf.Max(longest, list[i].Length);

        return longest;
    }

    public static bool AddUnique<T>(this List<T> list, T thing)
    {
        if (!list.Contains(thing))
        {
            list.Add(thing);
            return true;
        }

        return false;
    }

    public static string Log(this List<int> list)
    {
        string value = "";
        int count = list.Count, max = count - 1;
        for (int i = 0; i < count; i++)
            value += list[i] + (i < max ? " | " : "");

        return value;
    }
    
    public static string Log(this List<float> list)
    {
        string value = "";
        int count = list.Count, max = count - 1;
        for (int i = 0; i < count; i++)
            value += list[i] + (i < max ? " | " : "");

        return value;
    }
    
    public static string Log(this List<Color> list)
    {
        int count = list.Count;

        string line = "";
        for (int i = 0; i < count; i++)
            line += "<color=" + list[i].ToHex() + ">█</color>";

        return line;
    }
    
    public static string Log(this List<Color32> list)
    {
        int count = list.Count;

        string line = "";
        for (int i = 0; i < count; i++)
            line += "<color=" + list[i].ToHex() + ">█</color>";

        return line;
    }

    public static void CopyFrom<T> (this List<T> list, List<T> other)
    {
        int count = other.Count;
        
        list.Clear();
        
        for (int i = 0; i < count; i++)
            list.Add(other[i]);
    }
    
    
    public static List<T> Randomize<T> (this List<T> list)
    {
        int count = list.Count;

        for (int i = 0; i < count; i++)
        {
            T value = list[i];
            int switchWith = Random.Range(0, count);
            list[i] = list[switchWith];
            list[switchWith] = value;
        }

        return list;
    }
    
    
    public static List<T> RandomizeRange<T> (this List<T> list, int start, int end)
    {
        int count = end - start;

        for (int i = 0; i < count; i++)
        {
            int index =  + start;
            T value = list[index];
            int switchWith = Random.Range(0, count) + start;
            list[index] = list[switchWith];
            list[switchWith] = value;
        }

        return list;
    }


    public static bool RemoveIfInList<T>(this List<T> list, T thing)
    {
        if (list.Contains(thing))
        {
            list.Remove(thing);
            return true;
        }
        return false;
    }
}
