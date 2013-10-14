using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable()]
public class ScoreEntry
{
	[NonSerialized()]
	public string imageName;
	public Texture2D image;
	public float score;
	public System.DateTime date;
	//IP
	//Mac Address etc..
	
	public ScoreEntry()
	{
		imageName = "";
		image = null;
		score = 0;
	}
}

public class HighScore
{
	public List<ScoreEntry> Scores
	{
		get; private set;
	}
	public HighScore()
	{
		Scores = new List<ScoreEntry>();
	}
	public ScoreEntry record_score(float aScore, AlternativeImageViewer aIV)
	{
		ScoreEntry score = new ScoreEntry();
		score.score = aScore;
		score.image = aIV.take_color_image();
		score.imageName = "hsimage_"+DateTime.Now.ToString();
		return score;
	}
	public void add_score(ScoreEntry aScore)
	{
		//TODO delete old scores
	}
	
	public void load_scores()
	{
		//TODO deserialize Scores
		//Scores = 
		foreach(ScoreEntry e in Scores) //TODO order and take first 10 only
		{
			//TODO load images...
			//e.image = 
		}
	}
}
