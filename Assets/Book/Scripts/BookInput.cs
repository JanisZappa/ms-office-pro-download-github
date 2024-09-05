using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class BookInput : MonoBehaviour
{
    public BookAnim anim;
    private Camera cam;

    private readonly StringBuilder builder = new StringBuilder(1000000);
   
    private bool render;
    private int page;

    private PageMaster pageMaster;

    private readonly List<string> chapters = new List<string>(10000);
    private int linesToShow;
    
    private readonly StringBuilder pagePrepare = new StringBuilder(1000000);
    


    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
        cam.enabled = false;
        
        chapters.AddRange(BookRW.ReadLines("Book000001", "Chapter000001"));
        pageMaster = GetComponent<PageMaster>().Init();
        
        //UpdatePage();

        FillPage();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            linesToShow++;
            ShowLines();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            linesToShow = Mathf.Max(0, linesToShow - 1);
            ShowLines();
        }
        
        
        foreach (char character in Input.inputString)
        {
            switch (character)
            {
                case '\b':
                {
                    builder.Length--;
                    UpdatePage();
                    break;
                }
                case '\r':
                {
                    builder.Append("\n");
                    UpdatePage();
                    break;
                }
                default:
                    builder.Append(character);
                    UpdatePage();
                    break;
            }
        }
        
        if(Input.GetKeyDown(KeyCode.F1))
            BookRW.Write("Book000001", "Chapter000001", builder.ToString());

        if (!anim.animating)
        {
            if(page > 0 && Input.GetKey(KeyCode.LeftArrow))
            {
                page = Mathf.Max(0, page - 1);
                PageUpdate();
                anim.TurnPage(true, () => { });
            }
        
            if(Input.GetKey(KeyCode.RightArrow))
            {
                anim.TurnPage(false, () => { page = page + 1;
                    PageUpdate();});
            }
        }
    }


    private void FillPage()
    {
        //pageMaster.ShowPage(0);

        float t = Time.realtimeSinceStartup;
        builder.Length = 0;
        linesToShow = 0;
        while (true)
        {
            builder.Append("  ").AppendLine(chapters[linesToShow++]);
            if (pageMaster.PageIsFull(builder.ToString()))
                break;
            
            if(linesToShow == 20)
                break;
        }

        t = Time.realtimeSinceStartup - t;
        Debug.Log(t.ToString("F5") + " -> " + Mathf.FloorToInt(1f / t));
        
        pageMaster.SetText(builder.ToString());
        render = true;
    }


    private void ShowLines()
    {
        
        for (int i = 0; i < linesToShow; i++)
            builder.Append("  ").AppendLine(chapters[i]);
        
        UpdatePage();
        
        //pageMaster.PageInfo();
    }


    private void LateUpdate()
    {
        if (render)
        {
            cam.Render();
            render = false;
        }
    }


    private void UpdatePage()
    {
        pageMaster.SetText(builder.ToString());
        render = true;
    }

    
    private void PageUpdate()
    {
        pageMaster.ShowPage(page);
        render = true;
    }
}
