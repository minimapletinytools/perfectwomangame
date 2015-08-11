using UnityEngine;
using System.Collections;
#if UNITY_XBOXONE 
using Users;
using DataPlatform;
using ConsoleUtils;
using TextSystems;
using UnityAOT;
using UnityPlugin;
#endif


//TODO this needs to handle initialization of all the plugins after a user is logged in???

public class XboneAll {
    #if UNITY_XBOXONE 

    public User ActiveUser { get; private set; }
    public User LastActiveUser { get; private set; }
    public bool IsSomeoneSignedIn{ get { return UsersManager.IsSomeoneSignedIn; } }
    public bool IsActiveUserInitialized { get; private set; }

    string[] stats = new string[] { "TimesBorn", "TimesGruesomeDeath" };

    public void Start () {
        //initialize vars
        ManagerManager.Manager.GameEventDistributor += game_event_listener;

        //initialize all plugins
        DataPlatformPlugin.InitializePlugin(0);
        TextSystemsManager.Create();
        UsersManager.Create();
        AchievementsManager.Create();
        StatisticsManager.Create();
        EventManager.Create(@"G:\Data\StreamingAssets\Events-PRFW.0-4A0A3432.man");
        Storage.StorageManager.Create();

        //setup callbacks
        TextSystemsManager.OnPresenceSet += OnPresenceSet;
        AchievementsManager.OnAchievementNotification += delegate(AchievementNotification notice) { ManagerManager.Log("Achievement unlocked " + notice.AchievementId); };
        UsersManager.OnUsersChanged += OnUsersChanged;
        UsersManager.OnUserSignIn += OnUserSignIn;
        UsersManager.OnUserSignOut += OnUserSignOut;
        UsersManager.OnSignOutStarted += OnUserSignOutStarted;
        UsersManager.OnDisplayInfoChanged += OnUserDisplayInfoChange;
    }

    bool firstTime = true;
    public void Update()
    {
        if (firstTime)
        {
            firstTime = false;
            SanityCheckApplicationSetup();

            if(!IsSomeoneSignedIn)
                UsersManager.RequestSignIn(Users.AccountPickerOptions.AllowGuests);

            //RTAManager.CreateAsync(UsersManager.Users[0].Id, OnRTACreated);
        }

        if (!IsActiveUserInitialized && ManagerManager.Manager.mCharacterBundleManager.is_initial_loaded() && IsSomeoneSignedIn)
        {
            //users
            ActiveUser = UsersManager.Users[0];
            LastActiveUser = ActiveUser;

            //title screen
            ManagerManager.Log("User Initialized " + ActiveUser.GameDisplayName);
            ManagerManager.Manager.mTransitionCameraManager.you_are_playing_as(ActiveUser.GameDisplayName);

            //game
            if(!XboxOnePLM.AmConstrained())
                ManagerManager.Manager.GameEventDistributor("RESUME", null);

            //rich user presence
            SetRichUserPresence();
            
            //storage
            MicrosoftZig.Inst.mStorage.InitializeUserStorage();
            ManagerManager.Manager.mMetaManager.StartSaveThread(); 

            IsActiveUserInitialized = true;
        }

        if (KeyMan.GetKeyDown("LeftThumbstick"))
        {
            //ManagerManager.Log("getSingle");
            //StatisticsManager.GetSingleUserStatisticsAsyncMultipleStats(UsersManager.Users[0].Id, UsersManager.Users[0].UID, ConsoleUtilsManager.PrimaryServiceConfigId(), stats,null);
        }
    }

    void game_event_listener(string name, object[] args)
    {
        if (name == "START GAME")
        {
            //ManagerManager.Manager.mTransitionCameraManager.you_are_playing_as(ActiveUser.GameDisplayName);
        }
    }

    //Rich User Presence stuff
    void SetRichUserPresence()
    {
        PresenceData data = PresenceService.CreatePresenceData(ConsoleUtilsManager.PrimaryServiceConfigId(), "default", null);
        PresenceService.SetPresenceAsync(ActiveUser.Id, true, data, OnPresenceDataSet);
    }
    void OnPresenceSet(bool setOk, int resultCode)
    {
        ManagerManager.Log("Presence: [" + (setOk ? "Ok" : "Failed") + "] resultCode: [" + resultCode + "]");
    }
    private void OnPresenceDataSet(AsyncStatus status, ActionAsyncOp op)
    {
        if (!op.Success)
        {
            ManagerManager.Log("Failed to set presence string return code: [0x" + op.Result.ToString("X") + "]");
            return;
        }
        ManagerManager.Log("Attempt to set presence string succeeded. Remember rendering this presence string can still fail if require statistics are not defined.");
    }


    //RTA stuff
    RTA m_RTA;
    void OnRTACreated(RTA rta)
    {
        if (null == rta)
        {
            ManagerManager.Log("ERROR: rta controller for my user could not be created!");
            return;
        }
        ManagerManager.Log("RTA created");
        rta.OnStatisticChanged += OnStatisticChanged;
        rta.OnSubscribed += OnSubscribed;
        foreach(var e in stats)
            rta.SubscribeToStatistic(e);
        m_RTA = rta;
    }
    void OnStatisticChanged(uint hresult, RealTimeActivityStatisticChangeEventArgs args)
    {
        if (args != null)
        {
            ManagerManager.Log("StatChange: " + args.Subscription.StatisticName + " = " + args.StatisticValue + "\n");
        }
    }

    void OnSubscribed(uint hresult, RealTimeActivityStatisticChangeSubscription sub, string statName)
    {
        if (sub != null)
        {
            ManagerManager.Log("StatSubscribe: " + sub.StatisticName + " = " + sub.InitialStatisticValue + "\n");
        }
        else
        {
            ManagerManager.Log("SUBSCRIBE FAIL: [" + statName + "] [0x" + hresult.ToString("X8") + "]");
        }
    }


    

    //USER STUFF
    string GetUserName(int id)
    { 
        CommonProfile profile;
        UsersManager.GetCommonProfile(id, out profile);
        return profile.OnlineID;
    }
    void OnUsersChanged(int id,bool wasAdded)
    {
        ManagerManager.Log("OnUsersChanged " + GetUserName(id) + " " + wasAdded);
    }
    
    void OnUserSignIn(int id)
    {
        ManagerManager.Log("OnUserSignIn " + GetUserName(id));
        if (ActiveUser == null)
        {
            if(id != LastActiveUser.Id)
                ManagerManager.Manager.restart_game();
            ActiveUser = UsersManager.Users[0];
        }
    }

    void OnUserSignOut(int id)
    {
        ManagerManager.Log("OnUserSignOut " + GetUserName(id));
        if (ActiveUser.Id == id)
        {
            ManagerManager.Manager.GameEventDistributor("PAUSE", null);
            ActiveUser = null;
            IsActiveUserInitialized = false;
            ManagerManager.Log("Active user + " + GetUserName(id) + " signed out.");
            //TODO pause game
            //tell user game will restart if they log in as someone else
            //does this get called if user logs out while game is suspended????
            UsersManager.RequestSignIn(Users.AccountPickerOptions.AllowGuests);
        }
    }
    
    void OnUserSignOutStarted(int id, System.IntPtr deferred)
    {
        ManagerManager.Log("OnUserSignOutStarted " + GetUserName(id));
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
        ManagerManager.Log("OnUserDisplayInfoChange " + id);
    }



    //OTHER
    bool SanityCheckApplicationSetup()
    {
        // We sanity check some things before we allow you to run, achievements and other live services
        // really do not like to be used against the wrong sandbox or with the wrong TID/SCID.
        string warningText = "";

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

    #else
    public bool IsSomeoneSignedIn{ get { return false; } }
    public void Start () {
    }
    public void Update(){}
    #endif
}
