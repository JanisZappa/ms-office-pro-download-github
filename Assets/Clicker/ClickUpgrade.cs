using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClickUpgrade : MonoBehaviour
{
    public double prize;
    public double multiplier;
    
    private TextMeshPro a;
    private Vector2 size;
    
    private void Start()
    {
        a = transform.GetChild(0).GetComponent<TextMeshPro>();
        size = a.rectTransform.sizeDelta;
    }
    
    private void Update()
    {
        a.text = name + " " + $"{prize:0,0}" + " " + multiplier + "x";

        DRAW.Rectangle(transform.position, size).SetColor(Color.white);
    }


    public bool GotClicked(Vector2 mPos)
    {
        Vector2 offset = mPos - transform.position.V2();

        if (Mathf.Abs(offset.x) <= size.x * .5f && Mathf.Abs(offset.y) <= size.y * .5f)
        {
            if (Clicker.CanYouAffordThis(prize))
                Clicker.UseUpgrade(this);
            
            return true;
        }

        return false;
    }
}
