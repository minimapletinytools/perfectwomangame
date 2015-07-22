using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.Text;


[System.Serializable]
public class Unlockables
{
    //one option is to just replace it with an int of number characters played 
    //TODO is this what causes the JIT problem and how do I make it not JIT
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
			//else if(e.Choice < 4)
            else if(e.Choice == 3)
				unlockedCharacters[e] = 1; //2 - locked, 1 - avail
			else
				unlockedCharacters[e] = 1; 
		}
	}
}

public class UnlockManager
{
	Unlockables mUnlocked;
    ManagerManager mManager;
	
    public UnlockManager(ManagerManager aManager)
	{
		mUnlocked = new Unlockables();
        mManager = aManager;
		//read_unlock();
	}
	
    public UnlockRequirements.UnlockData did_unlock(CharacterIndex aChar,List<PerformanceStats> aStats)
    {
        //return new UnlockRequirements.UnlockData();
        return UnlockRequirements.requirements [new UnlockRequirements.FakeCharIndex(aChar.LevelIndex,aChar.Choice)](aStats, mUnlocked.gameHistory);
    }

    public void unlock(CharacterIndex aIndex)
    {
        mUnlocked.unlockedCharacters [aIndex] = 1;
    }

	public void game_finished(List<PerformanceStats> aStats)
	{
        //TODO renable
        mUnlocked.gameHistory.Add(aStats);

        //TODO maybe consider pruning after it reaches like over 1000 playthroughs


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

        //write_unlock();
        mManager.mZigManager.ZgInterface.write_data(serialize(),"unlock");
	}

    public List<CharacterIndex> get_unlocked_characters()
    {
        return CharacterIndex.sAllCharacters.Where(e => mUnlocked.unlockedCharacters [e] == 1).ToList();
    }

    public List<CharacterIndex> get_unlocked_characters_at_level(int aLevelIndex)
    {
        List<CharacterIndex> r = new List<CharacterIndex> ();
        foreach (var e in CharacterIndex.sAllCharacters.Where(e=>e.LevelIndex == aLevelIndex))
            if (mUnlocked.unlockedCharacters [e] == 1)
                r.Add (e);
        return r;
    }

    public byte[] serialize()
    {
        return (new UnicodeEncoding()).GetBytes(JsonConvert.SerializeObject(mUnlocked));

        /* old C# serialization/deserialization, wont work on XB1 due to JIT issue
        MemoryStream stream = new MemoryStream();
        try{
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, mUnlocked);
            return stream.GetBuffer();
        }catch(System.Exception e){
            throw e;
        }finally{
            stream.Close();
        }*/
    }

    public void deserialize(byte[] aData)
    {
        Debug.Log((new UnicodeEncoding()).GetString(aData));
        mUnlocked = JsonConvert.DeserializeObject<Unlockables>((new UnicodeEncoding()).GetString(aData));

        /* old C# serialization/deserialization, wont work on XB1 due to JIT issue
        try{
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(aData);
            mUnlocked = (Unlockables) formatter.Deserialize(stream);
            stream.Close();
        } catch {}
        */
    }

    //TODO DELETE
    //TODO do this through ZgInterface
	void read_unlock()
	{
		try{
			IFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream("gg.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
			mUnlocked = (Unlockables) formatter.Deserialize(stream);
			stream.Close();
		} catch {} //no such file, must be first launch OR save data corrupted
	}

    //TODO DELETE
    //TODO do this through ZgInterface
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
