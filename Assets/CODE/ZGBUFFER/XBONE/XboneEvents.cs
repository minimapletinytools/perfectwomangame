using UnityEngine;
using System.Collections;

#if UNITY_XBOXONE
using Users;
using DataPlatform;

public class XboneEvents{
    System.Guid mSessionId;
    ManagerManager mManager;
    public XboneEvents(ManagerManager aManager)
    {
        mSessionId = System.Guid.NewGuid();
        mManager = aManager;
        mManager.GameEventDistributor += game_event_listener;

        EventManager.Create(@"G:\Data\StreamingAssets\Events-PRFW.0-4A0A3432.man");

        ManagerManager.Log("Events created");
        
        AchievementsManager.Create();
        AchievementsManager.OnAchievementNotification += delegate(AchievementNotification notice)
        {
            mManager.mDebugString = "Achievement unlocked " + notice.AchievementId;
            Debug.Log("Achievement unlocked " + notice.AchievementId);

            ManagerManager.Log("Achievement unlocked " + notice.AchievementId);
        };

    }

    void game_event_listener(string name, object[] args)
    {
        if (name == "NEW CHARACTER")
        {
            if ((CharacterIndex)args [0] == CharacterIndex.sFetus)
            {
                Debug.Log("BORN EVENT");
                ManagerManager.Log("BORN EVENT");
                DataPlatform.Events.SendBorn(UsersManager.Users [0].Id.ToString(), ref mSessionId);
            }
            if ((CharacterIndex)args [0] == CharacterIndex.sOneHundred)
            {
                Debug.Log("TRANSCEND EVENT");
                ManagerManager.Log("TRANSCEND EVENT");
                DataPlatform.Events.SendTranscend(UsersManager.Users [0].Id.ToString(), ref mSessionId);

            }
        }
        if (name == "DEATH")
        {
            ManagerManager.Log("DEATH EVENT");
            Debug.Log("DEATH EVENT " + (mManager.mGameManager.mModeNormalPlay.CurrentPerformanceStat.BadPerformance ? "GRUESOME" : "NORMAL"));
            DataPlatform.Events.SendDeath(
                UsersManager.Users [0].Id.ToString(),
                ref mSessionId, 
                (int)mManager.mGameManager.mModeNormalPlay.TotalScore,
                mManager.mGameManager.mModeNormalPlay.CurrentPerformanceStat.BadPerformance,
                mManager.mGameManager.CurrentCharacterIndex.Age);

            DataPlatform.Events.SendGameProgress(
                UsersManager.Users [0].Id.ToString(),
                ref mSessionId,
                mManager.mMetaManager.UnlockManager.get_unlocked_characters().Count/27f,
                mManager.mMetaManager.UnlockManager.get_unlocked_characters().Count);

        }
    }

    //just for testing CAN DELETE
    public void SendDeathEvent()
    {
        DataPlatform.Events.SendDeath(
            UsersManager.Users [0].Id.ToString(),
            ref mSessionId, 
            (int)mManager.mGameManager.mModeNormalPlay.TotalScore,
            mManager.mGameManager.mModeNormalPlay.CurrentPerformanceStat.BadPerformance,
            mManager.mGameManager.CurrentCharacterIndex.Age);
    
    }
    public void Start(){}

    public void Update(){


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
