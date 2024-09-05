using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;

public class OwnTextRenderer : MonoBehaviour
{
    public TMP_FontAsset font;

    private Mesh mesh;
    private readonly List<int> tri = new List<int>();
    private readonly List<Vector3> verts = new List<Vector3>();
    private readonly List<Vector2> uvs   = new List<Vector2>();

    private Vector2 texSize;
    private static readonly int FaceColor = Shader.PropertyToID("_FaceColor");
    private static readonly int OutlineSoftness = Shader.PropertyToID("_OutlineSoftness");
    private const float f = 1f / 1024f * 28.5f;
    
    private readonly List<string> chapters = new List<string>(10000);
    private readonly StringBuilder builder = new StringBuilder(1000000);

    private const float xMargin = 10, yMargin = 12;

    private static float Line => (148f - yMargin * 2) / -36f;


    private void Start()
    {
        mesh = new Mesh();
        mesh.MarkDynamic();
        gameObject.AddComponent<MeshFilter>().mesh = mesh;

        Material mat = Instantiate(font.material);
        mat.SetColor(FaceColor, Color.red);
        //mat.SetFloat(OutlineSoftness, .05f);
        gameObject.AddComponent<MeshRenderer>().material = mat;
        
        texSize = new Vector2(font.atlasTexture.width, font.atlasTexture.height);
        
        chapters.AddRange(BookRW.ReadLines("Book000001", "Chapter000001"));
        for (int i = 0; i < 10; i++)
            builder.Append("  ").AppendLine(chapters[i]);

        string v = builder.ToString();
        CreateMesh(v);
        CalculatePage(v);
    }


    private void Update()
    {
        DRAW.Rectangle(Vector3.zero, new Vector3(105, 148)).SetColor(Color.red.A(.1f));
        DRAW.Rectangle(Vector3.zero, new Vector3(105 - xMargin * 2, 148 - yMargin * 2)).SetColor(Color.red.A(.1f));
    }

    
    private void CreateMesh(string value)
    {
          tri.Clear();
        verts.Clear();
          uvs.Clear();

        Vector3 offset = Vector3.up * Line * .75f;

        const float xMax = 105 - xMargin * 2;
        int lines = 0;
        int steps = 0;
        foreach (char character in value)
        {
            bool space = character == ' ';
            if (space && offset.x >= xMax || character == '\n')
            {
                offset.x = 0;
                offset.y += Line;
                lines++;

                if (lines == 36)
                    break;
                continue;
            }
            Glyph g = font.characterLookupTable[character].glyph;
           
            offset = GetChar(g, steps++, offset, space? 1.75f : 1f);
        } 
        
        mesh.SetVertices(verts);
        mesh.SetTriangles(tri, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateBounds();
    }
    
    
    private void CalculatePage(string value)
    {
        float t = Time.realtimeSinceStartup;
        
        Vector3 offset = Vector3.up * Line * .75f;

        const float xMax = 105 - xMargin * 2;
        int lines = 0;
        foreach (char character in value)
        {
            bool space = character == ' ';
            if (space && offset.x >= xMax || character == '\n')
            {
                offset.x = 0;
                offset.y += Line;
                lines++;

                if (lines == 36)
                    break;
                continue;
            }
            Glyph g = font.characterLookupTable[character].glyph;
           
            offset = GetOffset(g, offset, space? 1.75f : 1f);
        } 
        
        Debug.Log(Time.realtimeSinceStartup - t);
    }


    private Vector3 GetChar(Glyph g, int id, Vector3 offset, float multi = 1)
    {
        const float e = 0, ee = 0f;
        GlyphRect rect = g.glyphRect;
        GlyphMetrics m = g.metrics;
    
        Vector2 size = new Vector2(m.width, m.height) * f;
        //Vector3 shift = new Vector2(m.horizontalBearingX + glyphAdjustments.xPlacement, m.horizontalBearingY + glyphAdjustments.yPlacement) * f;    
        Vector3 shift = new Vector2(m.horizontalBearingX, m.horizontalBearingY) * f + new Vector2(xMargin - 52.5f, -yMargin + 74);  
        
        verts.Add(new Vector3(-e,         -size.y -e, 0) + offset + shift);
        verts.Add(new Vector3(-e,                  e, 0) + offset + shift);
        verts.Add(new Vector3(e + size.x,          e, 0) + offset + shift);
        verts.Add(new Vector3(e + size.x, -size.y -e, 0)  + offset + shift);
        
        
        
        Vector2 min = new Vector2(rect.x / texSize.x, rect.y / texSize.y);
        Vector2 max = new Vector2(rect.width / texSize.x, rect.height / texSize.y) + min;
        
        uvs.Add(new Vector2(min.x - ee, min.y - ee));
        uvs.Add(new Vector2(min.x - ee, max.y + ee));
        uvs.Add(new Vector2(max.x + ee, max.y + ee));
        uvs.Add(new Vector2(max.x + ee, min.y - ee));

        id *= 4;
        tri.Add(id);
        tri.Add(id + 1);
        tri.Add(id + 2);
        
        tri.Add(id + 2);
        tri.Add(id + 3);
        tri.Add(id);
        
        return offset + Vector3.right * m.horizontalAdvance * f * multi;
    }
    
    
    private Vector3 GetOffset(Glyph g, Vector3 offset, float multi = 1)
    {
        return offset + Vector3.right * g.metrics.horizontalAdvance * f * multi;
    }
    
}


/*TMP_GlyphValueRecord glyphAdjustments = new TMP_GlyphValueRecord();
            uint baseGlyphIndex = g.index;

            if (i < cCount - 1)
            {
                uint nextGlyphIndex = font.characterLookupTable[c[i + 1]].glyph.index;
                uint key = nextGlyphIndex << 16 | baseGlyphIndex;

                //if (font.fontFeatureTable.m_GlyphPairAdjustmentRecordLookupDictionary.TryGetValue(key, out TMP_GlyphPairAdjustmentRecord adjustmentPair))
                //    glyphAdjustments = adjustmentPair.firstAdjustmentRecord.glyphValueRecord;
            }

            if (i >= 1)
            {
                uint key = baseGlyphIndex << 16 | previousGlyphIndex;

                //if (font.fontFeatureTable.m_GlyphPairAdjustmentRecordLookupDictionary.TryGetValue(key, out TMP_GlyphPairAdjustmentRecord adjustmentPair))
                //    glyphAdjustments += adjustmentPair.secondAdjustmentRecord.glyphValueRecord;
            }

            previousGlyphIndex = baseGlyphIndex;*/