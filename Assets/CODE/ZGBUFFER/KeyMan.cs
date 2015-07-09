using UnityEngine;
using System.Collections;

public static class KeyMan 
{
//XB1
#if UNITY_XBOXONE && !UNITY_EDITOR
    //we use non-generic version of HashTable here to get around the AOT compile issue with value types in generics.
    public static Hashtable keyMap = new Hashtable()
    {
        {"HardSkip",XboxOneKeyCode.Gamepad1ButtonA},
        {"SoftSkip",XboxOneKeyCode.Gamepad1ButtonX},
        {"Perfect",XboxOneKeyCode.Gamepad1ButtonB},
        {"Fast",XboxOneKeyCode.Gamepad1ButtonY},
        {"Die",XboxOneKeyCode.GamepadButtonLeftShoulder},
        {"Restart",XboxOneKeyCode.Gamepad1ButtonView},
        {"Choice1",XboxOneKeyCode.Gamepad1ButtonDPadLeft},
        {"Choice2",XboxOneKeyCode.Gamepad1ButtonDPadUp},
        {"Choice3",XboxOneKeyCode.Gamepad1ButtonDPadDown},
        {"Choice4",XboxOneKeyCode.Gamepad1ButtonDPadRight},
        {"Quit",null},
        {"DepthToggle",XboxOneKeyCode.Gamepad1ButtonMenu},
        {"Pause", null},

        {"lvl1",null},
        {"lvl2",null},
        {"lvl3",null},
        {"lvl4",null},
        {"lvl5",null},
        {"lvl6",null},
        {"lvl7",null},
        {"lvl8",XboxOneKeyCode.Gamepad1ButtonY}, //bad key, pick a different one

        //use these for testing
        {"LeftThumbstick",XboxOneKeyCode.Gamepad1ButtonLeftThumbstick},
        {"RightThumbstick",XboxOneKeyCode.Gamepad1ButtonRightThumbstick},
    };

    public static bool GetKey(string aKey)
    {
        if (keyMap [aKey] == null)
            return false;
        return XboxOneInput.GetKey((XboxOneKeyCode)keyMap[aKey]);
    }
    public static bool GetKeyDown(string aKey)
    {
        if (keyMap [aKey] == null)
            return false;
        return XboxOneInput.GetKeyDown((XboxOneKeyCode)keyMap[aKey]);
    }
    public static bool GetKeyUp(string aKey)
    {
        if (keyMap [aKey] == null)
            return false;
        return XboxOneInput.GetKeyUp((XboxOneKeyCode)keyMap[aKey]);
    }
#else

    //we use non-generic version of HashTable here to get around the AOT compile issue with value types in generics.
    public static Hashtable keyMap = new Hashtable()
    {


        {"HardSkip",KeyCode.Alpha0},
        {"SoftSkip",KeyCode.Alpha9},
        {"Perfect",KeyCode.A},
        {"Fast",KeyCode.O},
		{"Die",KeyCode.D},
        {"Restart",KeyCode.Backspace},
        {"Choice1",KeyCode.Alpha1},
        {"Choice2",KeyCode.Alpha2},
        {"Choice3",KeyCode.Alpha3},
        {"Choice4",KeyCode.Alpha4},
        {"Quit",KeyCode.Escape},
        {"DepthToggle",KeyCode.K},
        {"Pause", KeyCode.P},

        {"lvl1",KeyCode.Q},
        {"lvl2",KeyCode.W},
        {"lvl3",KeyCode.E},
        {"lvl4",KeyCode.R},
        {"lvl5",KeyCode.T},
        {"lvl6",KeyCode.Z},
        {"lvl7",KeyCode.U},
        {"lvl8",KeyCode.I},

        //XB1 stuff, can ignore
        {"LeftThumbstick",null},
        {"RightThumbstick",null}
    };
    
    public static bool GetKey(string aKey)
    {
        if (keyMap [aKey] == null)
            return false;
        return Input.GetKey((KeyCode)keyMap[aKey]);
    }
    public static bool GetKeyDown(string aKey)
    {
        if (keyMap [aKey] == null)
            return false;
        return Input.GetKeyDown((KeyCode)keyMap[aKey]);
    }
    public static bool GetKeyUp(string aKey)
    {
        if (keyMap [aKey] == null)
            return false;
        return Input.GetKeyUp((KeyCode)keyMap[aKey]);
    }
#endif
}
