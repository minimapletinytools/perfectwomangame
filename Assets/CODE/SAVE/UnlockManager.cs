using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class UnlockRequirements
{
	public class UnlockData
	{
		public string Sentence{get;set;}
		public CharacterIndex[] Related{get;set;}
		public UnlockData()
		{
			Related = new CharacterIndex[0];
			Sentence = "";
		}
		public static implicit operator UnlockData(string aString)
		{
			return new UnlockData(){Sentence = aString};
		}
	}
    public static Dictionary<CharacterIndex, System.Func<List<PerformanceStats>, List<List<PerformanceStats> >, UnlockData> > requirements = new Dictionary<CharacterIndex,System.Func<List<PerformanceStats>,List<List<PerformanceStats> >, UnlockData>>()
	{
        { new CharacterIndex(2,1), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
			{
                if(PerformanceStats.history_contains(aHistory,new CharacterIndex[]{CharacterIndex.sStar,CharacterIndex.sGang}))
                {
    				return new UnlockData(){
    					Sentence = "Having been both a Gang Leader and a Child Star when young made you think of combining bad attitude and music!",
                        Related = new CharacterIndex[]{CharacterIndex.sStar,CharacterIndex.sGang}
    				};	
                }
                return null;
			}
        },{ new CharacterIndex(2,2), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                return null;
            }
        },{ new CharacterIndex(2,3), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(aStats.Count() > 7)
                    return "Getting very old made you understand that life should not always be fun and games";
                return null;
            }
        }
	};
}

[System.Serializable]
public class Unlockables
{
	public List<List<PerformanceStats> > gameHistory = new List<List<PerformanceStats>>();
	public CharIndexContainerInt unlockedCharacters = new CharIndexContainerInt();
	public bool skipAvail = false;
	
	public Unlockables()
	{
		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
            //1 unlocked
            //2 hidden
            //0 unknown (secret!)

			if(e.Choice == 0)
				unlockedCharacters[e] = 1; 
            else if(e.LevelIndex == 1)
                unlockedCharacters[e] = 1;
			else if(e.Choice < 4)
				unlockedCharacters[e] = 2; 
			else
				unlockedCharacters[e] = 0; 
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
	
    public UnlockRequirements.UnlockData did_unlock(CharacterIndex aChar,List<PerformanceStats> aStats)
    {
        return UnlockRequirements.requirements [aChar](aStats, mUnlocked.gameHistory);
    }
	public void game_finished(List<PerformanceStats> aStats)
	{
        mUnlocked.gameHistory.Add(aStats);
        //TODO maybe consider pruning after it reaches like over 100 playthroughs


		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
			if(mUnlocked.unlockedCharacters[e] != 1)
				if(UnlockRequirements.requirements.ContainsKey(e))
				{
                    var msg = did_unlock(e,aStats);
					if(msg != null)
                        mUnlocked.unlockedCharacters[e] = 1;
				}
		}
		write_unlock();
	}

    
    public List<CharacterIndex> get_unlocked_characters_at_level(int aLevelIndex)
    {
        List<CharacterIndex> r = new List<CharacterIndex> ();
        foreach (var e in CharacterIndex.sAllCharacters.Where(e=>e.LevelIndex == aLevelIndex))
            if (mUnlocked.unlockedCharacters [e] == 1)
                r.Add (e);
        return r;
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
