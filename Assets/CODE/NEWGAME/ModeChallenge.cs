using UnityEngine;
using System.Collections;

public class ModeChallenge
{
	NewGameManager NGM {get; set;}
	ManagerManager mManager {get; set;}
	
	public ModeChallenge(NewGameManager aNgm)
	{
		NGM = aNgm;
		mManager = aNgm.mManager;
	}

	public void character_loaded()
    {
        //TODO start PLAY
	}
	
	public void update () 
	{
        //TODO if PLAY, then do grade
        //if CHOOSING 
	}

}
