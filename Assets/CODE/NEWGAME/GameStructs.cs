using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public struct CharacterIndex
{
	public const int NUMBER_AGES = 10;
	public const int NUMBER_CHARACTERS = 31;
	public static string[] LEVEL_TO_AGE = new string[10] { "0", "05", "16", "27", "34", "45", "60", "85", "100", "999" };
	public static string [] INDEX_TO_SHORT_NAME = new string[31]
	{
		"Fetus",
		"Princess","Star","Rebel","",
		"Sexy","Punk","Sister","",
		"Mother","Bartender","Terrorist","",
		"Professor","Gay","Mourning","",
		"Married","Dancer","Single","",
		"Ambassador","Speaker","Pissed","",
		"Grandmother","Senile","Religious","",
		"Old",
		"Dead"
	};
	public static string [] INDEX_TO_FULL_NAME = new string[31]
	{
		"Fetus",
		"Princess","Star","Rebel","",
		"Sexy Girl","Punk Girl","Responsible Sister","",
		"Mother","Oktoberfest Bartender","Terrorist","",
		"MIT Professor","Foster Mother","Mourning Mother","",
		"Wealthy Wife","Bellydance instructor","Single Woman","",
		"Ambassador","Charity Speaker","Mother Angry at Daughter","",
		"Grandmother","Senile Senior","Religious Woman","",
		"Old",
		"Dead"
	};
	public static string [] INDEX_TO_DESCRIPTION = new string[31]
	{
		"Inside your mother's womb",
		"Princess",
		"",
		"",
		"","","","","","","","","","","","","","","","","","","","","","","","","","",""
	};
	
	public static List<CharacterIndex> sAllCharacters = new List<CharacterIndex>()
	{
		new CharacterIndex(0),
		new CharacterIndex(1),new CharacterIndex(2),new CharacterIndex(3),
		new CharacterIndex(5),new CharacterIndex(6),new CharacterIndex(7),
		new CharacterIndex(9),new CharacterIndex(10),new CharacterIndex(11),
		new CharacterIndex(13),new CharacterIndex(14),new CharacterIndex(15),
		new CharacterIndex(17),new CharacterIndex(18),new CharacterIndex(19),
		new CharacterIndex(21),new CharacterIndex(22),new CharacterIndex(23),
		new CharacterIndex(25),new CharacterIndex(26),new CharacterIndex(27),
		new CharacterIndex(29),
		new CharacterIndex(30)
	};
	public static string[] sIndexToName = new string[]
	{
		"In your mother's womb"
	};
	
	
	public static CharacterIndex RandomCharacter{
		get{
			return new CharacterIndex(UnityEngine.Random.Range(1,30));
		}
	}
	public int Index { get; private set; } //-1 means no character
	public int Choice{
		get{
			if(IsSolo) return 0;
			else return (Index - 1) % 4;
		}
	}
	public int Level{ //AKA AgeIndex
		get{
			if(Index == 0) return 0;
			else if(Index == 29) return 8;
			else if(Index == 30) return 9;
			else if(Index < 29) return (Index-1)/4+1;
			else return -1;
		}
	}
	public int Age{
		get{
			return System.Convert.ToInt32(LEVEL_TO_AGE[Level]);
		}
	}
	public string StringIdentifier{
		get{
			if(Index == 29) return "100";
			if(Index == 30) return "999";
			return LEVEL_TO_AGE[Level] + "-" + (Choice+1);
		}
	}
	public bool IsSolo{
		get{
			return (Level == 0 || Level > 7);
		}
	}
	public List<CharacterIndex> Neighbors{
		get{
			List<CharacterIndex> r = new List<CharacterIndex>();
			if(!IsSolo)
				for(int i = 0; i < 4; i++)
					if(i != Choice)
						r.Add(new CharacterIndex(Level,i));
			return r;
		}
	}
	public List<CharacterIndex> NeighborsAndSelf{
		get{ 
			List<CharacterIndex> r = new List<CharacterIndex>();
			if(!IsSolo)
				for(int i = 0; i < 4; i++)
					r.Add(new CharacterIndex(Level,i));
			else r.Add(this);
			return r;
		}	
	}
	public int NumberInRow{
		get{
			if(Index == 0 || Index == 29 || Index == 30)
				return 1;
			else return 3; //4
		}
	}
	
	public string Description{get{return INDEX_TO_DESCRIPTION[Index];}}
	public string ShortName{get{return INDEX_TO_SHORT_NAME[Index];}}
	public string FullName{get{return INDEX_TO_FULL_NAME[Index];}}
	
	public CharacterIndex get_future_neighbor(int choiceIndex)
	{
		if(Level == 9) return new CharacterIndex(-1);
		return new CharacterIndex(Level +1,choiceIndex);
	}
	public CharacterIndex get_neighbor(int choiceIndex)
	{
		return new CharacterIndex(Level, choiceIndex);
	}
	public CharacterIndex(int aId)
	{
		Index = aId;
	}
	public CharacterIndex(int levelIndex, int choiceIndex)
	{
		if(levelIndex == 0 && choiceIndex == 0) Index = 0;
		else if(levelIndex == 8 && choiceIndex == 0) Index = 29;
		else if(levelIndex == 9 && choiceIndex == 0) Index = 30;
		else if(levelIndex == -1) Index = -1;
		else Index = 4*(levelIndex-1) + choiceIndex + 1;
	}
	public CharacterIndex(string aBundleName)
	{
		if(aBundleName == "0-1")
			Index = 0;
		else if(aBundleName == "100")
			Index = 29;
		else if(aBundleName == "999")
			Index = 30;
		else
		{
			string[] split = aBundleName.Split('-');
			int i = 0;
			for(;i<LEVEL_TO_AGE.Length;i++)
				if(LEVEL_TO_AGE[i] == split[0])
					break;
			Index = 4*(i-1) + Convert.ToInt32(split[1]);
		}
	}
}

public class CharacterStats
{
	public CharacterIndex Character { get; set; }
	public int Perfect { get; set; }
	public int Difficulty { get; set; }
	
	public CharacterStats()
	{
		Character = new CharacterIndex(-1);
		Perfect = 0;
		Difficulty = 0;
	}
}
public class PerformanceStats
{
	public CharacterIndex Character {
		get{
			return Stats.Character;
		}
		set{
			Stats.Character = value;
		}
	}
	
	public float Score{
		get{
			float r = 0;
			for(int i = 1; i < mScore.Count; i++)
			{
				r += mScore[i].Value * (mScore[i].Key-mScore[i-1].Key);
			}
			return r;
		}
	}
	
	public bool Finished { get; set; } //did we finish with this character already
	public float DeathTime { get; set; } //what time (0,1) did this character die
	
	public PerformanceGraphObject PerformanceGraph { get; private set; }
	
	public CharacterStats Stats
	{ get; private set; }
	
	public PerformanceStats(CharacterIndex aChar)
	{
		
		Stats = new CharacterStats();
		Stats.Character = aChar;
	
		Finished = false;
		DeathTime = -1;
		
		PerformanceGraph = new PerformanceGraphObject(11);
		
		mScore = new List<KeyValuePair<float, float>>();
		update_score(0,0); //this is a dummy point
	}
	
	List<KeyValuePair<float,float>> mScore; //time, score
	
	public void update_score(float aTime, float aScore)
	{
		mScore.Add(new KeyValuePair<float,float>(aTime,aScore));
		PerformanceGraph.update_graph(aTime,aScore);
	}
	
	//this returns the score in the last <timeBack> 
	//use this for death
	public float last_score(float timeBack)
	{
		float currentTime = mScore.Last().Key;
		float r = 0;
		for(int i = mScore.Count; i > 0; i--)
		{
			if(Mathf.Abs(mScore[i].Key - currentTime) > timeBack)
				break;
			r += mScore[i].Value * (mScore[i].Key-mScore[i-1].Key);
			
		}
		return r;
	}
}

public class CutsceneData
{
	//TODO
}
