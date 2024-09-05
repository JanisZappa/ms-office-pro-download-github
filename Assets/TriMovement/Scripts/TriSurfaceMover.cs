using UnityEngine;

public class TriSurfaceMover : MonoBehaviour
{
    public TriSurface s;
    public float speed;
    private TriPos pos;
    
    private void Start()
    {
        pos = s.GetRandomTriPos();
    }

    
    private void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.N))
            pos = s.GetRandomTriPos();
        
        pos = s.Move(pos, Time.deltaTime * speed);
        
        Vector3 p = s.GetPos(pos, true);
        Vector3 n = s.GetNormal(pos, true);
        Vector3 f = Vector3.Cross(n, Vector3.Cross(n, pos.dir).normalized).normalized;

        transform.position = p;
        
        
        
        transform.rotation = Quaternion.LookRotation(f, n);

        //DRAW.Vector(p, n * .25f).SetColor(Color.white);
        //DRAW.Vector(p + n * .025f, s.transform.TransformVector(pos.dir) * .125f).SetColor(Color.green);
    }
}
