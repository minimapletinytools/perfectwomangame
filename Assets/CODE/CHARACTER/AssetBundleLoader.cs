using UnityEngine;
using System.Collections.Generic;

public class AssetBundleLoader : FakeMonoBehaviour
{
 

    public delegate void AssetBundleLoadedDelegate(AssetBundle bundle);
    System.Collections.Generic.Dictionary<WWW, AssetBundleLoadedDelegate> mRequestLists = new System.Collections.Generic.Dictionary<WWW, AssetBundleLoadedDelegate>();

    public AssetBundleLoader(ManagerManager aManager)
        : base(aManager) 
    {}

    public override void Update()
    {

        List<WWW> removal = new System.Collections.Generic.List<WWW>();
        //TODO gets out of sync error here why??
        //not a huge deal since itonly happens in my recording scene. Probbably some stupid threading error...
        foreach (KeyValuePair<WWW,AssetBundleLoadedDelegate> e in mRequestLists) 
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


    public class CharacterBundleLoadedCallback
    {
        string BundleName { get; set; }
        public CharacterBundleLoadedCallback(string aBundleName)
        {
            BundleName = aBundleName;
        }
        public void call(AssetBundle aBundle)
        {
            ManagerManager.Manager.mGameManager.scene_loaded_callback(aBundle, BundleName);
        }
    }

    public class MiniBundleLoadedCallback
    {
        string BundleName { get; set; }
        public MiniBundleLoadedCallback(string aBundleName)
        {
            BundleName = aBundleName;
        }
        public void call(AssetBundle aBundle)
        {
            //TODO
            ManagerManager.Manager.mInterfaceManager.mini_bundle_loaded_listener(aBundle, BundleName);
        }
    }

    //public System.Collections.IEnumerable load_character(string aChar)
    public void load_character(string aChar)
    {
        string filename = "file://" + Application.dataPath + "/Resources/" + aChar + ".unity3d";
        Debug.Log("loading from " + filename);
        mRequestLists.Add(new WWW(filename), (new CharacterBundleLoadedCallback(aChar)).call);
    }

    public void load_mini_characater(string aChar)
    {
        string filename = "file://" + Application.dataPath + "/Resources/" + aChar + "_mini.unity3d";
        Debug.Log("loading from " + filename);
        mRequestLists.Add(new WWW(filename), (new MiniBundleLoadedCallback(aChar)).call);
    }

    public void load_poses(string aAssetBundle)
    {
        string filename = "file://" + Application.dataPath + "/Resources/" + aAssetBundle + ".unity3d";
        Debug.Log("loading from " + filename);
        WWW request = new WWW(filename);
        request.threadPriority = ThreadPriority.High;
        mRequestLists.Add(request, ManagerManager.Manager.mGameManager.pose_bundle_loaded_callback);
    }
	
	public void new_load_character(string aChar, AssetBundleManager aManager)
	{
		string filename = "file://" + Application.dataPath + "/Resources/" + aChar + ".unity3d";
        Debug.Log("loading from " + filename);
        mRequestLists.Add(new WWW(filename), (delegate(AssetBundle aBundle) { aManager.scene_loaded_callback(aBundle,aChar); }));
	}
	
	public void new_load_mini_characater(string aChar, AssetBundleManager aManager)
    {
        string filename = "file://" + Application.dataPath + "/Resources/" + aChar + "_mini.unity3d";
        Debug.Log("loading mini char from " + filename);
		//TODO
		//var cbDelegate = delegate(AssetBundle aBundle) { aManager.scene_loaded_callback(aBundle,aChar); };
        //mRequestLists.Add(new WWW(filename), cbDelegate);
    }
	
 	public void new_load_poses(string aAssetBundle, AssetBundleManager aManager)
    {
        string filename = "file://" + Application.dataPath + "/Resources/" + aAssetBundle + ".unity3d";
        Debug.Log("loading poses from " + filename);
        WWW request = new WWW(filename);
        request.threadPriority = ThreadPriority.High;
        mRequestLists.Add(request, delegate(AssetBundle aBundle) { aManager.pose_bundle_loaded_callback(aBundle); });
    }
	
}
