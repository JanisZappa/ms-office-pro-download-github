using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ChessStuff;
using TMPro;


namespace ChessStuff
{
    public enum CPiece
    {
        King, Queen, Bishop, Knight, Rook, Pawn
    }

    [System.Serializable]
    public class ChessChar
    {
        public CPiece type;
        public bool teamA;
        public byte tile;
        public int id;
        
        private Quaternion baseRot;
        
        
        private readonly Transform t;

        
        public ChessChar(int id, CPiece type, bool teamA, Mesh[] meshes, Material mat, Transform parent)
        {
            this.id    = id;
            this.type  = type;
            this.teamA = teamA;
            
            GameObject gO = new GameObject(type + (teamA? "_A" : "_B"));
            t = gO.transform;
            t.SetParent(parent);
            
            gO.AddComponent<MeshFilter>().mesh = meshes[(int)type];
            gO.AddComponent<MeshRenderer>().material = mat;
            
            Die();
        }


        public Vector2Int TileCoords
        {
            get
            {
                int y = Mathf.FloorToInt(tile * 1f / 8);
                int x = tile % 8;
                
                return new Vector2Int(x, y);
            }
        }


        private Vector3 Pos
        {
            get
            {
                Vector2Int tileCoords = TileCoords;
                return new Vector3(-3.5f + tileCoords.x, 0, -3.5f + tileCoords.y);
            }
        }


        public void SetTile(byte tile, bool setPos = true)
        {
            this.tile = tile;

            if (setPos)
            {
                SetPos(Pos);
                SetAngle();
            } 
        }


        public void SetPos(Vector3 pos)
        {
            t.localPosition = pos;
        }


        public void SetAngle(float angle = 0)
        {
            t.localRotation = Quaternion.AngleAxis(angle, Vector3.up) * baseRot;
        }


        public void Die()
        {
            t.position = Vector3.one * 1000;
        }


        public void SetBaseRot(bool white)
        {
            baseRot = white? Quaternion.identity : Quaternion.AngleAxis(180, Vector3.up);
        }
    }

    [System.Serializable]
    public class ChessTile
    {
        private readonly Color color;
        private readonly Material mat;
        private static readonly int Color = Shader.PropertyToID("_Color");

        public ChessTile(Color color, Material mat)
        {
            this.color = color;
            this.mat   = mat;
        }


        public void Reset()
        {
            mat.SetColor(Color, color);
        }


        public void Highlight(Color highlight)
        {
            mat.SetColor(Color, UnityEngine.Color.Lerp(color, highlight, .55f));
        }
    }
}


public class Chess : MonoBehaviour
{
    public int    opponent;
    public string opponentName;
    
    [Space]
    public GameObject tilePrefab;
    public GameObject charsPrefab;
    public Material mat;
    
    [Space]
    public Color[] colors;
    public Color[] tileHighlight;
    public Color[] effectColors;
    
    
    private readonly List<ushort> moves     = new List<ushort>(), 
                                  recording = new List<ushort>(), 
                                  messages  = new List<ushort>();
    
    
    private readonly byte[] startLayout = { 4,  3,  2,  5,  1,  6,  0,  7, 
                                            8,  9, 10, 11, 12, 13, 14, 15,
                                           60, 59, 58, 61, 57, 62, 56, 63,
                                           48, 49, 50, 51, 52, 53, 54, 55};
    
    private readonly CPiece[] pieceTypes =
    {
        CPiece.King, CPiece.Queen, CPiece.Bishop, CPiece.Bishop, CPiece.Knight, CPiece.Knight, CPiece.Rook, CPiece.Rook,
        CPiece.Pawn, CPiece.Pawn,  CPiece.Pawn,   CPiece.Pawn,   CPiece.Pawn,   CPiece.Pawn,   CPiece.Pawn, CPiece.Pawn, 
    };
    
    private ChessChar[] chars;
    private ChessChar[] tileChars;
    
    private static readonly int _Color = Shader.PropertyToID("_Color");
    
    private ChessTile[] tiles;
    private int[] hitValues;
    private ChessChar selected;
    
    private bool teamA, yourMove;
    private bool gameOver, animating;
    private int charAnims;
    private bool CharAnim { get { return charAnims > 0; }}
    
    private GameObject hitObj;
    private Material hitMat;
    
    private float hitShake;
    private Transform trans;
    private Vector3 root;


    private ChessApp app;

    
    private bool Animating
    {
        get { return animating || CharAnim; }
    }
    
    private readonly TextMeshPro[] texts = new TextMeshPro[2];


    #region Mono
    public void Init(int opponent, string name, Vector3 pos)
    {
        this.opponent = opponent;
        opponentName = name;
        
        trans = transform;
        SetPos(pos);
        
        app = trans.parent.GetComponent<ChessApp>();
        
        hitObj = trans.GetChild(0).gameObject;
        
        texts[0] = trans.GetChild(1).GetComponent<TextMeshPro>();
        texts[1] = trans.GetChild(2).GetComponent<TextMeshPro>();
        
        texts[0].text = name;
        texts[1].text = "YOU";
        
        hitMat = CreateMat(effectColors[0]);
        hitObj.GetComponent<MeshRenderer>().material = hitMat;
        
        tiles     = new ChessTile[64];
        hitValues = new int[64];
        for (int i = 0; i < 64; i++)
        {
            GameObject gO = Instantiate(tilePrefab);
            gO.SetParent(trans);
            
            int y = Mathf.FloorToInt(i * 1f / 8);
            int x = i % 8;
            gO.transform.localPosition = new Vector3(-3.5f + x, -10, -3.5f + y);
            
            int   id = i;
            int   t = (id + (Mathf.FloorToInt(id * 1.0f / 8) % 2 == 0? 1 : 0)) % 2 == 0? 1 : 0;
            
            Color    c = colors[t];
            Material m = CreateMat(c);
            gO.GetComponent<MeshRenderer>().material = m;
            
            tiles[i] = new ChessTile(c, m);
        }
        
        Mesh[] meshes = new Mesh[6];
        for (int i = 0; i < 6; i++)
            meshes[i] = charsPrefab.transform.GetChild(i).GetComponent<MeshFilter>().sharedMesh;
        
        chars = new ChessChar[32];
        
        for (int i = 0; i < 32; i++)
            chars[i] = new ChessChar(i, pieceTypes[i % 16], i < 16, meshes, CreateMat(colors[i < 16? 2 : 3]), trans);
        
        tileChars = new ChessChar[64];
        
        LoadGame();
    }


    public void SetPos(Vector3 pos)
    {
        trans.position = pos;
        root = pos;
    }
    
    
    private void Update()
    {
        if(Animating)
            return;
        
        if (messages.Count > 0)
        {
            ExecuteMove(messages[0]);
            
            messages.RemoveAt(0);
            return;
        }
        
        if(!ChessCam.Zoomed || ChessCam.Animating)
            return;
        
        int tile = MouseTile;
        if(tile == -1)
            return;
        
        if (yourMove && !gameOver && Input.GetMouseButtonDown(0))
        {
            if (selected == null)
            {
                ChessChar hit = tileChars[tile];
                selected = hit != selected? hit : null;
            }
            else
            {
                int value = hitValues[tile];

                switch (value)
                {
                    case 1:
                    case 2:    
                        ushort move = (ushort)(selected.id * 1000 + tile);
                        ExecuteMove(move);  
                        app.SendMessage(opponent, move);
                        break;
                            
                    case -1:
                        //Debug.Log("Selecting Other");
                        selected = tileChars[tile];
                        break;
                }
            }
            
            CheckTiles();
        }
        
        /*if(Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(PlayRecording());

        if (Input.GetMouseButtonDown(1))
        {
            ExecuteMove(64000);
            app.SendMessage(opponent, 65000);
        }*/
    }


    private void LateUpdate()
    {
        float t = Time.realtimeSinceStartup * 25;
        
        Vector3 p = 
            new Vector3(
                Mathf.PerlinNoise(t, .1f) - Mathf.PerlinNoise(t + 5674.2f, .1f), 
                0, 
                Mathf.PerlinNoise(t + 35674.2f, .1f) - Mathf.PerlinNoise(t + 85674.2f, .1f));
        
        hitShake = Mathf.Max(0, hitShake - Time.deltaTime * 5);
        trans.position = root + p * hitShake * .2f;
        
        t = Time.realtimeSinceStartup;
        texts[0].enabled =  yourMove || t * .75f % 1 < .7f;
        texts[1].enabled =  !yourMove || t * .75f % 1 < .7f;
    }
#endregion


#region Move Check
    private Material CreateMat(Color color)
    {
        Material m = Instantiate(mat);
        m.SetColor(_Color, color);
        return m;
    }


    private static int PosToTile(Vector3 pos)
    {
        pos += Vector3.one * 4;
        Vector2Int tileCoord = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));
        
        return Valid(tileCoord)? tileCoord.x + tileCoord.y * 8 : -1;
    }


    private int MouseTile
    {
        get
        {
            return PosToTile(trans.InverseTransformPoint(ChessCam.TouchPos));
        }
    }


    private static Vector3 TileToPos(int tile)
    {
        int y = Mathf.FloorToInt(tile * 1f / 8);
        int x = tile % 8;
        return new Vector3(-3.5f + x, 0, -3.5f + y);
    }
    
    
    private static bool Valid(Vector2Int tileCoord)
    {
        return tileCoord.x >= 0 && tileCoord.x <= 7 && tileCoord.y >= 0 && tileCoord.y <= 7;
    }


    private int TileValue(Vector2Int tileCoord)
    {
        if(!Valid(tileCoord))
            return -1;
        
        ChessChar cC = tileChars[tileCoord.x + tileCoord.y * 8];
        if(cC == null)
            return 0;
        
        return cC.teamA == teamA? 1 : 2;
    }
    
    
     private void StraighCheck(Vector2Int tileCoord, int dist)
    {
        for (int i = 0; i < 4; i++)
        for (int e = 0; e < dist; e++)
        {
            Vector2Int check = new Vector2Int(0, 1 + e);
            for (int f = 0; f < i; f++)
                check = new Vector2Int(check.y, check.x * -1);
                    
            check += tileCoord;
            int value = TileValue(check);

            if(value == 0 || value == 2)
                hitValues[check.x + check.y * 8] = value == 0? 1 : 2;
                    
            if(value != 0)
                break;
        }
    }


    private void DiagonalCheck(Vector2Int tileCoord, int dist)
    {
        for (int i = 0; i < 4; i++)
        for (int e = 0; e < dist; e++)
        {
            Vector2Int check = new Vector2Int(1 + e, 1 + e);
            for (int f = 0; f < i; f++)
                check = new Vector2Int(check.y, check.x * -1);
                    
            check += tileCoord;
            int value = TileValue(check);
            if(value == 0 || value == 2)
                hitValues[check.x + check.y * 8] = value == 0? 1 : 2;
                    
            if(value != 0)
                break;
        }
    }


    private void CheckTiles()
    {
        for (int i = 0; i < 64; i++)
            hitValues[i] = -1;

        if (selected != null && selected.teamA == teamA)
        {

            hitValues[selected.tile] = 0;

            Vector2Int tileCoord = selected.TileCoords;
            Vector2Int check;
            int value;

            switch (selected.type)
            {
                case CPiece.King:
                    StraighCheck(tileCoord, 1);
                    DiagonalCheck(tileCoord, 1);
                    break;

                case CPiece.Queen:
                    StraighCheck(tileCoord, 7);
                    DiagonalCheck(tileCoord, 7);
                    break;

                case CPiece.Bishop:
                    DiagonalCheck(tileCoord, 7);
                    break;

                case CPiece.Knight:
                    for (int i = 0; i < 4; i++)
                    for (int e = 0; e < 2; e++)
                    {
                        check = new Vector2Int(-1 + 2 * e, 2);
                        for (int f = 0; f < i; f++)
                            check = new Vector2Int(check.y, check.x * -1);

                        check += tileCoord;
                        value = TileValue(check);
                        if (value == 0 || value == 2)
                            hitValues[check.x + check.y * 8] = value == 0 ? 1 : 2;
                    }

                    break;


                case CPiece.Rook:
                    StraighCheck(tileCoord, 7);
                    break;


                case CPiece.Pawn:
                    int sign = teamA ? 1 : -1;
                    bool starting = teamA ? tileCoord.y == 1 : tileCoord.y == 6;

                    check = tileCoord + new Vector2Int(-1, 1 * sign);
                    value = TileValue(check);

                    if (value == 2)
                        hitValues[check.x + check.y * 8] = 2;

                    check = tileCoord + new Vector2Int(1, 1 * sign);
                    value = TileValue(check);

                    if (value == 2)
                        hitValues[check.x + check.y * 8] = 2;


                    check = tileCoord + new Vector2Int(0, 1 * sign);
                    value = TileValue(check);

                    if (value == 0)
                    {
                        hitValues[check.x + check.y * 8] = 1;

                        if (starting)
                        {
                            check = tileCoord + new Vector2Int(0, 2 * sign);
                            value = TileValue(check);

                            if (value == 0)
                                hitValues[check.x + check.y * 8] = 1;
                        }
                    }

                    break;
            }
        }

    //  Mark  Tiles  //
        for (int i = 0; i < 64; i++)
        {
            int value = hitValues[i];
            if(value != -1)
                tiles[i].Highlight(tileHighlight[value]);
            else
                tiles[i].Reset();
        }
    }
#endregion


    private void CharReset()
    {
        Quaternion r = Quaternion.AngleAxis(teamA? 0 : 180, Vector3.up);
        trans.rotation = r;
        r = r * Quaternion.AngleAxis(90, Vector3.right);
        texts[0].transform.localRotation = r;
        texts[1].transform.localRotation = r;
        
        for (int i = 0; i < 32; i++)
            chars[i].SetBaseRot(teamA);
        
        for (int i = 0; i < 32; i++)
            chars[i].SetTile(startLayout[i]);

        for (int i = 0; i < 64; i++)
            tileChars[i] = null;
        
        for (int i = 0; i < 32; i++)
        {
            ChessChar cC = chars[i];
            tileChars[cC.tile] = cC;
        }
    }


    private void ExecuteMove(ushort move, bool animate = true)
    {
        int id = Mathf.FloorToInt(move / 1000f);

        switch (id)
        {
            default:
                int tile = move % 1000;
        
                ChessChar hitChar  = tileChars[tile];
                ChessChar moveChar = chars[id];
                tileChars[moveChar.tile] = null;
                tileChars[tile]    = moveChar;

                if (animate)
                    StartCoroutine(CharMove(moveChar, hitChar, tile));
                else
                {
                    hitChar?.Die();
                    moveChar.SetTile((byte)tile);
                }
            
                yourMove = !yourMove;
                break;
            
            case 65:
                NewGame(true);
                break;
            case 64:
                NewGame(false);
                break;
        }
        
        moves.Add(move);
        if(!animating)
            SaveRecording();
        
        selected = null;
    }


    private void NewGame(bool white)
    {
        Debug.LogFormat("{0}: Playing " + (white? "White" : "Black"), opponentName);
        
        teamA    = white;
        yourMove = teamA;
        gameOver = false;
        
        moves.Clear();
        SaveRecording();
        
        selected = null;
        
        CheckTiles();
        CharReset();
    }


    public void SetMessage(ushort message)
    {
        messages.Add(message);
    }


#region Recording
    private string RecordingPath { get { return ChessApp.Folder + "\\" + opponent + ".chess"; }}

    
    private void SaveRecording()
    {
        int count = moves.Count;
        Debug.LogFormat("{1}: Saving {0} Moves", count, opponentName);
        using (MemoryStream m = new MemoryStream())
        using (BinaryWriter w = new BinaryWriter(m))
        {
            w.Write(count);
            for (int i = 0; i < count; i++)
                w.Write(moves[i]);
            
            File.WriteAllBytes(RecordingPath, m.ToArray());
        }
    }


    private int LoadRecording()
    {
        recording.Clear();
        
        if(!File.Exists(RecordingPath))
            return 0;
        
        int count;
        using (MemoryStream m = new MemoryStream(File.ReadAllBytes(RecordingPath)))
        using (BinaryReader w = new BinaryReader(m))
        {
            count = w.ReadInt32();
            for (int i = 0; i < count; i++)
                recording.Add(w.ReadUInt16());
        }
        
        Debug.LogFormat("{1}: Loaded {0} Moves", count, opponentName);
        
        return count;
    }

    
    private IEnumerator PlayRecording()
    {
        animating = true;
        
        int count = LoadRecording();
            
        yield return new WaitForSeconds(.7f);
        
        for (int i = 0; i < count; i++)
        {
            ExecuteMove(recording[i]);
            while(CharAnim)
                yield return null;
            
            yield return new WaitForSeconds(.285f);
        }
        
        animating = false;
    }


    private void LoadGame()
    {
        int count = LoadRecording();
        
        for (int i = 0; i < count; i++)
            ExecuteMove(recording[i], false);
    }
    
    
#endregion


#region Animation
    private IEnumerator CharMove(ChessChar moveChar, ChessChar hitChar, int tile)
    {
        charAnims++;
        
        Vector3 a = TileToPos(moveChar.tile).SetY(2);
        Vector3 b = TileToPos(tile).SetY(2);
        
        float dist  = (b - a).magnitude;
        float multi = Mathf.Lerp(2f, 6.5f / dist, .45f) * 1.05f;
        
        bool charGotHit = hitChar == null;
        
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * multi;
            
            Vector3 p = Vector3.Lerp(a, b, Mathf.SmoothStep(0, 1, Mathf.Clamp01(t)));
            
            moveChar.SetPos(p);

            if (!charGotHit && (p - b).sqrMagnitude <= .5f * .5f)
            {
                charGotHit = true;
                
                if (!animating && hitChar.type == CPiece.King)
                    gameOver = true;
                
                hitShake = hitChar.type == CPiece.King? 2 : .45f;
            
                StartCoroutine(DeathAnim(hitChar));
                StartCoroutine(HitAnim(b));
            }
            
            yield return null;
        }
        
        moveChar.SetTile((byte)tile);
        
        charAnims--;
    }


    private IEnumerator DeathAnim(ChessChar hitChar)
    {
        charAnims++;
        
        Vector3 a = TileToPos(hitChar.tile).SetY(4);
        float y = 1;
        float x = Random.Range(.3f, 1) * .1f * (Random.Range(0, 2) == 0? -1 : 1);
        float r = Random.Range(.5f, 1) * 300f * (Random.Range(0, 2) == 0? -1 : 1);
        float angle = 0;
        float fallDir = teamA? 1 : -1;

        while (true)
        {
            float t = Time.deltaTime * 5;
            y -= t;
            x *= 1f - t * .1f;
            r *= 1f - t * .2f;
            angle += r * t;
            
            a += new Vector3(x, 0, y * fallDir) * t;
            
            hitChar.SetPos(a);
            hitChar.SetAngle(angle);

            if (teamA && a.z < -10 || 
               !teamA && a.z > 10)
            {
                hitChar.Die();
                if(!animating && gameOver)
                    StartCoroutine(GameOverAnim());
                break;
            }
            
            yield return null;
        }
        
        charAnims--;
    }


    private IEnumerator GameOverAnim()
    {
        yield return StartCoroutine(app.ui.ShowMessage(yourMove? "You lost..." : "You won!"));
        gameOver = false;
        
        yield return StartCoroutine(PlayRecording());
        yield return StartCoroutine(app.ui.ShowMessage("<size=22>New Game with</size>\n" + opponentName));
            
        if (!yourMove)
        {
            ExecuteMove(64000);
            app.SendMessage(opponent, 65000);
        }
    }
    
    
    private IEnumerator HitAnim(Vector3 pos)
    {
        hitObj.SetActive(true);
        hitObj.transform.localPosition = pos.SetY(6);
        hitObj.transform.rotation      = Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);
        
        hitMat.SetColor(_Color, Color.Lerp(effectColors[0], effectColors[1], Random.Range(0, 1f)));
            
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 15.5f;
            hitObj.transform.localScale = Vector3.one * (.3f + .7f * t);
            yield return null;
        }
        
        
        hitObj.SetActive(false);
    }
#endregion
}
