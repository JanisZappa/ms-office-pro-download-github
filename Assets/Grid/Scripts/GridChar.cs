using GridData;
using UnityEngine;


public class GridChar : MonoBehaviour
{
    public float accel, damp, speed, rollMulti;
    
    [Space]
    public CharState state;
    
    private SpriteInfo none, run, roll;

    private Vector3 pos;
    private readonly Vector3Force walkForce = new Vector3Force();
    private readonly Vector3Force steerForce = new Vector3Force().SetSpeed(500).SetDamp(40);
    private int dir, animDir;
    private int frame;

    
    
    private float stateTime;

    private const int dirs = 16;
    private ControllDir rollSteer;
    private float speedMulti = 1;

    public delegate void Roll();

    public event Roll OnRoll;

    private int lightOn = 1;
    
    
    private struct ControllDir
    {
        public Vector3 steer;
        public int dir;

        public ControllDir(Vector3 steer, int dir)
        {
            this.steer = steer;
            this.dir   = dir;
        }

        public bool Steering => dir != -1;

        public ControllDir Normalized => new ControllDir(steer.normalized, dir);


        public Vector3 StepSteer => dir == -1 ? Vector3.zero : steer.magnitude >= .5f ? steer.normalized : steer.normalized * .5f;
    }


    public enum CharState
    {
        Idle,
        Running,
        Rolling
    }
    
    
    private void Start()
    {
        none = ItemInfo.GetSpriteInfo("character", "None");
        run  = ItemInfo.GetSpriteInfo("character", "Run");
        roll = ItemInfo.GetSpriteInfo("character", "Roll");
        
        GridPhysics.OnGridUpdateStart += OnGridUpdateStart;
        GridPhysics.OnGridUpdate      += OnGridUpdate;
    }


    private void OnGridUpdateStart()
    {
        if (Input.GetKeyDown(KeyCode.L))
            lightOn = (lightOn + 1) % 3;
    }


    private ControllDir ControllSteer(float dt)
    {
        Vector3 steer = (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) + KeySteer(dt)).FlipZ();
        float mag = Mathf.Min(steer.magnitude, 1);
        if(mag < .05f)
            return new ControllDir(Vector3.zero, -1);
        
        float angle = Vector3.SignedAngle(steer, Vector3.forward, Vector3.up);
        if (angle < 0)
            angle = 360 + angle;
        
        int d = Mathf.FloorToInt((angle / 360f + 1f / 16 * .5f) * dirs) % dirs;
        
        steer = Quaternion.AngleAxis(d * (360f / dirs), Vector3.up) * -Vector3.forward * mag;
        return new ControllDir(steer, d);
    }


    private static bool ControllRoll()
    {
        return Input.GetKeyDown(KeyCode.LeftShift) || 
               Input.GetKeyDown(KeyCode.LeftControl) ||
               Input.GetKeyDown(KeyCode.Space) ||
               Input.GetKeyDown(KeyCode.JoystickButton0);
    }


    private void OnGridUpdate(float dt)
    {
        ControllDir steer = ControllSteer(dt);
        bool rollCommand = ControllRoll();
       

    //  State Switching  //
        switch (state)
        {
            case CharState.Idle:
                if (steer.Steering)
                {
                    if (rollCommand)
                    {
                        state     = CharState.Rolling;
                        rollSteer = steer.Normalized;
                        OnRoll?.Invoke();
                    }
                    else
                        state = CharState.Running;
                    stateTime = GridTime.Now;
                } 

                break;
            
            case CharState.Running:
                if (!steer.Steering)
                {
                    state     = CharState.Idle;
                    stateTime = GridTime.Now;
                }
                else if (rollCommand)
                {
                    state     = CharState.Rolling;
                    rollSteer = steer.Normalized;
                    OnRoll?.Invoke();
                    stateTime = GridTime.Now;
                }
                break;
            
            case CharState.Rolling:
                float rollTime = GridTime.Now - stateTime;
                if (rollTime >= roll.frames * (1f / 24))
                {
                    if (steer.Steering)
                    {
                        if (rollCommand)
                        {
                            state = CharState.Rolling;
                            OnRoll?.Invoke();
                        }
                        else
                            state = CharState.Running;
                    } 
                    else
                        state = CharState.Idle;
                    
                    stateTime = GridTime.Now;
                }
                break;
        }
        
    //  State Execute  //
        switch (state)
        {
            case CharState.Idle:
                break;
                
            case CharState.Running:
                dir = steer.dir;
                speedMulti = Mathf.Clamp(speedMulti - dt * 3.35f, 1, 10);
                break;
                
            case CharState.Rolling:
                //if (steer.Steering)
                //    rollSteer = steer.Normalized;
                steer = rollSteer;
                dir = rollSteer.dir;
                speedMulti = rollMulti;
                break;
        }
        

        int newFrame = Mathf.FloorToInt(GridTime.Now * 48);
        if (newFrame != frame)
        {
            frame = newFrame;

            if (animDir != dir)
            {
                int diff  = dir - animDir;
                int diff2 = dir - dirs - animDir;
                int diff3 = dir + dirs - animDir;
                diff = Mathf.Abs(diff) < Mathf.Abs(diff2) ? Mathf.Abs(diff) < Mathf.Abs(diff3)? diff : diff3 : Mathf.Abs(diff2) < Mathf.Abs(diff3)? diff2 : diff3;
                animDir = (dirs + animDir + (int)Mathf.Sign(diff)) % dirs;
            }
        }
        
        pos += walkForce.SetSpeed(accel).SetDamp(damp).Update(steer.steer, dt) * speed * speedMulti * dt;
    }


    private SpriteInfo StateAnim
    {
        get
        {
            switch (state)
            {
                default:                return none;
                case CharState.Running: return run;
                case CharState.Rolling: return roll;
            }
        }
    }


    private void OnEnable()
    {
        DynamicSprites.OnSpriteCollect += OnSpriteCollect;
    }


    private void OnDisable()
    {
        DynamicSprites.OnSpriteCollect -= OnSpriteCollect;
    }

    private Vector3 cursorPos;

    private void OnSpriteCollect()
    {
        cursorPos = lightOn < 2? GridCam.CursorPos(.85f) : cursorPos;
        GridLights.SetLightPos(lightOn < 1? pos + Vector3.up * .85f : cursorPos, 1);    
        
        SpriteInfo anim = StateAnim;
        int f = Mathf.FloorToInt((GridTime.Now-stateTime) * 24) % anim.frames;
        DynamicSprites.RenderThis(new ScatterObject(pos, ItemInfo.SpriteIndex(anim.item, anim.anim, f, animDir), 1, 0));
    }


    private void LateUpdate()
    {
        GridPhysics.DrawCloseColliders(pos, .165f, COLOR.green.spring);
    }


    private Vector3 KeySteer(float dt)
    {
        Vector3 steer = new Vector3(K(KeyCode.LeftArrow, KeyCode.A)? -1 : K(KeyCode.RightArrow, KeyCode.D)? 1 : 0, 
            0,
            K(KeyCode.DownArrow, KeyCode.S)? -1 : K(KeyCode.UpArrow, KeyCode.W)? 1 : 0);

        return steerForce.Update(steer, dt * 10);
    }


    private static bool K(KeyCode k, KeyCode k2)
    {
        return Input.GetKey(k) || Input.GetKey(k2);
    }


    public Vector3 GetPos => pos;
}
