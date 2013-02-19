using UnityEngine;
using System.Collections;

public class SetupForRecordingBehaviour : MonoBehaviour {

    bool done = false;
	// Use this for initialization
	void Start () {
        ManagerManager.Manager.mRecordMode = true;
        ManagerManager.Manager.mEventManager.character_changed_event += character_changed_listener;
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
