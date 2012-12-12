using UnityEngine;
using System.Collections;

public class AssetBundleLoader : FakeMonoBehaviour
{


    public AssetBundleCreateRequest ActiveRequest
    {
        get;
        private set;
    }

    public WWW ActiveWWWRequest
    {
        get;
        private set;
    }
    public AssetBundleLoader(ManagerManager aManager)
        : base(aManager) 
    {
        ActiveRequest = null;
        ActiveWWWRequest = null;
    }

    public override void Update()
    {
        if (ActiveRequest != null)
        {
            if (ActiveRequest.isDone)
            {
                GameObject pf = (GameObject)ActiveRequest.assetBundle.Load("0",typeof(GameObject));
                Debug.Log("trying to instatiate " + pf);
                GameObject instant = (GameObject)GameObject.Instantiate(pf);
            }
        }

        if (ActiveWWWRequest != null)
        {
        }

        
    }

    //public System.Collections.IEnumerable load_character(string aChar)
    public void load_character(string aChar)
    {

        Debug.Log("wtf");
        string filename = "file://" + Application.dataPath + "Resources/" + aChar;
        Debug.Log("loading from " + filename);
        ActiveWWWRequest = new WWW(filename);
        
        TextAsset fuck = (TextAsset)Resources.Load("0", typeof(TextAsset));
        ActiveRequest = AssetBundle.CreateFromMemory(fuck.bytes);

        /*
        AssetBundle character = req.assetBundle;
        Debug.Log(character);
        GameObject pf = (GameObject)character.Load("0/0", typeof(GameObject));
        Debug.Log(pf);
        pf = (GameObject)character.Load("0", typeof(GameObject));
        Debug.Log(pf);
        throw new UnityException("poo");*/
    }
}
