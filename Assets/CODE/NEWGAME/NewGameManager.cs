using UnityEngine;
using System.Collections;


public class NewGameManager : FakeMonoBehaviour
{
    public NewGameManager(ManagerManager aManager)
        : base(aManager) 
    {
    }
	public TimedEventDistributor TED { get; private set; }
	
	//TODO implement these
	public int CurrentLevel
    { get; private set; }
	
	
	
	
	//actual game data
	//PerformanceStats[] mPerformanceStats = new PerformanceStats[10];
	
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
	
	public void character_changed_listener(CharacterLoader aCharacter)
	{
		//at this point, we can assume both body manager, music and background managers have been set accordingly
		
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
	
	public void unset_character()
	{
	}
	
	public void set_character()
	{
	}
	
	public void cleanup()
	{
		//TODO
	}
	
	
	public void hack_choice(int choice, float time = -1)
	{
		//TODO
	}
    
}
