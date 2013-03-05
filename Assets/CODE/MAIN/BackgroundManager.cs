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

    public void character_changed_listener(CharacterLoader aCharacter)
    {
		

        mBackground.mImage.set_new_texture(aCharacter.Images.background1,aCharacter.Sizes.mBackSize);

        mBackgroundElements.destroy();
        mForegroundElements.destroy();
        for (int i = 0; i < aCharacter.Images.backgroundElements.Count; i++)
        {
            mBackgroundElements.add_image(aCharacter.Images.backgroundElements[i], aCharacter.Sizes.mBackgroundPositions[i],aCharacter.Sizes.mBackgroundSizes[i]);
            mBackgroundElements.mElements[mBackgroundElements.mElements.Count - 1].Element.Events.add_event(FlatElementAnimations.position_jiggle_delegate(Mathf.Infinity, 5),0);
            mBackgroundElements.mElements[mBackgroundElements.mElements.Count - 1].Element.Events.add_event((new FlatElementAnimations.FloatingAnimation(Random.Range(0f, 10f))).animate,0);
        }
        for (int i = 0; i < aCharacter.Images.foregroundElements.Count; i++)
        {
            mForegroundElements.add_image(aCharacter.Images.foregroundElements[i], aCharacter.Sizes.mForegroundPositions[i],aCharacter.Sizes.mForegroundSizes[i]);
        }

       

        set_background_layer(mBackgroundLayer);
        set_foreground_layer(mForegroundLayer);
        //resize the camera
        foreach (Camera c in mManager.mCameraManager.AllCameras)
            resize_camera(c, aCharacter.Sizes.mBackSize);
    }

    public static void resize_camera(Camera aCam, Vector2 aSize, float aDistance = 1)
    {
        //TODO what if camera is not orthographic
        float texRatio = aSize.x / (float)aSize.y;
        float camRatio = aCam.aspect;
        if (camRatio > texRatio) //match width
            aCam.orthographicSize = BodyManager.convert_units(aSize.x / camRatio) / 2.0f;
        else
            aCam.orthographicSize = BodyManager.convert_units(aSize.y) / 2.0f;
    }
    //TODO delete this
    public static void resize_camera_against_texture(Camera aCam, Texture aTex, float aDistance = 1)
    {
        resize_camera(aCam, new Vector2(aTex.width, aTex.height), aDistance);
    }
}
