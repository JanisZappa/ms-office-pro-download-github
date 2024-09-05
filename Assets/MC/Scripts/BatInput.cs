using CameraProjectionRenderingToolkit;
using UnityEngine;


public class BatInput : MonoBehaviour
{
    public bool auto;
    
    private bool mouse;

    private const float gamepadMulti = .85f;



    private float SteerHRaw
    {
        get
        {
            if(auto)
                return Mathf.PerlinNoise(Time.realtimeSinceStartup + 333f + rand * 445f, -2f) * 2 -1;
            
            if (mouse)
                return (Input.mousePosition.x - Screen.width * .5f) / Screen.width * 2;
            
            return Input.GetAxis("Horizontal") * gamepadMulti;
        }
    }
    
    
    private float SteerVRaw
    {
        get
        {
            if(auto)
                return Mathf.PerlinNoise(Time.realtimeSinceStartup + 88933f  - rand * 3445f, -2f) * 2 -1;
            
            if (mouse)
                return(Input.mousePosition.y - Screen.height * .5f) / Screen.height * 2;
            
            return Input.GetAxis("Vertical") * gamepadMulti;
        }
    }


    public float SteerV
    {
        get
        {
            float v = Mathf.Clamp(SteerVRaw, -1, 1);
            return (1 - Mathf.Pow(1 - Mathf.Abs(v), 1.25f)) * Mathf.Sign(v);
        }
    }

    public float SteerH
    {
        get
        {
            float v = Mathf.Clamp(SteerHRaw, -1, 1);
            return (1 - Mathf.Pow(1 - Mathf.Abs(v), 1f)) * Mathf.Sign(v);
        }
    }

    private float StrafeH
    {
        get
        {
            if(auto)
                return Mathf.PerlinNoise(Time.realtimeSinceStartup + 8.13211f  - rand * 534f, -2f) * 2 -1;
            
            return mouse? (Input.GetKey(KeyCode.A)? -1f : 0) + (Input.GetKey(KeyCode.D)? 1f : 0) : Input.GetAxis("HorizontalB");
        }
    }
    
    
    private float StrafeV
    {
        get
        {
            if(auto)
                return Mathf.PerlinNoise(Time.realtimeSinceStartup + 533.55f  - rand * 345f, -2f) * 2 -1;
            
            return mouse? (Input.GetKey(KeyCode.S)? -1f : 0) + (Input.GetKey(KeyCode.W)? 1f : 0) : Input.GetAxis("VerticalB");
        }
    }
    
    
    

    private bool Accel
    {
        get
        {
            if(auto)
                return true;
            
            return mouse? Input.GetMouseButton(1) : Input.GetAxis("LR") < -.01f;
        }
    }
    
    
    private bool Break
    {
        get
        {
            return mouse? Input.GetMouseButton(2) : Input.GetAxis("LR") > .01f;
        }
    }


    public Vector3 Vector
    {
        get
        {
            return new Vector3(StrafeH, StrafeV, (Accel? 1 : 0) + (Break? -1 : 0));
        }
    }
    
    
    private Vector3 mP, gP;
    
    private Camera uiCam, straightCam;
    private CPRT cprt;
    private Transform camTrans, cross;
    private Vector3 cP;
    
    private float rand;
    
    
    private void Start()
    {
        mP = Input.mousePosition;
        
        camTrans = transform.GetChild(0);
        uiCam    = transform.GetChild(1).GetComponent<Camera>();
        cprt     = camTrans.GetComponent<CPRT>();
        cross    = uiCam.transform.GetChild(0);
        straightCam = camTrans.GetChild(0).GetComponent<Camera>();
        
        Cursor.visible = false;
        
        cP = CursorPos;
        
        rand = Random.Range(0, 1f);

        if (auto)
        {
            camTrans.GetComponent<AudioListener>().enabled = false;
            cross.gameObject.SetActive(false);
            uiCam.enabled = false;
        }  
    }

    
    private void Update()
    {
        if(auto)
            return;
        
        if (!mouse)
        {
            if (MouseAction())
                mouse = !mouse;
        }
        else
        {
            if (GamepadAction())
                mouse = !mouse;
        }  
    }


    private Vector3 CursorPos
    {
        get
        {
            float o = uiCam.orthographicSize * 2;
            float x = o * uiCam.aspect;
            
            return new Vector3(x * -.5f + Input.mousePosition.x * 1f / Screen.width  * x, 
                               o * -.5f + Input.mousePosition.y * 1f / Screen.height * o, 
                               1);
        }
    }


    public Ray CursorRay { get { return cprt.enabled? cprt.ScreenPointToRay(Input.mousePosition) : straightCam.ScreenPointToRay(Input.mousePosition); } }


    private void LateUpdate()
    {
        if(auto)
            return;
        
        cP = Vector3.Lerp(cP, CursorPos, Time.deltaTime * 13);
        cross.localPosition = cP;
    }


    private bool MouseAction()
    {
        Vector3 newMP = Input.mousePosition;
        bool mouseMoved = (mP - newMP).sqrMagnitude > .01f;
        mP = newMP;
        
        return mouseMoved ||
           Input.GetKey(KeyCode.A) || 
           Input.GetKey(KeyCode.D) || 
           Input.GetKey(KeyCode.W) ||
           Input.GetKey(KeyCode.S) ||
           Input.GetMouseButton(0) ||
           Input.GetMouseButton(1);
    }
    

    private bool GamepadAction()
    {
        return Mathf.Abs(Input.GetAxis("Horizontal")) > .1f ||
               Mathf.Abs(Input.GetAxis("HorizontalB")) > .1f ||
               Mathf.Abs(Input.GetAxis("Vertical")) > .1f ||
               Mathf.Abs(Input.GetAxis("VerticalB")) > .1f ||
               Input.GetAxis("LR") < -.1f ||
               Input.GetAxis("LR") > .1f
               ;
    }
    
}
