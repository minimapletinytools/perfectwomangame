using UnityEngine;
using System.Collections.Generic;

public class CharacterHelper 
{
	public CharacterStats[] Characters
	{ get; private set; }
	
	public CharacterHelper()
	{
		 int[] mPerfectness = new int[]{ 0, 
            3, 1, 0, 2, 
            0, 1, 3, 2, 
            3, 0, 2, 1, 
            1, 3, 0, 2,
            1, 0, 3, 2, 
            0, 3, 1, 2, 
            1, 0, 2, 3, 0, 0 };
		
		Characters = new CharacterStats[CharacterIndex.NUMBER_CHARACTERS];
		for(int i = 0; i < Characters.Length; i++)
			Characters[i] = null;
		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
			Characters[e.Index] = new CharacterStats(){Character = e};
			Characters[e.Index].Difficulty = e.Index < 5 ? 0 : 1; //05 characters are easier
			Characters[e.Index].Perfect = mPerfectness[e.Index]; //TODO maybe just make this random lol?
		}
	}
}
