using UnityEngine;
using System.Collections.Generic;

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
	
	
	
	public void load_images(CharacterLoader aCharacter, FlatElementMultiImage aMulti, string aPrefix, int aBegin = 0)
	{
		//last to first stupid hack..
		List<CharacterData.ImageSizeOffsetAnimationData> dataList = new List<CharacterData.ImageSizeOffsetAnimationData>();
		List<string> nameList = new List<string>();
		for(int i = aBegin; i < 100; i++)
		{
			string name = aPrefix+i;
			CharacterData.ImageSizeOffsetAnimationData data = aCharacter.Sizes.find_static_element(name);
			if(data == null)
				break;
			dataList.Add(data);
			nameList.Add(name);
		}
		
		for(int i = dataList.Count-1; i>=0; i--)
		{
			Texture2D tex = aCharacter.Images.staticElements[nameList[i]];
			//if(tex == null)
				//throw new UnityException("data exists for " + data.Name + " but texture does not");
			aMulti.add_image(tex,dataList[i].Offset,dataList[i].Size);
				
			//dataList[i].AnimationEffect
		}
	}
	
	
	//note aCharacter need not be the same as teh original character (use this or death)
	public void load_cutscene(int aNum, CharacterLoader aCharacter)
	{
		foreach(FlatElementMultiBase.ElementOffset e in mBackgroundElements.mElements)
			e.Element.SoftColor = new Color(1,1,1,0);
		foreach(FlatElementMultiBase.ElementOffset e in mForegroundElements.mElements)
			e.Element.SoftColor = new Color(1,1,1,0);
		
		string prefix = "CUTSCENE"+aNum+"_";
		load_images(aCharacter,mForegroundElements,prefix);
		set_foreground_layer(mForegroundLayer);
	}
	
    public void character_changed_listener(CharacterLoader aCharacter)
    {
        mBackground.mImage.set_new_texture(aCharacter.Images.background1,aCharacter.Sizes.mBackSize);
        mBackgroundElements.destroy();
        mForegroundElements.destroy();
		
		load_images(aCharacter,mBackgroundElements,"BG-",1);
		load_images(aCharacter,mForegroundElements,"FG-",1);

        set_background_layer(mBackgroundLayer);
        set_foreground_layer(mForegroundLayer);
		
        //resize the camera
        foreach (Camera c in mManager.mCameraManager.AllCameras)
            resize_camera(c, aCharacter.Sizes.mBackSize);
		
		
		//TODO could in theory buffre cutscenes and deaths here.
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
