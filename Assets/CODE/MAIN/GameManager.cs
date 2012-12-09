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

    public const float LEVEL_TIME_TOTAL = 30;

    public Camera mCamera;
    public AudioSource mSource;
    public int CurrentLevel
    { get; private set; }
    public int CurrentIndex
    { get; private set; }
    public int TotalScore
    { get; private set; }
    public float TimeRemaining
    { get; private set; }

    public ProGrading.Pose CurrentPose
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
        CurrentPose = null;
        CurrentLevel = 0;
        TotalScore = 0;
        TimeRemaining = 3;
        for (int i = 0; i < 29; i++)
            mDifficulties[i] = 0;
    }
    public override void Start()
    {
        mManager.gameObject.AddComponent<AudioListener>();
        mSource = mManager.gameObject.AddComponent<AudioSource>();
        mManager.mEventManager.character_changed_event += character_changed_listener;

        GameObject dummyChar = (GameObject)GameObject.Instantiate(mManager.mReferences.mCharacters[0]);
        mManager.mBackgroundManager.set_background(dummyChar.GetComponent<CharacterTextureBehaviour>());
        GameObject.Destroy(dummyChar);
    }
    public override void Update()
    {
        User = (mManager.mZigManager.has_user());

        //TODO indicate to player to start the game
        if (!Started && !User)
        {
        }
        else if (User)
        {
        }

        if (!Started && User && Time.timeSinceLevelLoad > mMinStartTime)
        {
            start_character(0);
            //maybe less time for fetus???
            Started = true;
        }

        if (Started)
        {
            TimeRemaining -= Time.deltaTime;


            if (User)
            {
                CurrentPose = ProGrading.snap_pose(mManager);
                Debug.Log("waist angle " + mManager.mZigManager.Joints[ZigJointId.Waist].Rotation.flat_rotation());
            }
            if (CurrentPose != null && CurrentIndex != 0 && mManager.mTransparentBodyManager.mFlat.mTargetPose != null)
            {
                mManager.mInterfaceManager.mGrade = ProGrading.grade_pose(CurrentPose, mManager.mTransparentBodyManager.mFlat.mTargetPose);
            }
        }
    }

    void start_character(int index)
    {
        GameObject demoChar = (GameObject)GameObject.Instantiate(mManager.mReferences.mCharacters[index]);
        mManager.mEventManager.character_changed_event(demoChar.GetComponent<CharacterTextureBehaviour>());
        mManager.mEventManager.character_setup_event(demoChar.GetComponent<CharacterTextureBehaviour>());
        GameObject.Destroy(demoChar);

        TimeRemaining = LEVEL_TIME_TOTAL;
        //TODO create event to make the obnoxious CHOOSE_NEXT thingy


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
