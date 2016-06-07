﻿using UnityEngine;
using System.Collections;
#if UNITY_XBOXONE 
using Users;
using DataPlatform;
using ConsoleUtils;
using TextSystems;
using UnityAOT;
using UnityPlugin;
using System.IO;
#endif


//TODO this needs to handle initialization of all the plugins after a user is logged in???

public class XboneAll {
    #if UNITY_XBOXONE 

    int activeUserId = -1;
    int lastActiveUserId = -1;
    public User ActiveUser { get { return UsersManager.FindUserById(activeUserId); } private set { activeUserId = (value == null) ? -1 :  value.Id; } }
    public User LastActiveUser { get { return UsersManager.FindUserById(lastActiveUserId); } }
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

        string words = "";
        using (StreamReader reader = new StreamReader(@"G:\Data\StreamingAssets\Events-PRFW.0-4A0A3432.man"))
        {
            words = reader.ReadToEnd();
        }
        EventManager.CreateFromText(words);
        Storage.StorageManager.Create();

        //setup callbacks
        TextSystemsManager.OnPresenceSet += OnPresenceSet;
        AchievementsManager.OnAchievementNotification += delegate(AchievementNotification notice) { ManagerManager.Log("Achievement unlocked " + notice.AchievementId); };
        UsersManager.OnUsersChanged += OnUsersChanged;
        UsersManager.OnUserSignIn += OnUserSignIn;
        UsersManager.OnUserSignOut += OnUserSignOut;
        UsersManager.OnSignOutStarted += OnUserSignOutStarted;
        UsersManager.OnDisplayInfoChanged += OnUserDisplayInfoChange;
        UsersManager.OnAppCurrentUserChanged += OnAppCurrentUserChanged;

        UsersManager.OnSignInComplete += OnSignInComplete;
    }

    void OnSignInComplete(int aStatus, int aUserId)
    {
        siDialog = false;

        activeUserId = aUserId;
        if (aUserId != -1)
            lastActiveUserId = activeUserId;

        ManagerManager.Log("OnSignInComplete " + aUserId + " aStatus");
    }
     
    bool siDialog = false; //if the sign in dialog is up or not
    void RequestSignin()
    {
        if (siDialog == false)
        {
            if(UsersManager.IsSomeoneSignedIn) //apparently requestsignin is called automatically if no one is signed in so we only do this if people are signed in and the user is allowed to pick who they want to play as
                UsersManager.RequestSignIn(AccountPickerOptions.None);
            siDialog = true;
        }
    }

    bool firstTime = true;
    public void Update()
    {
        if (firstTime)
        {
            firstTime = false;
            SanityCheckApplicationSetup();

            //easiest way to set active user (rather than scanning for first input)
            RequestSignin();
        }

        if (ActiveUser == null)
        {
            RequestSignin();
        }

        if(!IsActiveUserInitialized && ManagerManager.Manager.mCharacterBundleManager.is_initial_loaded() && IsSomeoneSignedIn)
        {
            //title screen
            ManagerManager.Log("User Initialized " + ActiveUser.GameDisplayName);
            ManagerManager.Manager.mTransitionCameraManager.you_are_playing_as(ActiveUser.GameDisplayName);

            //game
            if(!XboxOnePLM.AmConstrained())
                ManagerManager.Manager.GameEventDistributor("RESUME", null);

            //event
            ManagerManager.Manager.GameEventDistributor("START", null);

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

        if (UsersManager.Users.Count == 0)
            ManagerManager.Manager.mDebugString = "NO USERS";
        else
            ManagerManager.Manager.mDebugString = "Current user: " + UsersManager.GetAppCurrentUser().Id + " " + UsersManager.GetAppCurrentUser().GameDisplayName;
    }

    //use this function for the version where you scan for first input to determine who is the engaged user
    public IEnumerator WaitForFirstInputCoroutine()
    {
        //TODO this is annoying...
        yield return null;
    }

    void game_event_listener(string name, object[] args)
    {
        if (name == "START GAME")
        {
            ManagerManager.Manager.mTransitionCameraManager.you_are_playing_as(ActiveUser.GameDisplayName);
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
        PlatformProfile pprofile;
        UsersManager.GetPlatformProfile(id, out pprofile);
        return pprofile.GameDisplayName;
        //CommonProfile profile;
        //UsersManager.GetCommonProfile(id, out profile);
        //return profile.OnlineID;
    }

    void OnAppCurrentUserChanged()
    {
    }

    void OnUsersChanged(int id,bool wasAdded)
    {
        ManagerManager.Log("OnUsersChanged " + id + " " + GetUserName(id) + " " + wasAdded);
    }
    
    void OnUserSignIn(int id)
    {
        ManagerManager.Log("OnUserSignIn " + id + " " + GetUserName(id));
    }

    void OnUserSignOut(int id)
    {
        ManagerManager.Log("OnUserSignOut " + id + " " + GetUserName(id));
       
        if (ActiveUser != null)
        {
            if (ActiveUser.Id != id)
            {
                MicrosoftZig.Inst.mStorage.CloseStorage();
                //ManagerManager.Manager.GameEventDistributor("PAUSE", null);
                ActiveUser = null;
                IsActiveUserInitialized = false;
                ManagerManager.Manager.restart_game();
            }
        } else
        {
            RequestSignin();
        }
    }
    
    void OnUserSignOutStarted(int id, System.IntPtr deferred)
    {
        ManagerManager.Log("OnUserSignOutStarted " + id + " " + GetUserName(id));
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
