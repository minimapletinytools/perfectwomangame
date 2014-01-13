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

	Texture2D mOutputTexture;
	FlatElementImage mPlayerImage;

	public void initialize()
	{
		IsLoaded = false;
		TED = new TimedEventDistributor();
		mFlatCamera = new FlatCameraManager(mManager.mCameraManager.BackgroundCamera.transform.position, 10);
		mFlatCamera.fit_camera_to_game(); 
		mFlatCamera.Camera.cullingMask = (1 << 3) | (1 << 0);
		mFlatCamera.set_render_texture_mode(true);


		mPlayerImage = new FlatElementImage(null,10);
		mPlayerImage.PrimaryGameObject.name = "PLAYERIMAGE";
		mElement.Add(mPlayerImage);
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


	class PlayerStageGroup
	{
		public Texture2D playerTex = null;
		public CharacterIndex Index {get; set;}
	}

	List<PlayerStageGroup> mStages = new List<PlayerStageGroup>();

	public int gift_count() { return mStages.Count; }

	public void add_character(CharacterIndex aIndex)
	{
		mStages.Add(new PlayerStageGroup(){Index = aIndex});
	}
	public void capture_player()
	{
		var tex = mManager.mZigManager.ImageView.take_color_image();
		mStages.Last().playerTex = new Texture2D(tex.width,tex.height,tex.format,false);
		mStages.Last().playerTex.SetPixels(tex.GetPixels());
		mStages.Last().playerTex.Apply();
	}

	public void set_background_for_render()
	{
		mManager.mBackgroundManager.character_changed_listener(mManager.mGameManager.CurrentCharacterLoader);
	}
	public Texture render_gift(int index)
	{
		if(index < mStages.Count)
		{
			mPlayerImage.set_new_texture(mStages[index].playerTex,new Vector2(1280,960));
			//mElement.Add(construct_flat_image("REWARD_"+mStages[index].Index.StringIdentifier,index));
			update(); //note if you have any TED stuff, this will update it a second time
			ModeNormalPlay.draw_render_texture(mFlatCamera);
		}
		return mFlatCamera.RT;
	}
	
}