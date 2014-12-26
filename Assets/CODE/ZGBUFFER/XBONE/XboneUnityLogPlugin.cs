using UnityEngine;
using System.Collections;


#if UNITY_XBOXONE 
using UnityPluginLog;
#endif


public class XboneUnityLogPlugin 
{
    
    #if UNITY_XBOXONE 
    
    public void Start()
    {
        
        //plugin stuff TODO move to a diff file
        PluginLogManager.Create();
        PluginLogManager.SetLogPath("G:\\plugins.log");
        PluginLogManager.OnLog += OnLog;
    }

    
    
    void OnLog(UnityPluginLog.LogChannels channel, string message)
    {
        ManagerManager.Log(message + channel.ToString());
    }

    public void UnityLog(string aMsg)
    {
        ManagerManager.Log("trying to unity log " + aMsg);
        PluginLogManager.Log(aMsg);
    }
    
    #else
    public void Start(){}
    public void UnityLog(string aMsg){}
    #endif
}
