using UnityEngine;
using System.Collections.Generic;

public class SunsetManager 
{
	ManagerManager mManager;
    public SunsetManager(ManagerManager aManager)
	{
		mManager = aManager;
		IsLoaded = false;
	}

	public TimedEventDistributor TED { get; private set; }
    public FlatCameraManager mFlatCamera;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	public void initialize()
	{
		TED = new TimedEventDistributor();
		mFlatCamera = new FlatCameraManager(new Vector3(10000, -3000, 0), 10);
		mFlatCamera.fit_camera_to_game();
	}
	
	
	CharacterLoader mLoader;

	FlatElementImage mBackground;
	FlatElementImage mStubbyHairyGrass;
	FlatElementImage mSun;
	FlatElementImage mLightRay;
	List<FlatElementImage> mCharacters = new List<FlatElementImage>();
	
	bool IsLoaded{get;set;}
	
	
	FlatElementImage construct_flat_image(string aName, int aDepth)
	{
		Debug.Log(aName);
		var sizing = mLoader.Sizes.find_static_element(aName);
		var r = new FlatElementImage(mLoader.Images.staticElements[aName],sizing.Size,aDepth);
		r.HardPosition = sizing.Offset;
		return r;
	}
	
	public void sunset_loaded_callback(AssetBundle aBundle, string aBundleName)
	{
		NewMenuReferenceBehaviour refs = mManager.mNewRef;
		
		mLoader = new CharacterLoader();
        mLoader.complete_load_character(aBundle,aBundleName);
		
		
		
		//should turn this into a function geez...
		mBackground = new FlatElementImage(mLoader.Images.background1,mLoader.Sizes.mBackSize,0);
		mBackground.HardPosition = mFlatCamera.get_point(0,0);
		//mStubbyHairyGrass = construct_flat_image("STUBBY_HAIRY_GRASS",2);
		
		mElement.Add(mBackground);
		//mElement.Add(mStubbyHairyGrass);
		
		
		//mManager.mCharacterBundleManager.get_image();
		//TODO read this from image asset bundle
		//mSun = construct_flat_image("SUN",1);
		//mLightRay = construct_flat_image("SHINE",10);
		//mLightRay.HardPosition = mFlatCamera.get_point(0,-10000);
		//mElement.Add(mSun);
		//mElement.Add(mLightRay);
		
		
		IsLoaded = true;
	}
	
	public void add_character(CharacterIndex aChar)
	{
		var addMe = construct_flat_image("SUNSET_"+aChar.StringIdentifier,4);
		mCharacters.Add(addMe);
		mElement.Add(addMe);
		
	}
	
	public void update()
	{
		mFlatCamera.update(Time.deltaTime);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		TED.update(Time.deltaTime);
		
		
		//TODO should render mFlatCamera to a render texture
	}
	
	
	
	
	//TODO I just copied this from NIM...
	//new PopupTextObject("",10,loader.Images.staticElements["BUBBLE"]); //add sizing...
	public PopupTextObject add_timed_text_bubble(string aMsg, float duration, float yRelOffset = 0)
	{
		PopupTextObject to = new PopupTextObject(aMsg,30);
		//to.HardPosition = random_position();
		to.HardColor = GameConstants.UiWhiteTransparent;
		to.SoftColor = GameConstants.UiWhite;
		to.set_text_color(GameConstants.UiWhiteTransparent,true);
		to.set_text_color(GameConstants.UiRed);
		to.set_background_color(GameConstants.UiWhiteTransparent,true);
		to.set_background_color(GameConstants.UiPopupBubble);
		TimedEventDistributor.TimedEventChain chain = TED.add_event(
			delegate(float aTime)
			{
				to.SoftPosition = mFlatCamera.get_point(0,yRelOffset); //fly in
				//to.HardPosition = mFlatCamera.get_point(0.40f,yRelOffset); //cut in
				mElement.Add(to);
				return true;
			},
        0).then(
			delegate(float aTime)
			{
				if(aTime > duration) 
					return true;
			/*
				if(DoSkipSingleThisFrame)
				{
					DoSkipSingleThisFrame = false;
					return true;
				}*/
				return false;
			},
		0).then_one_shot(
			delegate()
			{
				//cutout
				//mElement.Remove(to);
				//to.destroy();
				
				//fadeout
				to.fade_out();  
			},
		0).then_one_shot(
			delegate()
			{
				//fadeout
				mElement.Remove(to);
				to.destroy();
			},
		2);
		return to;
	}
}
