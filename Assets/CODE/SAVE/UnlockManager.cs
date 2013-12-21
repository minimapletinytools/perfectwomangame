using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class UnlockRequirements
{
	public static Dictionary<CharacterIndex, System.Func<List<PerformanceStats>, string> > 
		requirements = new Dictionary<CharacterIndex,System.Func<List<PerformanceStats>, string>>()
	{
		{ new CharacterIndex(1,1), delegate(List<PerformanceStats> aStats)
			{
				return "Playing the game once made you realize you can be star even when really young!";
			}
		},{ new CharacterIndex(2,1), delegate(List<PerformanceStats> aStats)
			{
				return "";
			}
		},{	new CharacterIndex(1,2), delegate(List<PerformanceStats> aStats)
			{
				if(aStats.Count() > 7)
					return "Getting very old made you understand that life should not always be fun and games";
				return "";
			}
		}
	};
}

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
				unlockedCharacters[e] = 1;//2; //hidden
			else
				unlockedCharacters[e] = 0; //unknown
		}
	}
}

public class UnlockManager
{
	
	Unlockables mUnlocked;
	
	public UnlockManager()
	{
		mUnlocked = new Unlockables();

		//read_unlock();
	}
	
	public void game_finished(List<PerformanceStats> aStats)
	{
		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
			if(mUnlocked.unlockedCharacters[e] != 1)
				if(UnlockRequirements.requirements.ContainsKey(e))
				{
					string msg = UnlockRequirements.requirements[e](aStats);
					if(msg != "")
						;//TODO
				}
		}

		write_unlock();
	}

	void read_unlock()
	{
		try{
			IFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream("gg.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
			mUnlocked = (Unlockables) formatter.Deserialize(stream);
			stream.Close();
		} catch {} //no such file, must be first launch
	}

	public void write_unlock()
	{

		IFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream("gg.bin", FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize(stream, mUnlocked);
		stream.Close();
		
	}
	
	public int is_unlocked(CharacterIndex aIndex)
	{
		return mUnlocked.unlockedCharacters[aIndex];
	}
}
