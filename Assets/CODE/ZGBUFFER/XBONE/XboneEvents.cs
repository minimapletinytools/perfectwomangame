using UnityEngine;
using System.Collections;
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

        AchievementsManager.Create();
        AchievementsManager.OnAchievementNotification += delegate(AchievementNotification notice)
        {
            mManager.mDebugString = "Achievement unlocked " + notice.AchievementId;
            Debug.Log("Achievement unlocked " + notice.AchievementId);
        };

    }

    void game_event_listener(string name, object[] args)
    {
        if (name == "NEW CHARACTER")
        {
            if ((CharacterIndex)args [0] == CharacterIndex.sFetus)
            {
                Debug.Log("BORN EVENT");
                DataPlatform.Events.SendBorn(UsersManager.Users [0].Id.ToString(), ref mSessionId);
            }
            if ((CharacterIndex)args [0] == CharacterIndex.sOneHundred)
            {
                Debug.Log("TRANSCEND EVENT");
                DataPlatform.Events.SendTranscend(UsersManager.Users [0].Id.ToString(), ref mSessionId);

            }
        }
        if (name == "DEATH")
        {
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

    public void Start(){}

    public void Update(){


    }

}
