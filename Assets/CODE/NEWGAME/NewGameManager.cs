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
	
	public CharacterIndex CurrentCharacterIndex
	{ get { return CurrentPerformanceStat.Character; } }
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
	
	//TODO rename
	public void initialize_GRAVE()
	{
		mManager.mAssetLoader.new_load_character("999",mManager.mCharacterBundleManager);
	}
	
	public bool character_changed_listener(CharacterLoader aCharacter)
	{
		//at this point, we can assume both body manager, music and background managers have been set accordingly
		//i.e. this is part of transition to PLAY or GRAVE
		CurrentCharacterLoader = aCharacter;
		
		//set new character data
		//TODO finish
		mPerformanceStats.Add(new PerformanceStats());
		CurrentPerformanceStat.Character = new CharacterIndex(aCharacter.Name);
		mManager.mInterfaceManager.begin_new_character(CurrentPerformanceStat);
		
		//TODO
		switch(aCharacter.Name)
		{
			case "0-1":	
				DeathCharacter = aCharacter; //TODO AssetBundle.undload will actually mess this up...
				TimeRemaining = 30f;
				setup_next_poses(true);
				transition_to_PLAY();
				break;
			case "100":
				TimeRemaining = 30f;
				setup_next_poses(true);
				transition_to_PLAY();
				break;
			case "999":
				setup_next_poses(true);
				transition_to_GRAVE();
				break;
			default:
				TimeRemaining = 30f;
				transition_to_PLAY();
				setup_next_poses();
				break;
		}
		
		mManager.mTransitionCameraManager.fade_in_with_sound();
		
		if(aCharacter.Name == "0-1") //in this very special case, we keep the bundle to load the death cutscene
			return false;
		
		return true;
	}
    
	
	
    public override void Update()
    {
        CurrentPose = ProGrading.snap_pose(mManager); 
		
		if(GS == GameState.PLAY)
		{
			if(Input.GetKeyDown(KeyCode.Alpha0))
			{
				TimeRemaining = 0;
			}
			update_PLAY();
		}
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
	
	public ProGrading.Pose CurrentPose
	{ get; private set; }
	public ProGrading.Pose CurrentTargetPose
    { get; private set; }
	public PoseAnimation CurrentPoseAnimation
	{ get; private set; }
	
	public void update_PLAY()
	{
		TimeRemaining -= Time.deltaTime;
			
		
		//this basically means we aren't 0 or 100 or 999
		if (CurrentPoseAnimation != null)
        {
			
			ProGrading.Pose newPose = CurrentPoseAnimation.get_pose((int)(Time.deltaTime/5f));
			if(CurrentTargetPose != newPose)
			{
				CurrentTargetPose = newPose;
				mManager.mTransparentBodyManager.set_target_pose(newPose);
			}
			
            float grade = ProGrading.grade_pose(CurrentPose, CurrentTargetPose);
			
			//update graph
			CurrentPerformanceStat.PerformanceGraph.update_graph(PercentTimeCompletion,grade);
			
			//update score
			mManager.mInterfaceManager.update_bb_score(TotalScore);	
        }
		
		if(TimeRemaining < 0)
		{
			finish_PLAY();
			if(CurrentPerformanceStat.Character.Index == 0)
				transition_to_CUTSCENE();
			else transition_to_CHOICE();
		}
		
		//early death
		//TODO
		//hack death
		if(Input.GetKeyDown(KeyCode.D))
		{
			transition_to_DEATH();
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
			Debug.Log ("choice is made " + choice);
			transition_to_TRANSITION_play(CurrentPerformanceStat.Character.get_future_neighbor(choice));
		}
		
		//hack graveo
		if(Input.GetKeyDown(KeyCode.D))
		{
			transition_to_TRANSITION_play(new CharacterIndex("999"));
		}
		
	}
	public void transition_to_CUTSCENE()
	{
		GS = GameState.CUTSCENE;
		mManager.mBodyManager.transition_character_out();
		mManager.mTransparentBodyManager.transition_character_out();
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
		
		mManager.mInterfaceManager.set_for_DEATH(CurrentPerformanceStat.Character)
			.then_one_shot(delegate(){mManager.mTransitionCameraManager.fade_out_with_sound(initialize_GRAVE);},3);
	}
	
	public void transition_to_GRAVE()
	{
		GS = GameState.GRAVE;
		mManager.mInterfaceManager.set_for_GRAVE(mPerformanceStats, 
			delegate()
			{
				mManager.mTransitionCameraManager.fade_out_with_sound(mManager.restart_game);
			}
		);
	}
	
	public void transition_to_CHOICE()
	{
		GS = GameState.CHOICE;
		mChoiceHelper.shuffle_and_set_choice_poses(mManager.mInterfaceManager);
		//TODO mManager.mMusicManager.play_sound_effect(
		mManager.mInterfaceManager.set_for_CHOICE();	
	}
	
	//make sure the next character is set before callincg this
	public void setup_next_poses(bool setNull = false)
	{
		if(setNull)
		{
			CurrentPoseAnimation = null;
			CurrentTargetPose = null;
		}
		else
		{
			CurrentPoseAnimation = mManager.mCharacterBundleManager.get_pose(CurrentCharacterIndex,CurrentPerformanceStat.Difficulty);
			CurrentTargetPose = CurrentPoseAnimation.get_pose(0);
		}
	}
	
	public void transition_to_PLAY()
	{
		GS = GameState.PLAY;
		mManager.mInterfaceManager.set_for_PLAY();
	}
	
	public void transition_to_TRANSITION_play(CharacterIndex aNextCharacter)
	{
		GS = GameState.TRANSITION;
		mManager.mInterfaceManager.set_for_PLAY(); //this is jsut visual
		TED.add_one_shot_event(
			delegate(){
				mManager.mTransitionCameraManager.fade_out_with_sound(
					delegate(){
						mManager.mAssetLoader.new_load_character(aNextCharacter.StringIdentifier,mManager.mCharacterBundleManager);
					}
				);
			},
		1);
	}
	
	public void hack_choice(int choice, float time = -1)
	{
		//TODO
	}
    
}
