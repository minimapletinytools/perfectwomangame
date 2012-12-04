using UnityEngine;
using System.Collections;

public class GameManager : FakeMonoBehaviour
{
    public Camera mCamera;
    public AudioSource mSource;
    public int CurrentLevel
    { get; private set; }
    public GameManager(ManagerManager aManager) : base(aManager) 
    {
        CurrentLevel = 0;
    }
    public override void Start()
    {
        mCamera = mManager.gameObject.AddComponent<Camera>();
        mCamera.transform.position = new Vector3(0, 0, 10);
        mCamera.transform.LookAt(Vector3.zero, Vector3.up);
        mCamera.isOrthoGraphic = true;

        mCamera.gameObject.AddComponent<AudioListener>();
        mSource = mCamera.gameObject.AddComponent<AudioSource>();

        mManager.mEventManager.character_changed_event += character_changed_listener;
    }
    public override void Update()
    {

    }
    public void character_changed_listener(CharacterTextureBehaviour aCharacter)
    {
        mSource.clip = aCharacter.backgroundMusic;
        mSource.loop = true;
        mSource.Play();
    }
}
