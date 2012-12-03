using UnityEngine;
using System.Collections;

public class BackgroundManager  : FakeMonoBehaviour
{
    GameObject mBackground1;
    Camera mCamera;
    public BackgroundManager(ManagerManager aManager) : base(aManager) { }

	public override void Start () {
        mManager.mEventManager.character_changed_event += character_changed_listener;

        //make sure camera is setup!
        mCamera = mManager.mGameManager.mCamera;
        mBackground1 = GameObject.CreatePrimitive(PrimitiveType.Plane);
        mBackground1.renderer.material = new Material(mManager.mReferences.mDefaultCharacterShader);
        mBackground1.transform.rotation = Quaternion.AngleAxis(90, Vector3.right) * mBackground1.transform.rotation;
        mBackground1.transform.position = mBackground1.transform.position + new Vector3(0, 0, -2);
	}
	
    public override void Update()
    {
	}

    public void character_changed_listener(CharacterTextureBehaviour aCharacter)
    {
        //resize the background and set the texture
        mBackground1.transform.localScale = new Vector3(
            BodyManager.convert_units(aCharacter.background1.width) / 10.0f, 1, 
            BodyManager.convert_units(aCharacter.background1.height) / 10.0f);
        mBackground1.renderer.material.mainTexture = aCharacter.background1;
        
        //resize the camera
        float texRatio = aCharacter.background1.width/(float)aCharacter.background1.height;
        float camRatio = mCamera.aspect;
        if (camRatio > texRatio) //match width
            mCamera.orthographicSize = BodyManager.convert_units(aCharacter.background1.width / camRatio) / 2.0f;
        else
            mCamera.orthographicSize = BodyManager.convert_units(aCharacter.background1.height) / 2.0f;

        //reposition the characeter
        mManager.mBodyManager.move_center(new Vector3(BodyManager.convert_units(aCharacter.background1.width) / 4.0f, 0, 0));
        mManager.mTransparentBodyManager.move_center(new Vector3(BodyManager.convert_units(aCharacter.background1.width) / 4.0f, 0, 0));
    }
}
