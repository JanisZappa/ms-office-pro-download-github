using UnityEngine;

[System.Serializable]
public struct Placement
{
    public Vector3    pos;
    public Quaternion rot;
        
        
    public Placement(Vector3 pos, Quaternion rot)
    {
        this.pos = pos;
        this.rot = rot;
    }
        
        
    public Placement(Vector3 pos, float angle)
    {
        this.pos = pos;
        rot = Rot.Z(angle);
    }


    public Placement(Transform transform, bool local = false)
    {
        if (local)
        {
            pos = transform.localPosition;
            rot = transform.localRotation;
        }
        else
        {
            pos = transform.position;
            rot = transform.rotation;
        }
    }

    public Placement(Transform transform, Transform relativeTo)
    {
        pos = relativeTo.InverseTransformPoint(transform.position);
        rot = Quaternion.Inverse(relativeTo.rotation) * transform.rotation;
    }
        
    public static Placement Lerp(Placement placementA, Placement placementB, float lerp)
    {
        return new Placement(Vector3.Lerp(placementA.pos, placementB.pos, lerp), 
            Quaternion.Slerp(placementA.rot, placementB.rot, lerp));
    }

    public static Vector3 PosLerp(Placement placementA, Placement placementB, float lerp)
    {
        return Vector3.Lerp(placementA.pos, placementB.pos, lerp);
    }
        
    public static Placement OutOfSight = new Placement(V3.away, Rot.Zero);
        
    public static Placement Zero = new Placement(V3.zero, Quaternion.identity);


    public static bool operator ==(Placement a, Placement b)
    {
        return a.pos == b.pos && a.rot == b.rot;
    }

    public static bool operator !=(Placement a, Placement b)
    {
        return  a.pos != b.pos || a.rot != b.rot;
    }


    public static Placement operator -(Placement a, Placement b)
    {
        return new Placement(a.pos - b.pos, a.rot * Quaternion.Inverse(b.rot));
    }
        
    public static Placement operator +(Placement a, Placement b)
    {
        return new Placement(a.pos + b.pos, a.rot * b.rot);
    }

    public static Placement operator +(Placement a, Vector3 b)
    {
        return new Placement(a.pos + b, a.rot);
    }
    public static Placement operator -(Placement a, Vector3 b)
    {
        return new Placement(a.pos - b, a.rot);
    }
    
    public void DebugIfNaN(string name)
    {
        if(float.IsNaN(pos.x))
            Debug.LogFormat("{0}_X is fucked", name);
        if(float.IsNaN(pos.y))
            Debug.LogFormat("{0}_Y is fucked", name);
    }


    public void Apply(Transform trans, bool local = false)
    {
        if (local)
        {
            trans.localPosition = pos;
            trans.localRotation = rot;
        }
        else
        {
            trans.position = pos;
            trans.rotation = rot;
        }
    }


    public Matrix4x4 Matrix
    {
        get
        {
            return Matrix4x4.TRS(pos, rot, Vector3.one);
        }
    }
    
    
    public Vector3 up { get { return rot * Vector3.up; }}
    public Vector3 right { get { return rot * Vector3.right; }}
    public Vector3 forward { get { return rot * Vector3.forward; }}
}
