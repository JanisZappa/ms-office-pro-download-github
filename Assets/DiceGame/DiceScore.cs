using TMPro;
using UnityEngine;


public class DiceScore : MonoBehaviour
{
    public Mesh dirMesh;

    public Transform[] dice;

    private int count;
    
    private Vector3[] dirs;

    private Vector3[] rest;
    private Vector3 center;
    private Rigidbody[] rB;

    private TextMeshProUGUI tmp;
    
    
    private void Start()
    {
        count = dice.Length;
        rest = new Vector3[count];
        rB = new Rigidbody[count];
        
        center = Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            rest[i] = dice[i].position;
            center += rest[i] * (1f / count);
            rB[i] = dice[i].GetComponent<Rigidbody>();
        }

        for (int i = 0; i < count; i++)
            rest[i] = rest[i] - center;
            
        
        ResetDice();
        
        dirs = dirMesh.vertices;
        Debug.Log(dirs.Length);

        tmp = GetComponent<TextMeshProUGUI>();
    }


    private void ResetDice()
    {
        Quaternion groupRot = Random.rotation;
        
        for (int i = 0; i < count; i++)
        {
            dice[i].rotation = Random.rotation;
            dice[i].position = center + groupRot * rest[i];
            rB[i].velocity += (Random.rotation * Vector3.up).MultiY(.25f) * 3 + new Vector3(14, 9, -2) * Random.Range(.75f, 1.25f) * 1.4f;
        }
    }

    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        ResetDice();
    }

    
    private void LateUpdate()
    {
        int total = 0;
        string v = "";
        for (int i = 0; i < count; i++)
        {
            float best = -2;
            int s = 0;
            Quaternion rot = dice[i].rotation;
            for (int e = 0; e < 20; e++)
            {
                float d = (rot * dirs[e]).y;

                if (d > best)
                {
                    best = d;
                    s = e + 1;
                }
            }

            v += s + (i == count - 1 ? "     " : " + ");
            total += s;
        }

        v += total;

        tmp.text = v;
    }
}
