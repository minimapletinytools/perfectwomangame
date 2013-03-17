using UnityEngine;
using System.Collections.Generic;
using System;

public struct CharacterIndex
{
	public static int NUMBER_AGES = 10;
	public static string[] sLevelToAge = new string[10] { "0", "05", "16", "27", "34", "45", "60", "80", "100", "999" };
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
	
	
	public int Index { get; private set; } //-1 means no character
	public int Choice{
		get{
			if(Level == 0) return 0;
			else if (Level > 7) return 0;
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
			return System.Convert.ToInt32(sLevelToAge[Level]);
		}
	}
	public string StringIdentifier{
		get{
			return sLevelToAge[Level] + "-" + (Choice+1);
		}
	}
	public int NumberInRow{
		get{
			if(Index == 0 || Index == 29 || Index == 30)
				return 1;
			else return 3; //4
		}
	}
	public CharacterIndex(int aId)
	{
		Index = aId;
	}
	public CharacterIndex(int ageIndex, int choiceIndex)
	{
		if(ageIndex == 0 && choiceIndex == 0) Index = 0;
		else if(ageIndex == 8 && choiceIndex == 0) Index = 29;
		else if(ageIndex == 9 && choiceIndex == 0) Index = 30;
		else if(ageIndex == -1) Index = -1;
		else Index = 4*(ageIndex-1) + choiceIndex + 1;
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
			for(;i<sLevelToAge.Length;i++)
				if(sLevelToAge[i] == split[0])
					break;
			Index = 4*(i-1) + Convert.ToInt32(split[1]);
		}
	}
}
public class PerformanceStats
{
	public CharacterIndex Character { get; set; }
	public float Score { get; set; }
	public int Perfect { get; set; }
	public int Difficulty { get; set; }
	public PerformanceGraphObject PerformanceGraph { get; private set; }
	public PerformanceStats()
	{
		Character = new CharacterIndex(-1);
		Score = 0;
		Perfect = 0;
		Difficulty = 0;
		PerformanceGraph = new PerformanceGraphObject(11);
	}
}
