using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class CharIndexContainerInt
{
	public int[][] Contents 
	{get; set;}
	public CharIndexContainerInt()
	{
		int ageCount = 10;
		Contents = new int[ageCount][];
		for(int i = 0; i < ageCount; i++)
		{
			int charCount = ((i==0 || i == 8 || i == 9) ? 1 : 5);
			for(int j = 0; j < charCount; j++)
			{
				Contents[i] = new int[charCount];
			}
		}
	}
	public int this[CharacterIndex aI]
	{
		get{
			return this[aI.LevelIndex, aI.Choice];
		}
		set{
			this[aI.LevelIndex, aI.Choice] = value;
		}
	}
	public int this[int x, int y]
	{
		get{
			return Contents[x][y];
		}
		set{
			Contents[x][y] = value;
		}
	}
	
	public int[] to_array()
	{
		List<int> r = new List<int>();
		foreach(int[] e in Contents)
			foreach(int f in e)
				r.Add(f);
		return r.ToArray();
	}
}

public class CharIndexContainerCharacterLoader
{
	public CharacterLoader[][] Contents 
	{get; set;}
	public CharIndexContainerCharacterLoader()
	{
		int ageCount = 10;
		Contents = new CharacterLoader[ageCount][];
		for(int i = 0; i < ageCount; i++)
		{
			int charCount = ((i==0 || i == 8 || i == 9) ? 1 : 5);
			for(int j = 0; j < charCount; j++)
			{
				Contents[i] = new CharacterLoader[charCount];
			}
		}
	}
	public CharacterLoader this[CharacterIndex aI]
	{
		get{
			return this[aI.LevelIndex, aI.Choice];
		}
		set{
			this[aI.LevelIndex, aI.Choice] = value;
		}
	}
	public CharacterLoader this[int x, int y]
	{
		get{
			return Contents[x][y];
		}
		set{
			Contents[x][y] = value;
		}
	}
}
	


public class CharIndexContainerString
{
	public String[][] Contents 
	{get; set;}
	public CharIndexContainerString()
	{
		int ageCount = 10;
		Contents = new String[ageCount][];
		for(int i = 0; i < ageCount; i++)
		{
			int charCount = ((i==0 || i == 8 || i == 9) ? 1 : 5);
			for(int j = 0; j < charCount; j++)
			{
				Contents[i] = new String[charCount];
			}
		}
	}
	public String this[CharacterIndex aI]
	{
		get{
			return this[aI.LevelIndex, aI.Choice];
		}
		set{
			this[aI.LevelIndex, aI.Choice] = value;
		}
	}
	public String this[int x, int y]
	{
		get{
			return Contents[x][y];
		}
		set{
			Contents[x][y] = value;
		}
	}
	public List<string> to_list()
	{
		List<String> r = new List<String>();
		foreach(String[] e in Contents)
			foreach(String f in e)
				if(f != null && f != "")
					r.Add(f);
		return r;
	}
}
	



public class CharIndexContainerCharacterStats
{
	public CharacterStats[][] Contents 
	{get; set;}
	public CharIndexContainerCharacterStats()
	{
		int ageCount = 10;
		Contents = new CharacterStats[ageCount][];
		for(int i = 0; i < ageCount; i++)
		{
			int charCount = ((i==0 || i == 8 || i == 9) ? 1 : 5);
			for(int j = 0; j < charCount; j++)
			{
				Contents[i] = new CharacterStats[charCount];
			}
		}
	}
	public CharacterStats this[CharacterIndex aI]
	{
		get{
			return this[aI.LevelIndex, aI.Choice];
		}
		set{
			this[aI.LevelIndex, aI.Choice] = value;
		}
	}
	public CharacterStats this[int x, int y]
	{
		get{
			return Contents[x][y];
		}
		set{
			Contents[x][y] = value;
		}
	}
	
	public CharacterStats[] to_array()
	{
		List<CharacterStats> r = new List<CharacterStats>();
		foreach(CharacterStats[] e in Contents)
			foreach(CharacterStats f in e)
				r.Add(f);
		return r.ToArray();
	}
}
	



public class CharIndexContainerCharacterIconObject
{
	public CharacterIconObject[][] Contents 
	{get; set;}
	public CharIndexContainerCharacterIconObject()
	{
		int ageCount = 10;
		Contents = new CharacterIconObject[ageCount][];
		for(int i = 0; i < ageCount; i++)
		{
			int charCount = ((i==0 || i == 8 || i == 9) ? 1 : 5);
			for(int j = 0; j < charCount; j++)
			{
				Contents[i] = new CharacterIconObject[charCount];
			}
		}
	}
	public CharacterIconObject this[CharacterIndex aI]
	{
		get{
			return this[aI.LevelIndex, aI.Choice];
		}
		set{
			this[aI.LevelIndex, aI.Choice] = value;
		}
	}
	public CharacterIconObject this[int x, int y]
	{
		get{
			return Contents[x][y];
		}
		set{
			Contents[x][y] = value;
		}
	}
}
	