using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ChessApp : MonoBehaviour
{
    public int playerID;
    public string playerName;
    public string playerCode;
    
    
    private readonly List<Chess> games = new List<Chess>();
    
    [Space]
    public bool startFresh;
    
    [Space]
    public List<Opponent> opponents; 
    private readonly WaitForSeconds MessageFrequency = new WaitForSeconds(2);
    
    private GameObject prefab;
    [HideInInspector]public ChessUI ui;
    
    private Transform newGame;
    
    
    [Serializable]
    public class Opponent
    {
        public string name;
        public int id;
        
        public Opponent(int id, string name)
        {
            this.id   = id;
            this.name = name;
        }

        public bool Same(int id, string name)
        {
            return this.id == id && this.name == name;
        }
    }


    #region Mono
    private void Start()
    {
        ui = transform.GetChild(0).GetComponent<ChessUI>();
        ui.Init();
        newGame = transform.GetChild(1);
        
        ChessCam.FrameGames();
        StartCoroutine(PlayerInit());
    }


    private void Update()
    {
        if(ChessCam.Animating)
            return;
        
        if (!ChessCam.Zoomed && !ChessUI.IsActive && Input.GetMouseButtonDown(0))
        {
            int hit = PosToId(ChessCam.TouchPos);
            if (hit != -1)
            {
                if(hit == games.Count)
                    StartCoroutine(NewGameButton());
                else
                {
                    Debug.Log("Clicked a Game");
                    ChessCam.Zoom(hit);
                }
            }
        }

        if (ChessCam.Zoomed && !ChessUI.IsActive && Input.GetMouseButtonDown(1))
        {
            Debug.Log("Zooming Out");
            ChessCam.Zoom(-1);
        }
    }

    #endregion
    
    private IEnumerator PlayerInit()
    {
        yield return StartCoroutine(ui.ShowMessage("ChessNut"));
        yield return StartCoroutine(CheckGameVersion());
        
        playerID = PlayerPrefs.GetInt("chessplayer");
    
        if (playerID == 0 || startFresh)
        {
            DeleteFiles();
            yield return StartCoroutine(ui.EnterName());
            yield return StartCoroutine(RegisterPlayer());
        }
        
            
        playerID   = PlayerPrefs.GetInt("chessplayer");
        playerName = PlayerPrefs.GetString("chessname");
        playerCode = AndyCrypt.Encrypt(playerID);
        
        ui.ShowFriendCode(playerName, playerCode);
        
        LoadOpponents();
        
        SetupGames();
            
        ChessCam.FrameGames(); 
            
        StartCoroutine(ServerTalk());
    }
    
#region Init
    private IEnumerator RegisterPlayer()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", PlayerPrefs.GetString("chessname"));
            
        UnityWebRequest www = UnityWebRequest.Post(PHP("register"), form);
        yield return www.SendWebRequest();
            
        string response = www.downloadHandler.text;
        if(response[0] != '!')
            yield break;
            
        response = response.Substring(1, response.Length - 1);
            
        PlayerPrefs.SetInt("chessplayer", int.Parse(response));
    }
#endregion

    private IEnumerator ServerTalk()
    {
        while (true)
        {
            yield return StartCoroutine(ReceiveMessage());
            yield return MessageFrequency;
        }
    }
    
    
    private IEnumerator Send(int opponent, int message)
    {
        WWWForm form = new WWWForm();
        form.AddField("opponent", opponent);
        form.AddField("player",   playerID);
        form.AddField("message",  message);
        
        UnityWebRequest www = UnityWebRequest.Post(PHP("send"), form);
        yield return www.SendWebRequest();
    }
    
    
    private IEnumerator ReceiveMessage()
    {
        WWWForm form = new WWWForm();
        form.AddField("player", playerID);
        
        UnityWebRequest www = UnityWebRequest.Post(PHP("receive"), form);
        yield return www.SendWebRequest();
        
        string response = www.downloadHandler.text;
        if(response[0] != '!')
            yield break;
        
        response = response.Substring(1, response.Length - 1);
        //Debug.Log(response);
        
        string[] values = response.Split('_');
        int messageCount = values.Length / 2;
        for (int i = 0; i < messageCount; i++)
        {
            int opponent = int.Parse(values[i * 2]);
            ushort message = ushort.Parse(values[i * 2 + 1]);
            Chess g = GetGame(opponent);
            if (g != null)
            {
                if (message == 63000)
                {
                    games.Remove(g);
                    Destroy(g.gameObject);
                    
                    string oName = "";
                    int oCount = opponents.Count;
                    for (int j = 0; j < oCount; j++)
                        if (opponents[i].id == opponent)
                        {
                            oName = opponents[i].name;
                            opponents.RemoveAt(i);
                            break;
                        }
                    SaveOpponents();
                    LoadOpponents();
                    yield return StartCoroutine(ui.ShowMessage(oName + "\n<size=22>has declined</size>"));
                    
                    int gCount = games.Count;
                    for (int j = 0; j < gCount; j++)
                        games[i].SetPos(IdToGamePos(i));
                    
                    newGame.position = IdToGamePos(gCount);
                }
                else
                    g.SetMessage(message);
                
            } 
            else
            {
                if (message != 63000)
                {
                    string opponentName = "";
                    yield return StartCoroutine(FindOpponent(opponent, s => opponentName = s));
                    if (opponentName != "")
                    {
                        bool play = false;
                        yield return StartCoroutine(ui.AcceptRequest(opponentName, b => play = b));
                        if (play)
                        {
                            Opponent opp = new Opponent(opponent, opponentName);
                            opponents.Add(opp);
                            SaveOpponents();
                            LoadOpponents();
                        
                            yield return StartCoroutine(ui.ShowMessage("<size=22>Starting Game\nwith\n</size>"+opponentName));
                        
                            NewGame(opp).SetMessage(ushort.Parse(values[i * 2 + 1]));
                        }
                        else
                            SendMessage(opponent, 63000);
                    }
                }
            }
        }
    }


    private IEnumerator CheckGameVersion()
    {
        UnityWebRequest www = UnityWebRequest.Get(PHP("version"));
        yield return www.SendWebRequest();
        string response = www.downloadHandler.text;
        if(response[0] != '!')
            yield break;
        
        int serverVersion = int.Parse(response.Replace("!", ""));
        int myVersion     = PlayerPrefs.GetInt("chessversion");

        if (serverVersion != myVersion)
        {
            Debug.Log("Deleting Files");
            LoadOpponents();
            
            int oCount = opponents.Count;
            for (int i = 0; i < oCount; i++)
                SendMessage(opponents[i].id, 63000);
            
            DeleteFiles();
            PlayerPrefs.SetInt("chessversion", serverVersion);
        }
    }


    private Chess GetGame(int opponent)
    {
        int gameCount = games.Count;

        for (int i = 0; i < gameCount; i++)
        {
            Chess g = games[i];
            if(g.opponent == opponent)
                return  g;
        }
        
        return null;
    }
    
    
    private static string PHP(string name)
    {
        return "http://checkandiout.com/Chess/" + name + ".php";
    }


    public void SendMessage(int opponent, int message)
    {
        StartCoroutine(Send(opponent, message));
    }

    
#region Layout
    private const float spacingX = 9, spacingZ = 17;
    
    private IEnumerator NewGameButton()
    {
        Debug.Log("Setting up new Game");
        
        string code = "";
        yield return StartCoroutine(ui.EnterFriendCode(v => {code = v;}));
        int id = AndyCrypt.Decrypt(code);
        
        if (id != playerID)
        {
            bool valid = true;
            int oCount = opponents.Count;
            for (int i = 0; i < oCount; i++)
                if (opponents[i].id == id)
                {
                    valid = false;
                    break;
                }

            if (valid)
            {
                newGame.position = Vector3.one * 10000;
                string opponentName = "";
                yield return StartCoroutine(FindOpponent(id, s => opponentName = s));
                if (opponentName != "")
                {
                    Opponent opp = new Opponent(id, opponentName);
                    opponents.Add(opp);
                    SaveOpponents();
                    LoadOpponents();
        
                    yield return StartCoroutine(ui.ShowMessage("<size=22>Starting Game\nwith\n</size>"+opponentName));
                
                    NewGame(opp).SetMessage(64000);
                    SendMessage(id, 65000);
                }
                else
                {
                    yield return StartCoroutine(ui.ShowMessage("<size=22>Not a Player...\nSorry!</size>"));
                    newGame.position = IdToGamePos(games.Count);
                }
            }
            else
            {
                yield return StartCoroutine(ui.ShowMessage("<size=22>Already an Opponent</size>"));
                newGame.position = IdToGamePos(games.Count);
            }
        }
        else
        {
            yield return StartCoroutine(ui.ShowMessage("<size=22>Can't play\nwith\nyourself!</size>"));
        }   
    }
    
    
    private void SetupGames()
    {
        int count = opponents.Count;
        for (int i = 0; i < count; i++)
            NewGame(opponents[i]);
    }
    
    
    private Chess NewGame(Opponent opponent)
    {
        if(prefab == null)
            prefab = Resources.Load<GameObject>("Chess");
        
        Chess c = Instantiate(prefab).GetComponent<Chess>();
        
        c.transform.SetParent(transform);
        c.Init(opponent.id, opponent.name, IdToGamePos(games.Count));
        
        games.Add(c);
        
        newGame.position = IdToGamePos(games.Count);
        
        return c;
    }

    public static Vector3 IdToGamePos(int id)
    {
        float x = id % 2 * spacingX;
        float z = Mathf.FloorToInt(id * 1f / 2) * -spacingZ;
        return new Vector3(x, 0, z);
    }

    private int PosToId(Vector3 pos)
    {
        Bounds b = GetAllGameBounds();
               
        Vector3 min = b.min, max = b.max;
        if(min.x > pos.x || max.x < pos.x || min.z > pos.z || max.z < pos.z)
            return -1;
        
        float x = pos.x + spacingX * .5f; 
        float z = pos.z - spacingZ * .5f;
        
        int id = Mathf.FloorToInt(Mathf.Abs(z) / spacingZ) * 2 + Mathf.FloorToInt(x / spacingX);
        return id <= games.Count? id : -1;
    }
    
    
    public static Bounds GetAllGameBounds()
    {
        Chess[] games = FindObjectsOfType<Chess>();
        
        int count = games.Length;
        Bounds b;
        if (count == 0)
        {
            b = GameBounds(IdToGamePos(0));
            b.Encapsulate(GameBounds(IdToGamePos(1)));
        }
        else
        {
            b = GameBounds(IdToGamePos(0));
               
            int max = Mathf.Min(4, count + 1);
            for (int i = 1; i < count + 1; i++)
                b.Encapsulate(GameBounds(IdToGamePos(i)));
        }
        
        return b;
    }


    public static Bounds GameBounds(Vector3 center)
    {
        return new Bounds(center, new Vector3(spacingX, 1, spacingZ));
    }
#endregion


#region Opponents

    private IEnumerator FindOpponent(int id, Action<string> nameReturn)
    {
        WWWForm form = new WWWForm();
        form.AddField("player", id);
        
        UnityWebRequest www = UnityWebRequest.Post(PHP("getname"), form);
        yield return www.SendWebRequest();
        
        string response = www.downloadHandler.text;
        if(response[0] != '!')
            yield break;
        
        response = response.Substring(1, response.Length - 1);
        
        int count = opponents.Count;
        for (int i = 0; i < count; i++)
            if(opponents[i].Same(id, response))
                yield break;
        
        nameReturn.Invoke(response);
    }


    private void LoadOpponents()
    {
        opponents.Clear();
        
        if(!File.Exists(OpponentPath))
            return;
        
        using (MemoryStream m = new MemoryStream(File.ReadAllBytes(OpponentPath)))
        using (BinaryReader w = new BinaryReader(m))
        {
            int count = w.ReadInt32();
            for (int i = 0; i < count; i++)
                opponents.Add(new Opponent(w.ReadInt32(), w.ReadString()));
        }
    }


    private static string OpponentPath { get { return Folder + "\\oppo.nents"; } }

   
    private void SaveOpponents()
    {
        using (MemoryStream m  = new MemoryStream())
        using (BinaryWriter w = new BinaryWriter(m))
        {
            int count = opponents.Count;
            w.Write(count);
            for (int i = 0; i < count; i++)
            {
                Opponent o = opponents[i];
                w.Write(o.id);
                w.Write(o.name);
            }
            
            File.WriteAllBytes(OpponentPath, m.ToArray());
        }
    }


    private static void DeleteFiles()
    {
        string[] files = Directory.GetFiles(Folder);

        int fileCount = files.Length;
        for (int i = 0; i < fileCount; i++)
        {
            string f = files[i];
            if(f.Contains(".chess") || f.Contains(".nents"))
                File.Delete(f);
        }
    }


    public static string Folder
    {
        get
        {
            string dir = Application.persistentDataPath +  (Application.isEditor? "\\Editor" : Application.isMobilePlatform? "" : "\\" + Application.productName);
            
            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            return dir;
        }
    }
    

#endregion
}
