using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;


public class Clicker : Singleton<Clicker>
{
    public TextMeshPro scoreText;
    public TextMeshPro levelText;
    
    public Vector2 pos;
    public float radius;

    public int level;

    public double clickScore;

    [Space] 
    public TextAsset[] data;
    public GameObject[] prefabs;
    

    private readonly FloatForce clickForce = new FloatForce(200, 500, 22).SetValue(1);
    private readonly Vector2Force posForce = new Vector2Force(200, 500, 5);
    private Camera cam;

    public delegate bool Check(Vector2 mPos);
    private readonly List<Check> checks = new List<Check>();
    private readonly List<ClickTool> tools = new List<ClickTool>();
    private readonly List<ClickUpgrade> allUpgrades = new List<ClickUpgrade>();
    private List<ClickUpgrade> visibleUpgrades = new List<ClickUpgrade>();
    
    private double production;
    private long productionStart;
    private long frameTime;
    private double clickMulti;
    
    private HashSet<string> upgradesShow = new HashSet<string>();


    private double Produced 
    {
        get
        {
            double span = frameTime - productionStart;
            double seconds = span / 10000000;
            return seconds * production;
        }
    }


    private double Money => clickScore + Produced;
    
    
    private void Start()
    {
        clickMulti = 1;
        cam = Camera.main;
        checks.Add(GotClicked);
        
        CreateTools();
        CreateUpgrades();
        CalculateProduction();
    }


    private void CreateTools()
    {
        Transform parent = new GameObject("Tools").transform;
        parent.SetSiblingIndex(1);
        Vector2 anchor = new Vector2(4, 2.5f);
        string[] lines = data[0].text.Split('\n');
        int count = lines.Length;
        for (int i = 0; i < count; i++)
        {
            string line = lines[i];
            if(string.IsNullOrEmpty(line))
                continue;

            string[] parts = System.Text.RegularExpressions.Regex.Split( line, @"\s{2,}");

            GameObject go = Instantiate(prefabs[0]);
            go.transform.parent = parent;
            go.transform.position = anchor + Vector2.down * .5f * i;

            ClickTool tool = go.GetComponent<ClickTool>();
            tool.name      = parts[0].Replace("_", " ");
            tool.prize     = double.Parse(parts[1], CultureInfo.InvariantCulture);
            tool.clickRate = double.Parse(parts[2], CultureInfo.InvariantCulture);
            
            tools.Add(tool);
            checks.Add(tool.GotClicked);
        }
    }

    private void CreateUpgrades()
    {
        Transform parent = new GameObject("Upgrades").transform;
        parent.SetSiblingIndex(2);
        
        string[] lines = data[1].text.Split('\n');
        int count = lines.Length;
        for (int i = 0; i < count; i++)
        {
            string line = lines[i];
            if(string.IsNullOrEmpty(line) || line.Length < 10)
                continue;

            string[] parts = System.Text.RegularExpressions.Regex.Split( line, @"\s{2,}");

            GameObject go = Instantiate(prefabs[1]);
            go.transform.parent = parent;

            ClickUpgrade upgrade = go.GetComponent<ClickUpgrade>();
            upgrade.name      = parts[0].Replace("_", " ");
            upgrade.prize     = double.Parse(parts[1], CultureInfo.InvariantCulture);
            upgrade.multiplier = double.Parse(parts[2], CultureInfo.InvariantCulture);
            
            allUpgrades.Add(upgrade);
        }
        
        RearrangeUpgrades();
    }


    private void RearrangeUpgrades()
    {
        int visibleCount = visibleUpgrades.Count;
        for (int i = 0; i < visibleCount; i++)
            checks.Remove(visibleUpgrades[i].GotClicked);
            
        visibleUpgrades.Clear();
        
        Vector2 anchor = new Vector2(-3.6f, 2.625f);
        
        int count = allUpgrades.Count;
        upgradesShow.Clear();
        
        for (int i = 0; i < count; i++)
            if (upgradesShow.Add(allUpgrades[i].name))
            {
                visibleUpgrades.Add(allUpgrades[i]);
                checks.Add(allUpgrades[i].GotClicked);
            }
            else
                allUpgrades[i].transform.position = Vector3.up * 10000;

        visibleUpgrades = visibleUpgrades.OrderBy(upgrade => upgrade.prize).ToList();

        count = visibleUpgrades.Count;
        
        for (int i = 0; i < count; i++)
            visibleUpgrades[i].transform.position = anchor + Vector2.down * .25f * i;
    }

    private bool GotClicked(Vector2 mPos)
    {
        float dist = (pos - mPos).sqrMagnitude;
        if (dist < Mathf.Pow(radius * 1.1f, 2))
        {
            clickForce.AddForce(-.0025f);
            posForce.AddForce((pos - mPos) * -.0015f);
            clickScore += clickMulti;
            return true;
        }  
        return false;
    }

    private void Update()
    {
        frameTime = DateTime.Now.Ticks;
        
        if (Input.GetKeyDown(KeyCode.Space))
            level++;
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mPos = cam.ScreenToWorldPoint(Input.mousePosition).V2();
            int cCount = checks.Count;
            for (int i = 0; i < cCount; i++)
                if (checks[i].Invoke(mPos))
                    break;
        }
        
        DRAW.Circle(pos + posForce.Update(Vector2.zero, Time.deltaTime), radius * clickForce.Update(1, Time.deltaTime), 100).SetColor(Color.white);

        double m = Money;
        scoreText.text = m < 10? Math.Floor(m).ToString() : $"{m:0,0}";
        
        levelText.text = $"{production:0,0.0}";
    }


    public static bool CanYouAffordThis(double value)
    {
        if (Inst.Money >= value)
        {
            Inst.clickScore = Inst.Money - value;
            return true;
        }
       
        return false;
    }


    public static void CalculateProduction()
    {
        int tCount = Inst.tools.Count;
        Inst.production = 0;

        for (int i = 0; i < tCount; i++)
            Inst.production += Inst.tools[i].CurrentClickRate;

        Inst.productionStart = Inst.frameTime;
        Inst.RearrangeUpgrades();
    }


    public static void UseUpgrade(ClickUpgrade upgrade)
    {
        if (upgrade.name == "Banana")
            Inst.clickMulti *= upgrade.multiplier;
        
        int tCount = Inst.tools.Count;
        for (int i = 0; i < tCount; i++)
            if (Inst.tools[i].name == upgrade.name)
            {
                Inst.tools[i].AddMultiplier(upgrade.multiplier);
                break;
            }
        
        Inst.allUpgrades.Remove(upgrade);
        Inst.visibleUpgrades.Remove(upgrade);
        Inst.checks.Remove(upgrade.GotClicked);
        Destroy(upgrade.gameObject);
        
        CalculateProduction();
    }
}
