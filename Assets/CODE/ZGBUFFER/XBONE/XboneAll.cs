using UnityEngine;
using System.Collections;
#if UNITY_XBOXONE 
using Users;
using DataPlatform;
using UnityPluginLog;
using ConsoleUtils;
#endif


//TODO this needs to handle initialization of all the plugins after a user is logged in???

public class XboneAll {
    #if UNITY_XBOXONE 

    public int ActiveUserId{ get; private set; }

    public void Start () {
        DataPlatformPlugin.InitializePlugin(0);
        UsersManager.Create();
        SetupUserManagerCallbacks();
        AchievementsManager.Create();
        StatisticsManager.Create();
        PluginLogManager.Create();
        PluginLogManager.SetLogPath("G:\\plugins.log");

        SetupUserManagerCallbacks();


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
            
            
            RTAManager.CreateAsync(ActiveUserId, OnRTACreated);
            RefreshStatisticsData();
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
    void SetupUserManagerCallbacks()
    {
        UsersManager.OnUsersChanged       += OnUsersChanged;
        UsersManager.OnUserSignIn         += OnUserSignIn;
        UsersManager.OnUserSignOut        += OnUserSignOut;
        UsersManager.OnSignOutStarted     += OnUserSignOutStarted;
        UsersManager.OnDisplayInfoChanged += OnUserDisplayInfoChange;
        
        if (!UsersManager.IsSomeoneSignedIn)
        {
            ActiveUserId = -1;
            UsersManager.RequestSignIn(Users.AccountPickerOptions.AllowGuests);
        }
        else
            ActiveUserId = UsersManager.Users [0].Id;
        

    }
    void OnUsersChanged(int id,bool wasAdded)
    {
        
    }
    
    void OnUserSignIn(int id)
    {
        if (ActiveUserId == -1 || ActiveUserId == id)
            ActiveUserId = id;
        else
        {
            //TODO tell the user that the game will restart unless they change back.. This involves pausing the game??
            //TODO or just restart the game?
        }
    }
    
    void OnUserSignOut(int id)
    {
        UsersManager.RequestSignIn (Users.AccountPickerOptions.AllowGuests);
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


    //RTA stuff

    string kTimesBorn = "TimesBorn";
    string kTimesDied = "TimesDied";
    public void RefreshStatisticsData()
    {
        string[] statsToQuery = new string[] { kTimesBorn, kTimesDied };
        
        // We use the actual statisticsData AsyncOp as our means of keeping the statistics view fresh and identifying when we are querying asynchronously.
        // IsComplete will not be true until the query is finished.
        
        // NOTE: first Id property is which user is making the query, the second parameter is which user we are querying about.
        var statisticsData = StatisticsManager.GetSingleUserStatisticsAsyncMultipleStats(ActiveUserId, UsersManager.Users[0].UID, ConsoleUtilsManager.PrimaryServiceConfigId(), statsToQuery, null);
        
        ManagerManager.Log("Refreshing statistics data");
    }

    void OnRTACreated(RTA rta)
    {
        if (null == rta)
        {
            ManagerManager.Log("ERROR: rta controller for my user could not be created!");
            return;
        }
        // Hook up some callbacks for this user so we can monitor things that are
        // changing about him. These will on trigger for things we subscribe to below.
        rta.OnStatisticChanged += OnStatisticChanged;
        rta.OnSubscribed       += OnSubscribed;
        
        // In angrybots we keep track of the number of doors you have opened.
        // Lets try to subscribe to that. NOTE: If you have NEVER opened a door
        // this subscription WILL fail.
        rta.SubscribeToStatistic(kTimesBorn);
        
        // In angrybots we keep some general information about how many enemies you have
        // defeated. Again if you have NEVER killed an enemy this event will have never fired
        // and this subscription will fail!
        rta.SubscribeToStatistic(kTimesDied);

    }

    
    void OnStatisticChanged(uint hresult, RealTimeActivityStatisticChangeEventArgs args)
    {
        ManagerManager.Log("Stat Change!!");
        int timesBorn, timesDied;
        // If we got some data cache the data for the next draw.
        if (args != null)
        {
            // Note that statistics operate by string name
            if (args.Subscription.StatisticName == kTimesBorn)
            {
                // The statistic value is a string, so you have to convert it.
                // expensive but okay.
                timesBorn = System.Convert.ToInt32(args.StatisticValue);
            }
            if (args.Subscription.StatisticName == kTimesDied)
            {
                timesDied = System.Convert.ToInt32(args.StatisticValue);
            }
            
            // Lets see the change visually.
            ManagerManager.Log("StatChange: " + args.Subscription.StatisticName + " = " + args.StatisticValue + "\n");
        }
    }
    
    void OnSubscribed(uint hresult, RealTimeActivityStatisticChangeSubscription sub)
    {
        // If this fails then we can guess that it probably
        // has never been triggered and thus the stat does not
        // exist on the server for this user.
        if (sub != null)
        {
            ManagerManager.Log("StatSubscribe: " + sub.StatisticName + " = " + sub.InitialStatisticValue + "\n");
        }
        else
        {
            // statName exists to help you try to re-subscribe since you will not have a subscription object
            // in the case of a failure.
            ManagerManager.Log("SUBSCRIBE FAIL: [" + sub.StatisticName + "] [0x" + hresult.ToString("X8") + "]");

        }
    }

    #else
    public void Start () {
    }
    
    #endif
}
