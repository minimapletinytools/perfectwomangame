using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum GameState
{
	NONE,PREPLAY,PLAY,CUTSCENE,DEATH,CHOICE,TRANSITION,GRAVE
}

public class NewGameManager : FakeMonoBehaviour
{
	
	const float PREPLAY_TIME = 2;
		
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
			//TODO you can probably come up with something better that this can't you???
			//TODO you could also keep track of the score separetly...
			//W/E
			return mPerformanceStats.Sum(delegate (PerformanceStats e) { return e.AdjustedScore; });
		}
	}
	
	public PerformanceStats CurrentPerformanceStat
	{ get { return mPerformanceStats[mPerformanceStats.Count-1]; } }
	
	public CharacterIndex CurrentCharacterIndex
	{ get { return CurrentPerformanceStat.Character; } }
	
	//actual game data
    CharacterHelper CharacterHelper
    {
        get
        {
            return mManager.mCharacterBundleManager.get_character_helper();
        }
    }
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
		
		mManager.mInterfaceManager.setup_bb();
		mManager.mInterfaceManager.setup_pb();
		mManager.mInterfaceManager.set_pb_character_icon_colors(CharacterHelper.Characters.Where(e=>e!=null).ToList());
		
		
		//TODO put this in its own function
		List<KeyValuePair<CharacterIndex,ProGrading.Pose>> poses = new List<KeyValuePair<CharacterIndex, ProGrading.Pose>>();
		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
			//poses.Add(new KeyValuePair<CharacterIndex,ProGrading.Pose>(e,mManager.mCharacterBundleManager.get_pose(e,mCharacterHelper.Characters[e.Index].Difficulty).get_pose(0)));
			var poseAnimation = mManager.mCharacterBundleManager.get_pose(e,CharacterHelper.Characters[e.Index].Difficulty);
			poses.Add(new KeyValuePair<CharacterIndex,ProGrading.Pose>(e,poseAnimation.get_pose(Random.Range(0,poseAnimation.poses.Count))));
		}
		mManager.mInterfaceManager.set_pb_character_icon_poses(poses);
		
		
		
		
	}
	
	
	public void set_time_for_PLAY(float aTime)
	{
		TimeRemaining = aTime;
		TimeTotal = aTime;
	}
	public bool character_changed_listener(CharacterLoader aCharacter)
	{
		//at this point, we can assume both body manager, music and background managers have been set accordingly
		//i.e. this is part of transition to PLAY or GRAVE
		CurrentCharacterLoader = aCharacter;
		
		//set new character data
		CharacterIndex newCharIndex = new CharacterIndex(aCharacter.Name);
		mPerformanceStats.Add(new PerformanceStats(newCharIndex));
		CurrentPerformanceStat.Stats = CharacterHelper.Characters[newCharIndex.Index];
		mManager.mInterfaceManager.begin_new_character(CurrentPerformanceStat);
		
		//TODO
		switch(aCharacter.Name)
		{
			case "0-1":	
				DeathCharacter = aCharacter; //TODO AssetBundle.undload will actually mess this up...
				set_time_for_PLAY(30f);
				setup_next_poses(true);
				transition_to_PLAY();
				break;
			case "100":
				set_time_for_PLAY(30f);
				setup_next_poses(true);
				transition_to_PLAY();
				break;
			case "999":
				setup_next_poses(true);
				transition_to_GRAVE();
				break;
			default:
				set_time_for_PLAY(30f);
				setup_next_poses(false);
				transition_to_PLAY();
				break;
		}
		
		mManager.mTransitionCameraManager.fade_in_with_sound();
		
		if(aCharacter.Name == "0-1") //in this very special case, we keep the bundle to load the death cutscene
			return false;
		
		return true;
	}
    
	
	
    public override void Update()
    {
		//if(mManager.mZigManager.has_user())
		{
        	CurrentPose = ProGrading.snap_pose(mManager); 
		}
		
		if(GS == GameState.PLAY)
		{
			update_PLAY();
			if(Input.GetKeyDown(KeyCode.Alpha0))
			{
				TimeRemaining = 0;
			}
		}
		else if(GS == GameState.CHOICE) 
			update_CHOICE();
        
		TED.update(Time.deltaTime);
	}
	
	public float TimeRemaining
	{ get; private set; }
	public float TimeTotal
	{ get; private set; }
	public float PercentTimeCompletion
	{ get { return 1-TimeRemaining/TimeTotal; } }
	
	public ProGrading.Pose CurrentPose
	{ get; private set; }
	public ProGrading.Pose CurrentTargetPose
    { get; private set; }
	public PoseAnimation CurrentPoseAnimation
	{ get; private set; }
	
	public float mLastGrade = 0.5f;
	
	public void update_PLAY()
	{
		TimeRemaining -= Time.deltaTime;
		
		mManager.mInterfaceManager.update_bb_score(TotalScore);
		
		if(CurrentPose != null) //this should never happen but just in case
		{
			mManager.mBodyManager.set_target_pose(CurrentPose);
		}
		
		//this basically means we aren't 0 or 100 or 999
		if (CurrentPoseAnimation != null)
        {
			
			CurrentTargetPose = CurrentPoseAnimation.get_pose((int)(Time.time/6f));
			mManager.mTransparentBodyManager.set_target_pose(CurrentTargetPose);
			
			
			
            float grade = ProGrading.grade_pose(CurrentPose, CurrentTargetPose);
			grade = ProGrading.grade_to_perfect(grade);
			
			float newGrade = mLastGrade*0.95f + grade*0.05f;
			if(newGrade < mLastGrade)
				mLastGrade = Mathf.Max(newGrade,mLastGrade - Time.deltaTime/6f);
			else mLastGrade = newGrade;
			grade = mLastGrade;
			
			CurrentPerformanceStat.update_score(PercentTimeCompletion,grade);			
			mManager.mCameraManager.set_camera_effects(grade);
			//update score
			mManager.mInterfaceManager.update_bb_score(TotalScore);	
        }
		else
			CurrentPerformanceStat.update_score(PercentTimeCompletion,0.5f);
		
		if(TimeRemaining < 0)
		{
			CurrentPerformanceStat.Finished = true;
			mManager.mCameraManager.set_camera_effects(0);
			mManager.mInterfaceManager.enable_warning_text(false);
			transition_to_CUTSCENE();
			
			//if we don't want fetus to have a cutscene use this
			//if(CurrentPerformanceStat.Character.Index != 0)
			//	transition_to_CUTSCENE();
			//else transition_to_CHOICE();
		}
		
		
		
		
		//early death
		bool die = false;
		die |= Input.GetKeyDown(KeyCode.D);
		if (CurrentPoseAnimation != null && mManager.mZigManager.has_user())
		{
			if(PercentTimeCompletion > 0.2f && CurrentPerformanceStat.last_score(1.5f/30f)/(1.5f/30f) < 0.2f)
				mManager.mInterfaceManager.enable_warning_text(true);
			else 
				mManager.mInterfaceManager.enable_warning_text(false);
			
			//mManager.mDebugString = "" + CurrentPerformanceStat.last_score(4/30f)/(4/30f);
			
			if(PercentTimeCompletion > 0.25f)
			{
				if(CurrentPerformanceStat.last_score(4/30f)/(4/30f) < 0.2f)
					die |= true;
			}
		}
		if(die)
		{
			CurrentPerformanceStat.Finished = true;
			mManager.mCameraManager.set_camera_effects(0);
			mManager.mInterfaceManager.enable_warning_text(false);
			transition_to_DEATH();
		}
		
		//if we don't want the music to play during the cutscenes and whatont...
		//if(GS != GameState.PLAY)
		//	mManager.mMusicManager.fade_out();
			
	}
	
	ChoiceHelper mChoiceHelper;
	public void update_CHOICE()
	{
		mManager.mInterfaceManager.set_bb_decider_pose(CurrentPose);
		mChoiceHelper.CurrentPose = CurrentPose;
		int choice = mChoiceHelper.update(mManager.mInterfaceManager);
		if(choice != -1)
		{
			mManager.mMusicManager.fade_out_choice_music();
			transition_to_TRANSITION_play(CurrentPerformanceStat.Character.get_future_neighbor(choice));
		}
	}
	public void transition_to_CUTSCENE()
	{
		GS = GameState.CUTSCENE;

        NUPD.ChangeSet changes;
        
        try
        {
			if(mManager.mCharacterBundleManager.get_character_stat(CurrentCharacterIndex).CharacterInfo.ChangeSet.Count>0)
			{
				//Debug.Log (mManager.mCharacterBundleManager.get_character_stat(CurrentCharacterIndex).CharacterInfo.ChangeSet[0].LowerThreshold);
				//Debug.Log (mManager.mCharacterBundleManager.get_character_stat(CurrentCharacterIndex).CharacterInfo.ChangeSet[0].UpperThreshold);
				//Debug.Log (CurrentPerformanceStat.Score);
			}
            changes = mManager.mCharacterBundleManager.get_character_stat(CurrentCharacterIndex).CharacterInfo.ChangeSet.Find(e => e.LowerThreshold <= CurrentPerformanceStat.Score && e.UpperThreshold >= CurrentPerformanceStat.Score);
        }
        catch
        {
            Debug.Log("could not find change in thershold with performance: " + CurrentPerformanceStat);
            changes = new NUPD.ChangeSet();
            changes.Changes.Add(new NUPD.ChangeSubSet() { Description = "No changes available!!" });
        }
		
		
		//visuals
		mManager.mBodyManager.transition_character_out();
		mManager.mTransparentBodyManager.transition_character_out();
		
		if(CurrentPerformanceStat.Score < 0.5f && CurrentCharacterLoader.has_cutscene(1))
			mManager.mBackgroundManager.load_cutscene(1,CurrentCharacterLoader);
		else
			mManager.mBackgroundManager.load_cutscene(0,CurrentCharacterLoader);
		
		
		mManager.mInterfaceManager.set_for_CUTSCENE(
			delegate() 
			{ 
				TED.add_one_shot_event(
					delegate() 
					{	 
				
				        foreach (var e in changes.Changes)
				        {
				            var diffChanges = e.Changes;
				            string changeMsg = e.Description;
				            for(int i = 0; i < diffChanges.Length; i++){
				                var cchar = new CharacterIndex(i);
								if(diffChanges[i] != 0)
								{
				                	int nDiff = mManager.mGameManager.change_character_difficulty(cchar, diffChanges[i]);
									change_interface_pose(cchar,nDiff);
								}
				            }
				        }
						mManager.mMusicManager.fade_out();
					}
				,0).then_one_shot(
					delegate() 
					{	 
						if(CurrentPerformanceStat.Character.Level > 6) //if age 85 or greater
						{
							//TODO conditions to get to age 100
							if(false)
							{
								transition_to_TRANSITION_play(new CharacterIndex("100"));
							}
							else
							{
								transition_to_TRANSITION_play(new CharacterIndex("999"));
								//transition_to_DEATH();
							}
						}
						else
							transition_to_CHOICE(); 
					}
				,2);
			}, changes
		);
		
	}
	
	CharacterLoader DeathCharacter
	{ get; set; }
	
	public void transition_to_DEATH()
	{
		GS = GameState.DEATH;	
		
		//mark time of death 
		CurrentPerformanceStat.DeathTime = PercentTimeCompletion;
		
		//set the cutscene
		mManager.mMusicManager.fade_out();
		mManager.mBodyManager.transition_character_out();
		mManager.mTransparentBodyManager.transition_character_out();
		
		if(CurrentCharacterLoader.has_cutscene(4))
			mManager.mBackgroundManager.load_cutscene(4,CurrentCharacterLoader);
		else
			mManager.mBackgroundManager.load_cutscene(4,DeathCharacter);
		
		mManager.mInterfaceManager.set_for_DEATH(CurrentPerformanceStat.Character)
			.then_one_shot(
				delegate()
				{
					transition_to_TRANSITION_play(new CharacterIndex("999"));
				}
			,0);
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
        //TODO update difficulties in NIM charactericons here in case the user skipped the cutscenes and the diffs did not get updated

		GS = GameState.CHOICE;
		mChoiceHelper.shuffle_and_set_choice_poses(mManager.mInterfaceManager);
		//TODO these bottom two functions should be absoredb by ChoiceHelper
		//lol this is a dumb hack to not choose the missing character
		var chars = new CharacterIndex(CurrentPerformanceStat.Character.Level+1,3).Neighbors;
		var perfs = chars.Select(e=>CharacterHelper.Characters[e.Index].Perfect).ToList();
		mManager.mInterfaceManager.set_bb_choice_perfectness(perfs);
		mManager.mInterfaceManager.set_bb_choice_bodies(CurrentCharacterIndex);
		mManager.mMusicManager.fade_in_choice_music();
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
			CurrentPoseAnimation = mManager.mCharacterBundleManager.get_pose(CurrentCharacterIndex,CurrentPerformanceStat.Stats.Difficulty);
			CurrentTargetPose = CurrentPoseAnimation.get_pose(0);
			//mManager.mTransparentBodyManager.set_target_pose(CurrentTargetPose);
		}
	}
	
	public void transition_to_PLAY()
	{
		GS = GameState.PREPLAY;
		//no target pose means we don't want a transparent body
		if(CurrentTargetPose == null)
			mManager.mTransparentBodyManager.transition_character_out();
		mManager.mInterfaceManager.set_for_PLAY();
		TED.add_one_shot_event(delegate(){ GS = GameState.PLAY; },PREPLAY_TIME);
	}
	
	public void transition_to_TRANSITION_play(CharacterIndex aNextCharacter)
	{
		GS = GameState.TRANSITION;
		mManager.mInterfaceManager.set_for_PLAY(); //this is just visual
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




    public int get_character_difficulty(CharacterIndex aChar)
    {
        return CharacterHelper.Characters[aChar.Index].Difficulty;
    }
	
	public int change_character_difficulty(CharacterIndex aChar,  int aChange)
	{
        //TODO if aChange is +/- 9 do something special instead
		CharacterHelper.Characters[aChar.Index].Difficulty = Mathf.Clamp(CharacterHelper.Characters[aChar.Index].Difficulty + aChange,0,3);
		return CharacterHelper.Characters[aChar.Index].Difficulty;
	}
	
	public void change_interface_pose(CharacterIndex aChar,  int aDiff)
	{
		List<KeyValuePair<CharacterIndex,ProGrading.Pose>> poses = new List<KeyValuePair<CharacterIndex, ProGrading.Pose>>();
		var poseAnimation = mManager.mCharacterBundleManager.get_pose(aChar,aDiff);
		poses.Add(new KeyValuePair<CharacterIndex,ProGrading.Pose>(aChar,poseAnimation.get_pose(Random.Range(0,poseAnimation.poses.Count))));
		mManager.mInterfaceManager.set_pb_character_icon_poses(poses);
	}
}
