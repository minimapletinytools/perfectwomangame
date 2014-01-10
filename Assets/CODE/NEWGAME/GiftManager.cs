using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class GiftManager 
{
	ManagerManager mManager;
	ModeNormalPlay mModeNormalPlay;
	public GiftManager(ManagerManager aManager,ModeNormalPlay aNormalPlay)
	{
		mModeNormalPlay = aNormalPlay;
		mManager = aManager;
	}

	public TimedEventDistributor TED { get; private set; }
    public FlatCameraManager mFlatCamera;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	CharacterLoader mLoader;
	public bool IsLoaded{get; private set;}
	
	public void initialize()
	{
		IsLoaded = false;
		TED = new TimedEventDistributor();
		mFlatCamera = new FlatCameraManager(new Vector3(10000, 3000, 0), 10);
		mFlatCamera.fit_camera_to_game(); 
	}

	
	public void gift_loaded_callback(AssetBundle aBundle, string aBundleName)
	{
		NewMenuReferenceBehaviour refs = mManager.mNewRef;
		mLoader = new CharacterLoader();
		mLoader.complete_load_character(aBundle,aBundleName);
		mManager.mCharacterBundleManager.add_bundle_to_unload(aBundle);
		IsLoaded = true;
	}

	
	FlatElementImage construct_flat_image(string aName, int aDepth)
	{
		var sizing = mLoader.Sizes.find_static_element(aName);
		var r = new FlatElementImage(mLoader.Images.staticElements[aName],sizing.Size,aDepth);
		r.HardPosition = mFlatCamera.Center + sizing.Offset;
		return r;
	}
	
	public void update()
	{
		mFlatCamera.update(Time.deltaTime);
		foreach (FlatElementBase e in mElement)
			e.update(Time.deltaTime);       
		TED.update(Time.deltaTime);
	}

	
}