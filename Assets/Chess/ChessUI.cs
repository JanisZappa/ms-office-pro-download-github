using System;
using System.Collections;
using TMPro;
using UnityEngine;


public class ChessUI : Singleton<ChessUI>
{
    private TextMeshProUGUI enter, friendCode, nameTxt;

    private const int nameMax = 12, codeMax = 8;
    
    public static bool IsActive { get { return Inst.enter.enabled; }}
    
    
    public void Init()
    {
        enter = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        enter.enabled = false;
        
        friendCode = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        friendCode.enabled = false;
        
        nameTxt = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        nameTxt.enabled = false;
    }
    
    
    private void UpdateEnter(string value, float t)
    {
        int add = nameMax - value.Length;
        if (add > 0)
        {
            value += t * .75f % 1 < .7f? "_" : "-";
            add--;
        }
        for (int i = 0; i < add; i++)
            value += "_";
        
        enter.text = "<size=22>Your Name?</size>\n\n" + value + "\n\n\n";
    }

    
    private void UpdateCode(string value, float t)
    {
        int add = codeMax - value.Length;
        if (add > 0)
        {
            value += t * .75f % 1 < .7f? "_" : "-";
            add--;
        }
        for (int i = 0; i < add; i++)
            value += "_";
        
        enter.text = "<size=22>Opponent's ID?</size>\n\n" + value + "\n\n\n";
    }
    

    private static char GetKeyChar(KeyCode code, bool numbers, bool space)
    {
        switch (code)
        {
            case KeyCode.Return:    return '!';
            case KeyCode.Backspace: return '/';
            
            case KeyCode.A:    return 'A';
            case KeyCode.B:    return 'B';
            case KeyCode.C:    return 'C';
            case KeyCode.D:    return 'D';
            
            case KeyCode.E:    return 'E';
            case KeyCode.F:    return 'F';
            case KeyCode.G:    return 'G';
            case KeyCode.H:    return 'H';
            
            case KeyCode.I:    return 'I';
            case KeyCode.J:    return 'J';
            case KeyCode.K:    return 'K';
            case KeyCode.L:    return 'L';
            
            case KeyCode.M:    return 'M';
            case KeyCode.N:    return 'N';
            case KeyCode.O:    return 'O';
            case KeyCode.P:    return 'P';
            
            case KeyCode.Q:    return 'Q';
            case KeyCode.R:    return 'R';
            case KeyCode.S:    return 'S';
            case KeyCode.T:    return 'T';
            
            case KeyCode.U:    return 'U';
            case KeyCode.V:    return 'V';
            case KeyCode.W:    return 'W';
            case KeyCode.X:    return 'X';
            
            case KeyCode.Y:    return 'Y';
            case KeyCode.Z:    return 'Z';
        }
        
        if(numbers)
            switch (code)
            {
                case KeyCode.Alpha0:    return '0';
                case KeyCode.Alpha1:    return '1';
                case KeyCode.Alpha2:    return '2';
                case KeyCode.Alpha3:    return '3';
                case KeyCode.Alpha4:    return '4';
                case KeyCode.Alpha5:    return '5';
                case KeyCode.Alpha6:    return '6';
                case KeyCode.Alpha7:    return '7';
                case KeyCode.Alpha8:    return '8';
                case KeyCode.Alpha9:    return '9';
            }
        
        if(space && code == KeyCode.Space)
            return ' ';
        
        return '?';
    }
    
    
    public IEnumerator EnterName()
    {
        enter.enabled = true;
        
        string value = "";
        float  t     = 0;
        UpdateEnter(value, 0);

        while (true)
        {
            t += Time.deltaTime;
            char add = GetKeyChar(keycodeExt.GetDownKey, false, true);
            
            int count = value.Length;

            if(add == '!' && count > 0)
                break;

            if (add != '?' && count < nameMax)
            {
                if(add != '/')
                    value += add;
                else
                    if(count > 0)
                        value = value.Substring(0, count - 1);
                
                t = 0;
            }
            
            UpdateEnter(value, t);
            
            yield return null;
        }
        
        PlayerPrefs.SetString("chessname", value);
        
        enter.text = "";
        yield return new WaitForSeconds(.5f);
        enter.enabled = false;
    }
    
    
    public IEnumerator EnterFriendCode(Action<string> returnCode)
    {
        enter.enabled = true;
        
        string value = "";
        float t = 0;
        UpdateCode(value, 0);

        while (true)
        {
            t += Time.deltaTime;
            char add = GetKeyChar(keycodeExt.GetDownKey, true, false);
            
            int count = value.Length;

            if(add == '!' && count == 8)
                break;

            if (add != '?' && count < nameMax)
            {
                if(add != '/')
                    value += add;
                else
                if(count > 0)
                    value = value.Substring(0, count - 1);
                
                t = 0;
            }
            
            UpdateCode(value, t);
            
            yield return null;
        }
        
        returnCode.Invoke(value);
        
        enter.text = "";
        yield return new WaitForSeconds(.5f);
        enter.enabled = false;
    }


    public IEnumerator AcceptRequest(string opponent, Action<bool> response)
    {
        enter.enabled = true;
        enter.text = opponent + "\n<size=22>wants to play.\n(y/n)</size>";
        
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                response.Invoke(true);
                break;
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                response.Invoke(false);
                break;
            }
            yield return null;
        }
        
        enter.text = "";
        yield return new WaitForSeconds(.5f);
        enter.enabled = false;
    }


    public IEnumerator ShowMessage(string message, float wait = 2)
    {
        enter.enabled = true;
        enter.text = message;
        
        yield return new WaitForSeconds(wait);
        enter.text = "";
        yield return new WaitForSeconds(.5f);
        enter.enabled = false;
    }
    

    public void ShowFriendCode(string name, string code, bool show = true)
    {
        friendCode.enabled = show;
           nameTxt.enabled = show;

       if (show)
       {
           friendCode.text = code;
           nameTxt.text = name;
       }
            
    }
}
