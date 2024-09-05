using UnityEngine;


public class MaterialTest : MonoBehaviour
{
    public Color color;
    
    [Range(0, 1)]
    public float glow, roughness, amount;
    [Range(-1, 5)]
    public float bump;
    
    private Mesh mesh;
    private Color[] setColors;
    private Vector4[] data;
    private Vector2[] uv;
    private int count;
    
    
    private void Start()
    {
        MeshFilter mF = GetComponent<MeshFilter>();
        mesh = Instantiate(mF.mesh);
        mF.mesh = mesh;
        
        count = mesh.vertexCount;
        setColors = new Color[count];
        data = new Vector4[count];
        uv = mesh.uv;
        
        MeshUpdate();
    }


    private void MeshUpdate()
    {
        Color c = new Color(color.r, color.g, color.b, 1f - glow);
        for (int i = 0; i < count; i++)
            setColors[i] = c;
        
        mesh.SetColors(setColors);

        for (int i = 0; i < count; i++)
        {
            Vector2 uvs = uv[i];
            data[i] = new Vector4(uvs.x, uvs.y, roughness + 99 - amount * 99, bump);
        }
            
        mesh.SetUVs(0, data);
    }


    private void OnValidate()
    {
        if(count == 0)
            return;
        
        MeshUpdate();
    }
}
