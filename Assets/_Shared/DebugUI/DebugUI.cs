using System.Text;
using TMPro;
using UnityEngine;


public class DebugUI : MonoBehaviour
{
    private TextMeshProUGUI[] txts;
    
    public delegate void OnDebugUI(StringBuilder builder);
    public static event OnDebugUI TL, TR, BL, BR, T;
    
    private readonly StringBuilder builder = new StringBuilder(10000);
    
    private readonly prefBool doSize = new prefBool("doSize");
    
    private readonly prefBool[] bools =
    {
        new prefBool("doTL"), new prefBool("doTR"), new prefBool("doBL"), new prefBool("doBR"), new prefBool("doT"), 
    };
    
    private const int Count = 4;


    private OnDebugUI Get(int i)
    {
        switch (i)
        {
            default: return TL;
            case 1:  return TR;
            case 2:  return BL;
            case 3:  return BR;
            case 4:  return T;
        }
    }
    
    
    private void Start()
    {
        Transform trans = transform;
        
        txts = new TextMeshProUGUI[Count];
        for (int i = 0; i < Count; i++)
            txts[i] = trans.GetChild(i).GetComponent<TextMeshProUGUI>();
        
        SetSize();
        
        bools[4].Set(true);
    }

    
    private void Update()
    {
        if (Input.GetKeyDown(Keys.Axon))// && Cursor.visible)
        {
            if (Input.mousePosition.x > Screen.width * .5f)
                Toggle(Input.mousePosition.y > Screen.height * .5f? 1 : 3);
            else
                Toggle(Input.mousePosition.y > Screen.height * .5f? 0 : 2);
        }
        

        if (Input.GetKeyDown(Keys.Plus))
        {
            doSize.Toggle();
            SetSize();         
        }
    }


    private void Toggle(int i)
    {
        if(!bools[i].Toggle())
            txts[i].text = "";
    }
    
    
    private void LateUpdate()
    {
        for (int i = 0; i < Count; i++)
        {
            OnDebugUI e = Get(i);
           
            if (e != null && bools[i])
            {
                builder.Length = 0;
                e.Invoke(builder);
                txts[i].text = builder.ToString();
            }
        }
    }


    private void SetSize()
    {
        float s = doSize? 11 : 8;
        for (int i = 0; i < Count; i++)
            txts[i].fontSize = s;
    }
}
