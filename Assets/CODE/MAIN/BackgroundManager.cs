using UnityEngine;
using System.Collections;

public class BackgroundManager  : FakeMonoBehaviour
{
    GameObject mBackground1;
    int mlayer = 0;
    public BackgroundManager(ManagerManager aManager) : base(aManager) { }

	public override void Start () {
        mManager.mEventManager.character_changed_event += character_changed_listener;

        //make sure camera is setup!
        mBackground1 = GameObject.CreatePrimitive(PrimitiveType.Plane);
        mBackground1.renderer.material = new Material(mManager.mReferences.mDefaultCharacterShader);
        mBackground1.transform.rotation = Quaternion.AngleAxis(90, Vector3.right) * mBackground1.transform.rotation;
        mBackground1.transform.position = mBackground1.transform.position + new Vector3(0, 0, -2);
	}
	
    public override void Update()
    {
	}

    public void set_layer(int aLayer)
    {
        mlayer = aLayer;
        mBackground1.layer = aLayer;
    }
    public void character_changed_listener(CharacterTextureBehaviour aCharacter)
    {
        //resize the background and set the texture
        mBackground1.transform.localScale = new Vector3(
            BodyManager.convert_units(aCharacter.background1.width) / 10.0f, 1, 
            BodyManager.convert_units(aCharacter.background1.height) / 10.0f);
        mBackground1.renderer.material.mainTexture = aCharacter.background1;
        
        //resize the camera
        foreach (Camera c in mManager.mCameraManager.AllCameras)
        {
            /*float texRatio = aCharacter.background1.width / (float)aCharacter.background1.height;
            float camRatio = c.aspect;
            if (camRatio > texRatio) //match width
                c.orthographicSize = BodyManager.convert_units(aCharacter.background1.width / camRatio) / 2.0f;
            else
                c.orthographicSize = BodyManager.convert_units(aCharacter.background1.height) / 2.0f;*/
            resize_camera_against_texture(c, aCharacter.background1);
        }
    }

    public static void resize_camera_against_texture(Camera aCam, Texture aTex, float aDistance = 1)
    {
        //TODO what if camera is not orthographic
        float texRatio = aTex.width / (float)aTex.height;
        float camRatio = aCam.aspect;
        if (camRatio > texRatio) //match width
            aCam.orthographicSize = BodyManager.convert_units(aTex.width / camRatio) / 2.0f;
        else
            aCam.orthographicSize = BodyManager.convert_units(aTex.height) / 2.0f;
    }
}
