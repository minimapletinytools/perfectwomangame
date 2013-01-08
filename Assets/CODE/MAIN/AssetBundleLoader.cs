using UnityEngine;
using System.Collections.Generic;

public class AssetBundleLoader : FakeMonoBehaviour
{
    [System.Serializable]
    public class CharacterData
    {
        //ordered as in CharacterPreprocessor.sLimbs
        public List<List<Vector3>> mMountingPositions = new List<List<Vector3>>();
        public List<Vector3> mBackgroundPositions = new List<Vector3>();
        public List<Vector3> mForegroundPositions = new List<Vector3>();

        public List<Vector2> mLimbSizes = new List<Vector2>();
        public List<Vector2> mBackgroundSizes = new List<Vector2>();
        public List<Vector2> mForegroundSizes = new List<Vector2>();

        public Vector2 mBackSize = new Vector2();

        public string mName = "";
    }

    public delegate void AssetBundleLoadedDelegate(AssetBundle bundle);
    System.Collections.Generic.Dictionary<WWW, AssetBundleLoadedDelegate> mRequestLists = new System.Collections.Generic.Dictionary<WWW, AssetBundleLoadedDelegate>();

    public AssetBundleLoader(ManagerManager aManager)
        : base(aManager) 
    {}

    public override void Update()
    {

        List<WWW> removal = new System.Collections.Generic.List<WWW>();
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
    //public System.Collections.IEnumerable load_character(string aChar)
    public void load_character(string aChar)
    {
        string filename = "file://" + Application.dataPath + "/Resources/" + aChar + ".unity3d";
        Debug.Log("loading from " + filename);
        mRequestLists.Add(new WWW(filename), (new CharacterBundleLoadedCallback(aChar)).call);
    }


    public void load_poses(string aAssetBundle)
    {
        string filename = "file://" + Application.dataPath + "/Resources/" + aAssetBundle + ".unity3d";
        Debug.Log("loading from " + filename);
        WWW request = new WWW(filename);
        request.threadPriority = ThreadPriority.High;
        mRequestLists.Add(request, ManagerManager.Manager.mGameManager.pose_bundle_loaded_callback);
    }
}
