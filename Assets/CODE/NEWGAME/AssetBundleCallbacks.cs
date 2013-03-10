using UnityEngine;
using System.Collections;

public class AssetBundleCallbacks {
	
	NewGameManager mManager;
	public AssetBundleCallbacks(NewGameManager aManager)
	{
		mManager = aManager;
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
		mLastCharacterBundle = aBundle;
        CharacterLoader loader = new CharacterLoader();
        loader.complete_load_character(aBundle,aBundleName);
	
		//TODO
	}
	
	public void pose_bundle_loaded_callback(AssetBundle aBundle)
    {
		//TODO store info or give it to NGM
        
        aBundle.Unload(true); //don't need this anymore I don't ithnk...
    }
}
