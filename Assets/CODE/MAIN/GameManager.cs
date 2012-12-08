using UnityEngine;
using System.Collections;

public class GameManager : FakeMonoBehaviour
{
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
