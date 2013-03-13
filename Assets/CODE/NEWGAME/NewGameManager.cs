using UnityEngine;
using System.Collections.Generic;

public enum GameState
{
	NONE,PLAY,CUTSCENE,DEATH,CHOICE,TRANSITION,GRAVE
}

public class NewGameManager : FakeMonoBehaviour
{
    public NewGameManager(ManagerManager aManager)
        : base(aManager) 
    {
    }
	public TimedEventDistributor TED { get; private set; }
	
	//TODO implement these or delete
	public int CurrentLevel
    { get; private set; }
	
	public GameState GS
	{ get; private set; }
	
	public PerformanceStats CurrentPerformanceStat
	{ get { return mPerformanceStats[mPerformanceStats.Count-1]; } }
	
	
	//actual game data
	List<PerformanceStats> mPerformanceStats = new List<PerformanceStats>();
	
	public override void Start()
	{
		GS = GameState.NONE;
		TED = new TimedEventDistributor();
		
		//TODO initialize game state
			//start in on loading screen
			//interfaceManager -> loading screen ...
		
		
		
		//initialize game data
		//initialize_fetus();
	
	}
	
	public void initialize_fetus()
	{
		mManager.mAssetLoader.new_load_character("0-1",mManager.mCharacterBundleManager);
		
		
	}
	
	public void initialize_choice(int choiceIndex)
	{
		//TODO	
	}
	
	public void initialize_grave()
	{
		mManager.mAssetLoader.new_load_character("999",mManager.mCharacterBundleManager);
	}
	
	public void character_changed_listener(CharacterLoader aCharacter)
	{
		//at this point, we can assume both body manager, music and background managers have been set accordingly
		//i.e. this is part of transition to PLAY or GRAVE
		//TODO
		switch(aCharacter.Name)
		{
			case "0-1":
				break;
			case "100":
				break;
			case "999":
				break;
			default:
				break;
		}
	}
    
    public override void Update()
    {
        //User = (mManager.mZigManager.has_user());
		
		//TODO handle tracking and scoring
		if(GS == GameState.PLAY)
		{
			
		}
        
		TED.update(Time.deltaTime);
	}
	
	public void transition_to_CUTSCENE()
	{
		GS = GameState.CUTSCENE;
		mManager.mInterfaceManager.set_for_CUTSCENE(
			delegate() { transition_to_CHOICE(); }
		);
		//mManager.mBackgroundManager
	}
	
	public void transition_to_DEATH()
	{
		GS = GameState.DEATH;	
		//mManager.mInterfaceManager
		//mManager.mBackgroundManager
		
		//initialize_grave();
	}
	
	public void transition_to_CHOICE()
	{
		GS = GameState.CHOICE;
		mManager.mInterfaceManager.set_for_CHOICE();	
	}
	
	public void transition_to_PLAY()
	{
		//mManager.mInterfaceManager.set_for_PLAY();
	}
	
	public void transition_to_TRANSITION_play()
	{
		GS = GameState.TRANSITION;
		//mManager.mTransitionCameraManager.fade
	}
	
	public void transition_to_TRANSITION_grave()
	{
		GS = GameState.TRANSITION;
		//mManager.mTransitionCameraManager.fade
	}
	
	public void transition_to_GRAVE()
	{
		GS = GameState.GRAVE;
		//TODO
	}
	
	public void cleanup()
	{
	}
	
	public void hack_choice(int choice, float time = -1)
	{
		//TODO
	}
    
}
