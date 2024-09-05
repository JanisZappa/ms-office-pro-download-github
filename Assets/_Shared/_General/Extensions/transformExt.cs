using UnityEngine;


public static class transformExt 
{
    public static Transform GetChild(this Transform trans, string name, bool containsIsEnough = false)
    {
        Transform[] children = trans.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
            if (containsIsEnough ? children[i].name.Contains(name) : children[i].name == name)
                return children[i];

        return null;
    }


    public static Transform[] GetChildren(this Transform t)
    {
        int count = t.childCount;
        Transform[] children = new Transform[count];
        for (int i = 0; i < count; i++)
            children[i] = t.GetChild(i);
        
        return children;
    }


    public static void SetPlacement(this Transform trans, Placement placement)
    {
        trans.position = placement.pos;
        trans.rotation = placement.rot;
    }


    public static Transform MakeChildOf(this Transform trans, Transform parent)
    {
        trans.SetParent(parent, true);
        return trans;
    }


    public static Transform CopyPose(this Transform trans, Transform other, bool local = false)
    {
        if (local)
        {
            trans.localPosition = other.localPosition;
            trans.localRotation = other.localRotation;
        }
        else
        {
            trans.position = other.position;
            trans.rotation = other.rotation;
        }
        
        return trans;
    }


    public static Placement GetPlacement(this Transform trans, bool local = false)
    {
        return local ? new Placement(trans.localPosition, trans.localRotation) : 
                       new Placement(trans.position, trans.rotation);
    }
}
