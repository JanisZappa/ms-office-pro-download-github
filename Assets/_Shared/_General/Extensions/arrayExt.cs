using System;
using UnityEngine;
using Random = UnityEngine.Random;


public static class arrayExt 
{
    public static int PlugRemoveAt<T>(this T[] array, int index, int length)
    {
        if (index >= length)
            return length;

        int newLength = length - 1;
        if (index < newLength)
            array[index] = array[newLength];
        
        return newLength;
    }
    
    
    public static int ShiftRemoveAt<T>(this T[] array, int index, int length)
    {
        if (index >= length)
            return length;

        int newLength = length - 1;
        if (index < newLength)
        {
            int start = index + 1;
            for (int i = start; i < length; i++)
                array[i - 1] = array[i];
        }
        
        return newLength;
    }
    
    public static int PlugRemove<T>(this T[] array, T thing, int length)
    {
        for (int i = 0; i < length; i++)
            if (array[i].Equals(thing))
                return array.PlugRemoveAt(i, length);
        
        return length;
    }
    
    public static int ShiftRemove<T>(this T[] array, T thing, int length)
    {
        for (int i = 0; i < length; i++)
            if (array[i].Equals(thing))
                return array.ShiftRemoveAt(i, length);
        
        return length;
    }
    
    public static int Add<T>(this T[] array, T thing, int length)
    {
        if (length >= array.Length)
            return length;

        array[length] = thing;
        
        return length + 1;
    }

    public static bool Contains<T>(this T[] array, T thing, int length)
    {
        for (int i = 0; i < length; i++)
            if (array[i].Equals(thing))
                return true;

        return false;
    }
    
    public static void Replace<T>(this T[] array, T a, T b, int length)
    {
        for (int i = 0; i < length; i++)
            if (array[i].Equals(a))
            {
                array[i] = b;
                return;
            }
    }
    
    public static int MaxStringLength(this string[] list)
    {
        int longest = 0;
        for (int i = 0; i < list.Length; i++)
            longest = Mathf.Max(longest, list[i].Length);

        return longest;
    }
    
    
    public static string Log(this int[] list)
    {
        string value = "";
        int count = list.Length, max = count - 1;
        for (int i = 0; i < count; i++)
            value += list[i] + (i < max ? " | " : "");

        return value;
    }
    
    public static string Log(this float[] list)
    {
        string value = "";
        int count = list.Length, max = count - 1;
        for (int i = 0; i < count; i++)
            value += list[i] + (i < max ? " | " : "");

        return value;
    }
    
    public static string Log(this Color[] list)
    {
        int count = list.Length;

        string line = "";
        for (int i = 0; i < count; i++)
            line += "<color=" + list[i].ToHex() + ">█</color>";

        return line;
    }
    
    public static string Log(this Color32[] array)
    {
        int count = array.Length;

        string line = "";
        for (int i = 0; i < count; i++)
            line += "<color=" + array[i].ToHex() + ">█</color>";

        return line;
    }
    
    
    public static T[] Copy<T> (this T[] array)
    {
        int count = array.Length;
        
        T[] copyArray = new T[count];
        
        for (int i = 0; i < count; i++)
            copyArray[i] = array[i];
        
        return copyArray;
    }
    
    public static T[] Randomize<T> (this T[] array)
    {
        int count = array.Length;

        for (int i = 0; i < count; i++)
        {
            T value = array[i];
            int switchWith = Random.Range(0, count);
            array[i] = array[switchWith];
            array[switchWith] = value;
        }

        return array;
    }
    
    public static void RandomizeRange<T>(this T[] array, int length)
    {
        for (int i = 0; i < length; i++)
        {
            T value = array[i];
            int switchWith = Random.Range(0, length);
            array[i] = array[switchWith];
            array[switchWith] = value;
        }
    }
    
    public static void RandomizeRange(this int[] array, int length)
    {
        for (int i = 0; i < length; i++)
            array[i] = i;

        for (int i = 0; i < length; i++)
        {
            int other      = Random.Range(0, length);
            int otherValue = array[other];
            int value      = array[i];
            array[other] = value;
            array[i] = otherValue;
        }
    }
    
    public static void RandomizeRange(this int[] array, System.Random random, int length)
    {
        /*for (int i = 0; i < length; i++)
            array[i] = i;
*/

        for (int i = 0; i < length; i++)
        {
            int other      = random.Range(0, length);
            int otherValue = array[other];
            int value      = array[i];
            array[other] = value;
            array[i] = otherValue;
        }
    }


    public static void Clear<T>(this T[] array)
    {
        Array.Clear(array, 0, array.Length);
    }
    
    
    public static void CopyFrom<T> (this T[] array, T[] other)
    {
        int count = Mathf.Min(array.Length, other.Length);
        
        for (int i = 0; i < count; i++)
            array[i] = other[i];
    }


    public static void Reverse<T>(this T[] array, int length)
    {
        int min = 0, max = length -1;

        while (min < max)
        {
            T vA = array[min];
            T vB = array[max];
            array[min] = vB;
            array[max] = vA;
            
            min++;
            max--;
        }
    }


    public static T[] SetAll<T>(this T[] array, T value)
    {
        int length = array.Length;

        for (int i = 0; i < length; i++)
            array[i] = value;
        
        return array;
    }
}
