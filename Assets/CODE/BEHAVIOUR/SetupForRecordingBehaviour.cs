using UnityEngine;
using System.Collections;

public class SetupForRecordingBehaviour : MonoBehaviour {

    public string prefix = "char";
    public bool record_manual = true;
    public bool record_kinect = true;

    bool done = false;

    //TODO use my new hack for loading scenes without all the other nonsense
	void Start () {
        ManagerManager.Manager.mRecordMode = true;
        ManagerManager.Manager.mEventManager.character_changed_event += character_changed_listener;

        ManagerManager.sTakeManual = record_manual;
        ManagerManager.sTakeKinect = record_kinect;
        ManagerManager.sScreenShotPrefix = prefix;
	}


    public void character_changed_listener(CharacterLoader aCharacter)
    {
        if (!done)
        {
            ManagerManager.Manager.mGameManager.hack_choice(0, 9999999);
            done = true;
        }
    }
	
}
