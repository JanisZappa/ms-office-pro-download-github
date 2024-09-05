using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PageMaster : MonoBehaviour
{
    public GameObject[] pagePrefabs;

    [Space] public Transform marking;

    private const int max = 100;
    private readonly Page[] pages = new Page[max];
    private int count;

    private Transform trans;
    private int page;
    
    private readonly List<int> blanks = new List<int>();
    

    private class Page
    {
        public TextMeshPro tmp;
        public Transform trans;


        public Page(TextMeshPro tmp)
        {
            this.tmp   = tmp;
            trans = tmp.transform;
        }


        public void SetText(string value)
        {
            tmp.text = value;
        }


        public void SetLinkedTMP(Page page)
        {
            tmp.linkedTextComponent = page.tmp;
        }


        public void SetViewIndex(int index)
        {
            switch (index)
            {
                default:    trans.position = Vector3.right * 1000;      return;
                
                case 0:     trans.position = new Vector3(0, 0, 0);      break;
                case 1:     trans.position = new Vector3(105, 0, 0);    break;
                case 2:     trans.position = new Vector3(105, -148, 0); break;
                case 3:     trans.position = new Vector3(0, -148, 0);   break;
                
                case 4:     trans.position = new Vector3(0, 0, 1);      break;
                case 5:     trans.position = new Vector3(105, 0, 1);    break;
                case 6:     trans.position = new Vector3(105, -148, 1); break;
                case 7:     trans.position = new Vector3(0, -148, 1);   break;
            }
        }
    }
    

    public void SetText(string text)
    {
        //pages[0].SetText(text.Replace("\n", "\n  ")); 
        pages[0].SetText(text); 
        ShowPage(page);
    }


    public PageMaster Init()
    {
        trans = transform;

        for (int i = 0; i < max; i++)
            AddPage();
            
        blanks.Add(0);
        blanks.Add(7);

        return this;
    }


    private void AddPage()
    {
        Page p = new Page(Instantiate(pagePrefabs[0], trans).GetComponent<TextMeshPro>());

        if (count > 0)
            pages[count - 1].SetLinkedTMP(p);

        pages[count++] = p;
    }


    private int GetOffset(int index)
    {
        int offset = 0;
        int bc = blanks.Count;
        for (int i = 0; i < bc; i++)
        {
            int b = blanks[i];
            if (b > index)
                break;
            
            offset++;
            index++;
        }

        return offset;
    }


    public void ShowPage(int page)
    {
        this.page = page;

        int start = page * 2;
        for (int i = 0; i < max; i++)
            pages[i].SetViewIndex(i - start + GetOffset(i));
    }


    /*public void PageInfo()
    {
        float t = Time.realtimeSinceStartup;
        Page p = pages[0];
        p.tmp.AndyHack();
        if (p.tmp.isTextOverflowing)
        {
            Debug.Log("Jo");
            
        }
        
        t = Time.realtimeSinceStartup - t;
        Debug.Log(t.ToString("F5") + " -> " + Mathf.FloorToInt(1f / t));
    }*/


    public bool PageIsFull(string value)
    {
        Page p = pages[0];
        //p.SetText(value);
        return p.tmp.AndyHack(value);
    }


    private void LateUpdate()
    {
        /*Page p = pages[0];
        TMP_MeshInfo[] meshinfo = p.tmp.GetTextInfo(p.tmp.text).meshInfo;
        TMP_MeshInfo lastMesh = meshinfo[0];
        if(lastMesh.vertices != null)
            marking.position = p.trans.TransformPoint(lastMesh.vertices[lastMesh.vertexCount - 1]);
        */
        
    }
}
