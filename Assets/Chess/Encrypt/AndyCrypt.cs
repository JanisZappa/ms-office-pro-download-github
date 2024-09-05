using System.Collections.Generic;
using UnityEngine;

public static class AndyCrypt
{
    private const int max = 8;
    
    static AndyCrypt()
    {
        const string value = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";//"W0bNC1DdEeF2GgThI3JjK5LlMmBnO8pPQqR4S7HtuUVvAwX6yYZ9";
        count = value.Length;
        
        chars = new char[count];
        dict = new Dictionary<char, int>();
        for (int i = 0; i < count; i++)
        {
            char c = value[i];
            chars[i] = c;
            dict.Add(c, i);
        }
        
        offsets = new [] { 63, 12, 66, 78, 34, 9, 55, 33 };
        work = new int[max];
    }
  
    private static readonly int count;
    private static readonly char[] chars;
    private static readonly int[] offsets, work;
    private static readonly Dictionary<char, int> dict;

    
    public static string Encrypt(int id, bool checkIt = false)
    {
        for (int i = 0; i < max; i++)
        {
            int pow = max - 1 - i;
            int div = i == max - 1? 1 : pow > 1? (int)Mathf.Pow(count, max - 1 - i) : count;
            work[i] = (id / div % count + offsets[i]) % count;
        }
        
        string code = "";
        for (int i = 0; i < max; i++)
            code += chars[work[i]];

        if (checkIt)
        {
            int check = Decrypt(code);
            bool good = check == id;
            if (!good)
                Debug.Log(id + " -> " + code + " -> " + check);
        }
        
        return code;
    }


    public static int Decrypt(string code)
    {
        for (int i = 0; i < max; i++)
            work[i] = (dict[code[i]] - offsets[i] + count * 10) % count;
        
        int value = 0;
        for (int i = 0; i < max; i++)
        {
            int pow = max - 1 - i;
            int div = i == max - 1? 1 : pow > 1? (int)Mathf.Pow(count, max - 1 - i) : count;
            value += work[i] * div;
        }
        
        return value;
    }


    public static bool ValidChar(char value)
    {
        return dict.ContainsKey(value);
    }
}
