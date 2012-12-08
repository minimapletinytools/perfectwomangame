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

    public TimedEventHandler mEvents = new TimedEventHandler();

    public GameManager(ManagerManager aManager) : base(aManager) 
    {
        CurrentLevel = 0;
        TotalScore = 0;
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
        if (User && Time.timeSinceLevelLoad > mMinStartTime)
        {

            Started = true;
        }
    }
    public void character_changed_listener(CharacterTextureBehaviour aCharacter)
    {
        mSource.clip = aCharacter.backgroundMusic;
        mSource.loop = true;
        mSource.Play();
    }
}
