using UnityEngine;
using System.Collections;

public class ModeTesting
{
	
	NewGameManager NGM {get; set;}
	ManagerManager mManager {get; set;}
	
	public ModeTesting(NewGameManager aNgm)
	{
		NGM = aNgm;
		mManager = aNgm.mManager;
	}
}
