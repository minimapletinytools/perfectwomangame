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
        mBackground1.renderer.material = new Material(mManager.mReferences.mDefaultCharacterShader);
        mBackground1.transform.rotation = Quaternion.AngleAxis(90, Vector3.right) * mBackground1.transform.rotation;
        mBackground1.transform.position = mBackground1.transform.position + new Vector3(0, 0, -1);
	}
	
    public override void Update()
    {
	}

    public void character_changed_listener(CharacterTextureBehaviour aCharacter)
    {
        mBackground1.transform.localScale = new Vector3(aCharacter.background1.width / 10.0f, 1, aCharacter.background1.height / 10.0f);
        mBackground1.transform.localScale *= 1 / 100f;//Screen.width / (float)aCharacter.background1.width;
        mBackground1.renderer.material.mainTexture = aCharacter.background1;
    }
}
