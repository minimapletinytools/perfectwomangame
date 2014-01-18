using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
public class CharacterBundleManager : FakeMonoBehaviour {
	
	public CharacterBundleManager(ManagerManager aManager) : base(aManager) 
	{
	}
	
	public override void Start()
	{
		//Debug.Log ("starting CBM");
		mManager.mAssetLoader.new_load_poses("POSES",this);
		mManager.mAssetLoader.new_load_interface_images("IMAGES",this);
		//load_mini_characters();
	}
			

	public bool is_initial_loaded()
	{
		return mPosesLoaded && (mNumberMiniCharactersLoading == 0) && mImagesLoaded;
			//&& (mManager.mGameManager.mModeNormalPlay.mSunsetManager.IsLoaded); //TODO may want to check game mode to make sure we aren't in testing or simian
	}
	
	
	
	
	[System.Serializable]
	public class ImageSizeData
	{
		public string Name {get; set;}
		public Vector2 Size {get; set;}
	}
	
	public class ImageSizePair
	{
		public ImageSizeData Data {get; set;}
		public Texture2D Image {get; set;}
	}
	AssetBundle ImageBundle {get; set;}
	List<ImageSizeData> ImageIndex {get; set;}
	bool mImagesLoaded = false;
	public void interface_loaded_callback(AssetBundle aBundle)
	{
        TextAsset cd = aBundle.Load("INDEX", typeof(TextAsset)) as TextAsset;
        System.IO.MemoryStream stream = new System.IO.MemoryStream(cd.bytes);
        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<ImageSizeData>));
        ImageIndex = xs.Deserialize(stream) as List<ImageSizeData>;
		ImageBundle = aBundle;
		add_bundle_to_unload(ImageBundle);
		mImagesLoaded = true;

		
		//foreach(ImageSizeData e in ImageIndex.OrderBy(f => f.Name))
			//Debug.Log (e.Name + " " + e.Size );
	}
	public ImageSizePair get_image(string identifier)
	{
		ImageSizePair r = new ImageSizePair();
		r.Image = ImageBundle.Load(identifier) as Texture2D;
		r.Data = ImageIndex.Find(e => e.Name == identifier); //TODO not finding size data

		if(r.Data == null || r.Image == null)
			Debug.Log("could not find id " + identifier + " image " + r.Image + " data " + r.Data);
		return r;
	}
	
	
	
	//----------
	//mini char loading
	//----------
	Mutex mMiniCharLock;
	CharIndexContainerCharacterLoader mMiniCharacters = new CharIndexContainerCharacterLoader();
	int mNumberMiniCharactersLoading = 0;
	//mini bundle related
	public void load_mini_characters()
	{
		mMiniCharLock = new Mutex();
		foreach(CharacterIndex index in CharacterIndex.sAllCharacters)
		{
			
			if(mManager.mAssetLoader.does_bundle_exist(index.StringIdentifier+"_mini"))
			{
				mNumberMiniCharactersLoading++;
				mManager.mAssetLoader.new_load_mini_characater(index.StringIdentifier, this);
			}
			else
				mMiniCharacters[index] = null;
		}
    }
	public void mini_loaded_callback(AssetBundle aBundle, string aBundleName)
	{
		CharacterIndex index = (new CharacterIndex(aBundleName));
		using(mMiniCharLock)
		{
			mMiniCharacters[index] = new CharacterLoader();
			mMiniCharacters[index].complete_load_character(aBundle,aBundleName);
		}
		aBundle.Unload(false);
		mNumberMiniCharactersLoading--;
	}
	public CharacterLoader get_mini_character(CharacterIndex aIndex)
	{
		using(mMiniCharLock)
			return mMiniCharacters[aIndex];
		/*
		if(mMiniCharacters[aIndex] == null)
		{
			CharacterTextureBehaviour ctb = (GameObject.Instantiate(mManager.mMenuReferences.miniMan) as  GameObject).GetComponent<CharacterTextureBehaviour>();
			return new FlatBodyObject(ctb,aDepth);
			GameObject.Destroy(ctb.gameObject);
		}
		else
			return new FlatBodyObject(mMiniCharacters[aIndex],aDepth);
			*/
	}
	

	//----------
	//scene bundle related
	//----------
	List<AssetBundle> mUnloadAtEnd = new List<AssetBundle>();
	public void add_bundle_to_unload(AssetBundle aBundle)
	{
		mUnloadAtEnd.Add(aBundle);
	}
	public void unload_bundle(AssetBundle aBundle)
	{
		aBundle.Unload(true);
		mUnloadAtEnd.Remove(aBundle);
	}

    public void scene_loaded_callback(AssetBundle aBundle, string aBundleName)
    {
        //Debug.Log("loading character in CharacterLoader " + aBundleName);
		//TODo don't do this serial
        CharacterLoader loader = new CharacterLoader();
        loader.complete_load_character(aBundle,aBundleName);
	
		//TODO potentially want to move this to NewGameManager
		//here we assume the game wants the new character to be loaded so we load it
		//set new character in the two body managers and in background manager
		mManager.mBackgroundManager.character_changed_listener(loader);
		if(aBundleName != "999"){ //special behaviour for grave
			mManager.mBodyManager.character_changed_listener(loader);
			mManager.mTransparentBodyManager.character_changed_listener(loader);
			//TODO set to actual pose that we want
			mManager.mTransparentBodyManager.set_target_pose(mManager.mReferences.mCheapPose.to_pose(),true);
			if(mManager.mZigManager.is_reader_connected() != 2)
				mManager.mBodyManager.set_target_pose(mManager.mReferences.mCheapPose.to_pose(),true);
		}
		else{
			mManager.mBodyManager.destroy_character();
			mManager.mTransparentBodyManager.destroy_character();
		}
		mManager.mMusicManager.character_changed_listener(loader);
		
		if(mManager.mGameManager.character_changed_listener(loader))
			aBundle.Unload(false);
		else mUnloadAtEnd.Add(aBundle);
	}
	
	
	
	//----------
	//pose bundle related
	//----------
	bool mPosesLoaded = false;
	Dictionary<string, Pose> mPoses = new Dictionary<string, Pose>();
	CharacterHelper mCharacterHelper = new CharacterHelper();
	public string construct_pose_string(CharacterIndex aIndex, int aDiff, int aStage)
	{
		string r = "";
		r += aIndex.StringIdentifier;
		r += "_";
		r += (new string[] {"a","b","c","d"})[aDiff];
		r += "_";
		r += aStage;
		return r;
	}
    public CharacterHelper get_character_helper() //this is just to save some time for refractoring, othrewise should aways use the routine below...
    {
        return mCharacterHelper;
    }
    public CharacterStats get_character_stat(CharacterIndex aChar)
    {
        return mCharacterHelper.Characters[aChar];
    }
	public PoseAnimation get_pose(CharacterIndex aIndex, int aDiff)
	{
		PoseAnimation r = new PoseAnimation();
		if(mPoses.ContainsKey(construct_pose_string(aIndex,aDiff,1)))
		{
			for(int stage = 1; ; stage++)
			{
				string find = construct_pose_string(aIndex,aDiff,stage);
				if(mPoses.ContainsKey(find))
				{
					r.poses.Add(mPoses[find]);
				}
				else
					break;
			}
			return r;
		}
		/*else if(aIndex.Choice > 0)
		{
			Debug.Log ("grabbed fallback for " + construct_pose_string(aIndex,aDiff,1));
			CharacterIndex fallback = new CharacterIndex(aIndex.LevelIndex,aIndex.Choice-1);
			return get_pose(fallback,aDiff);
		}*/
		else if(aIndex.LevelIndex > 1)
		{
			Debug.Log ("grabbed fallback for " + construct_pose_string(aIndex,aDiff,1));
			CharacterIndex fallback = new CharacterIndex(aIndex.LevelIndex-1,aIndex.Choice);
			return get_pose(fallback,aDiff);
		}
		else
		{
			r.poses.Add(mManager.mReferences.mCheapPose.to_pose());
			return r;
		}
	}
	public void pose_bundle_loaded_callback(AssetBundle aBundle)
    {
		
		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
			string txtName = "info_"+e.StringIdentifier;
			if(aBundle.Contains(txtName))
			{
				//Debug.Log ("loaded character info " + txtName);
				mCharacterHelper.Characters[e].CharacterInfo = 
					NUPD.CharacterInformationProcessor.process_character((aBundle.Load(txtName) as TextAsset).text);
				
				//kind of a hack.
				//TODO uncomment this when you get new character packages in...
				GameConstants.INDEX_TO_SHORT_NAME[e] = mCharacterHelper.Characters[e].CharacterInfo.ShortName;
				//GameConstants.INDEX_TO_FULL_NAME[e] = mCharacterHelper.Characters[e].CharacterInfo.LongName;
				GameConstants.INDEX_TO_DESCRIPTION[e] = mCharacterHelper.Characters[e].CharacterInfo.Description;
			}
			else
			{
				//Debug.Log ("no info found for " + txtName);
				mCharacterHelper.Characters[e].CharacterInfo = NUPD.CharacterInformation.default_character_info(e);
			}
		}

		//this is a hack
		//randomize difficulty changes for fetus
		var avail = CharacterIndex.sAllCharacters
			.Where(f => f.LevelIndex == 1)
				.Where(f=>mManager.mMetaManager.UnlockManager.is_unlocked(f) == 1).ToArray();
		ChoiceHelper.Shuffle(avail);
		var changeThisInfo = mCharacterHelper.Characters[CharacterIndex.sFetus].CharacterInfo;
		changeThisInfo.ChangeSet[0].Changes[1].Changes[avail[0]] = -1;
		changeThisInfo.ChangeSet[0].Changes[2].Changes[avail[1]] = 1;
		
		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
			for(int i = 0; i < 4; i++)
			{
				for(int j = 1; j < 10; j++) //assuming no mroe than 10 poses per animatino
				{
					string s = construct_pose_string(e,i,j);
					if(aBundle.Contains(s))
					{
						mPoses[s] = (aBundle.Load(s) as TextAsset).to_pose();
					}
				}
			}
		}
        aBundle.Unload(true); //don't need this anymore I don't ithnk...
		mPosesLoaded = true;
    }
	
	public void cleanup()
	{
		Debug.Log ("cleaning up asset bundles");
		foreach(AssetBundle e in mUnloadAtEnd)
		{
			e.Unload(true);
		}
		mUnloadAtEnd.Clear();
	}
}
