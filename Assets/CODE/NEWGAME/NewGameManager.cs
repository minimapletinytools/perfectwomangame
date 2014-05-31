using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NewGameManager : FakeMonoBehaviour
{
	public enum GameState
	{
		NONE,MENU,NORMAL,CHALLENGE,TEST,SIMIAN
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
	{ get; set; } //only public set because of hack in modenormalplay
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
	
	QuTimer mIdleTimer = new QuTimer(0,300);

	
	ModeTesting mModeTesting;
	ModeChallenge mModeChallenge;
	public ModeNormalPlay mModeNormalPlay;
	ModePerfectSimian mModeSimian;
	public void set_testing()
	{
		GS = GameState.TEST;
	}
	
	public override void Start()
	{
		CurrentCharacterLoader = null;

		if(GS != GameState.TEST)
			GS = GameState.MENU;
		
		
		
		mModeTesting = new ModeTesting(this);
		mModeChallenge = new ModeChallenge(this);
		mModeNormalPlay = new ModeNormalPlay(this);
		mModeSimian = new ModePerfectSimian(this);
	
	}
	
	public void start_game(CharacterIndex aChar)
	{
		//reset the scale in case we were in simian mode
		//GameConstants.SCALE = 1;

		if(mManager.mSimianMode)
		{
			Debug.Log("simian mode hehe");
			GS = GameState.SIMIAN;
			mModeSimian.initialize();
		}
		else if(GS != GameState.TEST)
		{
			GS = GameState.NORMAL;
			mModeNormalPlay.initialize_game_with_character(aChar);
		}
		else if(GS == GameState.TEST)
		{
			mManager.mAssetLoader.new_load_character(aChar.StringIdentifier,mManager.mCharacterBundleManager);
		}
	}
	
	public bool character_changed_listener(CharacterLoader aCharacter)
	{
		//at this point, we can assume both body manager, music and background managers have been set accordingly
		//i.e. this is part of transition to PLAY or GRAVE
		CurrentCharacterLoader = aCharacter;
		CurrentCharacterIndex = new CharacterIndex(aCharacter.Name);
		
		if (GS == GameState.NORMAL || GS == GameState.TEST || GS == GameState.CHALLENGE) {
			mManager.mBackgroundManager.character_changed_listener(aCharacter);
			if(aCharacter.Character != CharacterIndex.sGrave){ //special behaviour for grave
				mManager.mBodyManager.character_changed_listener(aCharacter);
				mManager.mTransparentBodyManager.character_changed_listener(aCharacter);
				//TODO set to actual pose that we want
				mManager.mTransparentBodyManager.set_target_pose(mManager.mReferences.mCheapPose.to_pose(),true);
				if(mManager.mZigManager.is_reader_connected() != 2)
					mManager.mBodyManager.set_target_pose(mManager.mReferences.mCheapPose.to_pose(),true);
			}
			else{
				mManager.mBodyManager.destroy_character();
				mManager.mTransparentBodyManager.destroy_character();
			}
			mManager.mMusicManager.character_changed_listener(aCharacter);

			if(GS == GameState.NORMAL)
				mModeNormalPlay.character_loaded();
			else if(GS == GameState.TEST)
				mModeTesting.character_loaded();
            else if(GS == GameState.CHALLENGE)
                mModeChallenge.character_loaded();
               
		}
		else if(GS == GameState.SIMIAN)
		{
			mManager.mBackgroundManager.character_changed_listener(aCharacter);
			mModeSimian.character_loaded();
		}
		
		if(aCharacter.Name == "0-1") //in this very special case, we keep the bundle to load the death cutscene
		{
			DeathCharacter = aCharacter;
			return false;
		}
		
		return true;
	}
    
	
	
    public override void Update()
    {
		if(mManager.mZigManager.has_user() && mManager.mZigManager.is_skeleton_tracked_alternative())
        	CurrentPose = ProGrading.snap_pose(mManager); 
		else CurrentPose = mManager.mReferences.mDefaultPose.to_pose();
		
		
		if (GS == GameState.NORMAL)
            mModeNormalPlay.update();
        else if (GS == GameState.TEST)
            mModeTesting.update();
        else if (GS == GameState.SIMIAN)
            mModeSimian.update();
        else if (GS == GameState.CHALLENGE)
            mModeChallenge.update();
        
		
		if(GS != GameState.SIMIAN)
		{
			//reader connected and no user
			if(!mManager.mZigManager.has_user() && mManager.mZigManager.is_reader_connected() == 2)
				mIdleTimer.update(Time.deltaTime);
			else mIdleTimer.reset();
			if(mIdleTimer.isExpired())
				mManager.restart_game();
		}
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
