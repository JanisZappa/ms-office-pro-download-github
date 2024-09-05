using System;
using System.IO;

public static class BookRW 
{
    public static void Write(string book, string name, string content)
    {
        string path = BookPath(book, name);
        File.WriteAllText(path, content);
    }
 
 
    public static string Read(string book, string name)
    {
        string path = BookPath(book, name);
        return File.Exists(path) ? File.ReadAllText(path) : "";
    }
    
    
    public static void WriteLines(string book, string name, string[] lines)
    {
        string path = BookPath(book, name);
        File.WriteAllLines(path, lines);
    }


    public static string[] ReadLines(string book, string name)
    {
        string path = BookPath(book, name);
        return File.Exists(path) ? File.ReadAllLines(path) : new string[0];
    }


    private static string BookPath(string book, string name)
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/") + "/Books/";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        path += book + "/";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path + name + ".txt";
    }
}
