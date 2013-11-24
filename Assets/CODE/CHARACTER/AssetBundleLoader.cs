using UnityEngine;
using System.Collections.Generic;
using System;
public class AssetBundleLoader : FakeMonoBehaviour
{
 

    System.Collections.Generic.Dictionary<WWW, Action<AssetBundle> > mRequestLists = new System.Collections.Generic.Dictionary<WWW, Action<AssetBundle>>();

    public AssetBundleLoader(ManagerManager aManager)
        : base(aManager) 
    {}
	
    public override void Update()
    {

        List<WWW> removal = new System.Collections.Generic.List<WWW>();
        //TODO gets out of sync error here why??
        //not a huge deal since itonly happens in my recording scene. Probbably some stupid threading error...
        foreach (KeyValuePair<WWW,Action<AssetBundle>> e in mRequestLists) 
        {
            if (e.Key.isDone)
            {
                removal.Add(e.Key);
                e.Value(e.Key.assetBundle);
            }
        }
        foreach (WWW e in removal)
            mRequestLists.Remove(e);
        
    }

    public bool does_bundle_exist(string aBundleName)
    {
        string filename = Application.dataPath + "/Resources/" + aBundleName + ".unity3d";
        return System.IO.File.Exists(filename);
    }
	
	public void new_load_character(string aChar, CharacterBundleManager aManager, System.Action<AssetBundle> aCallback)
	{
		string filename = "file://" + Application.dataPath + "/Resources/" + aChar + ".unity3d";
        //Debug.Log("loading from " + filename);
        mRequestLists.Add(new WWW(filename), aCallback);
	}
	
	public void new_load_character(string aChar, CharacterBundleManager aManager)
	{
		new_load_character(aChar,aManager,delegate(AssetBundle aBundle) { aManager.scene_loaded_callback(aBundle,aChar); });
	}
	
	public void new_load_mini_characater(string aChar, CharacterBundleManager aManager)
    {
        string filename = "file://" + Application.dataPath + "/Resources/" + aChar + "_mini.unity3d";
        //Debug.Log("loading mini char from " + filename);
        mRequestLists.Add(new WWW(filename), delegate(AssetBundle aBundle) { aManager.mini_loaded_callback(aBundle,aChar); });
    }
	
 	public void new_load_poses(string aAssetBundle, CharacterBundleManager aManager)
    {
        string filename = "file://" + Application.dataPath + "/Resources/" + aAssetBundle + ".unity3d";
        //Debug.Log("loading poses from " + filename);
        WWW request = new WWW(filename);
        request.threadPriority = ThreadPriority.High;
        mRequestLists.Add(request, delegate(AssetBundle aBundle) { aManager.pose_bundle_loaded_callback(aBundle); });
    }
	
	public void new_load_interface_images(string aAssetBundle, CharacterBundleManager aManager)
	{
		string filename = "file://" + Application.dataPath + "/Resources/" + aAssetBundle + ".unity3d";
        //Debug.Log("loading poses from " + filename);
        WWW request = new WWW(filename);
        request.threadPriority = ThreadPriority.High;
        mRequestLists.Add(request, delegate(AssetBundle aBundle) { aManager.interface_loaded_callback(aBundle); });
	}
	
	public void new_load_asset_bundle(string aAssetBundle, Action<AssetBundle> aAction)
	{
		string filename = "file://" + Application.dataPath + "/Resources/" + aAssetBundle + ".unity3d";
        //Debug.Log("loading poses from " + filename);
        WWW request = new WWW(filename);
        request.threadPriority = ThreadPriority.High;
        mRequestLists.Add(request, aAction);
	}
}
