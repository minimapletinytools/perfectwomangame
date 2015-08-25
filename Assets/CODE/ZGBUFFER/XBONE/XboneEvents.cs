using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

#if UNITY_XBOXONE
using Users;
using DataPlatform;
using ConsoleUtils;


public class XboneEvents{
    public System.Guid mSessionId;
    ManagerManager mManager;
    XboneAll mAll;

    public delegate void XB1EventsAdhocDelegate(string a1, ref Guid a2, Int32 a3);

    public XboneEvents()
    {
        mAll = MicrosoftZig.Inst.mAll;
        mSessionId = System.Guid.NewGuid();
        mManager = ManagerManager.Manager;
        mManager.GameEventDistributor += game_event_listener;
    }


    void game_event_listener(string name, object[] args)
    {
        if (mAll.LastActiveUser == null)
        {
            ManagerManager.Log("LastActiveUser is null, can't send event " + name);
            return;
        }

        if (name == "NEW CHARACTER")
        {
            if ((CharacterIndex)args [0] == CharacterIndex.sFetus)
            {
                //this is sent on user login/logout instead now
                //Events.SendPlayerSessionStart(mAll.LastActiveUser.UID, ref mSessionId, "", 0, 0);
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

            send_game_progress();

            Events.SendPassing(
                mAll.LastActiveUser.UID,
                ref mSessionId, 
                (int)mManager.mGameManager.mModeNormalPlay.TotalScore);

            if(gruesome)
                Events.SendGruesomePassing(
                    mAll.LastActiveUser.UID,
                    ref mSessionId,
                    mManager.mGameManager.mModeNormalPlay.CurrentPerformanceStat.Character.Index);

            var performance = args[0] as List<PerformanceStats>;
            handle_ad_hoc_events(performance);

        }

        if (name == "START")
        {
            Events.SendPlayerSessionStart(mAll.LastActiveUser.UID, ref mSessionId, "", 0, 0);
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

    void send_game_progress()
    {
        int lockedChars = mManager.mMetaManager.UnlockManager.mUnlocked.unlockedCharacters.to_array().Where(e => e == 2).Count();
        float progress = 1 - Mathf.Max(0, 7 - lockedChars) / 7f;
        ManagerManager.Log("EVENT: gameprogress " + progress + " " + mManager.mMetaManager.UnlockManager.get_unlocked_characters().Count);
        Events.SendGameProgress(
                mAll.LastActiveUser.UID,
                ref mSessionId,
                progress,
                mManager.mMetaManager.UnlockManager.get_unlocked_characters().Count);
    }
    void handle_ad_hoc_events(List<PerformanceStats> aStats)
    {
        //TODO TEST
        Dictionary<XB1EventsAdhocDelegate,List<CharacterIndex>> options = new Dictionary<XB1EventsAdhocDelegate,List<CharacterIndex>>()
        {
            {Events.SendAH1_career_woman, new List<CharacterIndex>(){CharacterIndex.sStar,CharacterIndex.sSchool,CharacterIndex.sGames,CharacterIndex.sProf,CharacterIndex.sBurnt}},
            {Events.SendAH2_conservative, new List<CharacterIndex>(){CharacterIndex.sPrincess,CharacterIndex.sMother,CharacterIndex.sClerk,CharacterIndex.sMarried,CharacterIndex.sAngry,CharacterIndex.sFundraiser,CharacterIndex.sPray}},
            {Events.SendAH3_hippie, new List<CharacterIndex>(){CharacterIndex.sPunk,CharacterIndex.sDrunk,CharacterIndex.sBeach,CharacterIndex.sDance,CharacterIndex.sTribal}},
            {Events.SendAH4_sexy, new List<CharacterIndex>(){CharacterIndex.sSexy,CharacterIndex.sDrunk,CharacterIndex.sDance,CharacterIndex.sPorn}},
            {Events.SendAH5_extremist, new List<CharacterIndex>(){CharacterIndex.sGang,CharacterIndex.sTerrorist,CharacterIndex.sWhale,CharacterIndex.sAngry,CharacterIndex.sPray}},
            {Events.SendAH6_boring, new List<CharacterIndex>(){CharacterIndex.sSchool,CharacterIndex.sMother,CharacterIndex.sClerk,CharacterIndex.sMarried,CharacterIndex.sFundraiser,CharacterIndex.sDemented}},
            {Events.SendAH7_family_life, new List<CharacterIndex>(){CharacterIndex.sSister,CharacterIndex.sMother,CharacterIndex.sBeach,CharacterIndex.sLeukemia,CharacterIndex.sGrandma}},
            {Events.SendAH8_hard_life, new List<CharacterIndex>(){CharacterIndex.sSlave,CharacterIndex.sSister,CharacterIndex.sLeukemia,CharacterIndex.sBurnt,CharacterIndex.sAngry}}
        };

        foreach(var e in options)
        {
            bool pass = true;
            foreach(var f in e.Value)
            {
                if(aStats.Where(g=>g.Character == f).Count() == 0) //if we are missing a character
                {
                    pass = false;
                    break;
                }
            }
            if(pass)
                e.Key(mAll.LastActiveUser.UID,ref mSessionId,1);
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
    public void Start(){}
    
    public void Update(){
        
        
    }
    public void SendDeathEvent()
    {
        
    }
    
}
#endif
