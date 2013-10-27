using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NewGameManager : FakeMonoBehaviour
{
	public enum GameState
	{
		NONE,MENU,NORMAL,CHALLENGE,TEST
	}

		
    public NewGameManager(ManagerManager aManager)
        : base(aManager) 
    {
    }
	
	
	
	public CharacterHelper CharacterHelper
    { get{return mManager.mCharacterBundleManager.get_character_helper();} }
	
	
	
	public GameState GS
	{ get; private set; }
	public CharacterLoader CurrentCharacterLoader
	{ get; private set; }
	public CharacterIndex CurrentCharacterIndex
	{ get; private set; }
	public int CurrentLevel
    { get {return CurrentCharacterIndex.LevelIndex;}}
	
	
	
	public Pose CurrentPose
	{ get; set; }
	public Pose CurrentTargetPose
    { get; set; }
	public PerformanceType CurrentPoseAnimation
	{ get; set; }
	
	public CharacterLoader DeathCharacter //hack to store fetus death
	{ get; set; }

	
	ModeTesting mModeTesting;
	ModeChallenge mModeChallenge;
	ModeNormalPlay mModeNormalPlay;
	
	public void set_testing()
	{
		GS = GameState.TEST;
	}
	
	public override void Start()
	{
		CurrentCharacterLoader = null;
		GS = GameState.MENU;
		
		
		
		mModeTesting = new ModeTesting(this);
		mModeChallenge = new ModeChallenge(this);
		mModeNormalPlay = new ModeNormalPlay(this);
	
	}
	
	public void start_game()
	{
		GS = GameState.NORMAL;
		mModeNormalPlay.initialize_fetus();
	}
	
	public bool character_changed_listener(CharacterLoader aCharacter)
	{
		//at this point, we can assume both body manager, music and background managers have been set accordingly
		//i.e. this is part of transition to PLAY or GRAVE
		CurrentCharacterLoader = aCharacter;
		CurrentCharacterIndex = new CharacterIndex(aCharacter.Name);
		
		if(GS == GameState.NORMAL)
			mModeNormalPlay.character_loaded();
		//TODO testing, challenge
		
		if(aCharacter.Name == "0-1") //in this very special case, we keep the bundle to load the death cutscene
		{
			DeathCharacter = aCharacter;
			return false;
		}
		
		return true;
	}
    
	
	
    public override void Update()
    {
		//if(mManager.mZigManager.has_user())
        	CurrentPose = ProGrading.snap_pose(mManager); 
		if(GS == GameState.NORMAL)
			mModeNormalPlay.update();
		else if (GS == GameState.TEST)
			mModeTesting.update();
        
		
	}
	
	
	
	
	
    public int get_character_difficulty(CharacterIndex aChar)
    {
        return CharacterHelper.Characters[aChar].Difficulty;
    }
	
	public int change_character_difficulty(CharacterIndex aChar,  int aChange)
	{
        //TODO if aChange is +/- 9 do something special instead
		CharacterHelper.Characters[aChar].Difficulty = Mathf.Clamp(CharacterHelper.Characters[aChar].Difficulty + aChange,0,3);
		return CharacterHelper.Characters[aChar].Difficulty;
	}
}
