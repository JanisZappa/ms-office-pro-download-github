using UnityEngine;


public static class FPS_Input
{
    private const float mouseSpeed = 1.5f;
    
    public static float H1 =>
        Mathf.Clamp(Input.GetAxis("Horizontal") + 
                    (Input.GetKey(KeyCode.A)? -1 : 0) + 
                    (Input.GetKey(KeyCode.D)?  1 : 0), -1, 1);

    public static float H2 =>
        Input.GetAxis("HorizontalB") + Input.GetAxis("Mouse X") * .865f * mouseSpeed;

    public static float V1 =>
        Mathf.Clamp(Input.GetAxis("Vertical") + 
                    (Input.GetKey(KeyCode.S)? -1 : 0) + 
                    (Input.GetKey(KeyCode.W)?  1 : 0), -1, 1);

    public static float V2
    {
        get
        {
            InputUpdate();
            return Input.GetAxis("VerticalB") + mouseLook;
        }
    }
    public static bool Jump =>
        Mathf.Max(0, Input.GetAxis("LR")) > .001f || 
        Input.GetKey(KeyCode.Space);

    public static bool Crouch => Input.GetButton("Crouch") || Input.GetKey(KeyCode.LeftControl);
    public static bool Sprint => Input.GetKey(KeyCode.LeftShift) && !Crouch;

    public static bool ActionToggle
    {
        get
        {
            InputUpdate();
            return action.value;
        }
    }
    
    private static HoldBool crouch = new HoldBool(), action = new HoldBool();
    
    private static int keyboardFrame = -1;
    private static float keyboardV2;
    private static float mouseLook;

    private static void InputUpdate()
    {
        if(Time.frameCount == keyboardFrame)
            return;

        keyboardFrame = Time.frameCount;
        
        mouseLook = Mathf.Clamp(mouseLook + Input.GetAxis("Mouse Y") * Time.deltaTime * 3.9272f * mouseSpeed, -1, 1);
        
        keyboardV2 = Mathf.Lerp(keyboardV2, (Input.GetKey(KeyCode.DownArrow)? -1 : 0) + 
                                            (Input.GetKey(KeyCode.UpArrow)? 1 : 0), Time.deltaTime * 3.5f);
        
        if (Mathf.Max(0, -Input.GetAxis("LR")) > .001f || Input.GetKey(KeyCode.E))
            action.Press();
        else
            action.Release();
        
        if (Input.GetButton("Crouch") || Input.GetKey(KeyCode.LeftControl))
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
