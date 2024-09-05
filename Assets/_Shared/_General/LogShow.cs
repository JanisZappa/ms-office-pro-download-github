using System;
using System.Collections.Generic;
using UnityEngine;


public class LogShow : MonoBehaviour
{
    public int   nrOfLines;
    public float lifetime;
    public int   maxLineLength;
    
    [Space]
    public int active;
    
    private int height;
    
    private class DisplayString
    {
        private readonly string text;
        private float  lifetime;
        private readonly LogType type;
        
        private float maxTime;

        public DisplayString(string text, float lifetime, LogType type)
        {
            this.text = text;
            this.lifetime = lifetime;
            this.type = type;
            
            maxTime = lifetime * .5f;
        }

        public bool AgedWell(float dt)
        {
            lifetime -= dt;
            return lifetime > 0;
        }

        public void Draw(Vector2 pos, Vector2 size)
        {
            GUI.color = (type == LogType.Exception? COLOR.red.tomato : 
                         type == LogType.Warning?   COLOR.yellow.golden : 
                         new Color(.1f, .1f, .1f, 1)).A(Mathf.Clamp01(Mathf.PingPong(lifetime, maxTime) * 4f));
            
            GUI.Label(new Rect(pos, size), text, guiStyle);
        }
    }

    
    private readonly List<DisplayString> allLines = new List<DisplayString>();
    private readonly List<string> insertLines = new List<string>();
    
    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;

        guiStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold, 
            alignment = TextAnchor.UpperLeft, 
            normal    = {textColor = Color.white}
        };
    }
    
    
    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    
    
    private void Update()
    {
        active = allLines.Count;
        for(int i = 0; i < active; i++)
            if (!allLines[i].AgedWell(Time.deltaTime))
            {
                allLines.RemoveAt(i);
                i--;
                active--;
            }
    }


    private void HandleLog(string message, string stackTrace, LogType type)
    {
        if (string.IsNullOrEmpty(message) || 
            string.IsNullOrWhiteSpace(message) ||
            type != LogType.Log && type != LogType.Exception && type != LogType.Warning)
            return;
        
        GetLines(FilterText(message));
        
        int count = allLines.Count + insertLines.Count;
        while(count > nrOfLines)
            allLines.RemoveAt(0);

        for (int i = 0; i < insertLines.Count; i++)
            allLines.Add(new DisplayString(insertLines[i], lifetime, type));
    }


    private string FilterText(string insertString)
    {
        return RemoveColor(insertString).Replace("<b>", "").Replace("</b>", "").Replace("</color>", "");
    }


    private void GetLines(string insertString)
    {
        insertLines.Clear();
        
        if (maxLineLength != 0)
        {
            string[] words = insertString.Split(' ');
            string line = "";
            int length = 0;
            int wordCount = words.Length;
            for (int i = 0; i < wordCount; i++)
            {
                string word = words[i];
                
                int nextLength = length + word.Length;

                if (nextLength >= maxLineLength)
                {
                    insertLines.Add(line);
                    line = word;
                    length = word.Length;
                }
                else
                {
                    line += (length > 0? " " : "") + word;
                    length = nextLength;
                }
            }
            
            if(line.Length > 0)
                insertLines.Add(line);
        }
        else
            insertLines.Add(insertString);
    }


    private static string RemoveColor(string insertString)
    {
        while(insertString.Contains("<color"))
        {
            int indexOfWord = insertString.IndexOf("<color", StringComparison.Ordinal);
            int nrOfLettersToRemove = 0;

            bool foundArrow = false;
           
            while(!foundArrow)
            {
                if (insertString[indexOfWord + nrOfLettersToRemove] == '>')
                    foundArrow = true;

                nrOfLettersToRemove++;
            }

            string newString = insertString.Remove(indexOfWord, nrOfLettersToRemove);
            insertString = newString;
        }

        return insertString;
    }


    private void OnGUI()
    {
        height            = Mathf.FloorToInt(Screen.height * 1f / nrOfLines);
        guiStyle.fontSize = Mathf.FloorToInt(Screen.height * 1f / nrOfLines * .9f);
            
        float margin = height * .5f;
                
        Vector2 pos  = new Vector2(margin, Screen.height - margin);
        Vector2 size = new Vector2(Screen.width - margin * 2, height);
        Vector2 step = new Vector2(0, -height);
        
        int count = allLines.Count;
        for (int i = count - 1; i > -1; i--)
            allLines[i].Draw(pos += step, size);
    }
    
    private static GUIStyle guiStyle;
}
