using UnityEngine;
using System.Collections;

#if UNITY_XBOXONE
using Users;
using DataPlatform;
using ConsoleUtils;

public class XboneEvents{
    public System.Guid mSessionId;
    ManagerManager mManager;
    XboneAll mAll;

    public XboneEvents()
    {
        mAll = MicrosoftZig.Inst.mAll;
        mSessionId = System.Guid.NewGuid();
        mManager = ManagerManager.Manager;
        mManager.GameEventDistributor += game_event_listener;
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
                Events.SendPlayerSessionStart(mAll.LastActiveUser.UID, ref mSessionId, "", 0, 0);
            }
            if (((CharacterIndex)args [0]).LevelIndex == 1)
            {
                //Debug.Log("BORN EVENT");
                ManagerManager.Log("BORN EVENT");
                Events.SendBorn(mAll.LastActiveUser.UID, ref mSessionId);
            }
            if ((CharacterIndex)args [0] == CharacterIndex.sOneHundred)
            {
                //Debug.Log("TRANSCEND EVENT");
                ManagerManager.Log("TRANSCEND EVENT");
                Events.SendTranscend(mAll.LastActiveUser.UID, ref mSessionId);

            }
        }
        if (name == "DEATH")
        {
            var gruesome = mManager.mGameManager.mModeNormalPlay.CurrentPerformanceStat.BadPerformance;
            ManagerManager.Log("DEATH EVENT " + (gruesome ? "GRUESOME" : "NORMAL"));

            Events.SendPassing(
                mAll.LastActiveUser.UID,
                ref mSessionId, 
                (int)mManager.mGameManager.mModeNormalPlay.TotalScore);

            if(gruesome)
                Events.SendGruesomePassing(
                    mAll.LastActiveUser.UID,
                    ref mSessionId,
                    mManager.mGameManager.mModeNormalPlay.CurrentPerformanceStat.Character.Index);

            Events.SendGameProgress(
                mAll.LastActiveUser.UID,
                ref mSessionId,
                mManager.mMetaManager.UnlockManager.get_unlocked_characters().Count/27f,
                mManager.mMetaManager.UnlockManager.get_unlocked_characters().Count);

        }

        if (name == "START")
        {
            //this is done elsewhere now since START event may happen before there are any users
            //Events.SendPlayerSessionStart(mAll.LastActiveUser.UID, ref mSessionId, "", 0, 0);
        }

        if (name == "PAUSE")
        {
            Events.SendPlayerSessionPause(mAll.LastActiveUser.UID, ref mSessionId, "");
        }
        if (name == "RESUME")
        {
            Events.SendPlayerSessionResume(mAll.LastActiveUser.UID, ref mSessionId, "", 0, 0);
        }

        if (name == "TERMINATE")
        {
            Events.SendPlayerSessionEnd(mAll.LastActiveUser.UID, ref mSessionId, "", 0, 0,0);
        }
    }

    //just for testing CAN DELETE
    public void SendDeathEvent()
    {
        ManagerManager.Log("Sending DEATH and BORN event");

        Events.SendBorn(mAll.LastActiveUser.UID, ref mSessionId);

        Events.SendPassing(
            mAll.LastActiveUser.UID,
            ref mSessionId, 
            (int)mManager.mGameManager.mModeNormalPlay.TotalScore);
    
    }
    public void Start(){}

    public void Update(){
        if (KeyMan.GetKeyDown("LeftThumbstick"))
        {
            /*StatisticsManager.GetSingleUserStatisticsAsync(mAll.LastActiveUser.Id,mAll.LastActiveUser.UID,"f3530100-c251-40ff-9d13-078c4a0a3432","TimesBorn",delegate(UserStatisticsResult obj, UnityAOT.GetObjectAsyncOp<UserStatisticsResult> op) {
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

            /*
            //TODO DELETE
            //this is test stuff and will crash if there is no user logged on at start of game
            var currentUser = mAll.LastActiveUser;
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
            );*/
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
