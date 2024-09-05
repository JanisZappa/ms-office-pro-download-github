using UnityEngine;


public static class Gamepad
{
    public static KeyCode Button(int gamepad, int button)
    {
        switch (button)
        {
            case 0:
                switch (gamepad)
                {
                    default:    return KeyCode.Joystick1Button0;
                    case 1:     return KeyCode.Joystick2Button0;
                    case 2:     return KeyCode.Joystick3Button0;
                    case 3:     return KeyCode.Joystick4Button0;
                    case 4:     return KeyCode.Joystick5Button0;
                    case 5:     return KeyCode.Joystick6Button0;
                    case 6:     return KeyCode.Joystick7Button0;
                    case 7:     return KeyCode.Joystick8Button0;
                }
            case 1:
                switch (gamepad)
                {
                    default:    return KeyCode.Joystick1Button1;
                    case 1:     return KeyCode.Joystick2Button1;
                    case 2:     return KeyCode.Joystick3Button1;
                    case 3:     return KeyCode.Joystick4Button1;
                    case 4:     return KeyCode.Joystick5Button1;
                    case 5:     return KeyCode.Joystick6Button1;
                    case 6:     return KeyCode.Joystick7Button1;
                    case 7:     return KeyCode.Joystick8Button1;
                }
            case 2:
                switch (gamepad)
                {
                    default:    return KeyCode.Joystick1Button2;
                    case 1:     return KeyCode.Joystick2Button2;
                    case 2:     return KeyCode.Joystick3Button2;
                    case 3:     return KeyCode.Joystick4Button2;
                    case 4:     return KeyCode.Joystick5Button2;
                    case 5:     return KeyCode.Joystick6Button2;
                    case 6:     return KeyCode.Joystick7Button2;
                    case 7:     return KeyCode.Joystick8Button2;
                }
            case 3:
                switch (gamepad)
                {
                    default:    return KeyCode.Joystick1Button3;
                    case 1:     return KeyCode.Joystick2Button3;
                    case 2:     return KeyCode.Joystick3Button3;
                    case 3:     return KeyCode.Joystick4Button3;
                    case 4:     return KeyCode.Joystick5Button3;
                    case 5:     return KeyCode.Joystick6Button3;
                    case 6:     return KeyCode.Joystick7Button3;
                    case 7:     return KeyCode.Joystick8Button3;
                }
            case 4:
                switch (gamepad)
                {
                    default:    return KeyCode.Joystick1Button4;
                    case 1:     return KeyCode.Joystick2Button4;
                    case 2:     return KeyCode.Joystick3Button4;
                    case 3:     return KeyCode.Joystick4Button4;
                    case 4:     return KeyCode.Joystick5Button4;
                    case 5:     return KeyCode.Joystick6Button4;
                    case 6:     return KeyCode.Joystick7Button4;
                    case 7:     return KeyCode.Joystick8Button4;
                }
            case 5:
                switch (gamepad)
                {
                    default:    return KeyCode.Joystick1Button5;
                    case 1:     return KeyCode.Joystick2Button5;
                    case 2:     return KeyCode.Joystick3Button5;
                    case 3:     return KeyCode.Joystick4Button5;
                    case 4:     return KeyCode.Joystick5Button5;
                    case 5:     return KeyCode.Joystick6Button5;
                    case 6:     return KeyCode.Joystick7Button5;
                    case 7:     return KeyCode.Joystick8Button5;
                }
            case 6:
                switch (gamepad)
                {
                    default:    return KeyCode.Joystick1Button6;
                    case 1:     return KeyCode.Joystick2Button6;
                    case 2:     return KeyCode.Joystick3Button6;
                    case 3:     return KeyCode.Joystick4Button6;
                    case 4:     return KeyCode.Joystick5Button6;
                    case 5:     return KeyCode.Joystick6Button6;
                    case 6:     return KeyCode.Joystick7Button6;
                    case 7:     return KeyCode.Joystick8Button6;
                }
            case 7:
                switch (gamepad)
                {
                    default:    return KeyCode.Joystick1Button7;
                    case 1:     return KeyCode.Joystick2Button7;
                    case 2:     return KeyCode.Joystick3Button7;
                    case 3:     return KeyCode.Joystick4Button7;
                    case 4:     return KeyCode.Joystick5Button7;
                    case 5:     return KeyCode.Joystick6Button7;
                    case 6:     return KeyCode.Joystick7Button7;
                    case 7:     return KeyCode.Joystick8Button7;
                }
            case 8:
                switch (gamepad)
                {
                    default:    return KeyCode.Joystick1Button8;
                    case 1:     return KeyCode.Joystick2Button8;
                    case 2:     return KeyCode.Joystick3Button8;
                    case 3:     return KeyCode.Joystick4Button8;
                    case 4:     return KeyCode.Joystick5Button8;
                    case 5:     return KeyCode.Joystick6Button8;
                    case 6:     return KeyCode.Joystick7Button8;
                    case 7:     return KeyCode.Joystick8Button8;
                }
            case 9:
                switch (gamepad)
                {
                    default:    return KeyCode.Joystick1Button9;
                    case 1:     return KeyCode.Joystick2Button9;
                    case 2:     return KeyCode.Joystick3Button9;
                    case 3:     return KeyCode.Joystick4Button9;
                    case 4:     return KeyCode.Joystick5Button9;
                    case 5:     return KeyCode.Joystick6Button9;
                    case 6:     return KeyCode.Joystick7Button9;
                    case 7:     return KeyCode.Joystick8Button9;
                }
        }
        
        return KeyCode.None;
    }
}
