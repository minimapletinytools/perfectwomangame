using UnityEngine;
using System.Collections;

public class BackgroundManager  : FakeMonoBehaviour
{
    public int mBackgroundLayer = 0; //should not be private TODO
    int mForegroundLayer = 0;

    FlatElementImage mBackground;

    public FlatElementMultiImage mBackgroundElements;

    FlatElementMultiImage mForegroundElements;
    
    public BackgroundManager(ManagerManager aManager) : base(aManager) { }

	public override void Start () {
        mManager.mEventManager.character_changed_event += character_changed_listener;

        mBackground = new FlatElementImage(null, 0);
        mBackground.HardPosition = Vector3.zero;

        mBackgroundElements = new FlatElementMultiImage(5);
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
        mBackgroundElements.destroy();
        mForegroundElements.destroy();
        for (int i = 0; i < aCharacter.backgroundElements.Length; i++)
        {
            mBackgroundElements.add_image(aCharacter.backgroundElements[i], FlatBodyObject.find_first_color(new Color(255, 0, 5 * i / (float)255), aCharacter.elementPositoner));
            //mBackgroundElements.mElements[mBackgroundElements.mElements.Count - 1].Element.Events.add_event(FlatElementAnimations.position_jiggle_delegate(Mathf.Infinity, 5),0);
            mBackgroundElements.mElements[mBackgroundElements.mElements.Count - 1].Element.Events.add_event((new FlatElementAnimations.FloatingAnimation(Random.Range(0f, 10f))).animate,0);
        }
        for (int i = 0; i < aCharacter.foregroundElements.Length; i++)
        {
            //mForegroundElements.add_image(aCharacter.foregroundElements[i], FlatBodyObject.find_first_color(new Color(0, 255, 5 * i / (float)255), aCharacter.elementPositoner));
        }

        set_background_layer(mBackgroundLayer);
        set_foreground_layer(mForegroundLayer);
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
