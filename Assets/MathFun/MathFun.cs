using GeoMath;
using UnityEngine;


public class MathFun : MonoBehaviour
{
    private Vector2 p1;
    private Camera cam;
    
    private float t1;
    
    private Capsule box;
    private IGJK[] gjkShapes;
    
    private const int triCount = 400;
    
    
    private void Start()
    {
        p1 = Random.insideUnitCircle * 2;
        
        cam = Camera.main;

        gjkShapes = new IGJK[triCount];
        for (int i = 0; i < triCount; i++)
            gjkShapes[i] = GetShape(Random.Range(0, 4));
    }


    private static IGJK GetShape(int i)
    {
        Vector2 pos = (Random.insideUnitCircle * 5).MultiX(16f / 9);
        float   rad = Random.Range(0, Mathf.PI * 2), angle = rad * Mathf.Rad2Deg;
        switch (i)
        {
            default:   return new Triangle(pos, Random.Range(0.5f, 1) * .5f, rad);
            case 1:    return new Circle(pos, Random.Range(0.5f, 1) * .5f);
            case 2:    return new Box(pos, angle, new Vector2(Random.Range(0.5f, 1), Random.Range(0.5f, 1)) * .5f);
            case 3:    return new Capsule(pos, Random.Range(0.75f, 1.5f) * .5f, Random.Range(0.2f, .4f) * .5f, angle);
        }
    }
    
    
    private void Update()
    {
        Vector2 mP = cam.CursorOrthoPos();
        
        if(Input.GetKey(KeyCode.Mouse0))
            p1 = mP;
            
        float dt = Time.deltaTime * 14;
              t1 += dt;
                
        //box = new Box(p1, t1, new Vector2(1, (1 + (Mathf.Sin(t1 * .075f) * .5f + .5f) * .7f) * 3));
        box = new Capsule(p1, (1 + (Mathf.Sin(t1 * .075f) * .5f + .5f) * 1.7f) * 3, .25f, t1);
        
        
        int hits = 0;
        float time = 0;
        for (int i = 0; i < triCount; i++)
        {
            float start = Time.realtimeSinceStartup;
            IGJK s = gjkShapes[i];
            GJKHit hit = GJK.IntersectionHit(box, s);
            time += Time.realtimeSinceStartup - start;
            //s.Draw(hit.hit? COLOR.yellow.fresh : COLOR.purple.orchid);

            if (hit.hit)
            {
                hits++;
                //hit.DrawNormal(-hit.normal * hit.depth);
                s.Shift(-hit.normal * hit.depth).Draw(COLOR.purple.orchid);//COLOR.yellow.fresh);
                //s.Draw(Color.white.A(.2f));
            }
            else
            {
                s.Draw(COLOR.purple.orchid);
            }
                
            
            //s.GetBounds.PadDir((s.Center - mP) * .1f).Draw(Color.white.A(.25f));
        }
        
        //Debug.Log(time);
        
        
        Color boxColor = hits > 0? COLOR.yellow.fresh : COLOR.red.tomato;
        box.Draw(boxColor);
        DRAW.MultiCircle(p1, .15f, 3, .04f, 30).SetColor(boxColor);
        //box.GetBounds.Draw(Color.white.A(.25f));
    }
}
