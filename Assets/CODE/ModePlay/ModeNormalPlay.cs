using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class ModeNormalPlay
{
	
	public enum NormalPlayGameState
	{
		NONE,PREPLAY,PLAY,CUTSCENE,DEATH,PRECHOICE, CHOICE,TRANSITION,GRAVE
	}
	
	public NewGameManager NGM {get; set;}
	ManagerManager mManager {get; set;}
	
    public bool Paused{ get; set; }
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
	{ 
        get{ 
            var scoreSum = 0f;
            foreach(var e in mPerformanceStats)
                scoreSum += e.AdjustedScore;
            return scoreSum;
        } 
    }
	public bool IsFever
	{ 
		get{ 
			float perc = GameConstants.playFeverRequiredTime/GameConstants.playDefaultPlayTime;
			return PercentTimeCompletion > perc && CurrentPerformanceStat.last_score(perc)/perc > GameConstants.playFeverCutoff;
		}
	}

    public void reset_stats_and_difficulties()
    {
        //reset difficulties
        foreach (var e in CharacterIndex.sAllCharacters.Where(e=>e.LevelIndex > 0))
            mManager.mCharacterBundleManager.get_character_stat(e).Difficulty = 1;
        mManager.mCharacterBundleManager.fetus_difficulty_shuffle_hack();
        //reset stats
        mPerformanceStats.Clear();
    }
	
    //PLAY related stuff
    AdvancedGrading mGrading = new AdvancedGrading();
    AstronautPlay mAstronaut;	
	public float mLastGrade = 0.5f;
    BodyParticleHelper mParticles = new BodyParticleHelper();

	List<PerformanceStats> mPerformanceStats = new List<PerformanceStats>();
	ChoiceHelper mChoiceHelper;
	
    //managers
	public NewInterfaceManager mInterfaceManager = null;
	public SunsetManager mSunsetManager = null;
	public ChoosingManager mChoosingManager = null;
	public GiftManager mGiftManager = null;

    //images
	//FlatElementImage mInterfaceImage;
	FlatElementImage mSunsetImage;
	FlatElementImage mChoosingImage;
	
    //FLAT
	public FlatCameraManager mFlatCamera;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	






	//performance stat hacking
	public void set_last_performance_stat_to_character(CharacterIndex aChar)
	{
		//clear out older ages
		while(mPerformanceStats.Count > 0 && mPerformanceStats.Last().Character.LevelIndex >= aChar.LevelIndex)
		{
			mSunsetManager.remove_last_character();
			mPerformanceStats.RemoveAt(mPerformanceStats.Count - 1);
		}
		//fill in yonuger ages randomly
		while(mPerformanceStats.Count > 0 && mPerformanceStats.Last().Character.LevelIndex < aChar.LevelIndex - 1)
		{
			PerformanceStats stat = new PerformanceStats(new CharacterIndex(mPerformanceStats.Last().Character.LevelIndex + 1,Random.Range(0,4)));
            if (GameConstants.allPerfectOnSkip)
            {
                stat.update_score(0, 1);
                stat.update_score(1, 1);
            }
            else
            {
                stat.update_score(0, Random.value);
                stat.update_score(1, Random.value);
            }
			stat.Stats = mManager.mGameManager.CharacterHelper.Characters[stat.Character];
			mPerformanceStats.Add(stat);
			mSunsetManager.add_character(stat.Character,!stat.BadPerformance,false);
		}
		PerformanceStats realStat = new PerformanceStats(aChar);
		realStat.Stats = mManager.mGameManager.CharacterHelper.Characters[realStat.Character];
		mPerformanceStats.Add(realStat);

	}
	
	//skipping
	public bool DoSkipMultipleThisFrame
	{get; set;}
	public void SkipMultiple()
	{ DoSkipMultipleThisFrame = true; }
	public bool DoSkipSingleThisFrame
	{get; set;}
	public void SkipSingle()
	{ DoSkipSingleThisFrame = true; }


	
	public ModeNormalPlay(NewGameManager aNgm)
	{
		NGM = aNgm;
		mManager = aNgm.mManager;
		
		GS = NormalPlayGameState.NONE;
		TED = new TimedEventDistributor();
		mChoiceHelper = new ChoiceHelper();
        mAstronaut = new AstronautPlay(this);

		DoSkipSingleThisFrame = false;
		DoSkipMultipleThisFrame = false;
	}

    bool mIsInitialized = false;
	public void initialize()
	{
		
		mInterfaceManager = new NewInterfaceManager(mManager,this);
		mInterfaceManager.initialize();
		//mInterfaceManager.mFlatCamera.set_render_texture_mode(true);
		mInterfaceManager.setup_bb();
		
		mSunsetManager = new SunsetManager(mManager,this);
		mSunsetManager.initialize();
		mSunsetManager.mFlatCamera.set_render_texture_mode(true);
		mManager.mAssetLoader.new_load_asset_bundle("SUNSET",
            delegate(AssetBundle aBundle){
				mSunsetManager.sunset_loaded_callback(aBundle,"SUNSET");
			}
		);

		mGiftManager = new GiftManager(mManager,this);
		mGiftManager.initialize();
		mManager.mAssetLoader.new_load_asset_bundle("GIFT",
        	delegate(AssetBundle aBundle){
				mGiftManager.gift_loaded_callback(aBundle,"GIFT");
			}
		);
		
		mChoosingManager = new ChoosingManager(mManager,this);
		mChoosingManager.initialize();
		mChoosingManager.mFlatCamera.set_render_texture_mode(true);
		
		mFlatCamera = new FlatCameraManager(new Vector3(-23200,3500,0),10);
		mFlatCamera.Camera.depth = 100;
		mFlatCamera.fit_camera_to_screen();
		
		mSunsetImage = new FlatElementImage(mSunsetManager.mFlatCamera.RT,0);
        var hs = Vector3.one * mFlatCamera.Width/mSunsetImage.mImage.PixelDimension.x;
        //TODO extra space for rounded edges
        //hs.y += 600; //additional 300 pixels..
        mSunsetImage.HardScale = hs;
		mSunsetImage.HardPosition = mFlatCamera.get_point(Vector3.zero) + Vector3.up*mSunsetImage.BoundingBox.height;
		//mSunsetImage.HardShader = mManager.mReferences.mRenderTextureShader;
		mSunsetImage.PositionInterpolationMinLimit = 10; //so it doesn't take forever to entirely cover the image underneath
		mElement.Add(mSunsetImage);
		
		
		mChoosingImage = new FlatElementImage(mChoosingManager.mFlatCamera.RT,1);
		mChoosingImage.HardScale = Vector3.one * mFlatCamera.Width/mChoosingImage.mImage.PixelDimension.x;
		mChoosingImage.HardPosition = mFlatCamera.get_point(Vector3.zero) + Vector3.right*mChoosingImage.BoundingBox.width;
		//mChoosingImage.HardShader = mManager.mReferences.mRenderTextureShader;
		mElement.Add(mChoosingImage);
		
		
		//this is silly, we forec the interface image to appear above everything else pretty much just so we can have text bubbles show above everything
		//kind of a dumb way to do it oh well.
		/*
		mInterfaceImage = new FlatElementImage(mInterfaceManager.mFlatCamera.RT,1);
		mInterfaceImage.HardScale = Vector3.one * mFlatCamera.Width/mInterfaceImage.mImage.PixelDimension.x;
		mInterfaceImage.HardPosition = mFlatCamera.get_point(Vector3.zero);
		mElement.Add(mInterfaceImage);*/

        mIsInitialized = true;
	}

    public void reset()
    {
        GS = NormalPlayGameState.NONE;
        mSunsetManager.reset_sunset();
        mInterfaceManager.reset();
        mAstronaut.finish_astro();

        //hope this is ok :O
        TED.clear_events();

        //clear out characters
        while (mPerformanceStats.Count > 0)
        {
            mSunsetManager.remove_last_character();
            mPerformanceStats.RemoveAt(mPerformanceStats.Count - 1);
        }

        //move the images out of the way
        slide_image(mFlatCamera, mChoosingImage, null, false, true);
        slide_image(mFlatCamera, mSunsetImage, null, false, true);
    }

    //this function is duplicated in several files. I would have used refs to TED and mElement but I can't use ref parameters inside of an anonymous function
    public void clear_TED_and_fade_out_bubbles()
    {
        TED.clear_events();
        foreach (var e in mElement)
        {
            var elt = e;
            if (elt.PrimaryGameObject.name == "genPOPUPTEXT")
            {
                TED.add_one_shot_event(
                    delegate()
                    {
                        elt.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0);
                    }
                ).then_one_shot(
                    delegate()
                    {
                        elt.destroy();
                        mElement.Remove(elt);
                    }
                , 5);
            }
        }
    }


	public void initialize_game_with_character(CharacterIndex aChar)
	{
		//load the character
		mManager.mAssetLoader.new_load_character(aChar.StringIdentifier,mManager.mCharacterBundleManager);
        GS = NormalPlayGameState.NONE;

        if(!mIsInitialized)
		    initialize();

	}
	
	public void set_time_for_PLAY(float aTime)
	{
		TimeRemaining = aTime;
		TimeTotal = aTime;
	}
	
	public void character_loaded()
	{
        //this means we are start/restarting the game
        if(NGM.CurrentCharacterLoader.Character == CharacterIndex.sFetus)
            reset_stats_and_difficulties();

        //reveal the character with sound
        mManager.mMusicManager.play_sound_effect("transitionOut");
        slide_image(mFlatCamera, mSunsetImage, null, false); 

		//we use the hack version instead that allows us to skip characters
		set_last_performance_stat_to_character(NGM.CurrentCharacterIndex);

		//mPerformanceStats.Add(new PerformanceStats(NGM.CurrentCharacterIndex));
		//CurrentPerformanceStat.Stats = NGM.CharacterHelper.Characters[NGM.CurrentCharacterIndex];

		mInterfaceManager.begin_new_character(CurrentPerformanceStat);
		//set particle color
		mParticles.mParticles.set_emitter_particle_color(mManager.mCharacterBundleManager.get_character_stat(NGM.CurrentCharacterIndex).CharacterInfo.CharacterOutlineColor/2f,2);
		if(NGM.CurrentCharacterLoader.Character != CharacterIndex.sFetus)
			mGiftManager.add_character(NGM.CurrentCharacterLoader.Character);
		switch(NGM.CurrentCharacterLoader.Name)
		{
			case "0-1":	
				set_time_for_PLAY(999999f);
				setup_next_poses(true);
				transition_to_PLAY();
				float gTextDisplayDur = 4;
				NGM.CurrentTargetPose = null;

                System.Action transPose = delegate()
                {
                    NGM.CurrentTargetPose = mManager.mReferences.mCheapPose.to_pose();
                    mManager.mTransparentBodyManager.set_target_pose(NGM.CurrentTargetPose);
                    mManager.mTransparentBodyManager.transition_character_in(mManager.mCharacterBundleManager.get_character_stat(NGM.CurrentCharacterIndex).CharacterInfo.CharacterOutlineColor);
                };

                if (mManager.mMetaManager.UnlockManager.mUnlocked.numberGamesPlayed == 0 || GameConstants.TUTORIAL_ONCE != true){
                    TED.add_event(
                        mInterfaceManager.skippable_text_bubble_event(GameStrings.GetString("MNPfetus1"), gTextDisplayDur),
                    3).then_one_shot(
                        transPose,
                    0).then(
                        mInterfaceManager.skippable_text_bubble_event(GameStrings.GetString("MNPfetus2"), gTextDisplayDur),
                    1.5f);
                }
                else{
                    TED.add_one_shot_event(transPose,2);
                }

				break;
			case "110":
				set_time_for_PLAY(GameConstants.playAstronautPlayTime); //astronaut scene is shorter
				setup_next_poses(true); //astronaut scene has no transparent pose
                mAstronaut.start_astro();
                mManager.mBackgroundManager.hide_bg_and_fg_elements(true);
                mInterfaceManager.hide_interface();
				transition_to_PLAY();
				TED.add_event(
                    mInterfaceManager.skippable_text_bubble_event(GameStrings.GetString("MNPfetus3"), 4),
				1);
                CurrentPerformanceStat.update_score(0, 1); //give astronaut perfect score
                CurrentPerformanceStat.update_score(1, 1);	
				break;
			default:
				set_time_for_PLAY(GameConstants.playDefaultPlayTime);
				setup_next_poses(false);
				transition_to_PLAY();
				break;
		}
	}
	
	public static void draw_render_texture(FlatCameraManager aCam)
	{
		//aCam.Camera.enabled = true;
		RenderTexture.active = aCam.RT;
		aCam.Camera.targetTexture = aCam.RT;
		aCam.Camera.backgroundColor = new Color(1,1,1,0);
		aCam.Camera.clearFlags = CameraClearFlags.SolidColor;
		//aCam.Camera.DoClear();
		aCam.Camera.Render();
		RenderTexture.active = null;
		//aCam.Camera.enabled = false;
	}
	
	public void update()
	{
		//cheater keys for skipping
        if(KeyMan.GetKeyDown("HardSkip"))
			DoSkipMultipleThisFrame = true;
        if(KeyMan.GetKeyDown("SoftSkip"))
			DoSkipSingleThisFrame = true;

        //TODO not sure why I put this here but this most most def. does not work
		//if(Input.GetKeyDown(KeyCode.Alpha8))
			//transition_to_TRANSITION_play(new CharacterIndex("999"));
			//mManager.mTransparentBodyManager.transition_character_in(GameConstants.UiWhiteTransparent);


        //handle cheater keys for choosing
		if(GS == NormalPlayGameState.CHOICE)
		{
			//cheater keys for difficulty
			if(
				NGM.CurrentCharacterIndex.LevelIndex < 7 &&
				(Input.GetKeyDown(KeyCode.Q) ||
			   Input.GetKeyDown(KeyCode.W) ||
			   Input.GetKeyDown(KeyCode.E) ||
			   Input.GetKeyDown(KeyCode.R)))
			{
				if(Input.GetKeyDown(KeyCode.Q))
					NGM.CharacterHelper.Characters[NGM.CurrentCharacterIndex.get_future_neighbor(0)].Difficulty += 1;
				if(Input.GetKeyDown(KeyCode.W))
					NGM.CharacterHelper.Characters[NGM.CurrentCharacterIndex.get_future_neighbor(1)].Difficulty += 1;
				if(Input.GetKeyDown(KeyCode.E))
					NGM.CharacterHelper.Characters[NGM.CurrentCharacterIndex.get_future_neighbor(2)].Difficulty += 1;
				if(Input.GetKeyDown(KeyCode.R))
					NGM.CharacterHelper.Characters[NGM.CurrentCharacterIndex.get_future_neighbor(3)].Difficulty += 1;
				for(int i = 0; i < 4; i++)
					NGM.CharacterHelper.Characters[NGM.CurrentCharacterIndex.get_future_neighbor(i)].Difficulty %= 4;
				mChoosingManager.set_bb_choices(NGM.CurrentCharacterIndex.get_future_neighbor(0).get_neighbors());
			}

			//KeyCode[] levelKeys = new KeyCode[]{KeyCode.Q,KeyCode.W,KeyCode.E,KeyCode.R,KeyCode.T,KeyCode.Z,KeyCode.U,KeyCode.I};
			//KeyCode[] choiceKeys = new KeyCode[]{KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3,KeyCode.Alpha4};
            string[] levelKeys = new string[]{"lvl1","lvl2","lvl3","lvl4","lvl5","lvl6","lvl7","lvl8"};
            string[] choiceKeys = new string[]{"Choice1","Choice2","Choice3","Choice4"};
			for(int i=0; i < levelKeys.Length; i++)
			{
				if(KeyMan.GetKey(levelKeys[i]))
				{
					for(int j=0; j < choiceKeys.Length; j++)
					{
						if(KeyMan.GetKeyDown(choiceKeys[j]))
						{
                            slide_image(mFlatCamera, mChoosingImage, null);
                            slide_image(mFlatCamera, mSunsetImage, null, false);
							mManager.mMusicManager.fade_out_extra_music();
							mManager.mMusicManager.fade_out(0);
							mManager.mAssetLoader.new_load_character((new CharacterIndex(i+1,j)).StringIdentifier,mManager.mCharacterBundleManager);

						}
					}
				}
			}
		}



		mInterfaceManager.Update();
		mSunsetManager.update();
		mChoosingManager.update();
		mGiftManager.update();

		//TODO only draw if necessary
		draw_render_texture(mSunsetManager.mFlatCamera);
		draw_render_texture(mChoosingManager.mFlatCamera);
		//draw_render_texture(mInterfaceManager.mFlatCamera);
		
		mFlatCamera.update(Time.deltaTime);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);     
		

		if(GS == NormalPlayGameState.PLAY)
		{		
			//if(Input.GetKeyDown(KeyCode.P))
			//	mParticles.create_particles(mGrading);
			update_PLAY();
            if(KeyMan.GetKeyDown("HardSkip"))
				TimeRemaining = -5; //0;
		}
		else if(GS == NormalPlayGameState.CHOICE) 
			update_CHOICE();
		
		mParticles.update(Time.deltaTime);
		TED.update(Time.deltaTime);
		
	
		//hacks
		if(DoSkipMultipleThisFrame)
		{
			mInterfaceManager.skip_cutscene();

			mSunsetManager.skip_grave();
			
			//grave skipping lul
			DoSkipMultipleThisFrame = false;
		}
		
		if(DoSkipSingleThisFrame)
			DoSkipSingleThisFrame = false;
	}
	
	
	
	public void update_PLAY()
	{
        TimeRemaining -= KeyMan.GetKey("Fast") ? Time.deltaTime * 5 : Time.deltaTime;
		
		if(NGM.CurrentPose != null) //this should never happen but just in case
		{
            if(KeyMan.GetKey("Perfect"))
                mManager.mBodyManager.set_target_pose(NGM.CurrentTargetPose);
			else
                mManager.mBodyManager.set_target_pose(NGM.CurrentPose);
				
		}
		
		
		//this basically means we aren't 0 or 100 or 999
		if (NGM.CurrentPoseAnimation != null && NGM.CurrentCharacterIndex.LevelIndex != 0)
        {
            NGM.CurrentTargetPose = NGM.CurrentPoseAnimation.get_pose(Time.time);
            mManager.mTransparentBodyManager.set_target_pose(NGM.CurrentTargetPose);

            mGrading.update(mManager.mBodyManager.get_current_pose(), mManager.mTransparentBodyManager.get_current_pose());
            float grade = ProGrading.grade_pose(mManager.mBodyManager.get_current_pose(), mManager.mTransparentBodyManager.get_current_pose());
            grade = ProGrading.grade_to_perfect(grade);

            //old smooth grading
            /*
			float newGrade = mLastGrade*0.95f + grade*0.05f;
			if(newGrade < mLastGrade)
				mLastGrade = Mathf.Max(newGrade,mLastGrade - Time.deltaTime/6f);
			else mLastGrade = newGrade;
			grade = mLastGrade;*/

            //new smooth grading, this version gives grace to sudden drops in performance
            if (grade < mLastGrade)
            {
                float newGrade = mLastGrade * 0.95f + grade * 0.05f;
                if (newGrade < mLastGrade)
                    grade = Mathf.Max(newGrade, mLastGrade - Time.deltaTime / 2f);
                else
                    grade = newGrade;
            }
            mLastGrade = grade;

            if (PercentTimeCompletion > 0.01f)
            {
                bool switchEffect = false;
                mParticles.create_particles(mGrading, true);
                if(GameConstants.NEW_POSE_SWITCHING)
                {
                    //TODO test if switch conditions are right
                        //TODO switch
                }
                else
                {
                    if (NGM.CurrentPoseAnimation.does_pose_change_precoginitive(Time.time, Time.deltaTime, 0.07f))
                    {
                        switchEffect = true;
                    }

                }

                if(switchEffect)
                {
                    mGiftManager.capture_player();
                    mParticles.create_particles(mGrading);
                    if (grade > GameConstants.playSuperCutoff && IsFever)
                        mManager.mMusicManager.play_sound_effect("pose5", 0.6f);
                    else
                        mManager.mMusicManager.play_sound_effect("pose" + Mathf.Clamp((int)(5 * grade), 0, 4), 0.8f);
                }
            }
			


			
            if (TimeRemaining > 0) //insurance, something funny could happen if music runs slightly longer than it should.
                CurrentPerformanceStat.update_score(PercentTimeCompletion, grade);			
			
            //mManager.mCameraManager.set_camera_effects(grade);
            //update score
            mInterfaceManager.update_bb_score(TotalScore);	
        } else if (NGM.CurrentCharacterIndex.LevelIndex == 0 && true) //fetus
        {
            if (NGM.CurrentTargetPose != null)
            {
                mManager.mTransparentBodyManager.set_target_pose(NGM.CurrentTargetPose);
                float grade = ProGrading.grade_pose(mManager.mBodyManager.get_current_pose(), NGM.CurrentTargetPose); //should be mManager.mTransparentBodyManager.get_current_pose() but it does not matter
                grade = ProGrading.grade_to_perfect(grade);
				
                //this is a total hack, but we don't use mTotalScore for the fetus anyways
                FieldInfo scoreProp = typeof(PerformanceStats).GetField("mTotalScore", BindingFlags.NonPublic | BindingFlags.Instance);
                float oldGrade = (float)scoreProp.GetValue(CurrentPerformanceStat);
                float newGrade = oldGrade * 0.93f + grade * 0.07f;
                scoreProp.SetValue(CurrentPerformanceStat, newGrade);
                if (newGrade > GameConstants.playFetusPassThreshold)
                {
                    //this may or may not work depending on which update gets called first
                    SkipSingle();
                    scoreProp.SetValue(CurrentPerformanceStat, 0);
                    mManager.mMusicManager.play_sound_effect("cutGood");
                    TimeRemaining = 0;
                }
            }
        } 
        else if (NGM.CurrentCharacterIndex == CharacterIndex.sOneHundred)
        {
            mAstronaut.update_astro();
        }
        else
			CurrentPerformanceStat.update_score(PercentTimeCompletion,0.5f);
		
		//warning
		if (NGM.CurrentPoseAnimation != null && NGM.CurrentCharacterIndex.LevelIndex != 0)
		{
			float perc = 3f/GameConstants.playDefaultPlayTime;
			if(PercentTimeCompletion > 0.25f && CurrentPerformanceStat.last_score(perc)/(perc) < 0.2f)
				mInterfaceManager.enable_warning_text(true);
			else 
				mInterfaceManager.enable_warning_text(false);
		}
		
		//make sure music is finished too!
		//if((TimeRemaining <= 0 && !mManager.mMusicManager.IsMusicSourcePlaying) || TimeRemaining < -4) //but don't wait too long
		if(TimeRemaining <= 0)
		{
			CurrentPerformanceStat.Finished = true;
			mInterfaceManager.enable_warning_text(false);
			transition_to_CUTSCENE();
			
			//if we don't want fetus to have a cutscene use this
			//if(CurrentPerformanceStat.Character.Index != 0)
			//	transition_to_CUTSCENE();
			//else transition_to_CHOICE();

            //handle astronaut
            //note maybe we want to use physics for cutscene as well in which case we should move this into transition_to_CUTSCENE
            if(NGM.CurrentCharacterIndex == CharacterIndex.sOneHundred)
                mAstronaut.finish_astro();
			
			return;
		}
		
		//if we don't want the music to play during the cutscenes and whatont...
		//if(GS != NormalPlayGameState.PLAY)
		//	mManager.mMusicManager.fade_out();
		
		
		
		//early death
		bool die = false;
		die |= KeyMan.GetKeyDown ("Die");
		//if (NGM.CurrentPoseAnimation != null && mManager.mZigManager.has_user() && NGM.CurrentCharacterIndex.LevelIndex != 0)
		if (NGM.CurrentPoseAnimation != null && mManager.mZigManager.is_reader_connected() == 2 && NGM.CurrentCharacterIndex.LevelIndex != 0)
		{
			if(PercentTimeCompletion > 0.35f)
			{
				float perc = GameConstants.deathRequiredTime/GameConstants.playDefaultPlayTime;
				if(CurrentPerformanceStat.last_score(perc)/perc < GameConstants.deathPerformanceThreshold)
					die |= true;
			}
		}
		if(die && NGM.CurrentCharacterIndex != CharacterIndex.sOneHundred) //can't die as astronaut, even if we want to
		{
			if(NGM.CurrentCharacterIndex != CharacterIndex.sFetus) //this only happens if we try and force die on fetus
				mGiftManager.capture_player();
			CurrentPerformanceStat.Finished = true;
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
            slide_image(mFlatCamera, mChoosingImage, null);
			//mSunsetManager.fade_characters(false);
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
			changes,
            space_camp_final_exam()
		);
		
	}

    bool space_camp_final_exam_internal()
    {
        ManagerManager.Log("testing " + mPerformanceStats.Count() + " chars");
        foreach (var e in mPerformanceStats)
        {
            ManagerManager.Log(e.Character.StringIdentifier + " " + e.Score);
        }
        ManagerManager.Log("spacecamp final exam " + mPerformanceStats.Where(e => e.Character != CharacterIndex.sFetus).Where(e => e.Score < GameConstants.astronautCutoff).Count());
        return mPerformanceStats.Where(e=>e.Character != CharacterIndex.sFetus).Where(e=>e.Score < GameConstants.astronautCutoff).Count() == 0;
    }

    bool space_camp_final_exam()
    {
        return GameConstants.showAstronaut
            && CurrentPerformanceStat.Character.LevelIndex == 7;
    }
	
	public void CUTSCENE_finished(NUPD.ChangeSet changes = null)
	{
		TED.add_one_shot_event(
			delegate() 
			{	 
                mManager.mMusicManager.fade_out(3); //fadeout whataver is playing, either the cutscene music or the character music (which plays after cutscene music finishes
				//apply all the diff changes again (in case we skipped and tehy werent applied duringcutscene)
				if(changes != null){
			        foreach (var e in changes.Changes)
			        {
			            CharIndexContainerInt diffChanges = e.Changes;
			            //string changeMsg = e.Description;
						foreach(CharacterIndex cchar in CharacterIndex.sAllCharacters)
							if(diffChanges[cchar] != 0)
			                	mManager.mGameManager.change_character_difficulty(cchar, diffChanges[cchar]);
			        }
				}
			}
		,0).then_one_shot(
			delegate() 
			{	 
				if(CurrentPerformanceStat.Character.LevelIndex > 6) //if natural death :)
				{
					if(space_camp_final_exam())
					{
                        transition_to_ASTRONAUT();
					}
                    else{
    					//natural death
    					mInterfaceManager.set_for_DEATH(CurrentPerformanceStat.Character)
    						.then_one_shot(
    							delegate()
    							{
    								transition_to_TRANSITION_play(new CharacterIndex("999"));
    							}
    						,0);
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
		if(NGM.CurrentCharacterIndex.LevelIndex == 7) //no retry at age 85
			firstDeath = false;
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
					mManager.mBodyManager.transition_character_out();
					mManager.mTransparentBodyManager.transition_character_out();
					mManager.mMusicManager.play_sound_effect("cutDie");
					mManager.mMusicManager.play_cutscene_music(NGM.CurrentCharacterLoader.Images.deathMusic);
					if(NGM.CurrentCharacterLoader.has_cutscene(4))
						mManager.mBackgroundManager.load_cutscene(4,NGM.CurrentCharacterLoader);

                    //CAN DELETE
                    //else if (NGM.DeathCharacter != null){mManager.mBackgroundManager.load_cutscene(4, NGM.DeathCharacter);}
				}
			);
		}
		
		//BAD PERFORMANCE
		chain = chain.then(
            mInterfaceManager.skippable_text_bubble_event(firstDeath ? GameStrings.GetString("MNPdeath1") : GameStrings.GetString("MNPdeath2"), gTextDisplayDur)
		,2);

		if(!firstDeath && NGM.CurrentLevel == 7)
		{
			chain = chain.then(
                mInterfaceManager.skippable_text_bubble_event(GameStrings.GetString("MNPdeath3"), gTextDisplayDur)
			,0).then(
                    mInterfaceManager.skippable_text_bubble_event(GameStrings.GetString("MNPdeath4"), gTextDisplayDur)
			,0);
		}
	
		
		//NEXT TIME YOU PERFORM THAT BAD YOU MIGHT DIE
		if(firstDeath) //if we haven't died previously
		{
            chain = chain.then(mInterfaceManager.skippable_text_bubble_event(GameStrings.GetString("MNPdeath5"), gTextDisplayDur),0);


			chain = chain.then_one_shot(
				delegate(){
					mInterfaceManager.set_for_CUTSCENE(
						delegate(){CUTSCENE_finished(CurrentPerformanceStat.CutsceneChangeSet);}, 
						CurrentPerformanceStat.CutsceneChangeSet,
                        false
					);
				}
			,0);
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

		//since cutscene does not play during horrible death, music does not get played so we start it up again here
		if(mManager.mMusicManager.MusicClip != NGM.CurrentCharacterLoader.Images.backgroundMusic)
		{
			mManager.mMusicManager.play_music(NGM.CurrentCharacterLoader.Images.backgroundMusic,0,true);	
			mManager.mMusicManager.fade_in(5,0.5f);
		}

        mManager.mMetaManager.UnlockManager.game_finished(mPerformanceStats);

        mSunsetManager.set_for_GRAVE(mPerformanceStats);

        mManager.GameEventDistributor("DEATH", new object[] { NGM.mModeNormalPlay.mPerformanceStats });
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


    public void transition_to_ASTRONAUT()
    {
        mSunsetManager.add_character(NGM.CurrentCharacterLoader.Character,!CurrentPerformanceStat.BadPerformance);
        slide_image(mFlatCamera, null, mSunsetImage, false);

        TED.add_event(
            delegate(float aTime){
            //mSunsetManager.fade_characters(true,true);
            mSunsetManager.set_sun();
            return true;
        }
        ,4).then(
                mSunsetManager.low_skippable_text_bubble_event(GameStrings.GetString("MNPastro"),4)
        ,4).then_one_shot(
            delegate(){
                transition_to_TRANSITION_play(CharacterIndex.sOneHundred);
            }
        ,0);
    }
	
	public void transition_to_CHOICE()
	{
		GS = NormalPlayGameState.PRECHOICE;

		mSunsetManager.add_character(NGM.CurrentCharacterLoader.Character,!CurrentPerformanceStat.BadPerformance);
        slide_image(mFlatCamera, null, mSunsetImage, false);
        CharacterIndex[] chars = available_choices(NGM.CurrentCharacterIndex.get_future_neighbor(0));
        mChoiceHelper.shuffle_and_set_choice_poses(chars.Length,mChoosingManager); 
        mChoosingManager.set_bb_choices(chars);

        if(!KeyMan.GetKey("HardSkip")) //if we are still holding the skip key at this point, go straight to choosing
		{
			//switch over to choice screen
			float gAgeDisplayDur = 7f;
			TED.add_event(
				delegate(float aTime){
					//mSunsetManager.fade_characters(true,true);
					mSunsetManager.set_sun();
					return true;
				}
			,5);
			TED.add_event(
				NGM.CurrentCharacterIndex == CharacterIndex.sFetus ?
                mSunsetManager.low_skippable_text_bubble_event(GameStrings.GetString("MNPtrans1"),gAgeDisplayDur) :
                mSunsetManager.low_skippable_text_bubble_event(GameStrings.GetString("MNPtrans2",NGM.CurrentCharacterIndex.get_future_neighbor(0).Age,gAgeDisplayDur)
			,2.5f).then_one_shot(
				delegate(){
                    slide_image(mFlatCamera, null, mChoosingImage);
					mManager.mMusicManager.fade_in_extra_music("choiceMusic");
					GS = NormalPlayGameState.CHOICE;
				}
			,0);
		}
		else
		{
			mSunsetManager.set_sun();
            slide_image(mFlatCamera, null, mChoosingImage);
			mManager.mMusicManager.fade_in_extra_music("choiceMusic");
			GS = NormalPlayGameState.CHOICE;
		}

	}
	
	
	
	public void transition_to_PLAY()
	{
		GS = NormalPlayGameState.PREPLAY;
		TED.add_one_shot_event(delegate(){GS = NormalPlayGameState.PLAY; },GameConstants.preplayTime);
		
		//no target pose means we don't want a transparent body
		if(NGM.CurrentTargetPose == null)
			mManager.mTransparentBodyManager.transition_character_out(true);
		
	}
	
	//TODO move to sunset
    //this gets called after choosing
    //this also gets called when you die/finish the game
    //also called when you get to age 110
	public void transition_to_TRANSITION_play(CharacterIndex aNextCharacter)
    {
        float gDiffDisplayDur = 4f;


		GS = NormalPlayGameState.TRANSITION;

        //TODO maybe play a sound "Too Easy" "Ok" "Hard" "That's Impossible!!"
        var diffPhrases = new string[]{	GameStrings.GetString("MNPdiff1"), 
            GameStrings.GetString("MNPdiff2"), 
            GameStrings.GetString("MNPdiff3"), 
            GameStrings.GetString("MNPdiff4")
		};

		TED.add_event(
			aNextCharacter.LevelIndex < 7 //if not grave or age 110 
			?
			mSunsetManager.low_skippable_text_bubble_event(diffPhrases [NGM.CharacterHelper.Characters [aNextCharacter].Difficulty], gDiffDisplayDur)
			:
			delegate(float aTime){return true;}
        ).then_one_shot(
            delegate()
            {
                if (aNextCharacter != CharacterIndex.sGrave){
                    mManager.mAssetLoader.new_load_character(aNextCharacter.StringIdentifier, mManager.mCharacterBundleManager);
                } else
                {
                    //This can probably stay here
                    //we just died, so add the last character we just played and set the sun
                    //normally this will get called right after you make a choice but if we're here that meeans we didn't make a choice because we died
                    mSunsetManager.add_character(NGM.CurrentCharacterLoader.Character, !CurrentPerformanceStat.BadPerformance, false);
                    mSunsetManager.set_sun();
                    slide_image(mFlatCamera, null, mSunsetImage, false, false);

                    if(NGM.CurrentCharacterIndex == CharacterIndex.sOneHundred)
                        mSunsetManager.ShowBackground = false;

                    transition_to_GRAVE();
                }
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
            Debug.Log("could not find change in threshold with performance: " + CurrentPerformanceStat);
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
                if(NGM.CurrentCharacterIndex != CharacterIndex.sOneHundred)
				    mManager.mMusicManager.play_sound_effect(CurrentPerformanceStat.BadPerformance ? "cutBad" : "cutGood");
				AudioClip cutsceneClip = NGM.CurrentCharacterLoader.Images.cutsceneMusic[CurrentPerformanceStat.BadPerformance ? 0 : 1];
				mManager.mMusicManager.play_cutscene_music(cutsceneClip);
				//mManager.mMusicManager.fade_in_cutscene_music(CurrentCharacterLoader.Images.cutsceneMusic[changes.UpperThreshold <= 0.5 ? 0 : 1]);

				TED.add_event(
					delegate(float aTime){
						if(mManager.mMusicManager.MusicClip == cutsceneClip)
						{
                            //if we're still in the cutscene and the cutscene music has stopped playing
                            if(!mManager.mMusicManager.IsMusicSourcePlaying && GS == NormalPlayGameState.CUTSCENE)
							{
								mManager.mMusicManager.play_music(NGM.CurrentCharacterLoader.Images.backgroundMusic,0,true);	
								mManager.mMusicManager.fade_in(5,0.2f);
								return true;
							}
						} else return true;
						return false;
					}
				,0);
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

        if (space_camp_final_exam())
            mManager.mBackgroundManager.hide_bg_and_fg_elements();
        else if (CurrentPerformanceStat.BadPerformance && NGM.CurrentCharacterLoader.has_cutscene(1) && NGM.CurrentCharacterIndex != CharacterIndex.sOneHundred) //if bad performance, has cutscene and is NOT astronaut
            mManager.mBackgroundManager.load_cutscene(1, NGM.CurrentCharacterLoader);
        else
            mManager.mBackgroundManager.load_cutscene(0, NGM.CurrentCharacterLoader);
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
			NGM.CurrentPoseAnimation.set_change_time(GameConstants.difficultyToChangeTime[diff]);

            if(GameConstants.NEW_POSE_SWITCHING)
            {
                NGM.CurrentPoseAnimation.ChangeTime = 10;
                NGM.CurrentPoseAnimation.PT = PerformanceType.PType.MANUAL;
            }

			
			NGM.CurrentTargetPose = NGM.CurrentPoseAnimation.get_pose(0);
			//mManager.mTransparentBodyManager.set_target_pose(CurrentTargetPose);
		}
	}

	public static void slide_image(FlatCameraManager aCam, FlatElementImage cur, FlatElementImage next, bool right = true, bool instant = false)
	{

		//TODO set triggers to deactivate the surfaces, maybe not here.. for performance..
		if(next!=null)
		{
            next.HardPosition = aCam.get_point(Vector3.zero) + (right ? Vector3.right * next.BoundingBox.width : Vector3.down * next.BoundingBox.height);
            next.SoftPosition = aCam.get_point(Vector3.zero);
			if(instant)
				next.HardPosition = next.SoftPosition;
		}
		if(cur != null)
		{
            cur.SoftPosition = aCam.get_point(Vector3.zero) - (right ? Vector3.right * cur.BoundingBox.width : Vector3.down * cur.BoundingBox.height);
			if(instant)
				cur.HardPosition = cur.SoftPosition;
		}
	}


}
