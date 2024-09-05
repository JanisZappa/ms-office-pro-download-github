using System;
using TMPro;
using UnityEngine;

public class ClickTool : MonoBehaviour
{
    public double prize;
    public double clickRate;

    private double bought;

    private Vector2 size;

    private double multiplier;


    private double BuyValue => Math.Round(prize * (bought < .1 ? 1 : Math.Pow(1.15f, bought)));

    private double MultiClickRate => clickRate * multiplier;
    public double CurrentClickRate => MultiClickRate * bought;


    private TextMeshPro a, b;
    
    
    private void Start()
    {
        multiplier = 1;
        
        a = transform.GetChild(0).GetComponent<TextMeshPro>();
        b = transform.GetChild(1).GetComponent<TextMeshPro>();

        size = a.rectTransform.sizeDelta;
    }

    
    private void Update()
    {
        a.text = name + " - " + MultiClickRate + "\n" + $"{BuyValue:0,0}";
        b.text = ""+ bought;

        DRAW.Rectangle(transform.position, size).SetColor(Color.white);
    }


    public bool GotClicked(Vector2 mPos)
    {
        Vector2 offset = mPos - transform.position.V2();

        if (Mathf.Abs(offset.x) <= size.x * .5f && Mathf.Abs(offset.y) <= size.y * .5f)
        {
            if (Clicker.CanYouAffordThis(BuyValue))
            {
                bought++;
                Clicker.CalculateProduction();
            }
            
            return true;
        }

        return false;
    }


    public void AddMultiplier(double m)
    {
        multiplier *= m;
    }
}
