using System.IO;
using UnityEngine;


public class FlightCore : MonoBehaviour
{
    public TextAsset data;
    
    public float speed = 3000;
    
    public Vector2 movementRange;
    
    public static float Speed;
    public static float Dist;
    public static Vector2 Range;
    
    private static Placement[] path;
    private static int count, step;
    
    
    public static bool Stop;
    public static float CurrentTime;
    
    
    
    private void Start()
    {
        using (MemoryStream m = new MemoryStream(data.bytes))
        using (BinaryReader reader = new BinaryReader(m))
        {
            count = reader.ReadInt32();
            step  = reader.ReadInt32();
            path  = new Placement[count];
            
            Quaternion flip = Quaternion.AngleAxis(180, Vector3.forward);
            for (int i = 0; i < count; i++)
            {
                Vector3    p = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Quaternion r = Quaternion.Euler(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                path[i] = new Placement(p, r);
            }
        }
    }


    private void Update()
    {
        if(PauseToggle)
            Stop = !Stop;
        
        if(Stop)
            return;
        
        CurrentTime += Time.deltaTime;
       
        Speed = speed;
        Dist += Speed * Time.deltaTime;
        
        Range = movementRange;
    }


    public static Placement Sample(float dist)
    {
        float t = Mathf.Max(0, (Dist + dist) / step);
        int index = Mathf.FloorToInt(t);
        return Placement.Lerp(path[index % count], path[(index + 1) % count], t % 1);
        
        int a = (index - 1).Repeat(count), b = index % count, c = (index + 1) % count, d = (index + 2) % count;
        
        //Debug.LogFormat("{0} {1} {2} {3}", a, b, c, d);
        return DeCasteljausAlgorithmPlacement(
            path[a],
            path[b],
            path[c],
            path[d],
            t % 1);
    }


    public static float WrapDist(float dist)
    {
        return Mathf.Max(0, Dist + dist) % (count * step);
    }
    
    
    private static Vector3 DeCasteljausAlgorithm(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
    {
        //Linear interpolation = lerp = (1 - t) * A + t * B
        //Could use Vector3.Lerp(A, B, t)

        //To make it faster
        float oneMinusT = 1f - t;
        
        //Layer 1
        Vector3 Q = oneMinusT * A + t * B;
        Vector3 R = oneMinusT * B + t * C;
        Vector3 S = oneMinusT * C + t * D;

        //Layer 2
        Vector3 P = oneMinusT * Q + t * R;
        Vector3 T = oneMinusT * R + t * S;

        //Final interpolated position
        return oneMinusT * P + t * T;
    }
    
    
    private static Vector3 DeCasteljausAlgorithmPos(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
    {
        //Layer 1
        Vector3 Q = Vector3.Lerp(A, B, t);
        Vector3 R = Vector3.Lerp(B, C, t);
        Vector3 S = Vector3.Lerp(C, D, t);

        //Layer 2
        Vector3 P = Vector3.Lerp(Q, R, t);
        Vector3 T = Vector3.Lerp(R, S, t);

        //Final interpolated position
        return Vector3.Lerp(P, T, t);
    }
    
    
    private static Quaternion DeCasteljausAlgorithmRot(Quaternion A, Quaternion B, Quaternion C, Quaternion D, float t)
    {
        //Layer 1
        Quaternion Q = Quaternion.Lerp(A, B, t);
        Quaternion R = Quaternion.Lerp(B, C, t);
        Quaternion S = Quaternion.Lerp(C, D, t);

        //Layer 2
        Quaternion P = Quaternion.Lerp(Q, R, t);
        Quaternion T = Quaternion.Lerp(R, S, t);

        //Final interpolated position
        return Quaternion.Lerp(P, T, t);
    }
    
    
    private static Placement DeCasteljausAlgorithmPlacement(Placement A, Placement B, Placement C, Placement D, float t)
    {
        t = .25f + t * .5f;
        return new Placement(DeCasteljausAlgorithmPos(A.pos, B.pos, C.pos, D.pos, t), 
                             DeCasteljausAlgorithmRot(A.rot, B.rot, C.rot, D.rot, t));
    }


    public void OnDrawGizmos()
    {
        if (false && path != null)
        {
            int c = Mathf.RoundToInt(Dist / step) % count;
            int mi = Mathf.Max(0, c - 50);
            int ma = Mathf.Min(count, c + 50);
            for (int i = mi; i < ma; i++)
            {
                Gizmos.color = i % 2 == 0? Color.red : Color.yellow;
                Gizmos.DrawLine(path[i].pos, path[(i + 1) % count].pos);
            }
        }  
    }


    public static bool PauseToggle
    {
        get
        {
            return Input.GetKeyDown(KeyCode.Space) || 
                   Input.GetKeyDown(KeyCode.JoystickButton7);
        }
    }
}
