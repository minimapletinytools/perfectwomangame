using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class UnlockRequirements
{
    public class FakeCharIndex 
    {
        public int Level{get;set;}
        public int Choice{get;set;}
        public FakeCharIndex(int aLevel, int aChoice)
        {
            Level = aLevel;
            Choice = aChoice;
        }
        public FakeCharIndex(CharacterIndex aIndex):this(aIndex.LevelIndex,aIndex.Choice)
        {}
    }

    public class FakeCharIndexComparer : IEqualityComparer<FakeCharIndex>
    {
        public bool Equals(FakeCharIndex b1, FakeCharIndex b2)
        {
            return b1.Level == b2.Level && b1.Choice == b2.Choice;
        }
        public int GetHashCode(FakeCharIndex bx)
        {
            return bx.GetHashCode();
        }
    }
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
    public static Dictionary<FakeCharIndex, System.Func<List<PerformanceStats>, List<List<PerformanceStats> >, UnlockData> > 
        requirements = new Dictionary<FakeCharIndex,System.Func<List<PerformanceStats>,List<List<PerformanceStats> >, UnlockData>>
            (new FakeCharIndexComparer())
	{
        { new FakeCharIndex(2,1), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
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
        },{ new FakeCharIndex(2,2), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(PerformanceStats.history_contains(aHistory,new CharacterIndex[]{CharacterIndex.sMother,CharacterIndex.sMarried}))
                {
                    return new UnlockData(){
                        Sentence = "Seeing both ups and downs of married life showed you the importance of family love and support",
                        Related = new CharacterIndex[]{CharacterIndex.sMother,CharacterIndex.sMarried}
                    };  
                }
                return null;
            }
        },{ new FakeCharIndex(2,3), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                try{
                    if(aStats.First(e=>e.Character == CharacterIndex.sProf).BadPerformance 
                       && aStats.First(e=>e.Character == CharacterIndex.sMinister).BadPerformance)
                    {
                        return new UnlockData(){
                            Sentence = "Failing as a Professor and Minister showed you that education should start early!",
                            Related = new CharacterIndex[]{CharacterIndex.sProf,CharacterIndex.sMinister}
                        };
                    }
                    return null;
                }
                catch{
                    return null;
                }
            }
        },{ new FakeCharIndex(3,1), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                try{
                    if(aStats.First(e=>e.Character == CharacterIndex.sProf).BadPerformance)
                    {
                        return new UnlockData(){
                            Sentence = "Failing as a Professor taught you that working hard while young sometimes just doesn't pay off!",
                            Related = new CharacterIndex[]{CharacterIndex.sProf}
                        };
                    }
                    return null;
                }
                catch{
                    return null;
                }
            }
        },{ new FakeCharIndex(3,2), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                try{
                    if(aStats.First(e=>e.Character == CharacterIndex.sMinister).BadPerformance)
                    {
                        return new UnlockData(){
                            Sentence = "Failing as a Minister showed you the hipocrisy of first world policital solutions",
                            Related = new CharacterIndex[]{CharacterIndex.sMinister}
                        };
                    }
                    return null;
                }
                catch{
                    return null;
                }
            }
        },{ new FakeCharIndex(3,3), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(aHistory.Count > 1 && aHistory.Max(e=>e.Sum(f=>f.Score)) < aStats.Sum(f=>f.Score))
                {
                    return new UnlockData(){
                        Sentence = "Getting a high score showed you the competitive spirit!"
                    };
                }
                return null;
            }
        },{ new FakeCharIndex(4,1), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                return new UnlockData(){
                    Sentence = "Playing the game once showed you that life doesn't need to be so serious"
                };
            }
        },{ new FakeCharIndex(4,2), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(aStats.Count < 5)
                {
                    return new UnlockData(){
                        Sentence = "Dying young showed you such tragedies can happen to YOU"
                    };
                }
                return null;
            }
        },{ new FakeCharIndex(4,3), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(PerformanceStats.history_contains(aStats,new CharacterIndex[]{CharacterIndex.sBeach}))
                {
                    return new UnlockData(){
                        Sentence = "Being so relaxed in your mid thirties reminded you that some people have financial responsibilities at this age :(",
                        Related = new CharacterIndex[]{CharacterIndex.sBeach}
                    };
                }
                return null;
            }
        },{ new FakeCharIndex(5,1), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(PerformanceStats.history_contains(aHistory,new CharacterIndex[]{CharacterIndex.sMother,CharacterIndex.sLeukemia,CharacterIndex.sSister}))
                {
                    return new UnlockData(){
                        Sentence = "Maybe the responsibilty and pain of close family ties just isn't worth it",
                        Related = new CharacterIndex[]{CharacterIndex.sMother,CharacterIndex.sLeukemia,CharacterIndex.sSister}
                    };
                }
                return null;
            }
        },{ new FakeCharIndex(5,2), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(aStats.Where(e=>e.BadPerformance).Count() == aStats.Count)
                {
                    return new UnlockData(){
                        Sentence = "Life is hard"
                    };
                }
                return null;
            }
        },{ new FakeCharIndex(5,3), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(aStats.Last().Character == CharacterIndex.sOneHundred)
                {
                    return new UnlockData(){
                        Sentence = "The beauty of space made you want to experience the beauty of the sea",
                        Related = new CharacterIndex[]{CharacterIndex.sOneHundred}
                    };
                }
                return null;
            }
        },{ new FakeCharIndex(6,1), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(PerformanceStats.history_contains(aStats,new CharacterIndex[]{CharacterIndex.sSlave,CharacterIndex.sSister}))
                {
                    return new UnlockData(){
                        Sentence = "Having been through slavery and war, you want to help the world",
                        Related = new CharacterIndex[]{CharacterIndex.sSlave,CharacterIndex.sSister}
                    };
                }
                return null;
            }
        },{ new FakeCharIndex(6,2), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(PerformanceStats.history_contains(aStats,new CharacterIndex[]{CharacterIndex.sPunk,CharacterIndex.sMother}))
                {
                    return new UnlockData(){
                        Sentence = "Sometimes kids just don't behave!",
                        Related = new CharacterIndex[]{CharacterIndex.sPunk,CharacterIndex.sMother}
                    };
                }
                return null;
            }
        },{ new FakeCharIndex(6,3), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(PerformanceStats.history_contains(aStats,new CharacterIndex[]{CharacterIndex.sSexy,CharacterIndex.sPunk,CharacterIndex.sFundraiser}))
                {
                    return new UnlockData(){
                        Sentence = "Using your physical charisma so much throughout your life showed you the power of sex appeal",
                        Related = new CharacterIndex[]{CharacterIndex.sSexy,CharacterIndex.sPunk,CharacterIndex.sFundraiser}
                    };
                }
                return null;
            }
        },{ new FakeCharIndex(7,1), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(aHistory.Count > 10)
                {
                    return new UnlockData(){
                        Sentence = "Having experienced 10 different lives makes you realize the possibilities in life can be a little overwhelming."
                    };
                }
                return null;
            }
        },{ new FakeCharIndex(7,2), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(PerformanceStats.history_contains(aStats,new CharacterIndex[]{CharacterIndex.sDemented}))
                {
                    return new UnlockData(){
                        Sentence = "Having being a demented old lady showed you the importance of faith in old age",
                        Related = new CharacterIndex[]{CharacterIndex.sDemented}
                    };
                }
                return null;
            }
        },{ new FakeCharIndex(7,3), delegate(List<PerformanceStats> aStats, List<List<PerformanceStats> > aHistory)
            {
                if(aStats.Last().Character == CharacterIndex.sPray && aStats.Last().DeathTime != -1)
                {
                    return new UnlockData(){
                        Sentence = "Death while praying expanded your spiritual possibilities",
                        Related = new CharacterIndex[]{CharacterIndex.sPray}
                    };
                }
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
        return UnlockRequirements.requirements [new UnlockRequirements.FakeCharIndex(aChar.LevelIndex,aChar.Choice)](aStats, mUnlocked.gameHistory);
    }
	public void game_finished(List<PerformanceStats> aStats)
	{
        mUnlocked.gameHistory.Add(aStats);
        //TODO maybe consider pruning after it reaches like over 100 playthroughs


		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
			if(mUnlocked.unlockedCharacters[e] != 1)
				if(UnlockRequirements.requirements.ContainsKey(new UnlockRequirements.FakeCharIndex(e)))
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
