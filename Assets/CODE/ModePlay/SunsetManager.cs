using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SunsetManager 
{
	ManagerManager mManager;
	ModeNormalPlay mModeNormalPlay;
    public SunsetManager(ManagerManager aManager,ModeNormalPlay aNormalPlay)
	{
		mModeNormalPlay = aNormalPlay;
		mManager = aManager;
		IsLoaded = false;
	}

	public TimedEventDistributor TED { get; private set; }
    public FlatCameraManager mFlatCamera;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	UnlockAnnouncer mUnlockAnnouncer;
	
	public void initialize()
	{
		IsLoaded = false;
		TED = new TimedEventDistributor();
		mFlatCamera = new FlatCameraManager(new Vector3(10000, -3000, 0), 10);
		mFlatCamera.fit_camera_to_game();
		mUnlockAnnouncer = new UnlockAnnouncer(this);
	}
	

	CharacterLoader mLoader;

	FlatElementImage mBackground;
	FlatElementImage mStubbyHairyGrass;
	FlatElementImage mSun;
	FlatElementImage mLightRay;
	List<FlatElementImage> mCharacters = new List<FlatElementImage>();
	List<FlatElementImage> mDiffLabels = new List<FlatElementImage>();
	List<FlatElementImage> mScoreLabels = new List<FlatElementImage>();
	List<FlatElementText> mScoreTexts = new List<FlatElementText>();
	
	public bool IsLoaded{get; private set;}
	
    bool mShowBackground = true;
    public bool ShowBackground{ get { 
            return mShowBackground;
        } set {
            mShowBackground = value; 
            if(!mShowBackground)
            {
                mBackground.PrimaryGameObject.GetComponentInChildren<Renderer>().enabled = false;
                //mBackground.HardShader = mManager.mReferences.mXB1ClearShader;
            }
            else
            {
                mBackground.PrimaryGameObject.GetComponentInChildren<Renderer>().enabled = true;
                //mBackground.HardShader = mManager.mReferences.mDefaultCharacterShader;
            }
        } 
    }

	FlatElementImage construct_flat_image(string aName, int aDepth)
	{
		var sizing = mLoader.Sizes.find_static_element(aName);
        try{
    		var r = new FlatElementImage(mLoader.Images.staticElements[aName],sizing.Size,aDepth);
    		r.HardPosition = mFlatCamera.get_point(Vector3.zero) + sizing.Offset;
    		return r;
        }
        catch(System.Exception e)
        {
            Debug.Log("couldn't find " + aName);
            throw e;
        }
	}
	
	public void sunset_loaded_callback(AssetBundle aBundle, string aBundleName)
	{
		mLoader = new CharacterLoader();
        mLoader.complete_load_character(aBundle,aBundleName);

		mBackground = new FlatElementImage(mLoader.Images.background1,mLoader.Sizes.mBackSize,0);
		mBackground.HardPosition = mFlatCamera.get_point(0,0);
		mStubbyHairyGrass = construct_flat_image("BG-1",2);
		
		mElement.Add(mBackground);
		mElement.Add(mStubbyHairyGrass);

		var sun = mManager.mCharacterBundleManager.get_image("SUN");
		mSun = new FlatElementImage(sun.Image,sun.Data.Size,1);
		var light = mManager.mCharacterBundleManager.get_image("SHINE");
		mLightRay = new FlatElementImage(light.Image,light.Data.Size,1);
		mLightRay.HardPosition = mFlatCamera.get_point(0,-10000);
		mElement.Add(mSun);
		mElement.Add(mLightRay);

		//mManager.mCharacterBundleManager.add_bundle_to_unload(aBundle);
		IsLoaded = true;

		//prep the sky for fetus
		set_sun(-1, true);
		set_sky_color(0);
	}

	int char_to_list_index(CharacterIndex aIndex)
	{
		if(aIndex == CharacterIndex.sGrave)
			return mCharacters.Count - 1;
		int r = aIndex.LevelIndex - 1;
		if(r >= mCharacters.Count)
			return -1;
		return r;
	}

	//this needs to be called in order of characters created....
	public void show_score(CharacterIndex aIndex, int aScore, float showTime, float jiggleDelayTime = 0)
	{
		int ind = char_to_list_index(aIndex);
		FlatElementText scoreText = new FlatElementText(mManager.mNewRef.fatFont,40, aScore.ToString(), 21);
		var scoreBgImage = mManager.mCharacterBundleManager.get_image("SCORELABEL");
		FlatElementImage scoreBg = new FlatElementImage(scoreBgImage.Image,scoreBgImage.Data.Size,20);
		scoreBg.HardPosition = mFlatCamera.get_point(0,1.5f);

        //SLOPPY, restart will clear out all characters but the TED will still be running and calling this function which will crash here
        if(mCharacters.Count > ind)
		    scoreBg.HardPosition = mCharacters[ind].SoftPosition + new Vector3(0,400,0);

		scoreText.HardPosition = scoreBg.HardPosition;
		scoreText.SoftPosition = scoreBg.SoftPosition;
		scoreText.Text = ""+aScore;
		scoreText.PositionInterpolationMinLimit = 200f;
		scoreBg.PositionInterpolationMinLimit = 200f;
		scoreText.ColorInterpolationMinLimit = 2f;
		scoreBg.ColorInterpolationMinLimit = 2f;

		mScoreLabels.Add(scoreBg);
		mScoreTexts.Add(scoreText);
		mElement.Add(scoreBg);
		mElement.Add(scoreText);

		System.Func<FlatElementBase,float,bool> scoreJiggleDelegate = 
			delegate(FlatElementBase aBase, float aTime2) 
		{
			aTime2 -= jiggleDelayTime;
			if(aTime2 > 0)
			{
				aBase.mLocalRotation = Quaternion.AngleAxis(Mathf.Sin(aTime2*Mathf.PI*2*4)*15f,Vector3.forward);
				if(aTime2 >= 0.7f) 
				{
					aBase.mLocalRotation = Quaternion.identity;
					return true;
				}
			}
			return false;
		};
		scoreBg.Events.add_event(scoreJiggleDelegate,0.03f);
		scoreText.Events.add_event(scoreJiggleDelegate,0.03f);

		//play the counting sound
		TED.add_one_shot_event(delegate(){mManager.mMusicManager.play_sound_effect("counting");},jiggleDelayTime);




		TED.add_event(
			delegate(float aTime) {
				if(aTime > showTime)
				{
					scoreBg.SoftColor = GameConstants.UiWhiteTransparent;
					scoreText.SoftColor = GameConstants.UiWhiteTransparent;
					return true;
				}
				return false;
			}
		,0).then_one_shot( 
			delegate(){ 
				mElement.Remove(scoreBg);
				mElement.Remove(scoreText);
				scoreBg.destroy();
				scoreText.destroy();
			}
		,3);
	}

	public void set_sky_color(int aIndex)
	{
		int index = aIndex;
		float gTotalIndices = 8;
		float lambda = index/gTotalIndices;
		Color leftColor = new Color32(25,25,112,255);
		Color highColor = new Color32(135,206,235,255);
		Color rightColor = leftColor;
        {
            if (lambda < 0.5f)
                mBackground.HardColor = Color.Lerp(leftColor, highColor, lambda * 2) / 2f;
            else
                mBackground.HardColor = Color.Lerp(highColor, rightColor, (lambda - 0.5f) * 2) / 2f;
        }
        //CAN DELETE
        //mBackground.HardColor = mBackground.SoftColor = GameConstants.UiWhiteTransparent;
	}

	public void set_sun(int aIndex, bool hard = false)
	{
		int index = aIndex;
		
		float gTotalIndices = 8;
		float lambda = index/gTotalIndices;

		mSun.PositionInterpolationMaxLimit = 300;

		//linear arc thing
		Vector3 sunHigh = mFlatCamera.get_point(Vector3.zero) + new Vector3(0,950,0);
		Vector3 sunLow = mFlatCamera.get_point(Vector3.zero) + new Vector3(0,-200,0);
		mSun.SoftPosition = lambda <= 0.5f ? 
			lambda*2*(sunHigh) + (1-lambda*2)*(sunLow) : 
			(1-(lambda-0.5f)*2)*(sunHigh) + (lambda-0.5f)*2*(sunLow);
		//circle arc
		//float ttt = Mathf.PI*lambda;
		//mSun.SoftPosition = mFlatCamera.Center + new Vector3(50*Mathf.Cos(ttt),700*Mathf.Sin(ttt),0);

        if (aIndex == 8) //no sun if it's the astronaut
            mSun.SoftPosition = mFlatCamera.get_point(Vector3.zero) + new Vector3(0, -400, 0); 

		if(hard) mSun.HardPosition = mSun.SoftPosition;

		if(!hard)
			ManagerManager.Manager.mMusicManager.play_sound_effect("sunRises");
	}
	public void set_sun()
	{
		set_sun (mCharacters.Count);
	}
	
	public void add_character(CharacterIndex aChar, bool aGood, bool aShowScore = true)
	{
		Vector3 gDiffLabelOffset = new Vector3(-100,-350,0);
		if(aChar != CharacterIndex.sFetus)
	{
            string imgname = "SUNSET_"+aChar.StringIdentifier;
            if(aChar != CharacterIndex.sGrave)
                imgname += "-" + (aGood?"a":"b");
			var addMe = construct_flat_image(imgname,3+mCharacters.Count*2);
            //Debug.Log("adding character " + aChar.StringIdentifier + " " + aGood + " " + imgname);

			//special positioning for grave
			if(aChar == CharacterIndex.sGrave)
			{
                //put it at the last character plus one position
                CharacterIndex gravePosition = (new CharacterIndex(mCharacters.Count,0)).get_future_neighbor(0);

                if(mCharacters.Count == 7) //there is no astronaut dot yet TODO need lea to add astronaut dot
                    gravePosition = new CharacterIndex(9,0); 

                //weird...
                FlatElementImage posImg;
                if(gravePosition.LevelIndex <= 8)
				    posImg = construct_flat_image("SUNSET_"+gravePosition.StringIdentifier+"-a",0);
                else
                    posImg = construct_flat_image("SUNSET_"+gravePosition.StringIdentifier,0);

				addMe.HardPosition = posImg.SoftPosition;
				posImg.destroy();
			} else if(mCharacters.Count < 7) { //add diff label, note no difficulty for age 100
				string[] labelNames = new string[]{"label_easy","label_normal","label_hard","label_extreme"};
				var diffLabel = mManager.mCharacterBundleManager.get_image(labelNames[mManager.mGameManager.get_character_difficulty(aChar)]);
				FlatElementImage diffLabelImage = new FlatElementImage(diffLabel.Image,diffLabel.Data.Size,19);
				diffLabelImage.HardPosition = addMe.HardPosition + gDiffLabelOffset;
				mDiffLabels.Add(diffLabelImage);
				mElement.Add(diffLabelImage);
			}

			mCharacters.Add(addMe);
			mElement.Add(addMe);

			if(aShowScore)
				show_score(aChar,(int)mManager.mGameManager.mModeNormalPlay.CurrentPerformanceStat.AdjustedScore,4,1.2f);

			set_sky_color(mCharacters.Count);
		}
	}

	public void remove_last_character()
	{
		if(mCharacters.Count > 0)
		{
			if(mDiffLabels.Count == mCharacters.Count)
			{
				var rdl = mDiffLabels.Last();
				mElement.Remove(rdl);
				rdl.destroy();
				mDiffLabels.Remove(rdl);
			}
			var rc = mCharacters.Last();
			mElement.Remove(rc);
			rc.destroy();
			mCharacters.Remove(rc);
		}
	}

	public void update()
	{
		mFlatCamera.update(Time.deltaTime);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		TED.update(Time.deltaTime);
		mUnlockAnnouncer.update();
	}
	

	int mLastSunsetBubbleIndex = 0;
	//bubble types 0 - regular, 1 - long small, 2 - long big
	public PopupTextObject add_timed_text_bubble(string aMsg, float duration, float yRelOffset = 0, int bubbleType = 0)
	{
		PopupTextObject to = null;
		if(bubbleType == 0)
			to = new PopupTextObject(aMsg,30);
		else if(bubbleType == 1)
		{
			string[] bubbleNames = new string[]{"SUNSET_BUBBLE-0","SUNSET_BUBBLE-1","SUNSET_BUBBLE-2"};
			//TODO put in actual random bubbless
			var bubbleImg = mManager.mCharacterBundleManager.get_image(bubbleNames[mLastSunsetBubbleIndex]);
			mLastSunsetBubbleIndex = (mLastSunsetBubbleIndex+1)%3;
			to = new PopupTextObject(aMsg,30,bubbleImg.Image,bubbleImg.Data.Size);
		} else if(bubbleType == 2)
		{
			var bubbleImg = mManager.mCharacterBundleManager.get_image("SELECTION_BUBBLE");
			to = new PopupTextObject(aMsg,30,bubbleImg.Image,bubbleImg.Data.Size);
			//hack fix, don't do this
			to.HardText = aMsg;
		}
		to.HardPosition = mFlatCamera.get_random_point_off_camera(-2300);
		to.HardColor = GameConstants.UiWhiteTransparent;
		to.SoftColor = GameConstants.UiWhite;
		//to.set_text_color(GameConstants.UiWhiteTransparent,true);
		to.set_text_color(GameConstants.UiRed,true);
		//to.set_background_color(GameConstants.UiWhiteTransparent,true);
		to.set_background_color(GameConstants.UiPopupBubble,true);
		TED.add_event(
			delegate(float aTime)
			{
				to.SoftPosition = mFlatCamera.get_point(0,yRelOffset); //fly in
				//to.HardPosition = mFlatCamera.get_point(0.40f,yRelOffset); //cut in
				mElement.Add(to);
				return true;
			},
        0.1f).then( //bleah hacky delay
			delegate(float aTime)
			{
				if(aTime > duration) 
					return true;

				if(mModeNormalPlay.DoSkipSingleThisFrame)
				{
					mModeNormalPlay.DoSkipSingleThisFrame = false;
					return true;
				}
				return false;
			},
		0).then_one_shot(
			delegate()
			{
				//cutout
				//mElement.Remove(to);
				//to.destroy();
				
				//fadeout
				to.fade_out();  
			},
		0).then_one_shot(
			delegate()
			{
				//fadeout
				mElement.Remove(to);
				to.destroy();
			},
		2);
		return to;
	}

	public System.Func<float,bool> low_skippable_text_bubble_event(string aText, float displayDur)
	{
		return skippable_text_bubble_event(aText,displayDur,-0.6f,1);
	}

	public System.Func<float,bool> skippable_text_bubble_event(string aText, float displayDur,float yRelOffset = 0, int bubbleType = 0)
	{
		System.Func<float,bool> skip_del = null;
		PopupTextObject po = null;
		return delegate(float aTime){
			if(po == null)
			{
				po = add_timed_text_bubble(aText,displayDur,yRelOffset,bubbleType);
				skip_del = PopupTextObject.skip(displayDur,po);
			}
			return skip_del(aTime);
		};
	}

	public void fade_characters(bool fade = true, bool hard = false)
	{
		foreach(FlatElementBase e in 
		        mCharacters.Cast<FlatElementBase>()
		        .Concat(mDiffLabels.Cast<FlatElementBase>())
		        .Concat(mScoreLabels.Cast<FlatElementBase>())
		        .Concat(mScoreTexts.Cast<FlatElementBase>()))
		{
			if(hard)
				e.HardColor = fade ? GameConstants.UiWhiteTransparent : GameConstants.UiWhite;
			else
				e.SoftColor = fade ? GameConstants.UiWhiteTransparent : GameConstants.UiWhite;
			

		}
	}





	public void create_shine_over_character(CharacterIndex aIndex,bool positive, float duration)
	{
		int index = aIndex.LevelIndex - 1;
		var shineImage = mManager.mCharacterBundleManager.get_image("SHINE");
		FlatElementImage shine = new FlatElementImage(shineImage.Image,shineImage.Data.Size,3);
		Vector3 targetPos = mCharacters[index].SoftPosition + new Vector3(0,shine.BoundingBox.height/2-340,0);
		shine.HardPosition = targetPos + new Vector3(0,2000,0);
		shine.SoftPosition = targetPos;
		shine.HardColor = positive ? GameConstants.UiYellow : GameConstants.UiRed; 
		shine.PositionInterpolationMinLimit = 200;
		shine.ColorInterpolationMaxLimit = Mathf.Infinity;
		shine.ColorInterpolationMinLimit = 1f;
		mElement.Add(shine);
		TED.add_one_shot_event(
			delegate() {
				shine.SoftColor = GameConstants.UiWhiteTransparent;
			},
		duration).then_one_shot(
			delegate() {
				mElement.Remove(shine);
				shine.destroy();
			},
		3);
	}

	public void set_popup_color_for_cutscene_particles(PopupTextObject aPopup, bool aPositive)
	{
		Color useColor = (!aPositive) ? GameConstants.ParticleStreamEasy : GameConstants.ParticleStreamHard;
		if(aPositive)
			aPopup.set_text_color(GameConstants.UiWhite,true);
		aPopup.set_background_color(useColor,true);
	}
	
	public void skip_grave()
	{
		if(mGraveCompleteCb != null && mGraveChain != null)
		{
			TED.remove_event(mGraveChain);
			mGraveCompleteCb();
			mGraveChain = null;
			mGraveCompleteCb = null;
		}
	}
	
	//delegates needed for skipping cleanly
    List<FlatElementBase> graveCleanup = new List<FlatElementBase>();
	QuTimer mGraveChain = null;
    QuTimer mGraveCompleteChain = null;
	System.Action mGraveCompleteCb = null;
	public void set_for_GRAVE(List<PerformanceStats> aStats)
	{
		//timing vars
		float gIntroText = 4.5f;
		float gPreScoreCount = 0.03f;
		float gScoreCount = 0.2f;
		float gPostScoreCount = 0.07f;
		float gRestart = 29;

		//disable the depth warning
		mManager.mZigManager.ForceShow = 2;
		
		//add the gravestone to the scene
		add_character(CharacterIndex.sGrave,true,false);
		
		//remove the grave
		if(aStats.Last().Character.Age == 999)
			aStats.RemoveAt(aStats.Count-1);
		
		//add in fetus in case we skipped it in debug mode
		if(aStats.First().Character.Age != 0)
			aStats.Insert(0, new PerformanceStats(new CharacterIndex(0,0)));

		//fake it for testing...
		/* TODO DELETE move this into ModeNormalPlay on force kill
		mCharacters.Last().destroy(); //remove the grave
		mElement.Remove(mCharacters.Last());
		mCharacters.RemoveAt(mCharacters.Count -1);
		Random.seed = 123;
		for(int i = 0; i < 8; i++)
		{
			if(aStats.Last().Character.Age < (new CharacterIndex(i,0)).Age)
			{
				PerformanceStats stat = new PerformanceStats(new CharacterIndex(i,Random.Range(0,4)));
				stat.update_score(0,Random.value);
				stat.update_score(1,Random.value);
				stat.Stats = mManager.mGameManager.CharacterHelper.Characters[stat.Character];
				aStats.Add(stat);

				add_character(stat.Character,false);
			}
		}
		add_character(CharacterIndex.sGrave,false); //add the grave back in
		*/

		
		//this is all a hack to get the score to show up right...
		float scoreIncrementor = 0;
		FlatElementText finalScoreText = new FlatElementText(mManager.mNewRef.serifFont,70,"0",21);
		finalScoreText.HardColor = (GameConstants.UiGraveText);
		FlatElementText finalAgeText = new FlatElementText(mManager.mNewRef.serifFont,50,"0",21);
		float ageIncrementer = 0;
		finalAgeText.HardColor = (GameConstants.UiGraveText);
		//perfectPercent.Text = ((int)(100*aStats.Sum(e=>e.Stats.Perfect+1)/(float)(aStats.Count*3))).ToString() + "%";
		//TODO why this no work??
		finalAgeText.Text = "0";//aStats.Last().Character.Age.ToString();
		
		//hack to put things into bg camera
		foreach (Renderer f in finalScoreText.PrimaryGameObject.GetComponentsInChildren<Renderer>())
			f.gameObject.layer = 4;
		foreach (Renderer f in finalAgeText.PrimaryGameObject.GetComponentsInChildren<Renderer>())
			f.gameObject.layer = 4;
		//foreach (Renderer f in perfectEngraving.PrimaryGameObject.GetComponentsInChildren<Renderer>()) f.gameObject.layer = 4;
		
		Vector3 graveCenter = mCharacters[mCharacters.Count-1].HardPosition + new Vector3(0, 50, 0);
		finalScoreText.HardPosition = graveCenter + new Vector3(30,-180,0);
		finalAgeText.HardPosition = graveCenter + new Vector3(25,0,0);
		mElement.Add(finalScoreText);
		//mElement.Add(perfectEngraving);
		mElement.Add(finalAgeText);

        graveCleanup.Add(finalScoreText);
        graveCleanup.Add(finalAgeText);


		TimedEventDistributor.TimedEventChain chain = TED.empty_chain();

		chain = chain.then_one_shot(
			delegate {
				set_sun();
			}
		,0);
		chain = chain.then(
            low_skippable_text_bubble_event(GameStrings.GetString("SM1"),gIntroText),3);

		/*.then( //wait a little bit to let the fading finish
		    	low_skippable_text_bubble_event("HERE IS YOUR LIFE STORY",gIntroText)
		    );*/

		for(int i = 1; i < aStats.Count; i++)
		{
			PerformanceStats ps = aStats[i];
		

			chain = chain.then_one_shot(
				delegate() {
                    if(ps.Character != CharacterIndex.sOneHundred)
					    show_score(ps.Character,(int)ps.AdjustedScore,gPreScoreCount + gScoreCount + gPostScoreCount + 1.5f);
				}
			,gPreScoreCount).then(
				delegate(float aTime)
				{
					if(aTime < gScoreCount)
					{
						float displayScore = scoreIncrementor + (aTime/gScoreCount)*ps.AdjustedScore;
						float displayAge = ageIncrementer + (aTime/gScoreCount)*(ps.Character.Age-ageIncrementer);
						finalScoreText.Text = ""+(int)displayScore;
						finalAgeText.Text = ""+(int)displayAge;
					}
					if(aTime >  gScoreCount + gPostScoreCount)
					{
						scoreIncrementor += ps.AdjustedScore;
						ageIncrementer = ps.Character.Age;
						finalScoreText.Text = ""+(int)scoreIncrementor;
						finalAgeText.Text = ""+(int)ageIncrementer;
						return true;
					}
					return false;
				},
			0);
		}

		chain = chain.wait(2.5f);

		//CONNECTIONS
		for(int i = 1; i < aStats.Count; i++)
		{
			PerformanceStats ps = aStats[i];

			
			
			float gFirstConnectionText = 5f;
			float gConnectionText = 5f;
			float gPreParticle = 2f;
			
			
			//TODO grave connections
			CharIndexContainerString connections;

			//TODO 
			bool wasHard = ps.Stats.Difficulty > 1;
			if(wasHard)
				connections = mManager.mCharacterBundleManager.get_character_stat(ps.Character).CharacterInfo.HardConnections;
			else
				connections = mManager.mCharacterBundleManager.get_character_stat(ps.Character).CharacterInfo.EasyConnections;
			
			
			//for each connection, check if it is relevent to the currently looping character
			for(int j = 1; j < aStats.Count; j++)
			{
				var targetCharacter = aStats[j].Character;				//charcter we are connecting to
				var targetConnection = connections[targetCharacter];	//message
				if(targetConnection != null && targetConnection != "")
				{
					int accumChange = 0; //accum change is targetCharacters effect on the current character
					accumChange = aStats[j].CutsceneChangeSet.accumulative_changes()[ps.Character];

					if( (wasHard && accumChange > 0) || //if was hard and effect was positive (i.e. hard)
					   (!wasHard && accumChange < 0)) //if was easy and effect was negative (i.e. easy)
					{
						string [] conText = targetConnection.Replace("<S>","@").Split('@');
						PopupTextObject npo = null;
						chain = chain.then (
							delegate(float aTime)
							{
								if(npo == null)
								{
									npo = add_timed_text_bubble(conText[0],gFirstConnectionText + gConnectionText,-0.6f,1);
									set_popup_color_for_cutscene_particles(npo,wasHard);
									create_shine_over_character(targetCharacter,!wasHard,gFirstConnectionText + gConnectionText);
									TED.add_one_shot_event(
										delegate() {
											create_shine_over_character(ps.Character,!wasHard,gFirstConnectionText + gConnectionText - 0.3f);
										}
									,0.3f);
								}
								if(npo.IsDestroyed || aTime > gPreParticle) 
								{
									return true;
								}
								return false;
							}
						,0);
						
						System.Func<FlatElementBase,float,bool> jiggleDelegate = 
							delegate(FlatElementBase aBase, float aTime2) 
							{
								aBase.mLocalRotation = Quaternion.AngleAxis(Mathf.Sin(aTime2*Mathf.PI*2*8)*10f,Vector3.forward);
								if(aTime2 >= (gFirstConnectionText-gPreParticle)/4f) 
								{
									aBase.mLocalRotation = Quaternion.identity;
									return true;
								}
								return false;
							};

						chain = chain.then_one_shot(
							delegate()
							{
								if(npo != null)
								{
									mCharacters[char_to_list_index(targetCharacter)].Events.add_event(jiggleDelegate,0);
									ManagerManager.Manager.mMusicManager.play_sound_effect("graveShine");
								}
								//create the shine
								
							}
						).then_one_shot(
							delegate()
							{
								if(npo != null && !npo.IsDestroyed) //if we used softskip, npo could be destroyed at this point
								{
									npo.Text = conText.Last();
									mCharacters[char_to_list_index(ps.Character)].Events.add_event(jiggleDelegate,0);
									ManagerManager.Manager.mMusicManager.play_sound_effect("graveShine");
								}
							},
						gFirstConnectionText - gPreParticle).then (
							delegate(float aTime){
								if(npo.IsDestroyed || aTime > gConnectionText)
								{
									npo = null;
									return true;
								}
								return false;
							}
						);
					}
				}
			}
		}

		string deathSentence = "";
		if(aStats[aStats.Count-1].DeathTime != -1)
            deathSentence += GameStrings.GetString("SM2");
		else
            deathSentence += GameStrings.GetString("SM3");
		if (!aStats [aStats.Count - 1].Character.IsDescriptionAdjective)
           {
            if("aeiouAEIOU".IndexOf(aStats[aStats.Count-1].Character.Description[0]) >= 0)
                deathSentence += GameStrings.GetString("SM4");
            else
                deathSentence += GameStrings.GetString("SM5");
        }
		deathSentence += aStats[aStats.Count-1].Character.Description;

		chain = chain.then(
			low_skippable_text_bubble_event(deathSentence,gIntroText)
		,1);

       
		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
            UnlockRequirements.UnlockData unlockData;
            if (mManager.mMetaManager.UnlockManager.unlockedThisGame.TryGetValue(new UnlockRequirements.FakeCharIndex(e), out unlockData))
			{
				if(unlockData != null){
                    ManagerManager.Log("announcing unlock " + e.StringIdentifier);
					CharacterIndex ce = new CharacterIndex(e);
					chain = chain.then_one_shot(
						delegate(){
							mUnlockAnnouncer.announce_unlock(ce,unlockData);
						}
					,0).then(
						delegate(float aTime){
							return !mUnlockAnnouncer.IsAnnouncing;
						}
					,0);
				}
			}
		}

        //so we don't announce unlock again when we restart
        mManager.mMetaManager.UnlockManager.unlockedThisGame.Clear();


		
		
		if(GameConstants.showReward && aStats[aStats.Count-1].Character.LevelIndex >= 7)
		{
			FlatElementImage rewardImage = null;
			FlatElementImage rewardFrame = null;
			mModeNormalPlay.mGiftManager.set_background_for_render();
			chain = chain.then_one_shot(
				delegate()  {
				
				var frameImg = mManager.mCharacterBundleManager.get_image("GIFT_frame");
				rewardFrame = new FlatElementImage(frameImg.Image,frameImg.Data.Size,24);
				rewardImage = new FlatElementImage(mModeNormalPlay.mGiftManager.render_gift(0),new Vector2(2001,1128),23);
				
				//TODO play sound effect
				rewardImage.HardPosition = mFlatCamera.get_point(0,3);
				rewardFrame.HardPosition = rewardImage.HardPosition;
				rewardImage.SoftPosition = mFlatCamera.get_point(Vector3.zero) + new Vector3(0,150,0);
				rewardFrame.SoftPosition = rewardImage.SoftPosition + new Vector3(0,70,0);
				mElement.Add(rewardImage);
				mElement.Add(rewardFrame);
                graveCleanup.Add(rewardImage);
                graveCleanup.Add(rewardFrame);
				
				var subChain = TED.empty_chain().wait(4);
				if(mModeNormalPlay.mGiftManager.gift_count() > 0)
				{
					for(int i = 1; i < 100; i++)
					{
						int localIndex = i%mModeNormalPlay.mGiftManager.gift_count();
						subChain = subChain.then_one_shot(
							delegate(){
							//TODO sound effect
							mModeNormalPlay.mGiftManager.render_gift(localIndex);
						}
						,1f);
					}
				}
				
			}
			,2);
			//chain = chain.wait(6);
			//chain = chain.then(skippable_text_bubble_event("YOU ARE THE PERFECT WOMAN!",5,-0.8f,2),0);
		}


		
		//variables for credits animation..
		float lastTime = 0;
        FlatElementImage[] logos = null;
		//PopupTextObject gameOver = null;
		List<FlatElementText> creditsText = new List<FlatElementText>();
		float scrollSpeed = 820;

		mGraveCompleteCb = delegate()
		{
			Vector3 barYPosition = mFlatCamera.get_point(Vector3.zero) + new Vector3(0,-700,0);
            TED.add_one_shot_event(
                delegate()
                {
                    mManager.mMusicManager.fade_in_extra_music("creditsMusic");
                    mManager.mMusicManager.fade_out();
                    var imgData = mManager.mCharacterBundleManager.get_image("BAR");
                    var barImg = new FlatElementImage(imgData.Image, imgData.Data.Size, 24);
                    barImg.HardPosition = barYPosition + new Vector3(0, -1000, 0);
                    barImg.SoftPosition = barYPosition;
                    mElement.Add(barImg);
                    graveCleanup.Add(barImg);
                }
            , 0).then_one_shot(
                delegate()
                {

                    float lastXPosition = mFlatCamera.get_point(Vector3.zero).x - mFlatCamera.Width / 2 - 100;
                    int counter = 0;
                    foreach (string e in GameConstants.credits)
                    {
                        string val = e;
                        if(System.Text.RegularExpressions.Regex.IsMatch(e, @"^\d+$"))
                            val = GameStrings.GetString("GCcredits"+e);
                            
                        var text = new FlatElementText(mManager.mNewRef.genericFont, 70, val, 25);
                        float textWidth = text.BoundingBox.width;
                        text.HardColor = new Color(1, 1, 1, 1);
                        text.HardPosition = new Vector3(lastXPosition - textWidth / 2f, barYPosition.y, 0);
                        lastXPosition += -textWidth - 75;
                        creditsText.Add(text);
                        mElement.Add(text);
                        graveCleanup.Add(text);
                        counter++;
                    }

                    if(GameConstants.SHOW_LOGOS)
                    {
                        logos = new FlatElementImage[3];
                        lastXPosition += -200;
                        string[] imageNames = new string[] { "LOGO_FA", "LOGO_AI", "LOGO_GL" };
                        for (int i = 0; i < imageNames.Length; i++)
                        {
                            var imgData = mManager.mCharacterBundleManager.get_image(imageNames[i]);
                            var img = new FlatElementImage(imgData.Image, imgData.Data.Size, 25);
                            float imgWidth = img.BoundingBox.width;
                            img.HardPosition = new Vector3(lastXPosition - imgWidth / 2, barYPosition.y, 0);
                            lastXPosition += -img.BoundingBox.width / 2f - 500;
                            logos[i] = img;
                            mElement.Add(img);
                            graveCleanup.Add(img);
                        }
                    }

                }
            , 1).then_one_shot(
                delegate()
                {

                    /* this will fade everything out super slowly
                    List<FlatElementBase> graveItems = new List<FlatElementBase>(){finalScoreText,perfectPercent};
                    foreach(FlatElementBase e in 
                        mCharacters.Cast<FlatElementBase>()
                        .Concat(mDiffLabels.Cast<FlatElementBase>())
                        .Concat(mScoreLabels.Cast<FlatElementBase>())
                        .Concat(mScoreTexts.Cast<FlatElementBase>())
                        .Concat(graveItems.Cast<FlatElementBase>()))
                    {
                        e.ColorInterpolationLimit = 0.05f;
                        e.SoftColor = GameConstants.UiWhiteTransparent;
                    }*/
                }
            , 0).then(
                delegate(float aTime)
                {

                    //scroll contents down
                    Vector3 scroll = new Vector3(scrollSpeed * (aTime - lastTime), 0, 0);
                    foreach (FlatElementText e in creditsText)
                    {
                        e.SoftPosition = e.SoftPosition + scroll;
                    }

                    if(logos != null){
                        foreach (FlatElementImage e in logos)
                        {
                            e.SoftPosition = e.SoftPosition + scroll;
                        }
                    }

                    lastTime = aTime;
                    if (Input.GetKeyDown(KeyCode.Alpha0))
                        return true;

                    if (aTime > gRestart)
                        return true;
                    return false;
                }
            , 0).then_one_shot(
                delegate(){
                    mManager.mMusicManager.fade_out_extra_music();
                    mManager.restart_game();   
                }
            , 0);
            mGraveCompleteChain = TED.LastEventKeyAdded;
		};
		
		chain = chain.then_one_shot(
			delegate()
			{
				mGraveCompleteCb();
				mGraveCompleteCb = null;
				mGraveChain = null;
			}
		,1);
		
		mGraveChain = TED.LastEventKeyAdded;
		
	}

    public void reset_sunset()
    {
        //reset sunset background
        ShowBackground = true;
        set_sun(0, true);

        //remove characters
        while (mCharacters.Count > 0)
            remove_last_character();

        clear_TED_and_fade_out_bubbles();

        /*
        //clear out grave events
        if (mGraveChain != null)
        {
            TED.remove_event(mGraveChain);
            mGraveChain = null;
        }
        if (mGraveCompleteChain != null)
        {
            TED.remove_event(mGraveCompleteChain);
            mGraveCompleteChain = null;
        }
        mGraveCompleteCb = null;*/

        //cleanup credits
        foreach (var e in graveCleanup)
        {
            var elt = e;
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
            , 2);
        }
        graveCleanup.Clear();
    }

    //this function is duplicated in NewInterfaceManager.cs and SunsetManager.cs. I would have used refs to TED and mElement but I can't use ref parameters inside of an anonymous function
    void clear_TED_and_fade_out_bubbles()
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
                , 2);
            }
        }
    }

}
