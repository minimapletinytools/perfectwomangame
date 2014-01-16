using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CharacterStats
{
	public CharacterIndex Character { get; set; }
	public int Perfect { get; set; } //TODO DELETE (I'm not actually going to delete this, but it's no longer needed)
	public int Difficulty { get; set; }
	public NUPD.CharacterInformation CharacterInfo { get; set; }
	
	public CharacterStats()
	{
		Character = new CharacterIndex(-1,0);
		CharacterInfo = NUPD.CharacterInformation.default_character_info(Character);
		Perfect = 2;
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
			return mTotalScore;
			/*
			float r = 0;
			for(int i = 1; i < mScore.Count; i++)
			{
				r += mScore[i].Value * (mScore[i].Key-mScore[i-1].Key);
			}
			return r;*/
		}
	}
	
	public float AdjustedScore{
		get{
			return mTotalScore * 600;//mTotalScore * (1+Stats.Perfect) * 300;
		}
	}
	
	public bool Finished { get; set; } //did we finish with this character already
	public float DeathTime { get; set; } //what time (0,1) did this character die
	
	//public PerformanceGraphObject PerformanceGraph { get; private set; }
	
	public CharacterStats Stats
	{ get; set; }
	
	public NUPD.ChangeSet CutsceneChangeSet
	{ 
		get
		{
			NUPD.ChangeSet r = null;
			var changeSet = Stats.CharacterInfo.ChangeSet;
			for(int i=0; i < changeSet.Count; i++)
			{
				if(changeSet[i].LowerThreshold <= Score && changeSet[i].UpperThreshold >= Score)
				{
					r = changeSet[i];
					break;
				}
			}	
			return r;
		}
	}

	public bool BadPerformance
	{
		get{
			return Score <= GameConstants.badPerformanceThreshold;
		}
	}
	
	public PerformanceStats(CharacterIndex aChar)
	{
		
		Stats = new CharacterStats();
		Stats.Character = aChar;
	
		Finished = false;
		DeathTime = -1;
		
		//PerformanceGraph = new PerformanceGraphObject(11);
		
		mScore = new List<KeyValuePair<float, float>>();
		update_score(0,0); //this is a dummy point
	}
	
	List<KeyValuePair<float,float>> mScore; //time, score
	float mTotalScore;
	
	public void update_score(float aTime, float aScore) //time should be between 0 and 1
	{
		if(mScore.Count > 0)
			mTotalScore += (aTime-mScore.Last().Key)*aScore;
		mScore.Add(new KeyValuePair<float,float>(aTime,aScore));
		//PerformanceGraph.update_graph(aTime,aScore);
	}
	
	//this returns the score in the last <timeBack> 
	//use this for death
	public float last_score(float timeBack)
	{
		float currentTime = mScore.Last().Key;
		float r = 0;
		for(int i = mScore.Count-1; i > 0; i--)
		{
			if(Mathf.Abs(mScore[i].Key - currentTime) > timeBack)
				break;
			r += mScore[i].Value * (mScore[i].Key-mScore[i-1].Key);
			
		}
		return r;
	}
	
	
}