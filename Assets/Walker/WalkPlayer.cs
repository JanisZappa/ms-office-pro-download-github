using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WalkPlayer : MonoBehaviour
{
    public int fps;

    private Transform trans;
    private bool animating;

    private Vector3 p;
    private Vector3 head => p + Vector3.up * .5f;
    

    private void Start()
    {
        trans = transform;
        p = trans.position;
    }


    private float GetAngle(bool left)
    {
        float a = left? -90 : 90;
        float resulta = a;
        Vector3 f = trans.forward;
                f = Quaternion.AngleAxis(a, Vector3.up) * f;
        while (Physics.Raycast(new Ray(head, f), 1))
        {
            resulta += a;
            f = Quaternion.AngleAxis(a, Vector3.up) * f;
        }

        return resulta;
    }

    
    private void Update()
    {
        if(animating)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            StartCoroutine(Turn(GetAngle(true)));
        else 
        if (Input.GetKeyDown(KeyCode.RightArrow))
            StartCoroutine(Turn(GetAngle(false)));
        else
            if (Input.GetKeyDown(KeyCode.UpArrow))
                StartCoroutine(Walk(false));
        else
            if (Input.GetKeyDown(KeyCode.DownArrow))
                StartCoroutine(Walk(true));
    }


    private float S(float v)
    {
        float a = 1f / fps;
        a *= .5f;
        v = Mathf.Clamp01(v * (1 + a * 2) - a);
        return Mathf.SmoothStep(0, 1, v);
    }
    


    private IEnumerator Turn(float amount)
    {
        animating = true;
        float t = 0;

        Quaternion r = trans.rotation;

        float duration = 1f / 2.55f * (1f + ((Mathf.Abs(amount) -90) / 90f) * .35f);
        float speed = 1f / duration;
        float steps = Mathf.Round(duration * fps);

        while (t < 1)
        {
            t += Time.deltaTime * speed;

            float tStep = t * steps;
                  tStep = (Mathf.Floor(tStep) + S(tStep % 1)) / steps;
                  tStep = Mathf.SmoothStep(0, 1, tStep);
            trans.rotation = r * Quaternion.AngleAxis(Mathf.Clamp01(tStep) * amount, Vector3.up);
            yield return null;
        }

        animating = false;
    }
    
    
    private IEnumerator Walk(bool stepBack)
    {
        animating = true;
        float t = 0;

        
        Vector3 f = trans.forward * (stepBack? -1 : 1);

        const float duration = 1f / 2.8f;
        const float speed = 1f / duration;
        float steps = Mathf.Round(duration * fps);
        int walked = 0;

        while ((!stepBack && Input.GetKey(KeyCode.UpArrow)) || walked == 0)
        {
            walked++;
            if(Physics.Raycast(new Ray(head, f), 1))
                break;
            
            t = t % 1;
            while (t < 1)
            {
                t += Time.deltaTime * speed;

                float tStep = t * steps;
                tStep = (Mathf.Floor(tStep) + S(tStep % 1)) / steps;
                tStep = Mathf.SmoothStep(0, 1, tStep);

                trans.position = p + f * Mathf.Clamp01(tStep);
                yield return null;
            }
            
            p = trans.position;
        }

        animating = false;
    }
}
