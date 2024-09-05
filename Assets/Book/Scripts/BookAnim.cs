using System;
using System.Collections;
using UnityEngine;


[ExecuteInEditMode]
public class BookAnim : MonoBehaviour
{
    public Material mat;

    [Space]
    public AnimationCurve turn;
    public AnimationCurve bend;

    [Space] public float speed = 1.4f;

    [Space] 
    public float bulgeL;
    public float bulgeR;
    
    public bool animating;


    private void OnDisable()
    {
        mat.SetFloat(Turn, 0);
        mat.SetFloat(Bend, 0);
    }


    public void TurnPage(bool back, Action callBack)
    {
        StartCoroutine(Anim(back, callBack));
    }

    private IEnumerator Anim(bool back, Action callBack)
    {
        animating = true;
        float t = 0;
        
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            float anim = Mathf.Clamp01(t);
            if (back)
            {
                mat.SetFloat(Turn, 1f - turn.Evaluate(anim));
                mat.SetFloat(Bend, bend.Evaluate(anim) * -.45f);
            }
            else
            {
                mat.SetFloat(Turn, turn.Evaluate(anim));
                mat.SetFloat(Bend, bend.Evaluate(anim) * .45f); 
            }

            if (t >= 1)
            {
                animating = false;
                callBack.Invoke();

                if (!back)
                {
                    mat.SetFloat(Turn, 0);
                    mat.SetFloat(Bend, 0); 
                }
            }   
            else
                yield return null;
        }
    }


    private void Update()
    {
        Shader.SetGlobalFloat(BulgeL, bulgeL);
        Shader.SetGlobalFloat(BulgeR, bulgeR);
    }
    
    
    private static readonly int Turn   = Shader.PropertyToID("_Turn");
    private static readonly int Bend   = Shader.PropertyToID("_Bend");
    private static readonly int BulgeL = Shader.PropertyToID("_BulgeL");
    private static readonly int BulgeR = Shader.PropertyToID("_BulgeR");
}
