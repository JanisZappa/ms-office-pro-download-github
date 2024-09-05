using UnityEngine;


public static class CrappyInput
{
    public static float H1
    {
        get
        {
            return Mathf.Clamp(Input.GetAxis("Horizontal") + 
                   (Input.GetKey(KeyCode.A)? -1 : 0) + 
                   (Input.GetKey(KeyCode.D)?  1 : 0), -1, 1);
        }
    }
    public static float H2
    {
        get
        {
            return Mathf.Clamp(Input.GetAxis("HorizontalB") + 
                   (Input.GetKey(KeyCode.LeftArrow)? -1 : 0) + 
                   (Input.GetKey(KeyCode.RightArrow)? 1 : 0), -1, 1);
        }
    }
    public static float V1
    {
        get
        {
            return Mathf.Clamp(Input.GetAxis("Vertical") + 
                   (Input.GetKey(KeyCode.S)? -1 : 0) + 
                   (Input.GetKey(KeyCode.W)?  1 : 0), -1, 1);
        }
    }
    public static float V2
    {
        get
        {
            InputUpdate();
            return Mathf.Clamp(Input.GetAxis("VerticalB") + keyboardV2, -1, 1);
        }
    }
    public static bool Jump
    {
        get
        {
            return Mathf.Max(0, Input.GetAxis("LR")) > .001f || 
                   Input.GetKey(KeyCode.Space) ||
                   Input.GetKey(KeyCode.Mouse2);
        }
    }
    public static bool Light
    {
        get
        {
            InputUpdate();
            return light.value;
        }
    }

    public static bool Crouch
    {
        get
        {
            //InputUpdate();
            //return crouch.value;
            return Input.GetButton("Crouch") || Input.GetKey(KeyCode.LeftShift);
        }
    }
    
    public static bool ActionToggle
    {
        get
        {
            InputUpdate();
            return action.value;
        }
    }
    
    private static HoldBool light = new HoldBool(), crouch = new HoldBool(), action = new HoldBool();
    
    private static int keyboardFrame = -1;
    private static float keyboardV2;

    private static void InputUpdate()
    {
        if(Time.frameCount == keyboardFrame)
            return;
        
        keyboardFrame = Time.frameCount;
        
        keyboardV2 = Mathf.Lerp(keyboardV2, (Input.GetKey(KeyCode.DownArrow)? -1 : 0) + 
                                            (Input.GetKey(KeyCode.UpArrow)? 1 : 0), Time.deltaTime * 3.5f);
        
        
        if (Input.GetButton("Light") || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Mouse1))
            light.Press();
        else
            light.Release();
        
        if (Mathf.Max(0, -Input.GetAxis("LR")) > .001f || Input.GetKey(KeyCode.E))
            action.Press();
        else
            action.Release();
        
        if (Input.GetButton("Crouch") || Input.GetKey(KeyCode.LeftShift))
            crouch.Press();
        else
            crouch.Release();
    }


    private struct HoldBool
    {
        public bool value;
        private bool hold;

        public void Press()
        {
            value = !hold;
            hold = true;
        }

        public void Release()
        {
            value = false;
            hold = false;
        }
    }
}
