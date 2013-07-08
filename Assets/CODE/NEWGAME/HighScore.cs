using UnityEngine;
using System.Collections.Generic;


public class ScoreEntry
{
	public Texture2D Image
	{ get; set; }
	public float Score
	{ get; set; }
	public System.DateTime Date
	{ get; set; }
	//IP
	//Mac Address etc..
	
	public ScoreEntry()
	{
		Image = null;
		Score = 0;
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
	public void record_score(float aScore, AlternativeImageViewer aIV)
	{
		ScoreEntry score = new ScoreEntry();
		score.Score = aScore;
		score.Image = aIV.take_color_image();
		add_score(score);
	}
	public void add_score(ScoreEntry aScore)
	{
		//TODO
	}
	public void write_scores()
	{
	}
	public void read_scores()
	{
		/*TODO Fuck me..
		if(PlayerPrefs.HasKey("scores"))
		{
			ScoreEntry addMe = new ScoreEntry();
			string stuff = PlayerPrefs.GetString("scores");
			var split = stuff.Split("&");
			for(int i = 0; i < split.Length/2; i++)
			{
				for(int j = 0; j < 2; j++)
				{
					
					addMe.Score = split[2*i+j];
					System.IO.BinaryReader
					Texture2D image = new Texture2D();
					image.LoadImage(data);
					addMe.Image = 
				}
			}
		}*/
		//read score 
	}
}
