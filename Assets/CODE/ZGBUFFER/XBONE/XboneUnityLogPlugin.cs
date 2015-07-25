using UnityEngine;
using System.Collections;



#if UNITY_XBOXONE 
using UnityPluginLog;

using DataPlatform;
#endif


public class XboneUnityLogPlugin 
{
    
    #if UNITY_XBOXONE 
    
    public void Start()
    {
        
        //plugin stuff TODO move to a diff file
        //PluginLogManager.Create();
        //PluginLogManager.SetLogPath("G:\\plugins.log");
        PluginLogManager.OnLog += OnLog;
        //ManagerManager.Log("Finished initializing plugin log");

        //DataPlatformPlugin.InitializePlugin(0);
        //LeaderboardManager.Create();
    }

    
    
    void OnLog(UnityPluginLog.LogChannels channel, string message)
    {
        ManagerManager.Log("PLUGINLOG: " + message + channel.ToString());
    }

    public void UnityLog(string aMsg)
    {
        PluginLogManager.Log(aMsg);
        PluginLogManager.LogError("error " + aMsg);
    }
    
    #else
    public void Start(){}
    public void UnityLog(string aMsg){}
    #endif
}
