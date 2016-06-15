using UnityEngine;
using System.Collections;
using System.Linq;

public static class GameConstants
{
    //XB1 specific stuff
#if UNITY_XBOXONE && !UNITY_EDITOR
    public static bool ALLOW_NO_KINECT = true;
    public static bool XB1 = true;
    public static int IDLE_RESTART_TIME = 9999999;
    public static bool KB_CONTROL = false;
    public static bool SHOW_DEBUG = false;
    public static bool UNLOCK_ALL = true;
    public static bool TUTORIAL_ONCE = true; //NOTE TUTORIAL_ONCE as true will not work if UNLOCK_ALL is true because that will disable saves
#else
    public static bool ALLOW_NO_KINECT = true;
    public static bool XB1 = false;
    public static int IDLE_RESTART_TIME = 300;
    public static bool KB_CONTROL = true;
    public static bool SHOW_DEBUG = false;
    public static bool UNLOCK_ALL = true;
    public static bool TUTORIAL_ONCE = false;
#endif



#if UNITY_XBOXONE && !UNITY_EDITOR
    public static string assetBundlePrefix = "/StreamingAssets/XB1/";
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
    public static string assetBundlePrefix = "/StreamingAssets/";
#elif UNITY_STANDALONE_OSX
    public static string assetBundlePrefix = "/Resources/Data/StreamingAssets/";
#endif

    //TODO only change this when you're about to load up simian mode
	//I didn't do the conversion properly on all the classes so it's a super bad half assed change
	//enough should bechanged such that perfect simian will work
	//I hope
	public static float SCALE = 1;

	//public static int CONTENT_WIDTH = 2880;
	public static int CONTENT_WIDTH = 3200;
	public static int CONTENT_HEIGHT = 1800;
	public static float TARGET_FRAMERATE = 50;


    public static bool FORCE_START = false;
    public static bool NEW_POSE_SWITCHING = false;

	public static int numberAges = 10; //fetus, 5, 16, 27, 34, 45, 60, 85, 100, DEAD
	public static int[] numberChoices = new int[]{1,10,10,10,10,10,10,10,1,1};






	public static Color[] IconDifficultyColors { get { return new Color[]{new Color32(28,182,71,255), new Color32(255,168,0,255), new Color32(234,84,2,255), new Color32(255,0,0,255)};}}
	public static Color[] IconDifficultyColorsOverTwo { get{ return IconDifficultyColors.Select(e=> e/2f).ToArray();}}
	
	
	public static Color ChoosingTextColor { get {return new Color(0,0,0); } } //TODO
	
	
	
	//static Color particleStreamEasy = new Color32(28,182,71,255);
	//public static Color ParticleStreamEasy { get { return new Color32(28,182,71,255); } }
	public static Color ParticleStreamEasy { get { return new Color32(255,255,0,255); } }
	//static Color particleStreamHard = new Color32(255,0,0,255);
	public static Color ParticleStreamHard { get { return new Color32(255,0,0,255); } }
	
	//game stuff
	public static float maxGradeNorm = 13f; //18 is good for final version, 16 for festival
	public static float minGradeNorm = 3f;	//use 1.5f for final 
    public static float gradingGraceDegrees = 1f;
    public static float astronautCutoff = .6f; //makes sense if this is the  same as badPerformanceThreshold
    public static float badPerformanceThreshold = 0.6f; //.4 for easy version
	public static float playSuperCutoff = 0.9f;
	public static float playFeverCutoff = 0.7f;
	public static float playFeverRequiredTime = 5f;
	public static float playDefaultPlayTime = 35f;
    public static float playAstronautPlayTime = 25f;//15f;
	public static float playFetusPassThreshold = 0.6f;
	public static float deathPerformanceThreshold = 0.1f;
	public static float deathRequiredTime = 8f;
	public static float[] difficultyToChangeTime = new float[]{4,2,1.4f,.7f};

    public static bool showReward = false;//true; //this might have broken sometime along the way...

    public static bool showAstronaut = true;

    public static bool allPerfectOnSkip = true; //for character skipping, sets performances of all characters to perfect (as oppose to random score)
	
	//grave stuff
	//public static string[] credits = {"Perfect Woman","created by", "Lea Sch\u00F6enfelder", "Peter Lu"};
    public static string[] credits = { 
        "1","Peter Lu","Lea Sch\u00F6enfelder"," ",

		"2","Steven Amrhein","Annika Bauer","Benedikt Haas","Marius Winter"," ",

		"3","Lukas Nowok", "Luigi Maria Rapisarda", "Jonas Schwall", "Steffen Thumm", 
		"Dirk Handreke", "Frank Simper", "Ingo Feuker", "David Hill", "Christian Barth", "Leo Frick", " ",

		"4","Teut Weidemann", "Marc Lutz", "Aliah Darke",  
		"Inga Von Staden", "Andreas Hykade", "Alex Rickett","Steven Amrhein",
		"David Elliott","Sabrina Winter", "Eddo Stern", "J\u00F6rg Ihle", "Tyler Stefanich", "Nikita Arefkia"
	};

	public static int numberRetries = 0;
	public static float preplayTime = .5f;
	public static float transitionToChoiceDelayTime = 1.2f;
	

    //colors
	//TODO should maybe rename the capital ones Ui<Color>Neutral or something
	public static Color uiPink = new Color(1, .8f, .8f, 1);
    public static Color UiPink { get { return uiPink * 0.5f; } }
	public static Color uiRed = new Color(1,0,0,1);
	public static Color UiRed{get {return uiRed*0.5f;}}
	public static Color UiRedTransparent{get {return new Color(uiRed.r,uiRed.g,uiRed.b,0)/2f; }}
	public static Color uiGreen = new Color(0,1,0,1);
	public static Color UiGreen{get {return uiGreen*0.5f;}}
	public static Color UiGreenTransparent{get {return new Color(uiGreen.r,uiGreen.g,uiGreen.b,0)/2f; }}
	public static Color uiYellow = new Color(1,1,0,1);
	public static Color UiYellow{get {return uiYellow*0.5f;}}
	public static Color UiYellowTransparent{get {return new Color(uiYellow.r,uiYellow.g,uiYellow.b,0)/2f; }}
	public static Color uiBlue = new Color(0,0,1,1);
	public static Color UiBlue{get {return uiBlue*0.5f;}}
    public static Color UiBlueTransparent { get { return new Color(uiBlue.r, uiBlue.g, uiBlue.b, 0) / 2f; } }
	public static Color uiPurple = new Color(1,0,1,1);
	public static Color UiPurple{get {return uiPurple*0.5f;}}
	public static Color UiPurpleTransparent{get {return new Color(uiPurple.r,uiPurple.g,uiPurple.b,0)/2f; }}
	public static Color uiWhite= new Color(1,1,1,1);
	public static Color UiWhite{get {return uiWhite * 0.5f;}}
	public static Color UiWhiteTransparent{get {return new Color(uiWhite.r,uiWhite.g,uiWhite.b,0)/2f; }}

	public static Color UiMiniMan{get {return new Color(0.8f,0.2f,0.8f); }}
	public static Color UiPopupBubble{get { return new Color(0.43f,0.43f,0.43f,0.42f); } }
	
	public static Color uiGraveText = new Color32(102,101,101,255);
	public static Color UiGraveText {get{return uiGraveText*0.5f;}}
	
	public static Color TransparentBodyDefaultColor {get {return new Color(0,0.317f,.898f,1);}}
    public static Color StartScreenLockedGray {get{return new Color(0,0,0,.2f);}}


	//TODO DELETE
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