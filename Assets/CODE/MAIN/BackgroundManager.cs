using UnityEngine;
using System.Collections;

public class BackgroundManager  : FakeMonoBehaviour
{
    GameObject mBackground1;
    public BackgroundManager(ManagerManager aManager) : base(aManager) { }

	public override void Start () {
        mManager.mEventManager.character_changed_event += character_changed_listener;

        //make sure camera is setup!
        Camera c = mManager.mGameManager.mCamera;
        mBackground1 = GameObject.CreatePrimitive(PrimitiveType.Plane);
        
	}
	
    public override void Update()
    {

	
	}

    public void character_changed_listener(CharacterTextureBehaviour aTexture)
    {
        

    }
}
