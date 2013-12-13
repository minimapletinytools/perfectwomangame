using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class ModeNormalPlay
{
	
	public enum NormalPlayGameState
	{
		NONE,PREPLAY,PLAY,CUTSCENE,DEATH,CHOICE,TRANSITION,GRAVE
	}
	
	NewGameManager NGM {get; set;}
	ManagerManager mManager {get; set;}
	
	public float TimeRemaining
	{ get; set; }
	public float TimeTotal
	{ get; set; }
	public float PercentTimeCompletion
	{ get { return 1-TimeRemaining/TimeTotal; } }
	
	public TimedEventDistributor TED { get; private set; }
	public NormalPlayGameState GS { get; private set; }
	public PerformanceStats CurrentPerformanceStat 
	{ get { return mPerformanceStats[mPerformanceStats.Count-1]; } }
	public float TotalScore
	{ get{ return mPerformanceStats.Sum(delegate (PerformanceStats e) { return e.AdjustedScore; }); } }
	
		
	public float mLastGrade = 0.5f;
	List<PerformanceStats> mPerformanceStats = new List<PerformanceStats>();
	BodyParticleHelper mParticles = new BodyParticleHelper();
	ChoiceHelper mChoiceHelper;
	
	public NewInterfaceManager mInterfaceManager = null;
	public SunsetManager mSunsetManager = null;
	public ChoosingManager mChoosingManager = null;

	FlatElementImage mInterfaceImage;
	FlatElementImage mSunsetImage;
	FlatElementImage mChoosingImage;
	
	public FlatCameraManager mFlatCamera;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	AdvancedGrading mGrading = new AdvancedGrading();
	
	public ModeNormalPlay(NewGameManager aNgm)
	{
		NGM = aNgm;
		mManager = aNgm.mManager;
		
		GS = NormalPlayGameState.NONE;
		TED = new TimedEventDistributor();
		mChoiceHelper = new ChoiceHelper();
		
		mInterfaceManager = new NewInterfaceManager(mManager,this);
		mInterfaceManager.initialize();
		mInterfaceManager.mFlatCamera.set_render_texture_mode(true);
		
		mSunsetManager = new SunsetManager(mManager);
		mSunsetManager.initialize();
		mSunsetManager.mFlatCamera.set_render_texture_mode(true);
		
		
		mChoosingManager = new ChoosingManager(mManager);
		mChoosingManager.initialize();
		mChoosingManager.mFlatCamera.set_render_texture_mode(true);
		
		mFlatCamera = new FlatCameraManager(new Vector3(-23234,3545,0),10);
		mFlatCamera.Camera.depth = 100;
		mFlatCamera.fit_camera_to_screen();

		mSunsetImage = new FlatElementImage(mSunsetManager.mFlatCamera.RT,0);
		mSunsetImage.HardScale = Vector3.one * mFlatCamera.Width/mSunsetImage.mImage.PixelDimension.x;
		mSunsetImage.HardPosition = mFlatCamera.Center + Vector3.right*mSunsetImage.BoundingBox.width;
		mElement.Add(mSunsetImage);

		mChoosingImage = new FlatElementImage(mChoosingManager.mFlatCamera.RT,0);
		mChoosingImage.HardScale = Vector3.one * mFlatCamera.Width/mChoosingImage.mImage.PixelDimension.x;
		mChoosingImage.HardPosition = mFlatCamera.Center + Vector3.right*mChoosingImage.BoundingBox.width;
		mElement.Add(mChoosingImage);


		//this is silly, we forec the interface image to appear above everything else pretty much just so we can have text bubbles show above everything
		//kind of a dumb way to do it oh well.
		mInterfaceImage = new FlatElementImage(mInterfaceManager.mFlatCamera.RT,1);
		mInterfaceImage.HardScale = Vector3.one * mFlatCamera.Width/mInterfaceImage.mImage.PixelDimension.x;
		mInterfaceImage.HardPosition = mFlatCamera.Center;
		mElement.Add(mInterfaceImage);
	}


	public void initialize_game_with_character(CharacterIndex aChar)
	{
		//load the character
		mManager.mAssetLoader.new_load_character(aChar.StringIdentifier,mManager.mCharacterBundleManager);
	
		
		//setup the interacem manager
		mInterfaceManager.setup_bb();
		
		//load sunset stuff
		mManager.mAssetLoader.new_load_asset_bundle("SUNSET",
			delegate(AssetBundle aBundle){
				mSunsetManager.sunset_loaded_callback(aBundle,"SUNSET");
			}
		);
	}
	
	public void set_time_for_PLAY(float aTime)
	{
		TimeRemaining = aTime;
		TimeTotal = aTime;
	}
	
	public void character_loaded()
	{
		
		mPerformanceStats.Add(new PerformanceStats(NGM.CurrentCharacterIndex));
		CurrentPerformanceStat.Stats = NGM.CharacterHelper.Characters[NGM.CurrentCharacterIndex];
		mInterfaceManager.begin_new_character(CurrentPerformanceStat);

		if(NGM.CurrentCharacterLoader.Name != "0-1")
			mSunsetManager.add_character(NGM.CurrentCharacterLoader.Character);
		switch(NGM.CurrentCharacterLoader.Name)
		{
			case "0-1":	
				//set_time_for_PLAY(30f);
				set_time_for_PLAY(999999f);
				setup_next_poses(true);
				transition_to_PLAY();
				float gTextDisplayDur = 4;
				NGM.CurrentTargetPose = null;
				TED.add_event(
					mInterfaceManager.skippable_text_bubble_event("TRY AND MAKE YOUR FIRST MOVEMENTS", gTextDisplayDur),
				3).then_one_shot(
					delegate(){
						NGM.CurrentTargetPose = mManager.mReferences.mCheapPose.to_pose();
						mManager.mTransparentBodyManager.set_target_pose(NGM.CurrentTargetPose);
						mManager.mTransparentBodyManager.transition_character_in(mManager.mCharacterBundleManager.get_character_stat(NGM.CurrentCharacterIndex).CharacterInfo.CharacterOutlineColor);
					},
				0).then(
					mInterfaceManager.skippable_text_bubble_event("MATCH THE POSE BEHIND YOU", gTextDisplayDur),
				1.5f);
				break;
			case "100":
				set_time_for_PLAY(30f);
				setup_next_poses(true);
				transition_to_PLAY();
				break;
			default:
				set_time_for_PLAY(30f);
				setup_next_poses(false);
				transition_to_PLAY();
				break;
		}
		mManager.mTransitionCameraManager.fade_in_with_sound();
	}
	
	public void draw_render_texture(FlatCameraManager aCam)
	{
		aCam.Camera.enabled = true;
		RenderTexture.active = aCam.RT;
		aCam.Camera.backgroundColor = new Color(1,1,1,0);
		aCam.Camera.clearFlags = CameraClearFlags.SolidColor;
		aCam.Camera.DoClear();
		aCam.Camera.Render();
		RenderTexture.active = null;
		aCam.Camera.enabled = false;
	}
	
	public void update()
	{
		mInterfaceManager.Update();
		mSunsetManager.update();
		mChoosingManager.update();

		//TODO only draw if necessary
		draw_render_texture(mSunsetManager.mFlatCamera);
		draw_render_texture(mChoosingManager.mFlatCamera);
		draw_render_texture(mInterfaceManager.mFlatCamera);
		
		mFlatCamera.update(Time.deltaTime);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);     
		
		if(GS == NormalPlayGameState.PLAY)
		{		
			if(Input.GetKeyDown(KeyCode.P))
				mParticles.create_particles(mGrading);
	
			update_PLAY();
			if(Input.GetKeyDown(KeyCode.Alpha0))
				TimeRemaining = -5; //0;
		}
		else if(GS == NormalPlayGameState.CHOICE) 
			update_CHOICE();
		
		mParticles.update(Time.deltaTime);
		TED.update(Time.deltaTime);
		
		
	}
	
	
	
	public void update_PLAY()
	{
		
		
		TimeRemaining -= Input.GetKey(KeyCode.O) ? Time.deltaTime * 5 : Time.deltaTime;
		
		mInterfaceManager.update_bb_score(TotalScore);
		
		if(NGM.CurrentPose != null) //this should never happen but just in case
		{
			if(!Input.GetKey(KeyCode.A))
				mManager.mBodyManager.set_target_pose(NGM.CurrentPose);
			else 
				mManager.mBodyManager.set_target_pose(NGM.CurrentTargetPose);
		}
		
		
		//this basically means we aren't 0 or 100 or 999
		if (NGM.CurrentPoseAnimation != null && NGM.CurrentCharacterIndex.LevelIndex != 0)
        {
			NGM.CurrentTargetPose = NGM.CurrentPoseAnimation.get_pose(Time.time);
			mManager.mTransparentBodyManager.set_target_pose(NGM.CurrentTargetPose);
			
			if(PercentTimeCompletion > 0.01f)
			{
				mParticles.create_particles(mGrading,true);
				if(NGM.CurrentPoseAnimation.does_pose_change_precoginitive(Time.time,Time.deltaTime,0.07f))
					mParticles.create_particles(mGrading);
			}
			
			
			
			
            float grade = ProGrading.grade_pose(mManager.mBodyManager.get_current_pose(),mManager.mTransparentBodyManager.get_current_pose());
			grade = ProGrading.grade_to_perfect(grade);
			mGrading.update(mManager.mBodyManager.get_current_pose(),mManager.mTransparentBodyManager.get_current_pose());
			
			float newGrade = mLastGrade*0.95f + grade*0.05f;
			if(newGrade < mLastGrade)
				mLastGrade = Mathf.Max(newGrade,mLastGrade - Time.deltaTime/6f);
			else mLastGrade = newGrade;
			grade = mLastGrade;
			
			if(TimeRemaining > 0) //insurance, something funny could happen if music runs slightly longer than it should.
				CurrentPerformanceStat.update_score(PercentTimeCompletion,grade);			
			
			//mManager.mCameraManager.set_camera_effects(grade);
			//update score
			mInterfaceManager.update_bb_score(TotalScore);	
        }
		else if(NGM.CurrentCharacterIndex.LevelIndex == 0 && true) //fetus
		{
			if(NGM.CurrentTargetPose != null){
				mManager.mTransparentBodyManager.set_target_pose(NGM.CurrentTargetPose);
	            float grade = ProGrading.grade_pose(mManager.mBodyManager.get_current_pose(),NGM.CurrentTargetPose); //should be mManager.mTransparentBodyManager.get_current_pose() but it does not matter
				grade = ProGrading.grade_to_perfect(grade);
				
				//this is a total hack, but we don't use mTotalScore for the fetus anyways
				FieldInfo scoreProp = typeof(PerformanceStats).GetField("mTotalScore",BindingFlags.NonPublic | BindingFlags.Instance);
				float oldGrade = (float)scoreProp.GetValue(CurrentPerformanceStat);
				float newGrade = oldGrade*0.93f + grade*0.07f;
				scoreProp.SetValue(CurrentPerformanceStat,newGrade);
				if(newGrade > 0.76f)
				{
					//this may or may not work depending on which update gets called first
					mInterfaceManager.SkipSingle();
					scoreProp.SetValue(CurrentPerformanceStat,0);
					TimeRemaining = 0;
				}
			}
		}
		else
			CurrentPerformanceStat.update_score(PercentTimeCompletion,0.5f);
		
		//warning
		if (NGM.CurrentPoseAnimation != null && NGM.CurrentCharacterIndex.LevelIndex != 0)
		{
			if(PercentTimeCompletion > 0.2f && CurrentPerformanceStat.last_score(1.5f/30f)/(1.5f/30f) < 0.2f)
				mInterfaceManager.enable_warning_text(true);
			else 
				mInterfaceManager.enable_warning_text(false);
		}
		
		//make sure music is finished too!
		//if((TimeRemaining <= 0 && !mManager.mMusicManager.IsMusicSourcePlaying) || TimeRemaining < -4) //but don't wait too long
		if(TimeRemaining <= 0)
		{
			CurrentPerformanceStat.Finished = true;
			mManager.mCameraManager.set_camera_effects(0);
			mInterfaceManager.enable_warning_text(false);
			transition_to_CUTSCENE();
			
			//if we don't want fetus to have a cutscene use this
			//if(CurrentPerformanceStat.Character.Index != 0)
			//	transition_to_CUTSCENE();
			//else transition_to_CHOICE();
			
			return;
		}
		
		//if we don't want the music to play during the cutscenes and whatont...
		//if(GS != NormalPlayGameState.PLAY)
		//	mManager.mMusicManager.fade_out();
		
		
		
		//early death
		bool die = false;
		die |= Input.GetKeyDown(KeyCode.D);
		//if (NGM.CurrentPoseAnimation != null && mManager.mZigManager.has_user() && NGM.CurrentCharacterIndex.LevelIndex != 0)
		if (NGM.CurrentPoseAnimation != null && mManager.mZigManager.is_reader_connected() == 2 && NGM.CurrentCharacterIndex.LevelIndex != 0)
		{
			if(PercentTimeCompletion > 0.35f)
			{
				if(CurrentPerformanceStat.last_score(7/30f)/(7/30f) < 0.17f)
					die |= true;
			}
		}
		if(die)
		{
			CurrentPerformanceStat.Finished = true;
			mManager.mCameraManager.set_camera_effects(0);
			mInterfaceManager.enable_warning_text(false);
			transition_to_DEATH();
		}
			
	}
	
	
	public void update_CHOICE()
	{
		mChoosingManager.set_bb_decider_pose(NGM.CurrentPose);
		mChoiceHelper.CurrentPose = NGM.CurrentPose;
		int choice = mChoiceHelper.update(new SetPlayChoice(mChoosingManager));
		if(choice != -1)
		{
			slide_image(mChoosingImage,mSunsetImage);
			mManager.mMusicManager.fade_out_extra_music();
			transition_to_TRANSITION_play(CurrentPerformanceStat.Character.get_future_neighbor(choice));
		}
	}
	public void transition_to_CUTSCENE()
	{
		GS = NormalPlayGameState.CUTSCENE;

        NUPD.ChangeSet changes = CurrentPerformanceStat.CutsceneChangeSet;
		
		load_CUTSCENE(changes);
		
		mInterfaceManager.set_for_CUTSCENE(
			delegate(){CUTSCENE_finished(changes);}, 
			changes
		);
		
	}
	
	public void CUTSCENE_finished(NUPD.ChangeSet changes = null)
	{
		TED.add_one_shot_event(
			delegate() 
			{	 
				//apply all the diff changes again (in case we skipped and tehy werent applied duringcutscene)
				if(changes != null){
			        foreach (var e in changes.Changes)
			        {
			            CharIndexContainerInt diffChanges = e.Changes;
			            string changeMsg = e.Description;
					
						foreach(CharacterIndex cchar in CharacterIndex.sAllCharacters)
						{
							if(diffChanges[cchar] != 0)
							{
			                	int nDiff = mManager.mGameManager.change_character_difficulty(cchar, diffChanges[cchar]);
							}
						}
			        }
				}
				mManager.mMusicManager.fade_out();
			}
		,0).then_one_shot(
			delegate() 
			{	 
				if(CurrentPerformanceStat.Character.LevelIndex > 6) //if age 85 or greater ?????? TODO whats going on here
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
		,GameConstants.transitionToChoiceDelayTime);
			
	}
	
	
	public void transition_to_DEATH()
	{
		float gTextDisplayDur = 5;
		
		bool firstDeath = mPerformanceStats.Where(e => e.DeathTime != -1).Count() < GameConstants.numberRetries;
		CurrentPerformanceStat.DeathTime = PercentTimeCompletion;
		
		var chain = TED.add_one_shot_event(delegate(){});
		
		if(firstDeath)
		{
			GS = NormalPlayGameState.CUTSCENE;
			mManager.mMusicManager.play_sound_effect("cutBad");
			load_CUTSCENE(CurrentPerformanceStat.CutsceneChangeSet);
		}
		else
		{
			
			GS = NormalPlayGameState.DEATH;
			chain = chain.then_one_shot(
				delegate()
				{
					mManager.mMusicManager.fade_out();
					mManager.mBodyManager.transition_character_out();
					mManager.mTransparentBodyManager.transition_character_out();
					mManager.mMusicManager.play_sound_effect("cutDie");
					mManager.mMusicManager.play_cutscene_music(NGM.CurrentCharacterLoader.Images.deathMusic);
					if(NGM.CurrentCharacterLoader.has_cutscene(4))
						mManager.mBackgroundManager.load_cutscene(4,NGM.CurrentCharacterLoader);
					else if(NGM.DeathCharacter != null) //this will fail if we skip the fetus 
						mManager.mBackgroundManager.load_cutscene(4,NGM.DeathCharacter);
				}
			);
		}
		
		//BAD PERFORMANCE
		chain = chain.then(
			mInterfaceManager.skippable_text_bubble_event("BAD PERFORMANCE", gTextDisplayDur)
		,2);
		
		//NEXT TIME YOU PERFORM THAT BAD YOU MIGHT DIE
		if(firstDeath) //if we haven't died previously
		{
			chain = chain.then(
				NGM.CurrentLevel == 7 ?
				mInterfaceManager.skippable_text_bubble_event("BUT IT'S OK BECAUSE YOU ARE ALREADY OLD", gTextDisplayDur)
				:
				mInterfaceManager.skippable_text_bubble_event("NEXT TIME YOU PERFORM THAT BAD YOU MIGHT DIE", gTextDisplayDur)
			).then_one_shot(
				delegate(){
					mInterfaceManager.set_for_CUTSCENE(
						delegate(){CUTSCENE_finished(CurrentPerformanceStat.CutsceneChangeSet);}, 
						CurrentPerformanceStat.CutsceneChangeSet
					);
				}
			);
			return;
		}
			
		
		chain = chain.then_one_shot(
			delegate(){
				
				mInterfaceManager.set_for_DEATH(CurrentPerformanceStat.Character)
				.then_one_shot(
						delegate()
						{
							transition_to_TRANSITION_play(new CharacterIndex("999"));
						}
				,0);
			}
		);
	}
	
	public void transition_to_GRAVE()
	{
		GS = NormalPlayGameState.GRAVE;

		mSunsetManager.set_for_GRAVE(mPerformanceStats, 
			delegate()
			{
				mManager.mTransitionCameraManager.fade_out_with_sound(mManager.restart_game);
			}
		);
	}


	public CharacterIndex[] available_choices(CharacterIndex age)
	{
		List<CharacterIndex> r = new List<CharacterIndex>();
		foreach(CharacterIndex e in age.get_neighbors())
		{
			if(mManager.mMetaManager.UnlockManager.is_unlocked(e) == 1)
			{
				r.Add(e);
			}
		}
		return r.ToArray();
	}
	
	public void transition_to_CHOICE()
	{
		GS = NormalPlayGameState.CHOICE;


		//TODO what happens when there is no future???
		CharacterIndex[] chars = available_choices(NGM.CurrentCharacterIndex.get_future_neighbor(0));
		mChoiceHelper.shuffle_and_set_choice_poses(chars.Length,mChoosingManager); 
		mChoosingManager.set_bb_choices(chars);
		//DELETE
		//mChoosingManager.set_bb_choice_bodies(NGM.CurrentCharacterIndex);

		
		mManager.mMusicManager.fade_in_extra_music("choiceMusic");

		//switch over to choice screen
		slide_image(null,mChoosingImage);

	}
	
	
	
	public void transition_to_PLAY()
	{
		GS = NormalPlayGameState.PREPLAY;
		TED.add_one_shot_event(delegate(){GS = NormalPlayGameState.PLAY; },GameConstants.preplayTime);
		
		//no target pose means we don't want a transparent body
		if(NGM.CurrentTargetPose == null)
			mManager.mTransparentBodyManager.transition_character_out();
		
	}
	
	public void transition_to_TRANSITION_play(CharacterIndex aNextCharacter)
	{
		
		float gDiffDisplayDur = 5f;
		float gAgeDisplayDur = 3f;
		GS = NormalPlayGameState.TRANSITION;

		var diffPhrases = new string[]{	"That's an easy choice. You should be able to manage that!", 
										"You made a normal choice. Show how good you are!", 
										"That's a hard one. Show your skills!", 
										"You made an extreme choice? Let's see if you survive!"
		};
		TED.add_event(
			aNextCharacter != CharacterIndex.sGrave 
			?
			mInterfaceManager.skippable_text_bubble_event(diffPhrases[NGM.CharacterHelper.Characters[aNextCharacter].Difficulty],gDiffDisplayDur)
			:
			delegate(float aTime){return true;}
		).then(
			
			NGM.CurrentCharacterIndex.LevelIndex < 7 //TODO
			?
			mInterfaceManager.skippable_text_bubble_event("You turn " + NGM.CurrentCharacterIndex.get_future_neighbor(0).Age,gAgeDisplayDur)
			:
			delegate(float aTime){return true;}
		,0).then_one_shot(
			//TODO before this, till mInterfaceManager to explain what choice the user just made
			//maybe play a sound "Too Easy" "Ok" "Hard" "That's Impossible!!"
			delegate(){
				mManager.mTransitionCameraManager.fade_out_with_sound(
					delegate(){
						if(aNextCharacter != CharacterIndex.sGrave) 
						{
							mManager.mAssetLoader.new_load_character(aNextCharacter.StringIdentifier,mManager.mCharacterBundleManager);
							slide_image(mSunsetImage,null);
						} else 
						{
							mManager.mTransitionCameraManager.fade_in_with_sound();
							slide_image(null,mSunsetImage,true);
							transition_to_GRAVE();
						}
					}
				);
			}
		);
	}
	
	
	
	//this function updates music and background and removes characters
	//pass in CurrentPerformanceStat.CutsceneChangeSet;
	public void load_CUTSCENE(NUPD.ChangeSet changes)
	{
		int changeIndex = -1;
		
        if(changes == null)
        {
            Debug.Log("could not find change in thershold with performance: " + CurrentPerformanceStat);
            changes = new NUPD.ChangeSet();
            changes.Changes.Add(new NUPD.ChangeSubSet() { Description = "No changes available!!" });
        } else changeIndex = changes.Index;
		
		//audio
		//TODO change index is no longe relevant
		if(changeIndex != -1)
		{
			//if(CurrentCharacterLoader.Images.cutsceneMusic.Count > changeIndex)
			if(NGM.CurrentCharacterLoader.Images.cutsceneMusic.Count > 1)
			{
				//mManager.mMusicManager.fade_in_cutscene_music(CurrentCharacterLoader.Images.cutsceneMusic[changeIndex]);
				//mManager.mMusicManager.play_cutscene_music(CurrentCharacterLoader.Images.cutsceneMusic[changeIndex]);
				mManager.mMusicManager.play_sound_effect(changes.UpperThreshold <= 0.5 ? "cutBad" : "cutGood");
				mManager.mMusicManager.play_cutscene_music(NGM.CurrentCharacterLoader.Images.cutsceneMusic[changes.UpperThreshold <= 0.5 ? 0 : 1]);
				//mManager.mMusicManager.fade_in_cutscene_music(CurrentCharacterLoader.Images.cutsceneMusic[changes.UpperThreshold <= 0.5 ? 0 : 1]);
			}
			else 
			{
				//Debug.Log("ERROR no music found for change index " + changeIndex + " only " + CurrentCharacterLoader.Images.cutsceneMusic.Count + " sounds");
				Debug.Log ("No cutscene music for " + NGM.CurrentCharacterIndex.StringIdentifier);
			}
		}
		
		//visuals
		mManager.mBodyManager.transition_character_out();
		mManager.mTransparentBodyManager.transition_character_out();

		if(CurrentPerformanceStat.Score < 0.5f && NGM.CurrentCharacterLoader.has_cutscene(1))
			mManager.mBackgroundManager.load_cutscene(1,NGM.CurrentCharacterLoader);
		else
			mManager.mBackgroundManager.load_cutscene(0,NGM.CurrentCharacterLoader);
	}
	
	
	
	
	//this is used by playq
	//make sure the next character is set before callincg this
	public void setup_next_poses(bool setNull = false)
	{
		if(setNull)
		{
			NGM.CurrentPoseAnimation = null;
			NGM.CurrentTargetPose = null;
		}
		else
		{
			int diff = CurrentPerformanceStat.Stats.Difficulty;
			NGM.CurrentPoseAnimation = new PerformanceType(mManager.mCharacterBundleManager.get_pose(NGM.CurrentCharacterIndex,diff),NGM.CurrentCharacterIndex);
			NGM.CurrentPoseAnimation.BPM = NGM.CharacterHelper.Characters[NGM.CurrentCharacterIndex].CharacterInfo.BPM;
			NGM.CurrentPoseAnimation.Offset = NGM.CharacterHelper.Characters[NGM.CurrentCharacterIndex].CharacterInfo.BPMOFFSET;
			
			//kind fast eh?
			if(diff == 0)
				NGM.CurrentPoseAnimation.set_change_time(4);
			else if(diff == 1)
				NGM.CurrentPoseAnimation.set_change_time(2);
			else if(diff == 2)
				NGM.CurrentPoseAnimation.set_change_time(1);
			else if(diff == 3)
				NGM.CurrentPoseAnimation.set_change_time(0.5f);
			
			NGM.CurrentTargetPose = NGM.CurrentPoseAnimation.get_pose(0);
			//mManager.mTransparentBodyManager.set_target_pose(CurrentTargetPose);
		}
	}

	public void slide_image(FlatElementImage cur, FlatElementImage next, bool instant = false)
	{

		//TODO set triggers to deactivate the surfaces, maybe not here.. for performance..
		if(next!=null)
		{
			next.HardPosition = mFlatCamera.Center + Vector3.right*next.BoundingBox.width;
			next.SoftPosition = mFlatCamera.Center;
			if(instant)
				next.HardPosition = next.SoftPosition;
		}
		if(cur != null)
		{
			cur.SoftPosition = mFlatCamera.Center - Vector3.right*cur.BoundingBox.width;
			if(instant)
				cur.HardPosition = cur.SoftPosition;
		}
	}


}
