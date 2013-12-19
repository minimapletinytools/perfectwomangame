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
	
	public void initialize()
	{
		IsLoaded = false;
		TED = new TimedEventDistributor();
		mFlatCamera = new FlatCameraManager(new Vector3(10000, -3000, 0), 10);
		mFlatCamera.fit_camera_to_game();
	}
	
	
	CharacterLoader mLoader;

	FlatElementImage mBackground;
	FlatElementImage mStubbyHairyGrass;
	FlatElementImage mSun;
	FlatElementImage mLightRay;
	List<FlatElementImage> mCharacters = new List<FlatElementImage>();
	List<FlatElementImage> mDiffLabels = new List<FlatElementImage>();
	
	public bool IsLoaded{get; private set;}
	
	
	FlatElementImage construct_flat_image(string aName, int aDepth)
	{
		var sizing = mLoader.Sizes.find_static_element(aName);
		var r = new FlatElementImage(mLoader.Images.staticElements[aName],sizing.Size,aDepth);
		r.HardPosition = mFlatCamera.Center + sizing.Offset;
		return r;
	}
	
	public void sunset_loaded_callback(AssetBundle aBundle, string aBundleName)
	{
		NewMenuReferenceBehaviour refs = mManager.mNewRef;
		
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

		mManager.mCharacterBundleManager.add_bundle_to_unload(aBundle);
		IsLoaded = true;

		set_sun(-2, true);
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
	
	public void show_score(CharacterIndex aIndex, int aScore, float showTime)
	{
		int ind = char_to_list_index(aIndex);
		FlatElementText scoreText = new FlatElementText(mManager.mNewRef.fatFont,40, aScore.ToString(), 21);
		var scoreBgImage = mManager.mCharacterBundleManager.get_image("SCORELABEL");
		FlatElementImage scoreBg = new FlatElementImage(scoreBgImage.Image,scoreBgImage.Data.Size,20);
		scoreBg.HardPosition = mFlatCamera.get_point(0,1.5f);
		scoreBg.SoftPosition = mCharacters[ind].SoftPosition + new Vector3(0,300,0);
		scoreText.HardPosition = scoreBg.HardPosition;
		scoreText.SoftPosition = scoreBg.SoftPosition;
		scoreText.Text = ""+aScore;
		mElement.Add(scoreBg);
		mElement.Add(scoreText);
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

	public void set_sun(int aIndex, bool hard = false)
	{
		int index = aIndex;
		
		float gTotalIndices = 6;
		float lambda = index/gTotalIndices;
		float ttt = Mathf.PI*lambda;
		mSun.PositionInterpolationMaxLimit = 300;

		//linear arc thing
		mSun.SoftPosition = lambda <= 0.5f ? 
			lambda*2*(mFlatCamera.Center + new Vector3(0,600,0)) + (1-lambda*2)*(mFlatCamera.Center + new Vector3(-50,0,0)) : 
			(1-(lambda-0.5f)*2)*(mFlatCamera.Center + new Vector3(0,500,0)) + (lambda-0.5f)*2*(mFlatCamera.Center + new Vector3(-50,0,0));
		//circle arc
		//mSun.SoftPosition = mFlatCamera.Center + new Vector3(50*Mathf.Cos(ttt),700*Mathf.Sin(ttt),0);


		if(hard) mSun.HardPosition = mSun.SoftPosition;

		mBackground.ColorInterpolationLimit = 0.5f;

		Color leftColor = new Color32(25,25,112,255);
		Color highColor = new Color32(135,206,235,255);
		Color rightColor = leftColor;
		if(lambda < 0.5f)
			mBackground.SoftColor = Color.Lerp(leftColor,highColor,lambda*2)/2f;
		else
			mBackground.SoftColor = Color.Lerp(highColor,rightColor,(lambda-0.5f)*2)/2f;
	}
	public void set_sun()
	{
		set_sun (mCharacters.Count);
	}
	
	public void add_character(CharacterIndex aChar, bool aShowScore = true)
	{
		Vector3 gDiffLabelOffset = new Vector3(-100,-350,0);
		if(aChar != CharacterIndex.sFetus)
		{
			var addMe = construct_flat_image("SUNSET_"+aChar.StringIdentifier,4 + mCharacters.Count);
			Debug.Log ("adding character " + aChar.StringIdentifier);

			//special positioning for grave
			if(aChar == CharacterIndex.sGrave)
			{
				var posImg = construct_flat_image("SUNSET_"+(new CharacterIndex(mCharacters.Count,0)).get_future_neighbor(0).StringIdentifier,0);
				addMe.HardPosition = posImg.SoftPosition;
				posImg.destroy();
			} else if(mCharacters.Count < 7) { //add diff label, note no difficulty for age 100
				string[] labelNames = new string[]{"label_easy","label_normal","label_hard","label_extreme"};
				var diffLabel = mManager.mCharacterBundleManager.get_image(labelNames[mManager.mGameManager.get_character_difficulty(aChar)]);
				FlatElementImage diffLabelImage = new FlatElementImage(diffLabel.Image,diffLabel.Data.Size,5);
				diffLabelImage.HardPosition = addMe.HardPosition + gDiffLabelOffset;
				mDiffLabels.Add(diffLabelImage);
				mElement.Add(diffLabelImage);
			}

			mCharacters.Add(addMe);
			mElement.Add(addMe);



			if(aShowScore)
				show_score(aChar,(int)mManager.mGameManager.mModeNormalPlay.CurrentPerformanceStat.AdjustedScore,13);
		}
	}

	public void update()
	{
		mFlatCamera.update(Time.deltaTime);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		TED.update(Time.deltaTime);
	}
	

	//bubble types 0 - regular, 1 - long small, 2 - long big
	public PopupTextObject add_timed_text_bubble(string aMsg, float duration, float yRelOffset = 0, int bubbleType = 0)
	{
		PopupTextObject to = null;
		if(bubbleType == 0)
			to = new PopupTextObject(aMsg,30);
		else if(bubbleType == 1)
		{
			var bubbleImg = mManager.mCharacterBundleManager.get_image("SUNSET_BUBBLE");
			to = new PopupTextObject(aMsg,30,bubbleImg.Image,bubbleImg.Data.Size);
		} else if(bubbleType == 2)
		{
			to = new PopupTextObject(aMsg,30); //TODO??
		}
		//to.HardPosition = random_position();
		to.HardColor = GameConstants.UiWhiteTransparent;
		to.SoftColor = GameConstants.UiWhite;
		to.set_text_color(GameConstants.UiWhiteTransparent,true);
		to.set_text_color(GameConstants.UiRed);
		to.set_background_color(GameConstants.UiWhiteTransparent,true);
		to.set_background_color(GameConstants.UiPopupBubble);
		TED.add_event(
			delegate(float aTime)
			{
				to.SoftPosition = mFlatCamera.get_point(0,yRelOffset); //fly in
				//to.HardPosition = mFlatCamera.get_point(0.40f,yRelOffset); //cut in
				mElement.Add(to);
				return true;
			},
        0).then(
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





	public void create_shine_over_character(CharacterIndex aIndex,bool positive, float duration)
	{
		//TODO positive/negative colors
		int index = aIndex.LevelIndex - 1;
		var shineImage = mManager.mCharacterBundleManager.get_image("SHINE");
		FlatElementImage shine = new FlatElementImage(shineImage.Image,shineImage.Data.Size,3);
		Vector3 targetPos = mCharacters[index].SoftPosition + new Vector3(0,shine.BoundingBox.height/2-300,0);
		shine.HardPosition = targetPos + new Vector3(0,2000,0);
		shine.SoftPosition = targetPos;
		shine.HardColor = positive ? GameConstants.UiGreen : GameConstants.UiRed; 
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
	QuTimer mGraveChain = null;
	System.Action mGraveCompleteCb = null;
	public void set_for_GRAVE(List<PerformanceStats> aStats, System.Action graveCompleteCb)
	{
		//timing vars
		float gIntroText = 4.5f;
		float gPreScoreCount = 0f;
		float gScoreCount = 0.7f;
		float gPostScoreCount = 1f;
		float gRestart = 65;

		//add the last character played to the scene (normally this gets added during choice)
		add_character(mModeNormalPlay.NGM.CurrentCharacterLoader.Character);

		//add the gravestone to the scene
		add_character(CharacterIndex.sGrave,false);
		
		//remove the grave
		if(aStats.Last().Character.Age == 999)
			aStats.RemoveAt(aStats.Count-1);
		
		//add in fetus in case we skipped it in debug mode
		if(aStats.First().Character.Age != 0)
			aStats.Insert(0, new PerformanceStats(new CharacterIndex(0,0)));

		//fake it for testing...
		mCharacters.Last().destroy(); //remove the grave
		mElement.Remove(mCharacters.Last());
		mCharacters.RemoveAt(mCharacters.Count -1);
		Random.seed = 344;
		for(int i = 0; i < 8; i++)
		{
			if(aStats.Last().Character.Age < (new CharacterIndex(i,0)).Age)
			{
				PerformanceStats stat = new PerformanceStats(new CharacterIndex(i,Random.Range(0,3)));
				stat.update_score(0,Random.value);
				stat.update_score(1,Random.value);
				stat.Stats = mManager.mGameManager.CharacterHelper.Characters[stat.Character];
				aStats.Add(stat);

				add_character(stat.Character,false);
			}
		}
		add_character(CharacterIndex.sGrave,false); //add the grave back in


		
		//this is all a hack to get the score to show up right...
		float scoreIncrementor = 0;
		FlatElementText finalScoreText = new FlatElementText(mManager.mNewRef.serifFont,70,"",10);
		finalScoreText.HardColor = (GameConstants.UiGraveText);
		//FlatElementImage perfectEngraving = new FlatElementImage(mManager.mNewRef.gravePerfectnessEngraving,10);
		FlatElementText perfectPercent = new FlatElementText(mManager.mNewRef.serifFont,40,"",11);
		float ageIncrementer = 0;
		perfectPercent.HardColor = (GameConstants.UiGraveText);
		//perfectPercent.Text = ((int)(100*aStats.Sum(e=>e.Stats.Perfect+1)/(float)(aStats.Count*3))).ToString() + "%";
		//TODO why this no work??
		perfectPercent.Text = aStats.Last().Character.Age.ToString();
		
		//hack to put things into bg camera
		foreach (Renderer f in finalScoreText.PrimaryGameObject.GetComponentsInChildren<Renderer>())
			f.gameObject.layer = 4;
		foreach (Renderer f in perfectPercent.PrimaryGameObject.GetComponentsInChildren<Renderer>())
			f.gameObject.layer = 4;
		//foreach (Renderer f in perfectEngraving.PrimaryGameObject.GetComponentsInChildren<Renderer>()) f.gameObject.layer = 4;
		
		Vector3 graveCenter = mCharacters[mCharacters.Count-1].HardPosition + new Vector3(0, 50, 0);
		finalScoreText.HardPosition = graveCenter + new Vector3(10,-150,0);
		//perfectEngraving.SoftPosition = graveCenter + new Vector3(35,250,0);
		perfectPercent.HardPosition = graveCenter + new Vector3(15,60,0);
		mElement.Add(finalScoreText);
		//mElement.Add(perfectEngraving);
		mElement.Add(perfectPercent);


		TimedEventDistributor.TimedEventChain chain;
		
		chain = TED.add_event(
				low_skippable_text_bubble_event("YOU REST HERE BENEATH THE EARTH...",gIntroText)
			);

		/*.then( //wait a little bit to let the fading finish
		    	low_skippable_text_bubble_event("HERE IS YOUR LIFE STORY",gIntroText)
		    );*/

		for(int i = 1; i < aStats.Count; i++)
		{
			PerformanceStats ps = aStats[i];
			
			chain = chain.then_one_shot(
				delegate() {
					show_score(ps.Character,(int)ps.AdjustedScore,gPreScoreCount + gScoreCount + gPostScoreCount);
				},
			0).then(
				delegate(float aTime)
				{
					aTime -= gPreScoreCount;
					if(aTime > 0 && aTime < gScoreCount)
					{
						float displayScore = scoreIncrementor + (aTime/gScoreCount)*ps.AdjustedScore;
						float displayAge = ageIncrementer + (aTime/gScoreCount)*(ps.Character.Age-ageIncrementer);
						finalScoreText.Text = ""+(int)displayScore;
						perfectPercent.Text = ""+(int)displayAge;
					}
					if(aTime >  gScoreCount + gPostScoreCount)
					{
						scoreIncrementor += ps.AdjustedScore;
						ageIncrementer = ps.Character.Age;
						finalScoreText.Text = ""+(int)scoreIncrementor;
						perfectPercent.Text = ""+(int)ageIncrementer;
						return true;
					}
					return false;
				},
			0);
		}
		
		//CONNECTIONS
		for(int i = 1; i < aStats.Count; i++)
		{
			PerformanceStats ps = aStats[i];

			
			
			float gFirstConnectionText = 3.5f;
			float gConnectionText = 4f;
			float gPreParticle = 1.5f;
			
			
			//TODO grave connections
			CharIndexContainerString connections;
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
					if(aStats[j].CutsceneChangeSet != null) //TODO this check should never fail
					{
						Debug.Log("accum change for " + aStats[j].Character.StringIdentifier + " is " + aStats[j].CutsceneChangeSet.accumulative_changes()[ps.Character]);
						accumChange = aStats[j].CutsceneChangeSet.accumulative_changes()[ps.Character];
					}
					else
					{
						Debug.Log ("null cutscene change for " + aStats[j].Character.StringIdentifier + " " + aStats[j].CutsceneChangeSet);
					}
					if( (wasHard && accumChange > 0) || //if was hard and effect was positive (i.e. hard)
					   (!wasHard && accumChange < 0)) //if was easy and effect was negative (i.e. easy)
					{
						string [] conText = targetConnection.Replace("<S>","@").Split('@');
						PopupTextObject npo = null;
						if(conText.Length == 2){
							chain = chain.then (
								delegate(float aTime)
								{
									if(npo == null)
									{
										npo = add_timed_text_bubble(conText[0],gFirstConnectionText + gConnectionText,-0.6f,1);
									}
									if(npo.IsDestroyed || aTime > gPreParticle) 
									{
										return true;
									}
									return false;
								}
							,0);
						} else {
							//TODO
							Debug.Log("Peter was too lazy to implement optional splitting. Connection text MUST be split");
							Debug.Log ("TNHOEUONSTUHNST");
						}



						System.Func<FlatElementBase,float,bool> jiggleDelegate = 
							delegate(FlatElementBase aBase, float aTime2) 
							{
								aBase.mLocalRotation = Quaternion.AngleAxis(Mathf.Sin(aTime2*Mathf.PI*2*4)*15f,Vector3.forward);
								if(aTime2 >= gFirstConnectionText-gPreParticle) 
									return true;
								return false;
							};



						chain = chain.then_one_shot(
							delegate()
							{
								if(npo != null)
									mCharacters[char_to_list_index(targetCharacter)].Events.add_event(jiggleDelegate,0);
								//create the shine
								create_shine_over_character(targetCharacter,!wasHard, gFirstConnectionText);
							}
						).then_one_shot(
							delegate()
							{
								if(npo != null)
								{
									npo.Text =  conText[conText.Length -1];
									mCharacters[char_to_list_index(ps.Character)].Events.add_event(jiggleDelegate,0);
									create_shine_over_character(ps.Character,!wasHard, gFirstConnectionText-gPreParticle);
								}
							},
						gFirstConnectionText-gPreParticle).then (
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
		
		
		
		//variables for credits animation..
		float lastTime = 0;
		FlatElementImage logo1 = null;
		FlatElementImage logo2 = null;

		PopupTextObject gameOver = null;



		List<FlatElementText> creditsText = new List<FlatElementText>();
		float scrollSpeed = 75;

		

		
		mGraveCompleteCb = delegate()
		{
			
			TED.add_one_shot_event(
				delegate()
				{
					mManager.mMusicManager.fade_in_extra_music("creditsMusic");
					mManager.mMusicManager.fade_out();
				}
			,0).then_one_shot(
				delegate()
				{
					gameOver = new PopupTextObject("G A M E O V E R",30);
					gameOver.HardPosition = mFlatCamera.Center + new Vector3(0,mFlatCamera.Height/2+450,0);
					gameOver.HardColor = GameConstants.UiWhiteTransparent;
					gameOver.SoftColor = GameConstants.UiWhite;
					gameOver.set_text_color(GameConstants.UiWhiteTransparent,true);
					gameOver.set_text_color(GameConstants.UiRed);
					gameOver.set_background_color(GameConstants.UiWhiteTransparent,true);
					gameOver.set_background_color(GameConstants.UiPopupBubble);
					mElement.Add(gameOver);

					int counter = 0;
					foreach(string e in GameConstants.credits.Reverse())
					{
						var text = new FlatElementText(mManager.mNewRef.genericFont,50,e,10);
						text.HardColor = new Color(0,0,1,1);
						text.HardPosition = mFlatCamera.Center + new Vector3(0,mFlatCamera.Height/2+1000,0) + (new Vector3(0,70,0))*counter;
						creditsText.Add(text);
						mElement.Add(text);
						counter++;
					}
					
					float logoStartHeight = mFlatCamera.Height/2 + 1000 + 70*counter + 500;
					logo1 = new FlatElementImage(mManager.mNewRef.gameLabLogo,10);
					logo2 = new FlatElementImage(mManager.mNewRef.filmAkademieLogoGrave,10);
					logo1.HardPosition = mFlatCamera.Center + new Vector3(0,logoStartHeight,0);
					logo2.HardPosition = mFlatCamera.Center + new Vector3(0,logoStartHeight + 700,0);


					
					mElement.Add(logo1);
					mElement.Add(logo2);
					
				}
			,0).then(
				delegate(float aTime)
				{
				
				//scroll contents down
				Vector3 scroll = -new Vector3(0,scrollSpeed*(aTime-lastTime),0);
				foreach(FlatElementText e in creditsText)
				{
					e.SoftPosition = e.SoftPosition + scroll;
				}
				foreach(FlatElementImage e in mCharacters)
				{
					e.SoftPosition = e.SoftPosition + scroll;
				}
				foreach(FlatElementImage e in mDiffLabels)
				{
					e.SoftPosition = e.SoftPosition + scroll;
				}
				finalScoreText.SoftPosition = finalScoreText.SoftPosition + scroll;
				perfectPercent.SoftPosition = perfectPercent.SoftPosition + scroll;
				gameOver.SoftPosition = gameOver.SoftPosition + scroll;
				logo1.SoftPosition = logo1.SoftPosition + scroll;
				logo2.SoftPosition = logo2.SoftPosition + scroll;
				
				lastTime = aTime;
				if(Input.GetKeyDown(KeyCode.Alpha0))
					return true;
				
				if(aTime > gRestart)
					return true;
				return false;
			}
			,0).then_one_shot(
				graveCompleteCb
				,0);
		};
		
		chain = chain.then_one_shot(
			delegate()
			{
				mGraveCompleteCb();
				mGraveCompleteCb = null;
				mGraveChain = null;
			}
		);
		
		mGraveChain = TED.LastEventKeyAdded;
		
	}

}
