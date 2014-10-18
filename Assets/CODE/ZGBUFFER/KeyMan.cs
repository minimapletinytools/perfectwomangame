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
		{"Die",null},
        {"Restart",XboxOneKeyCode.Gamepad1ButtonView},
        {"Choice1",XboxOneKeyCode.Gamepad1ButtonDPadLeft},
        {"Choice2",XboxOneKeyCode.Gamepad1ButtonDPadUp},
        {"Choice3",XboxOneKeyCode.Gamepad1ButtonDPadDown},
        {"Choice4",XboxOneKeyCode.Gamepad1ButtonDPadRight},
        {"Quit",null},
        {"DepthToggle",XboxOneKeyCode.Gamepad1ButtonMenu}
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
        {"DepthToggle",KeyCode.K}
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
