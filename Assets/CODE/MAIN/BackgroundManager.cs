using UnityEngine;
using System.Collections;

public class BackgroundManager  : FakeMonoBehaviour
{
    int mBackgroundLayer = 0;
    int mForegroundLayer = 0;

    FlatElementImage mBackground;

    FlatElementMultiImage mBackgroundElements;

    FlatElementMultiImage mForegroundElements;
    
    public BackgroundManager(ManagerManager aManager) : base(aManager) { }

	public override void Start () {
        mManager.mEventManager.character_changed_event += character_changed_listener;

        mBackground = new FlatElementImage(null, 0);
        mBackground.HardPosition = Vector3.zero;

        mBackgroundElements = new FlatElementMultiImage(1);
        mForegroundElements = new FlatElementMultiImage(100);
	}
	
    public override void Update()
    {
        mBackground.update(Time.deltaTime);
        mBackgroundElements.update(Time.deltaTime);
        mForegroundElements.update(Time.deltaTime);
	}

    public void set_background_layer(int aLayer)
    {
        mBackgroundLayer = aLayer;

        foreach (Renderer f in mBackground.PrimaryGameObject.GetComponentsInChildren<Renderer>())
            f.gameObject.layer = mBackgroundLayer;


        foreach( FlatElementMultiBase.ElementOffset e in mBackgroundElements.mElements)
            foreach(Renderer f in e.Element.PrimaryGameObject.GetComponentsInChildren<Renderer>())
                f.gameObject.layer = mBackgroundLayer;
    }

    public void set_foreground_layer(int aLayer)
    {
        mForegroundLayer = aLayer;
        foreach (FlatElementMultiBase.ElementOffset e in mForegroundElements.mElements)
            foreach (Renderer f in e.Element.PrimaryGameObject.GetComponentsInChildren<Renderer>())
                f.gameObject.layer = mForegroundLayer;
    }

    public void character_changed_listener(CharacterTextureBehaviour aCharacter)
    {
        set_background(aCharacter);
    }
    public void set_background(CharacterTextureBehaviour aCharacter)
    {

        mBackground.mImage.set_new_texture(aCharacter.background1);

        //TODO background and forgeground elements

        //resize the camera
        foreach (Camera c in mManager.mCameraManager.AllCameras)
            resize_camera_against_texture(c, aCharacter.background1);
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
