using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public struct CharacterIndex : IEquatable<CharacterIndex>
{
	public const int NUMBER_AGES = 10;
	public const int NUMBER_CHARACTERS = 31;
	public static string[] LEVEL_TO_AGE = new string[10] { "0", "05", "16", "27", "34", "45", "60", "85", "110", "999" };
	
	//TODO DELET
	public static CharacterIndex[] INDEX_TO_CHARACTER = new CharacterIndex[]
	{
		new CharacterIndex(0,0),
		new CharacterIndex(1,0),new CharacterIndex(1,1),new CharacterIndex(1,2),new CharacterIndex(1,3),
		new CharacterIndex(2,0),new CharacterIndex(2,1),new CharacterIndex(2,2),new CharacterIndex(2,3),
		new CharacterIndex(3,0),new CharacterIndex(3,1),new CharacterIndex(3,2),new CharacterIndex(3,3),
		new CharacterIndex(4,0),new CharacterIndex(4,1),new CharacterIndex(4,2),new CharacterIndex(4,3),
		new CharacterIndex(5,0),new CharacterIndex(5,1),new CharacterIndex(5,2),new CharacterIndex(5,3),
		new CharacterIndex(6,0),new CharacterIndex(6,1),new CharacterIndex(6,2),new CharacterIndex(6,3),
		new CharacterIndex(7,0),new CharacterIndex(7,1),new CharacterIndex(7,2),new CharacterIndex(7,3),
		new CharacterIndex(8,0),
		new CharacterIndex(9,0)
	};
	
	
	public static List<CharacterIndex> sAllCharacters = new List<CharacterIndex>()
	{
		new CharacterIndex(0,0),
		new CharacterIndex(1,0),new CharacterIndex(1,1),new CharacterIndex(1,2),new CharacterIndex(1,3),
		new CharacterIndex(2,0),new CharacterIndex(2,1),new CharacterIndex(2,2),new CharacterIndex(2,3),
		new CharacterIndex(3,0),new CharacterIndex(3,1),new CharacterIndex(3,2),new CharacterIndex(3,3),
		new CharacterIndex(4,0),new CharacterIndex(4,1),new CharacterIndex(4,2),new CharacterIndex(4,3),
		new CharacterIndex(5,0),new CharacterIndex(5,1),new CharacterIndex(5,2),new CharacterIndex(5,3),
		new CharacterIndex(6,0),new CharacterIndex(6,1),new CharacterIndex(6,2),new CharacterIndex(6,3),
		new CharacterIndex(7,0),new CharacterIndex(7,1),new CharacterIndex(7,2),new CharacterIndex(7,3),
		new CharacterIndex(8,0),
		new CharacterIndex(9,0)
	};
	public static string[] sIndexToName = new string[]
	{
		"In your mother's womb"
	};
	
	public static CharacterIndex sGrave = new CharacterIndex(9,0);
	public static CharacterIndex sFetus = new CharacterIndex(0,0);
	public static CharacterIndex sOneHundred = new CharacterIndex(8,0);
	
	public static CharacterIndex RandomCharacter{
		get{
			return new CharacterIndex(UnityEngine.Random.Range(1,7),UnityEngine.Random.Range(0,3));
		}
	}
	public int LevelIndex {get; private set;}
	public int Choice{get; private set;}
	public int Age{
		get{
			if(LEVEL_TO_AGE[LevelIndex] == "05") //hack, you should really just rename the files...
				return 9;
			return System.Convert.ToInt32(LEVEL_TO_AGE[LevelIndex]);
		}
	}
	public string StringIdentifier{
		get{
			if(LevelIndex == 8) return "110";
			if(LevelIndex == 9) return "999";
			return LEVEL_TO_AGE[LevelIndex] + "-" + (Choice+1);
		}
	}
	public bool IsSolo{
		get{
			return (LevelIndex == 0 || LevelIndex > 7);
		}
	}
	public List<CharacterIndex> Neighbors{
		get{
			List<CharacterIndex> r = new List<CharacterIndex>();
			if(!IsSolo)
				for(int i = 0; i < 4; i++)
					if(i != Choice)
						r.Add(new CharacterIndex(LevelIndex,i));
			return r;
		}
	}
	public List<CharacterIndex> NeighborsAndSelf{
		get{ 
			List<CharacterIndex> r = new List<CharacterIndex>();
			if(!IsSolo)
				for(int i = 0; i < 4; i++)
					r.Add(new CharacterIndex(LevelIndex,i));
			else r.Add(this);
			return r;
		}	
	}
	public int NumberInRow{
		get{
			if(LevelIndex == 0 || LevelIndex == 8 || LevelIndex == 9)
				return 1;
			else return 3; //4
		}
	}
	
	public string Description
	{
		get{
			if(LevelIndex == -1) return "none";
			string r = GameConstants.INDEX_TO_DESCRIPTION[this];
			r = r.Replace("<A> ","");
			r = r.Replace("<A>","");
			return r;
		}
	}
	public bool IsDescriptionAdjective
	{
		get{
			if(LevelIndex == -1) return false;
			return GameConstants.INDEX_TO_DESCRIPTION[this].Contains("<A>");
		}
	}
	public string ShortName{get{return LevelIndex == -1 ? "none" : GameConstants.INDEX_TO_SHORT_NAME[this];}}
	public string FullName{get{return LevelIndex == -1 ? "none" : GameConstants.INDEX_TO_FULL_NAME[this];}}
	
	public CharacterIndex get_future_neighbor(int choiceIndex)
	{
		if(LevelIndex == 9) return new CharacterIndex(-1,0);
		return new CharacterIndex(LevelIndex +1,choiceIndex);
	}
	public CharacterIndex get_past_neighbor(int choiceIndex)
	{
		if(LevelIndex == 1) return new CharacterIndex(0,0);
		if(LevelIndex == 0) return new CharacterIndex(-1,0);
		return new CharacterIndex(LevelIndex-1,choiceIndex);
	}
	public CharacterIndex get_neighbor(int choiceIndex)
	{
		return new CharacterIndex(LevelIndex, choiceIndex);
	}

	public CharacterIndex[] get_neighbors()
	{
		int li = LevelIndex;
		return sAllCharacters.Where(e=>e.LevelIndex == li).ToArray();
	}
	
	//ctors
	private void set_character(int aLevelIndex, int aChoiceIndex)
	{
		LevelIndex = aLevelIndex; 
		Choice = aChoiceIndex;
	}
	public CharacterIndex(int aLevelIndex, int choiceIndex)
	{
		LevelIndex = -1;
		Choice = 0;
		set_character(aLevelIndex,choiceIndex);
	}
	
	public CharacterIndex(string aBundleName)
	{
		LevelIndex = -1;
		Choice = 0;
		if(aBundleName == "0-1")
			set_character(0,0);
		else if(aBundleName == "100")
			set_character(8,0);
		else if(aBundleName == "999")
			set_character(9,0);
		else
		{
			string[] split = aBundleName.Split('-');
			int i = 0;
			for(;i<LEVEL_TO_AGE.Length;i++)
				if(LEVEL_TO_AGE[i] == split[0])
					break;
			set_character(i,Convert.ToInt32(split[1])-1);
		}
	}
	
	public bool Equals(CharacterIndex other) 
	{
		if (this.LevelIndex == other.LevelIndex && this.Choice == other.Choice)
			return true;
		else 
			return false;
	}
	
	public override bool Equals(object obj)
	{
	 	return false;
  	}   
	
	public static bool operator ==(CharacterIndex a, CharacterIndex b)
	{
		return a.Equals(b);
	}
	public static bool operator !=(CharacterIndex a, CharacterIndex b)
	{
		return !a.Equals(b);
	}
	public override int GetHashCode()
	{
		return LevelIndex * 100 + Choice;
	}
	
	
}


