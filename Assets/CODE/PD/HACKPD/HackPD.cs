using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class HackPDChangeSet
{
	public class ChangeGroup
	{
		public PDStats.Stats stat;
		public CharacterIndex character;
		public float changeAmount;
		public ChangeGroup(PDStats.Stats aStat, CharacterIndex aIndex, float aChangeAmount)
		{
			stat = aStat;
			character = aIndex;
			changeAmount = aChangeAmount;
		}
	}
	public List<ChangeGroup> mChanges;
	List<ChangeGroup> get_changes(PDStats.Stats aStats)
	{
		return mChanges.Where(e=>e.stat == aStats).ToList();
	}
}

public class CharacterDifficutyChange
{
	public CharacterIndex character;
	public float changeAmount;
	public CharacterDifficutyChange(CharacterIndex aIndex, float aChangeAmount)
	{
		character = aIndex;
		changeAmount = aChangeAmount;
	}
}

public class HackPD 
{
	
	//public static 
	//PDCharacters.characters
	
	//need to take two characters are randomly return a trait that they share in comon or not in common so we can generate this as the decisive sentence
	//better yet, takes one character and a list of characters and randomly decides hom to construct the sentences
	public static HackPDChangeSet choose_traits(CharacterIndex A, List<CharacterDifficutyChange> B)
	{
		HackPDChangeSet r = new HackPDChangeSet();
		Dictionary<PDStats.Stats,int> changedStats = new Dictionary<PDStats.Stats, int>();
		foreach(PDStats.Stats e in PDStats.EnumerableStats)
			changedStats[e] = 0;
		PDCharacterStats AStat = PDCharacters.characters[A.Index];
		
		//determine what stats changed
		foreach(CharacterDifficutyChange e in B)
		{
			PDCharacterStats BStat = PDCharacters.characters[e.character.Index];
			foreach(PDStats.Stats f in PDStats.EnumerableStats)
			{
				float ABProduct = AStat.DifficultyValues[f]*BStat.DifficultyValues[f];
				if(ABProduct != 0)
					changedStats[f]++;
			}
		}
		
		changedStats.OrderBy(e=>e.Value);
		
		
		//randomly pick 3 out of these
		return r;
	}
	
	//needs to take character, performance, and list of N characters to decide how to adjust their difficulties (so the distribution does not get too skewed)
	//this fuction will diretly modify aCharacters and return a list of characters that had their stats (true for increase in difficulty)
	public static List<KeyValuePair<CharacterIndex,bool>> get_difficulty_adjust(PerformanceStats aCharacter, List<CharacterStats> aCharacters)
	{
		
		return null;
	}
	
	
}
