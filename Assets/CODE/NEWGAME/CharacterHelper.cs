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
		{
			Characters[i] = new CharacterStats(){Character = new CharacterIndex(i)};
			Characters[i].Difficulty = i < 5 ? 0 : 1; //05 characters are easier
			Characters[i].Perfect = mPerfectness[i]; //TODO maybe just make this random lol?
		}
	}
}
