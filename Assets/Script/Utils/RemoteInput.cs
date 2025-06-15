using UnityEngine;

public static class RemoteInput
{
    public static bool IsMac => Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor;

    public static KeyCode A1 => IsMac ? KeyCode.Joystick1Button0 : KeyCode.Joystick1Button0;
    public static KeyCode A2 => IsMac ? KeyCode.Joystick2Button0 : KeyCode.Joystick2Button0;

    public static KeyCode B1 => IsMac ? KeyCode.Joystick1Button1 : KeyCode.Joystick1Button1;
    public static KeyCode B2 => IsMac ? KeyCode.Joystick2Button1 : KeyCode.Joystick2Button1;

    public static KeyCode Y1 => IsMac ? KeyCode.Joystick1Button4 : KeyCode.Joystick1Button3;
    public static KeyCode Y2 => IsMac ? KeyCode.Joystick2Button4 : KeyCode.Joystick2Button3;

    public static KeyCode RB1 => IsMac ? KeyCode.Joystick1Button7 : KeyCode.Joystick1Button5;
    public static KeyCode RB2 => IsMac ? KeyCode.Joystick2Button7 : KeyCode.Joystick2Button5;

    public static KeyCode START1 => IsMac ? KeyCode.Joystick1Button11 : KeyCode.Joystick1Button7;
    public static KeyCode START2 => IsMac ? KeyCode.Joystick2Button11 : KeyCode.Joystick2Button7;
}
