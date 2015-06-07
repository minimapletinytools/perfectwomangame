using UnityEngine;
using System.Collections;

#if UNITY_XBOXONE
using Users;
using DataPlatform;
using ConsoleUtils;

public class XboneEvents{
    System.Guid mSessionId;
    ManagerManager mManager;

    public XboneEvents(ManagerManager aManager)
    {
        mSessionId = System.Guid.NewGuid();
        mManager = aManager;
        mManager.GameEventDistributor += game_event_listener;

        //moved to XboneALl.cs
        //DataPlatformPlugin.InitializePlugin(0);
        //AchievementsManager.Create();
        //StatisticsManager.Create();
    }


    void game_event_listener(string name, object[] args)
    {
        if (UsersManager.Users.Count == 0)
        {
            ManagerManager.Log("NO USERS CAN'T SENT EVENT " + name);
        }

        if (name == "NEW CHARACTER")
        {
            if ((CharacterIndex)args [0] == CharacterIndex.sFetus)
            {
                //Debug.Log("BORN EVENT");
                ManagerManager.Log("BORN EVENT");
                Events.SendBorn(UsersManager.Users [0].Id.ToString(), ref mSessionId);
            }
            if ((CharacterIndex)args [0] == CharacterIndex.sOneHundred)
            {
                //Debug.Log("TRANSCEND EVENT");
                ManagerManager.Log("TRANSCEND EVENT");
                Events.SendTranscend(UsersManager.Users [0].Id.ToString(), ref mSessionId);

            }
        }
        if (name == "DEATH")
        {
            ManagerManager.Log("DEATH EVENT " + (mManager.mGameManager.mModeNormalPlay.CurrentPerformanceStat.BadPerformance ? "GRUESOME" : "NORMAL"));
            Events.SendDeath(
                UsersManager.Users [0].Id.ToString(),
                ref mSessionId, 
                (int)mManager.mGameManager.mModeNormalPlay.TotalScore,
                mManager.mGameManager.mModeNormalPlay.CurrentPerformanceStat.BadPerformance,
                mManager.mGameManager.CurrentCharacterIndex.Age);

            Events.SendGameProgress(
                UsersManager.Users [0].Id.ToString(),
                ref mSessionId,
                mManager.mMetaManager.UnlockManager.get_unlocked_characters().Count/27f,
                mManager.mMetaManager.UnlockManager.get_unlocked_characters().Count);

        }
    }

    //just for testing CAN DELETE
    public void SendDeathEvent()
    {
        ManagerManager.Log("Sending DEATH and BORN event");

        Events.SendBorn(UsersManager.Users [0].Id.ToString(), ref mSessionId);

        Events.SendDeath(
            UsersManager.Users [0].Id.ToString(),
            ref mSessionId, 
            (int)mManager.mGameManager.mModeNormalPlay.TotalScore,
            mManager.mGameManager.mModeNormalPlay.CurrentPerformanceStat.BadPerformance,
            mManager.mGameManager.CurrentCharacterIndex.Age);
    
    }
    public void Start(){}

    public void Update(){
        if (KeyMan.GetKeyDown("LeftThumbstick"))
        {
            /*StatisticsManager.GetSingleUserStatisticsAsync(UsersManager.Users [0].Id,UsersManager.Users [0].Id.ToString(),"f3530100-c251-40ff-9d13-078c4a0a3432","TimesBorn",delegate(UserStatisticsResult obj, UnityAOT.GetObjectAsyncOp<UserStatisticsResult> op) {
                ManagerManager.Log("stat callback " + op.Success.ToString() + " " + op.IsComplete + " " + obj.Length); 
               foreach(var e in obj)
                {
                    ManagerManager.Log("Made it in " + e.Length + " " + e.ServiceConfigurationId);
                    foreach(var f in e)
                    {
                        ManagerManager.Log(f.Name + " " + f.Value);
                    }
                }
            });*/


            //TODO DELETE
            //this is test stuff and will crash if there is no user logged on at start of game
            var currentUser = UsersManager.Users [0];
            ManagerManager.Log("querying for stats userid, uid, scid " + currentUser.Id + " " + currentUser.UID + " " + ConsoleUtilsManager.PrimaryServiceConfigId());;
            StatisticsManager.GetSingleUserStatisticsAsyncMultipleStats(
                currentUser.Id, 
                currentUser.UID, 
                ConsoleUtilsManager.PrimaryServiceConfigId(), 
                new string[]{"TimesBorn","HighScore","TimesDied","TimesGruesomeDeath"},
            delegate(UserStatisticsResult obj, UnityAOT.GetObjectAsyncOp<UserStatisticsResult> op)
            {
                ManagerManager.Log("inside stat callback " + op.ToString());
                ManagerManager.Log("Stat retrieval op: " + op.Success + " userid: " + obj.XboxUserId + " cnt " + obj.Length);
                ManagerManager.Log("inside stat callback 2");
                foreach (ServiceConfigurationStatistic ss in obj)
                {
                    ManagerManager.Log("inside2");
                    ManagerManager.Log(ss.ServiceConfigurationId + " cnt " + ss.Length);
                    foreach (Statistic stat in ss)
                    {
                        ManagerManager.Log(stat.Type.ToString() + " " + stat.Value);
                    }
                }
            }
            );
        }

    }

}
#else

public class XboneEvents{
    public XboneEvents(ManagerManager aManager)
    {
    }
    public void Start(){}
    
    public void Update(){
        
        
    }
    public void SendDeathEvent()
    {
        
    }
    
}
#endif
