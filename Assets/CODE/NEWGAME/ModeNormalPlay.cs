using UnityEngine;
using System.Collections;

public class ModeNormalPlay
{
	
	NewGameManager NGM {get; set;}
	ManagerManager mManager {get; set;}
	
	public ModeNormalPlay(NewGameManager aNgm)
	{
		NGM = aNgm;
		mManager = aNgm.mManager;
	}
}
