using UnityEngine;
using System.Collections.Generic;


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
	
	
	public PerformanceStats CurrentPerformanceStat
	{ get { return mPerformanceStats[mPerformanceStats.Count-1]; } }
	
	
	//actual game data
	List<PerformanceStats> mPerformanceStats = new List<PerformanceStats>();
	
	public override void Start()
	{
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
		
		//begin mode (nothing)
		//play mode (timer, score, and tracking running)
		//cutscene mode (disable characters, bg manager needs to transition into cutscene mode)
		//selection mode (nothing) -> prompts fade out
		//change character behind fade -> fade in
        
		TED.update(Time.deltaTime);
	}
	 
	
	public void transition_to_CUTSCENE()
	{
		//mManager.mInterfaceManager
		//mManager.mBackgroundManager
	}
	
	public void transition_to_DEATH()
	{
		//mManager.mInterfaceManager
		//mManager.mBackgroundManager
		
		//initialize_grave();
	}
	
	public void transition_to_CHOOSE()
	{
		//mManager.mInterfaceManager.set_for_choice();
		
	}
	
	public void transition_to_PLAY()
	{
		//mManager.mInterfaceManager.set_for_PLAY();
	}
	
	public void transition_to_TRANSITION_play()
	{
		//mManager.mTransitionCameraManager.fade
	}
	
	public void transition_to_TRANSITION_grave()
	{
		//mManager.mTransitionCameraManager.fade
	}
	
	
	public void cleanup()
	{
	}
	
	public void hack_choice(int choice, float time = -1)
	{
		//TODO
	}
    
}
