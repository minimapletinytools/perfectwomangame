using UnityEngine;
using System.Collections;

[System.Serializable]
public class Unlockables
{
	public CharIndexContainerInt unlockedCharacters = new CharIndexContainerInt();
	public bool skipAvail = false;
	
	public Unlockables()
	{
		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
			if(e.Choice == 0)
				unlockedCharacters[e] = 1; //unlocked
			else if(e.Choice < 4)
				unlockedCharacters[e] = 2; //hidden
			else
				unlockedCharacters[e] = 0; //unknown
		}
	}
}

public class UnlockManager
{
	
}
