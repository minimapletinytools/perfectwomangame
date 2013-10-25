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
	
}
