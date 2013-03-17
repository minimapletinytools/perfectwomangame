using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
	
	//TODO implement this or delete
	public int CurrentLevel
    { get; private set; }
	
	public CharacterLoader CurrentCharacterLoader
	{ get; private set; }
	
	public GameState GS
	{ get; private set; }
	
	public float TotalScore{ 
		get{
			return mPerformanceStats.Sum(delegate (PerformanceStats e) { return e.Score; });
		}
	}
	
	public PerformanceStats CurrentPerformanceStat
	{ get { return mPerformanceStats[mPerformanceStats.Count-1]; } }
	
	
	//actual game data
	List<PerformanceStats> mPerformanceStats = new List<PerformanceStats>();
	
	public override void Start()
	{
		CurrentCharacterLoader = null;
		GS = GameState.NONE;
		TED = new TimedEventDistributor();
		
		//TODO initialize game state
			//start in on loading screen
			//interfaceManager -> loading screen ...
		
		//TODO buffer grave
		
		//initialize game data
		//initialize_fetus();
		
		mChoiceHelper = new ChoiceHelper();
	
	}
	
	public void initialize_fetus()
	{
		mManager.mAssetLoader.new_load_character("0-1",mManager.mCharacterBundleManager);
		
		//do oI actually want this here??
		mManager.mInterfaceManager.setup_bb();
		mManager.mInterfaceManager.setup_pb();
	}
	
	public void initialize_choice(int choiceIndex)
	{
		//TODO	
	}
	
	
	public void character_changed_listener(CharacterLoader aCharacter)
	{
		//at this point, we can assume both body manager, music and background managers have been set accordingly
		//i.e. this is part of transition to PLAY or GRAVE
		CurrentCharacterLoader = aCharacter;
		
		//set new character data
		mPerformanceStats.Add(new PerformanceStats());
		CurrentPerformanceStat.Character = new CharacterIndex(aCharacter.Name);
		mManager.mInterfaceManager.set_new_character(CurrentPerformanceStat);
		
		//TODO
		switch(aCharacter.Name)
		{
			case "0-1":
				DeathCharacter = aCharacter; //so hopefully AssetBundle.unload doesn't fudge this up...
				TimeRemaining = 1f;
				transition_to_PLAY();
				break;
			case "100":
				TimeRemaining = 5;
				transition_to_PLAY();
				break;
			case "999":
				//transition_to_GRAVE();
				break;
			default:
				TimeRemaining = 5;
				transition_to_PLAY();
				break;
		}
	}
    
	
	
    public override void Update()
    {
        //User = (mManager.mZigManager.has_user());
		
		if(GS == GameState.PLAY)
			update_PLAY();
		if(GS == GameState.CHOICE)
			update_CHOICE();
        
		TED.update(Time.deltaTime);
	}
	
	public float TimeRemaining
	{ get; private set; }
	public float TimeTotal
	{ get; private set; }
	public float PercentTimeCompletion
	{ get { return TimeRemaining/TimeTotal; } }
	public ProGrading.Pose CurrentTargetPose
    { get; private set; }
	
	public void update_PLAY()
	{
		TimeRemaining -= Time.deltaTime;
		
		//this basically means we aren't 0 or 100 or 999
		//if (CurrentPerformanceStat.Character.Level != 0 && CurrentPerformanceStat.Character.Level != 8 && CurrentPerformanceStat.Character.Level != 9)
		if (CurrentTargetPose != null && mManager.mTransparentBodyManager.mFlat.mTargetPose != null)
        {
			//TODO
            //float grade = ProGrading.grade_pose(CurrentPose, mManager.mTransparentBodyManager.mFlat.mTargetPose);
			//TODO update interface with percent completion
			//PercentTimeCompletion
			
			//TODO update score
			//CurrentPerformanceStat.Score
			
			//TODO update graphics
			CurrentPerformanceStat.PerformanceGraph.update_graph(PercentTimeCompletion,0.5f);
        }
		
		if(TimeRemaining < 0)
		{
			finish_PLAY();
			transition_to_CUTSCENE();
		}
	}
	public void finish_PLAY()
	{
		//do I even need this??
	}
	
	ChoiceHelper mChoiceHelper;
	public void update_CHOICE()
	{
		int choice = mChoiceHelper.update(mManager.mInterfaceManager);
		if(choice != -1)
		{
			transition_to_PLAY();
			transition_to_TRANSITION_play(CurrentPerformanceStat.Character.get_neighbor(choice));
		}
		
	}
	public void transition_to_CUTSCENE()
	{
		GS = GameState.CUTSCENE;
		mManager.mInterfaceManager.set_for_CUTSCENE(
			delegate() { transition_to_CHOICE(); }
		);
		
		//TODO eventually wont be cutscene 0
		mManager.mBackgroundManager.load_cutscene(0,CurrentCharacterLoader);
	}
	
	CharacterLoader DeathCharacter
	{ get; set; }
	public void transition_to_DEATH()
	{
		GS = GameState.DEATH;	
		//mManager.mInterfaceManager
		//mManager.mBackgroundManager
		mManager.mBackgroundManager.load_cutscene(4,DeathCharacter);
		
		//TODO transition to grave
		//TODO get grave cutscene stuff..
		//initialize_grave();
		
		//var chain = TED.add_event(
		mManager.mInterfaceManager.set_for_DEATH(CurrentPerformanceStat.Character)
			.then_one_shot(delegate(){mManager.mTransitionCameraManager.fade(initialize_GRAVE);},3);
	}
	
	public void initialize_GRAVE()
	{
		GS = GameState.GRAVE;
		mManager.mAssetLoader.new_load_character("999",mManager.mCharacterBundleManager);
		//mManager.mInterfaceManager.
	}
	
	public void transition_to_CHOICE()
	{
		GS = GameState.CHOICE;
		mManager.mInterfaceManager.set_for_CHOICE();	
	}
	
	public void transition_to_PLAY()
	{
		GS = GameState.PLAY;
		mManager.mInterfaceManager.set_for_PLAY();
	}
	
	public void transition_to_TRANSITION_play(CharacterIndex aNextCharacter)
	{
		GS = GameState.TRANSITION;
		mManager.mTransitionCameraManager.fade(
			delegate(){
				mManager.mAssetLoader.new_load_character(aNextCharacter.StringIdentifier,mManager.mCharacterBundleManager);
			}
		);
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
