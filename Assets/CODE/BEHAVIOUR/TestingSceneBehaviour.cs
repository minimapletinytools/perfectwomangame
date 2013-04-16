using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
public class TestingSceneBehaviour : MonoBehaviour {

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
}
