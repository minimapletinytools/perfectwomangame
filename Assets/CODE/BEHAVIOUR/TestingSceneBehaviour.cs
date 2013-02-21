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
        ManagerManager.Manager.mAssetLoader.load_character(loadme);
        mFiles = parse_text_files();
        Debug.Log(mFiles);
	}


    string[] parse_text_files()
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/Resources/POSE_TESTING");
        //Debug.Log(Application.dataPath + "/Resources/POSE_TESTING");
        //Debug.Log(files.Length);
        foreach (string e in files)
            Debug.Log(Path.GetExtension(e));
        files = files.Where(s => Path.GetExtension(s) == ".txt").Select(s => Path.GetFileNameWithoutExtension(s)).ToArray();
        return files;
    }

    ProGrading.Pose get_pose_from_file(string aFile)
    {
        TextAsset t = Resources.Load("POSE_TESTING/" + aFile) as TextAsset;
        Debug.Log(t.text);
        return ProGrading.read_pose(t);
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ManagerManager.Manager.mTransparentBodyManager.mFlat.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.35f);
            ManagerManager.Manager.mTransparentBodyManager.mFlat.SoftPosition = new Vector3(700, 0, 0);
            ManagerManager.Manager.mTransparentBodyManager.set_target_pose(get_pose_from_file(mFiles[mIndex]));
            mIndex = (mIndex + 1) % mFiles.Length;
            
        }
	}
}
