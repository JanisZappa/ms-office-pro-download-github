using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LabyrinthTileSetup : MonoBehaviour
{
    public Material main, strokes, lights, glow, smoke, windows;

    [Space] 
    public bool showStrokes;
    public bool parseColliders;

    [Space] public TextAsset colliderData;
    public Transform colliders;
    
#if UNITY_EDITOR
    public void Setup()
    {
        MeshRenderer[] mr = GetComponentsInChildren<MeshRenderer>();
        int count = mr.Length;

        for (int i = 0; i < count; i++)
        {
            MeshRenderer m = mr[i];
            GameObject go = m.gameObject;
            if (m.name.Contains("collider"))
            {
                m.enabled = false;
                go.layer = LayerMask.NameToLayer("HardWalls");
                MeshCollider mc = m.gameObject.GetComponent<MeshCollider>();
                if (mc == null)
                    mc = m.gameObject.AddComponent<MeshCollider>();

                mc.sharedMesh = mc.GetComponent<MeshFilter>().sharedMesh;;
                goto donenostatic;
            }
            if (Detected(m, "main"))
                goto done;
            if (Detected(m, "lights"))
                goto done;
            if (Detected(m, "glow"))
                goto done;
            if (Detected(m, "smoke"))
                goto done;
            if (Detected(m, "windows"))
                goto done;
            if (Detected(m, "strokes"))
            {
                m.enabled = showStrokes;
                goto done;
            }

            done:
            go.isStatic = true;
            donenostatic:
            EditorUtility.SetDirty(go);
        }

        int c = colliders.childCount - 1;
        for (int i = c; i > -1; i--)
            DestroyImmediate(colliders.GetChild(i).gameObject);
        
        EditorUtility.SetDirty(colliders.gameObject);

        
            using (MemoryStream m = new MemoryStream(colliderData.bytes))
            using (BinaryReader r = new BinaryReader(m))
            {
                int boxes = r.ReadInt32();
                for (int i = 0; i < boxes; i++)
                {
                    Vector3 min = V(r);
                    Vector3 max = V(r);
                    if(!parseColliders)
                        continue;
                    
                    Vector3 size = max - min;
                    Vector3 center = min + size * .5f;
                    GameObject b = new GameObject("Box");
                    b.transform.position = center;
                    BoxCollider box = b.AddComponent<BoxCollider>();
                    box.size = size;
                    b.layer = LayerMask.NameToLayer("HardWalls");
                    b.transform.SetParent(colliders);
                    //b.isStatic = true;
                    EditorUtility.SetDirty(b);
                }
                int poles = r.ReadInt32();
                for (int i = 0; i < poles; i++)
                {
                    Vector3 pos = V(r);
                    GameObject b = new GameObject("Pole");
                    b.transform.position = pos;
                    CapsuleCollider capsule = b.AddComponent<CapsuleCollider>();
                    capsule.height = 7;
                    capsule.radius = r.ReadSingle();
                    b.layer = LayerMask.NameToLayer("HardWalls");
                    b.transform.SetParent(colliders);
                    //b.isStatic = true;
                    EditorUtility.SetDirty(b);
                }
            }
    }


    private Vector3 V(BinaryReader r)
    {
        return new Vector3(r.ReadSingle() * -10, r.ReadSingle() * 10, r.ReadSingle() * 10);
    }


    private Material Mat(string v)
    {
        switch (v)
        {
            default:        return main;
            case "lights":  return lights;
            case "glow":    return glow;
            case "smoke":   return smoke;
            case "strokes": return strokes;
            case "windows": return windows;
        }
    }

    private bool Detected(MeshRenderer mR, string v)
    {
        if (mR.name.Contains(v))
        {
            mR.material = Mat(v);
            return true;
        }

        return false;
    }
#endif
}


#if UNITY_EDITOR
[CustomEditor(typeof(LabyrinthTileSetup))]
public class LabyrinthTileSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        GUILayout.Space(10);
        
        if(GUILayout.Button("Setup"))
            (target as LabyrinthTileSetup)?.Setup();
    }
}
#endif