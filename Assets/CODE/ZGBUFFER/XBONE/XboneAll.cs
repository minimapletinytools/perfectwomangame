using UnityEngine;
using System.Collections;
#if UNITY_XBOXONE 
using Users;
using DataPlatform;
using UnityPluginLog;
using ConsoleUtils;
using TextSystems;
#endif


//TODO this needs to handle initialization of all the plugins after a user is logged in???

public class XboneAll {
    #if UNITY_XBOXONE 

    public int ActiveUserId{ get; private set; }
    public int LastActiveUserId{ get; private set; }
    public bool IsSomeoneSignedIn{ get { return UsersManager.IsSomeoneSignedIn; } }

    public void Start () {
        DataPlatformPlugin.InitializePlugin(0);
        TextSystemsManager.Create();
        UsersManager.Create();
        SetupUserManagerCallbacks();
        AchievementsManager.Create();
        StatisticsManager.Create();
        PluginLogManager.Create();
        PluginLogManager.SetLogPath("G:\\plugins.log");

    }

    bool firstTime = true;
    public void Update()
    {
        if (firstTime)
        {
            firstTime = false;
            SanityCheckApplicationSetup();
            
            
            //EventManager.Destroy(); //clean up from last time we loaded??
            EventManager.Create(@"G:\Data\StreamingAssets\Events-PRFW.0-4A0A3432.man");
            ManagerManager.Log("Events created " + EventManager.IsInitialized);
            AchievementsManager.OnAchievementNotification += delegate(AchievementNotification notice)
            {
                ManagerManager.Log("Achievement unlocked " + notice.AchievementId);
            };

            ManagerManager.Log("successfully ran xbone initialization coroutine");
        }
    }

    
    bool SanityCheckApplicationSetup()
    {
        // We sanity check some things before we allow you to run, achievements and other live services
        // really do not like to be used against the wrong sandbox or with the wrong TID/SCID.
        string warningText = "";
        if(!UsersManager.IsSomeoneSignedIn)
        {
            warningText += "\n\nERROR: You MUST have someone signed in to use this demo.";
            /*if(!haveRequestedSignIn)
            {
                UsersManager.RequestSignIn(AccountPickerOptions.AllowGuests);
                haveRequestedSignIn = true;
            }*/
        }
        
        
        if (ConsoleUtilsManager.SandboxId() != "PRFW.0")
        {
            warningText += "\n\nERROR: SandboxId not set to PRFW.0 sample will not run";
            warningText += "\n       >> Current Id: \"" + ConsoleUtilsManager.SandboxId() + "\"";
            warningText += "\n       >> Suppose to be: \"PRFW.0\"";
        }
        
        if (ConsoleUtilsManager.PrimaryServiceConfigId() != "f3530100-c251-40ff-9d13-078c4a0a3432")
        {
            warningText += "\n\nERROR: SCID not set to f3530100-c251-40ff-9d13-078c4a0a3432 sample will not run";
            warningText += "\n       >> Current Id: " + ConsoleUtilsManager.PrimaryServiceConfigId();
        }
        
        if (ConsoleUtilsManager.TitleIdHex() != "4A0A3432")
        {
            warningText += "\n\nERROR: TID not set to 4A0A3432 sample will not run";
            warningText += "\n       >> Current Id: " + ConsoleUtilsManager.TitleIdHex();
        }

        bool ok = warningText == "" && UsersManager.IsSomeoneSignedIn;
        if (!ok)
        {
            ManagerManager.Log(warningText);
            return false;
        }
        ManagerManager.Log("Sanity check OK");

        return ok;
    }

    //USER STUFF

    void NewPrimaryUserChanged()
    {
        //set rich presence strings
        PresenceData data = PresenceService.CreatePresenceData(ConsoleUtilsManager.PrimaryServiceConfigId(), "default", null);
        PresenceService.SetPresenceAsync(ActiveUserId, true, data, delegate(UnityPlugin.AsyncStatus status, UnityAOT.ActionAsyncOp op) {});
    }

    void SetupUserManagerCallbacks()
    {
        UsersManager.OnUsersChanged       += OnUsersChanged;
        UsersManager.OnUserSignIn         += OnUserSignIn;
        UsersManager.OnUserSignOut        += OnUserSignOut;
        UsersManager.OnSignOutStarted     += OnUserSignOutStarted;
        UsersManager.OnDisplayInfoChanged += OnUserDisplayInfoChange;
        
        if (!UsersManager.IsSomeoneSignedIn)
        {
            LastActiveUserId = -1;
            ActiveUserId = -1;
            UsersManager.RequestSignIn(Users.AccountPickerOptions.AllowGuests);
        } else
        {
            ActiveUserId = UsersManager.Users [0].Id;
            LastActiveUserId = ActiveUserId;
        }
    }
    void OnUsersChanged(int id,bool wasAdded)
    {
        
    }
    
    void OnUserSignIn(int id)
    {
        if (ActiveUserId == -1)
        {
            NewPrimaryUserChanged();
        }
    }
    
    void OnUserSignOut(int id)
    {
        if (ActiveUserId == id)
        {
            //TODO pause game
            //tell user game will restart if they log in as someone else
            //does this get called if user logs out while game is suspended????
            UsersManager.RequestSignIn(Users.AccountPickerOptions.AllowGuests);
        }
    }
    
    void OnUserSignOutStarted(int id, System.IntPtr deferred)
    {
        var deferral = new SignOutDeferral(deferred);
        var dummy = (new GameObject ("genDummy")).AddComponent<DummyBehaviour> ();
        dummy.StartCoroutine (deferral_thread (dummy.gameObject,deferral));
    }
    
    IEnumerator deferral_thread(GameObject aDestroy,SignOutDeferral aDef)
    {
        yield return null; 
        aDef.Complete ();
        GameObject.Destroy (aDestroy);
    }
    
    void OnUserDisplayInfoChange(int id)
    {
        
    }


    #else
    public bool IsSomeoneSignedIn{ get { return false; } }
    public void Start () {
    }
    public void Update(){}
    #endif
}
