using UnityEngine;
using System.Collections;
using System.Linq;

public static class GameConstants
{
	//public static int CONTENT_WIDTH = 2880;
	public static int CONTENT_WIDTH = 3200;
	public static int CONTENT_HEIGHT = 1800;

	
	
	public static int numberAges = 10; //fetus, 5, 16, 27, 34, 45, 60, 85, 100, DEAD
	public static int[] numberChoices = new int[]{1,10,10,10,10,10,10,10,1,1};
	
	public static Color[] IconDifficultyColors { get { return new Color[]{new Color32(28,182,71,255), new Color32(255,168,0,255), new Color32(234,84,2,255), new Color32(255,0,0,255)};}}
	public static Color[] IconDifficultyColorsOverTwo { get{ return IconDifficultyColors.Select(e=> e/2f).ToArray();}}
	
	
	public static Color ChoosingTextColor { get {return new Color(0,0,0); } } //TODO
	
	
	
	//static Color particleStreamEasy = new Color32(28,182,71,255);
	public static Color ParticleStreamEasy { get { return new Color32(28,182,71,255); } }
	//static Color particleStreamHard = new Color32(255,0,0,255);
	public static Color ParticleStreamHard { get { return new Color32(255,0,0,255); } }
	
	//game stuff
	public static float maxGradeNorm = 16;
	public static float minGradeNorm = 2;
	
	//grave stuff
	//public static string[] credits = {"Perfect Woman","created by", "Lea Sch\u00F6enfelder", "Peter Lu"};
    public static string[] credits = { 
		"Thanks:", "David Elliott","Sabrina Winter", "Eddo Stern", 
		"Ingo Von Staden", "Andreas Hykade", "Alex Rickett","Steven Amrhein",
		"Teut Weidemann", "Marc Lutz", "Annika Bauer", "Marius Winter","Aliah Darke", " ",
		
		"Musicians:", "Dirk Handreke", "Frank Simper", "Ingo Feuker", "David Hill", 
		"Lukas Nowok", "Luigi Maria Rapisarda", "Jonas Schwall", "Steffen Thumm", " ",
		
		"Lea Schoenfelder","Peter Lu", "Benedikts Haas"
	};
	
	
	/*
	public static int numberRetries = 0;
	public static float preplayTime = 0.1f;
	public static float fadingTime = 0.1f;
	public static float transitionToChoiceDelayTime = 0.1f;
	*/
	
	public static int numberRetries = 0;
	//public static int numberRetries = 1;
	public static float preplayTime = 2f;
	public static float fadingTime = 2.3f;
	public static float transitionToChoiceDelayTime = 1.2f;
	

    //colors
    static Color uiPink = new Color(1, .8f, .8f, 1);
    public static Color UiPink { get { return uiPink * 0.5f; } }
	static Color uiRed = new Color(1,0,0,1);
	public static Color UiRed{get {return uiRed*0.5f;}}
	public static Color UiRedTransparent{get {return new Color(uiRed.r,uiRed.g,uiRed.b,0)/2f; }}
	public static Color uiGreen = new Color(0,1,0,1);
	public static Color UiGreen{get {return uiGreen*0.5f;}}
	public static Color UiGreenTransparent{get {return new Color(uiGreen.r,uiGreen.g,uiGreen.b,0)/2f; }}
	static Color uiWhite= new Color(1,1,1,1);
	public static Color UiWhite{get {return uiWhite * 0.5f;}}
	public static Color UiWhiteTransparent{get {return new Color(uiWhite.r,uiWhite.g,uiWhite.b,0)/2f; }}
	
	public static Color UiMiniMan{get {return new Color(1,0.3f,0.2f); }}
	public static Color UiPopupBubble{get { return new Color(0.5f,0.5f,0.5f,0.4f); } }
	
	public static Color uiGraveText = new Color32(102,101,101,255);
	public static Color UiGraveText {get{return uiGraveText*0.5f;}}
	
	public static Color TransparentBodyDefaultColor {get {return new Color(0,0.317f,.898f,1);}}
	
	
	//characters
	//oddly enough, these are not constant :)
	//NOTE these get set in CharacterBundleManager, not the ideal way to do it but it works
	public static CharIndexContainerString INDEX_TO_SHORT_NAME = new CharIndexContainerString()
	{
		Contents = new string[][]{
			new string[]{""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"",},
			new string[]{"",}
		}
	};
	public static CharIndexContainerString INDEX_TO_FULL_NAME = new CharIndexContainerString()
	{
		Contents = new string[][]{
			new string[]{""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"",},
			new string[]{"",}
		}
	};
	public static CharIndexContainerString INDEX_TO_DESCRIPTION = new CharIndexContainerString()
	{
		Contents = new string[][]{
			new string[]{""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"","","","","","","",""},
			new string[]{"",},
			new string[]{"",}
		}
	};
}