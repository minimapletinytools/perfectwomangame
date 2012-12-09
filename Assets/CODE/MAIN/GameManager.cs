using UnityEngine;
using System.Collections.Generic;

public class GameManager : FakeMonoBehaviour
{
    public delegate bool GameEventDelegate(float time);
    public class TimedEventHandler
    {
        LinkedList<KeyValuePair<QuTimer, GameEventDelegate>> mTimedEvents = new LinkedList<KeyValuePair<QuTimer, GameEventDelegate>>();
        public void update(float aDeltaTime, FlatElementBase aElement)
        {
            foreach (KeyValuePair<QuTimer, GameEventDelegate> e in mTimedEvents)
            {
                e.Key.update(aDeltaTime);
                if (e.Key.isExpired())
                {
                    if (e.Value(aDeltaTime))
                        mTimedEvents.Remove(e);
                }
            }
        }
        public void add_event(GameEventDelegate aEvent, float aTime)
        {
            mTimedEvents.AddLast(new KeyValuePair<QuTimer, GameEventDelegate>(new QuTimer(0, aTime), aEvent));
        }
    }


    public Camera mCamera;
    public AudioSource mSource;
    public int CurrentLevel
    { get; private set; }
    public int TotalScore
    { get; private set; }
    public int TimeRemaining
    { get; private set; }

    float mMinStartTime = 0;
    public bool Started
    { get; private set; }
    public bool User
    { get; private set; }

    float[] mDifficulties = new float[29];

    public TimedEventHandler mEvents = new TimedEventHandler();

    public GameManager(ManagerManager aManager) : base(aManager) 
    {
        CurrentLevel = 0;
        TotalScore = 0;
        for (int i = 0; i < 29; i++)
            mDifficulties[i] = 0;
    }
    public override void Start()
    {
        mManager.gameObject.AddComponent<AudioListener>();
        mSource = mManager.gameObject.AddComponent<AudioSource>();

        mManager.mEventManager.character_changed_event += character_changed_listener;
    }
    public override void Update()
    {
        User = (mManager.mZigManager.has_user());
        if (!Started && User && Time.timeSinceLevelLoad > mMinStartTime)
        {
            GameObject demoChar = (GameObject)GameObject.Instantiate(mManager.mReferences.mDemoChar);
            mManager.mEventManager.character_changed_event(demoChar.GetComponent<CharacterTextureBehaviour>());
            mManager.mEventManager.character_setup_event(demoChar.GetComponent<CharacterTextureBehaviour>());

            Started = true;
        }
    }
    public void character_changed_listener(CharacterTextureBehaviour aCharacter)
    {
        mSource.clip = aCharacter.backgroundMusic;
        mSource.loop = true;
        mSource.Play();

        //set transparent target pose
        if (aCharacter.properPose != null)
        {
            mManager.mTransparentBodyManager.set_target_pose(ProGrading.read_pose(aCharacter.properPose));
            mManager.mTransparentBodyManager.mFlat.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        }
        else
        {
            mManager.mTransparentBodyManager.mFlat.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.0f);
        }

    }
}
