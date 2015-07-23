using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
public class TestingSceneBehaviour : MonoBehaviour {
	
	Texture2D mImage = null;
	//CharacterIndex mLastCharacter = new CharacterIndex(-1,0);
	public void Start()
	{
		ManagerManager.Manager.mGameManager.set_testing();
		GameConstants.FORCE_START = true;
		//ManagerManager.Manager.mRecordMode = true;
		

	}
	
	public void Update()
	{
		/* draws character icon in upper right corner lol..
		if(ManagerManager.Manager.mCharacterBundleManager.is_initial_loaded() && ManagerManager.Manager.mGameManager.DeathCharacter != null)
		{
			
			if(mLastCharacter != ManagerManager.Manager.mGameManager.CurrentCharacterIndex)
			{
				mLastCharacter = ManagerManager.Manager.mGameManager.CurrentCharacterIndex;
				mImage = ManagerManager.Manager.mCharacterBundleManager.get_image("BOX_"+mLastCharacter.StringIdentifier).Image;
			}
		}*/
	}
	
	void OnGUI()
	{
		
		GUI.Box(new Rect(0,0,128,128),mImage,new GUIStyle());
	}
	
	/*
    public string loadme = "0-1";
    string[] mFiles;
    int mIndex = 0;
	void Start () {
        //hack so game manager wont do it's mojo
        ManagerManager.Manager.mUpdateDelegates -= ManagerManager.Manager.mGameManager.Update;
        //ManagerManager.Manager.mAssetLoader.load_character(loadme);
        mFiles = parse_text_files();
        mIndex = mFiles.Length - 1;
	}


    string[] parse_text_files()
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/Resources/POSE_TESTING");
        files = files.Where(s => Path.GetExtension(s) == ".txt").Select(s => Path.GetFileNameWithoutExtension(s)).ToArray();
        return files;
    }

    ProGrading.Pose get_pose_from_file(string aFile)
    {
        TextAsset t = Resources.Load("POSE_TESTING/" + aFile) as TextAsset;
        return ProGrading.read_pose(t);
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            mIndex = (mIndex + 1) % mFiles.Length;
            ManagerManager.Manager.mTransparentBodyManager.mFlat.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.35f);
            ManagerManager.Manager.mTransparentBodyManager.mFlat.SoftPosition = new Vector3(-800, 0, 0);
            ManagerManager.Manager.mTransparentBodyManager.set_target_pose(get_pose_from_file(mFiles[mIndex]));
        }
	}

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 72;
        GUI.Box(new Rect(10, 10, 400, 75), mFiles[mIndex], style);
    }
    */
}
