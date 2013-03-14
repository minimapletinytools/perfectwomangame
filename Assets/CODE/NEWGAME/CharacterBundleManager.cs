using UnityEngine;
using System.Collections.Generic;

public class CharacterBundleManager : FakeMonoBehaviour {
	
	public CharacterBundleManager(ManagerManager aManager) : base(aManager) 
	{
	}
	
	public override void Start()
	{
		mManager.mAssetLoader.new_load_poses("POSES",this);
	}
			
	
	//mini bundle related
	public void load_mini_characters()
	{
	
	
    }

    //scene bundle related
	AssetBundle mLastCharacterBundle = null;
    public void scene_loaded_callback(AssetBundle aBundle, string aBundleName)
    {
		if(mLastCharacterBundle != null) //we are loading a new bundle so I guess we don't need the old bundle anymore
			mLastCharacterBundle.Unload(true);
	
        //Debug.Log("loading character in CharacterLoader " + aBundleName);
		//TODo don't do this serial
		mLastCharacterBundle = aBundle;
        CharacterLoader loader = new CharacterLoader();
        loader.complete_load_character(aBundle,aBundleName);
	
		//here we assume the game wants the new character to be loaded so we load it
		//set new character in the two body managers and in background manager
		mManager.mBackgroundManager.character_changed_listener(loader);
		if(aBundleName != "999"){ //special behaviour for grave
			mManager.mBodyManager.character_changed_listener(loader);
			mManager.mTransparentBodyManager.character_changed_listener(loader);
			//TODO set to actual pose that we want
			mManager.mTransparentBodyManager.set_target_pose(mManager.mMenuReferences.cheapPose.to_pose());
		}
		else{
			mManager.mBodyManager.destroy_character();
			mManager.mTransparentBodyManager.destroy_character();
		}
		mManager.mMusicManager.character_changed_listener(loader);
		mManager.mGameManager.character_changed_listener(loader);
	}
	
	
	Dictionary<string, ProGrading.Pose> mPoses = new Dictionary<string, ProGrading.Pose>();
	public string construct_pose_string(CharacterIndex aIndex, int aDiff, int aStage)
	{
		string r = "";
		r += aIndex.StringIdentifier;
		r += "_";
		r += (new string[] {"a","b","c","d"})[aDiff];
		r += "-";
		r += aStage;
		return r;
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
					r.poses.Add(mPoses[find]);
				else
					break;
			}
			return r;
		}
		else if(aIndex.Choice > 0)
		{
			CharacterIndex fallback = new CharacterIndex(aIndex.Level,aIndex.Choice-1);
			return get_pose(fallback,aDiff);
		}
		else
		{
			r.poses.Add(mManager.mMenuReferences.cheapPose.to_pose());
			return r;
		}
	}
	public void pose_bundle_loaded_callback(AssetBundle aBundle)
    {
		//TODO store info in mPoses
        aBundle.Unload(true); //don't need this anymore I don't ithnk...
    }
}
