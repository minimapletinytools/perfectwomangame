using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NewInterfaceManager : FakeMonoBehaviour {
	static float END_CUTSCENE_DELAY_TIME = 1;
	
	
	
    public NewInterfaceManager(ManagerManager aManager) : base(aManager) { }

	public TimedEventDistributor TED { get; private set; }
    public FlatCameraManager mFlatCamera;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	
    CharacterTextureBehaviour mMiniMan;
	FlatBodyObject mCurrentBody = null;
	
	//skipping
	bool DoSkipMultipleThisFrame
	{get; set;}
	public void SkipMultiple()
	{ DoSkipMultipleThisFrame = true; }
	bool DoSkipSingleThisFrame
	{get; set;}
	public void SkipSingle()
	{ DoSkipSingleThisFrame = true; }
	
	public override void Start()
    {
		DoSkipSingleThisFrame = false;
		DoSkipMultipleThisFrame = false;
		TED = new TimedEventDistributor();
        mFlatCamera = new FlatCameraManager(new Vector3(10000, 0, 0), 10);
		mFlatCamera.fit_camera_to_screen();
        mMiniMan = ((GameObject)GameObject.Instantiate(ManagerManager.Manager.mReferences.mMiniChar)).GetComponent<CharacterTextureBehaviour>();        
		
		/*
		var refs = mManager.mMenuReferences;
		FlatElementSpriteText spriteTex = new FlatElementSpriteText(refs.fontTex,20,"testmessage",10);
		spriteTex.SoftPosition = mFlatCamera.Center;
		spriteTex.SoftScale = new Vector3(0.5f,0.5f,0.5f);
		mElement.Add (spriteTex);*/
		
		
		/*var refs = mManager.mMenuReferences;
		//FlatElementText text = new FlatElementText(mManager.mNewRef.genericFontPrefab,50,"aeuaeuoe",10);
		FlatElementText text = new FlatElementText(refs.menuFont,50,"aeuaeuoe",10);
		text.SoftPosition = mFlatCamera.Center;
		text.SoftScale = new ector3(0.5f,0.5f,0.5f);
		mElement.Add (text);*/
    }
    public override void Update()
    {
		
		if(Input.GetKeyDown(KeyCode.Alpha0))
			DoSkipMultipleThisFrame = true;
		if(Input.GetKeyDown(KeyCode.Alpha9))
			DoSkipSingleThisFrame = true;
		
		
        mFlatCamera.update(Time.deltaTime);
        if (mCurrentBody != null)
            mCurrentBody.match_body_to_projection(mManager.mProjectionManager);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		
		TED.update(Time.deltaTime);
		
		
		//hacks
		if(DoSkipMultipleThisFrame)
		{
			if(mLastCutsceneCompleteCb != null && mLastCutsceneChain != null)
			{
				TED.remove_event(mLastCutsceneChain);
				mLastCutsceneCompleteCb();
				mLastCutsceneChain = null;
				mLastCutsceneCompleteCb = null;
			}
			
			if(mGraveChain != null && mGraveCompleteCb != null)
			{
				TED.remove_event(mGraveChain);
				mGraveCompleteCb(true);
				mGraveChain = null;
				mGraveCompleteCb = null;
			}
			
			//grave skipping lul
			DoSkipMultipleThisFrame = false;
		}
		
		if(DoSkipSingleThisFrame)
			DoSkipSingleThisFrame = false;
    }
    
    Vector3 random_position()
    {
        //UGG piece of junk...
        return (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)).normalized * Random.Range(2000,20000);
    }
	
	//BLUE BAR
	FlatElementImage mBB;
	Vector3 mBBBasePosition;
	//PLAY
	PerformanceStats mBBLastPerformanceGraph = null; //owned by Character
	FlatElementText mBBText;
	FlatElementImage mBBScoreFrame;
	FlatElementText mBBScoreText;
	FlatElementText mBBWarningText = null;
	FlatElementImage mBBMultiplierImage; //replace with difficulty image
	FlatElementImage[] mBBPerfectStars;
	
	//CHOOSING
	int BB_NUM_CHOICES = 3;
	List<NewChoiceObject> mBBChoices = new List<NewChoiceObject>();
	List<FlatBodyObject> mBBChoiceBodies = new List<FlatBodyObject>();
	FlatElementImage mBBChoosingBackground;
	ColorTextObject mBBQuestionText;
    FlatElementText mBBQuestionTextPrefix;
	FlatBodyObject mBBMiniMan;
	Vector3 mBBMiniManBasePosition;
	
	
	
	
	//called by NewGameManager
	public void setup_bb()
	{
		var newRef = mManager.mNewRef;
		var refs = mManager.mReferences;
		
		mBB = new FlatElementImage(mManager.mNewRef.bbBackground,8);
		mBB.HardPosition = random_position();
		mBBBasePosition = mFlatCamera.get_point(-0.5f, 0); 
		mBB.SoftPosition = mBBBasePosition;
		mElement.Add(mBB);
		
		//BB small nonsense
		mBBText = new FlatElementText(mManager.mNewRef.genericFont,60,"",10);
		mBBScoreFrame = new FlatElementImage(mManager.mNewRef.bbScoreBackground,9);
		mBBScoreText = new FlatElementText(mManager.mNewRef.genericFont,60,"0",10);
		//mBBWarningText = new FlatElementText(mManager.mNewRef.genericFont,150,"WARNING",12);
		mBBWarningText = new FlatElementText(mManager.mNewRef.genericFont,400,"WARNING",20);
		mBBWarningText.HardColor = new Color(0.5f,0.5f,0.5f,0);
		mBBMultiplierImage = new FlatElementImage(null,15);
		mBBText.HardPosition = random_position();
        mBBText.HardColor = GameConstants.UiRed;
		mBBText.Alignment = TextAlignment.Left;
		mBBText.Anchor = TextAnchor.MiddleLeft;
		mBBScoreFrame.HardPosition = random_position();
		mBBScoreText.HardPosition = random_position();
		mElement.Add(mBBText);
		mElement.Add(mBBScoreFrame);
		mElement.Add(mBBScoreText);
		mElement.Add(mBBWarningText);
		mElement.Add(mBBMultiplierImage);
		mBBPerfectStars = new FlatElementImage[4];
		for(int i = 0; i < 4; i++)
		{
			mBBPerfectStars[i] = new FlatElementImage(mManager.mNewRef.uiPerfectStar,10);
			mBBPerfectStars[i].SoftColor = new Color(0.5f,0.5f,0.5f,0);
			//mElement.Add(mBBPerfectStars[i]);
		}
		
		
		
		//BB choice nonsense
		//TODO reposition further to the left need to make boxes smaller too...
		var miniMan = ((GameObject)GameObject.Instantiate(refs.mMiniChar)).GetComponent<CharacterTextureBehaviour>();
		Vector3 miniManScale = (new Vector3(1,1,1))*1.5f;
		float padding = 400;
		float netWidth = (BB_NUM_CHOICES)*padding;
		float awkwardOffset = netWidth/2 - padding/1.35f;
		for(int i = 0; i < BB_NUM_CHOICES; i++)
		{
			mBBChoices.Add(new NewChoiceObject(11));
			mBBChoiceBodies.Add(new FlatBodyObject(miniMan,12));
			float xOffset = awkwardOffset - padding*i;
			mBBChoices[i].HardPosition = mFlatCamera.get_point(0.5f, 0) + new Vector3(xOffset,0,0);
			mBBChoiceBodies[i].HardShader = refs.mMiniCharacterShader;
			mBBChoiceBodies[i].HardPosition = mFlatCamera.get_point(0.5f, 0) + new Vector3(xOffset,-195,0);
			mBBChoiceBodies[i].HardScale = miniManScale;
			mElement.Add(mBBChoices[i]);
			mElement.Add(mBBChoiceBodies[i]);
		}
		
		mBBChoosingBackground = new FlatElementImage(null,mFlatCamera.Size,0);
		mBBChoosingBackground.HardPosition = mFlatCamera.Center;
		mBBQuestionText = new ColorTextObject(10);
        mBBQuestionTextPrefix = new FlatElementText(newRef.genericFont, 100, "", 10);
		mBBQuestionText.HardPosition = mFlatCamera.get_point(0.5f,0.75f) + new Vector3(awkwardOffset-padding,-75,0);
        mBBQuestionTextPrefix.HardPosition = mFlatCamera.get_point(0.5f, 0.75f) + new Vector3(awkwardOffset - padding,75, 0);
		mBBQuestionText.SoftInterpolation = 1;
        mBBQuestionTextPrefix.SoftInterpolation = 1;
		mBBMiniMan = new FlatBodyObject(miniMan,20);
		mBBMiniMan.HardScale = miniManScale;
		mBBMiniManBasePosition = mFlatCamera.get_point(0.5f, -0.7f) + new Vector3(awkwardOffset - padding,0,0);
		mBBMiniMan.HardPosition = mBBMiniManBasePosition;
		
		
		
		mElement.Add(mBBChoosingBackground);
		mElement.Add(mBBMiniMan);
		mElement.Add(mBBQuestionText);
        mElement.Add(mBBQuestionTextPrefix);
		GameObject.Destroy(mMiniMan.gameObject);
		
	}
	
	
	//--------
	//related to updating play
	//--------
	//called by NewGameManager
	public void enable_warning_text(bool enable)
	{
		mBBWarningText.HardColor = (((int)(Time.time*8)) % 2 == 0) && enable ? new Color(0.75f,0.05f,0.0f,0.5f) : new Color(0,0,0,0);
	}
	
	//called by NewGameManager
	public void update_bb_score(float aScore)
	{
		mBBScoreText.Text = ((int)aScore).ToString();
	}
	
	
	
	//related to transitioning between PLAY and CHOOSING
	//called by set_bb_small/full
	void fade_choosing_contents(bool small)
	{
		Color smallColor = small ? new Color(0.5f,0.5f,0.5f,1) : new Color(0.5f,0.5f,0.5f,0);
		Color fullColor = !small ? new Color(0.5f,0.5f,0.5f,1) : new Color(0.5f,0.5f,0.5f,0);
		//Color smallColor = small ? new Color(1,1,1,1) : new Color(1,1,1,0);
		//Color fullColor = !small ? new Color(1,1,1,1) : new Color(1,1,1,0);
	
		/* new ui, we no longer need to hide BB contents
		mBBText.SoftColor = smallColor;
		mBBScoreFrame.SoftColor = smallColor;
		mBBScoreText.SoftColor = smallColor;
		mBBLastPerformanceGraph.PerformanceGraph.SoftColor = smallColor;
		mBBMultiplierImage
		mBBPerfectStars
		//mBBTextshould revert to red here...
		*/
		
		foreach(FlatBodyObject e in mBBChoiceBodies)
			e.SoftColor = fullColor;
		foreach(NewChoiceObject e in mBBChoices)
			e.SoftColor = fullColor;
		mBBMiniMan.SoftColor = fullColor;
		mBBQuestionText.SoftColor = fullColor;
        mBBQuestionTextPrefix.SoftColor = fullColor*GameConstants.UiRed*2;
		mBBChoosingBackground.SoftColor = fullColor*(new Color(0.6f,0.6f,1))*1;//0.2f;
	}
	
	//make sure choice contents are made first before calling this
	//called by set_for_CHOICE()
	public void set_bb_for_choosing()
	{
		//Vector2 baseSize = new Vector2(mBB.BoundingBox.width,mBB.BoundingBox.height);
		//Vector2 desiredSize = new Vector2(mFlatCamera.Width+200,mFlatCamera.Height+200);
		//mBB.SoftScale = new Vector3(desiredSize.x/baseSize.x,desiredSize.y/baseSize.y,1);
		//mBB.SoftPosition = mFlatCamera.get_point(0, 0);
		fade_choosing_contents(false);
		mBBMiniMan.SoftColor = GameConstants.UiMiniMan;
        //mBB.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
	}
	
	//make sure begin_new_character is called before this
	//called by set_for_PLAY()
	void set_bb_for_play(float aBBOffset = 0)
	{
		float bottomVOffset = -50;
		//mBB.SoftScale = new Vector3(1,1,1);
		mBB.SoftPosition = mBBBasePosition + new Vector3(0,aBBOffset,0);
		mBBText.SoftPosition = mBB.SoftPosition + new Vector3(460,160,0);
		mBBScoreFrame.SoftPosition = mBB.SoftPosition + new Vector3(-350,bottomVOffset-50,0);
		mBBScoreText.SoftPosition = mBB.SoftPosition + new Vector3(-350,bottomVOffset-90,0);
		mBBLastPerformanceGraph.PerformanceGraph.SoftPosition = mBB.SoftPosition + new Vector3(150,bottomVOffset,0);
		mBBWarningText.HardPosition = mFlatCamera.get_point(0.45f,0);//mBB.SoftPosition + new Vector3(150,bottomVOffset-40,0);
		mBBMultiplierImage.SoftPosition = mBB.SoftPosition + new Vector3(-350,bottomVOffset + 170,0);
		
		for(int i = 0; i < 4; i++)
			mBBPerfectStars[i].SoftPosition = mBBScoreFrame.SoftPosition + new Vector3(mBBScoreFrame.BoundingBox.width/2f - 37 - i * mBBScoreFrame.BoundingBox.width/3f,130,0); //should be /4
		
		//return bodies if needed
		//OLD, we no longer do the mini char body borrowing thing
		/*
		foreach(NewChoiceObject e in mBBChoices)
		{
			if(e.Character.Index != -1)
			{
				mPBCharacterIcons[e.Character.Index].return_body(e.take_body());
			}
			e.Character = new CharacterIndex(-1);
		}*/
		
		fade_choosing_contents(true);
		mBB.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		
		//meter objects overrides soft color so we have to manually turn the meter off..
		TED.add_event(
			delegate(float aTime){
				foreach(NewChoiceObject e in mBBChoices)
					e.Percentage = Mathf.Clamp01(e.Percentage * (1-aTime) + 0 * aTime);
				return (aTime >= 1);
			},
		0);
	}
	
	
	//--------
	//related to setting for CHOOSING
	//--------
	//called by NewGameManager
	public void set_bb_decider_pose(Pose aPose)
	{
		mBBMiniMan.set_target_pose(aPose);
	}
	
	//called by ChoiceHelper
	public void set_bb_choice_poses(List<Pose> aPoses)
	{
		for(int i = 0; i < BB_NUM_CHOICES; i++)
		{
			mBBChoiceBodies[i].set_target_pose(aPoses[i]);
		}
	}
	public void set_bb_choice_perfectness(List<int> aPerfect)
	{
		for(int i = 0; i < BB_NUM_CHOICES; i++)
		{
			mBBChoices[i].set_perfectness(aPerfect[i]);
		}
	}
	//called by ChoiceHelper
	//this should really be called set_bb_choice_icons now
	public void set_bb_choice_bodies(CharacterIndex aIndex)
	{
		CharacterIndex index = new CharacterIndex(aIndex.LevelIndex+1,0);
		var all = index.NeighborsAndSelf;
		all.Add(index);
		for(int i = 0; i < 3; i++)
		{
			//mBBChoices[i].set_actual_character(mManager.mCharacterBundleManager.get_mini_character(all[i]));
			mBBChoices[i].Character = all[i];
			mBBChoices[i].set_actual_character(all[i]);
			mBBChoices[i].set_difficulty(mManager.mCharacterBundleManager.get_character_helper().Characters[all[i]].Difficulty);
			//TODO set icons for choice
			
			//OLD when using mini char icons we took from PB
			//mBBChoices[i].return_body(mPBCharacterIcons[all[i].Index].take_body()); //make sure to return the body
		}
	}
	//this is the character that is curretnly being selected
	//called by ChoiceHelper
	public void set_bb_choice(int aIndex)
	{
		if(aIndex == -1) //no choice
		{
			mBBMiniMan.SoftColor = GameConstants.UiRedTransparent;
			mBBMiniMan.SoftPosition = mBBMiniManBasePosition;
            mBBQuestionTextPrefix.Text = "What will you be like";
			mBBQuestionText.set_text(
				new string[]{("at age " + mBBLastPerformanceGraph.Character.get_future_neighbor(0).Age) + "?"},
				new Color[]{GameConstants.UiRed});
		}
		else{
			mBBMiniMan.SoftColor = GameConstants.UiMiniMan;
			mBBMiniMan.SoftPosition = mBBChoiceBodies[aIndex].SoftPosition;
			var nChar = mBBLastPerformanceGraph.Character.get_future_neighbor(aIndex);
			var nCharDiff = mManager.mCharacterBundleManager.get_character_helper().Characters[nChar];
			var diffPhrases = new string[]{" easy", " normal", " hard", " extreme"};
			//var perfectPhrases = new string[]{" horrible", " passable", " perfect", " PERFECT"};
			//var perfectColors = new Color[]{new Color32(200,173,27,255),new Color32(240,220,130,255),new Color32(253,238,0,255),new Color32(255,126,0,255)};
			var diffColors = new Color[]{new Color(0,0.8f,0,1), new Color(0.8f,0.8f,0,1), new Color(0.9f,0.4f,0,1), new Color(0.8f,0,0,1)};
            mBBQuestionTextPrefix.Text = "That is a";
			mBBQuestionText.set_text(
				//new string[]{("Will you be " + nChar.Description + "\nThat is a " can't do this because my multicolor font thing can't handle new line
				new string[]{
                    //("That is a "),
					//perfectPhrases[nCharDiff.Perfect], 
					//Mathf.Abs((3-nCharDiff.Difficulty) - nCharDiff.Perfect) > 1 ? " but" : " and",
					diffPhrases[nCharDiff.Difficulty],
					" choice."},
				new Color[]{
                    //GameConstants.UiPink,
					diffColors[nCharDiff.Difficulty]/2f,
					//GameConstants.UiPink,
					//perfectColors[nCharDiff.Perfect]/2f,
					GameConstants.UiPink});
		}
	}
	//called by ChoiceHelper
	public void set_bb_choice_percentages(int aIndex, float aPercent)
	{
		//int index = aIndex == 3 ? 2 : aIndex; //TODO delete this
		if(aIndex == 3) return; //TODO delete this
		
		int index = aIndex;
		mBBChoices[index].Percentage = aPercent;
	}
	
	
	
	
	//PINK BAR
	FlatElementImage mPB;
	CharIndexContainerCharacterIconObject mPBCharacterIcons = new CharIndexContainerCharacterIconObject();
	
	//called by NewGameManager
	public void setup_pb()
	{
		var newRef = mManager.mNewRef;
		mPB = new FlatElementImage(newRef.pbBackground,1);
		mPB.HardPosition = random_position();
		mPB.SoftPosition = mBBBasePosition;
		
		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
			//OLD
			//mPBCharacterIcons[e.Index] = new CharacterIconObject(mManager.mCharacterBundleManager.get_mini_character(e),1);
			mPBCharacterIcons[e] = new CharacterIconObject(e,mPB.Depth + 1);
			mPBCharacterIcons[e].set_name(e.ShortName);
			mElement.Add(mPBCharacterIcons[e]);
		}
		mElement.Add(mPB);
		
		//TODO DELETE
		//set_pb_character_icon_colors(new CharIndexContainerCharacterIconObject());
		
		position_pb_character_icons(0);
	}
	
	//OLD no longer need to set icon poses
	public void set_pb_character_icon_poses(List<KeyValuePair<CharacterIndex,Pose>> aChars)
	{
		
		foreach(var e in aChars)
		{
			//mPBCharacterIcons[e.Key.Index].mBody.set_target_pose(e.Value);
		}
	}
	
	
	
	public void set_pb_character_icon_colors(CharIndexContainerCharacterStats aChars)
	{
		foreach(CharacterStats e in aChars.to_array())
		{
			//mPBCharacterIcons[e.Character.Index].set_background_color(Color.Lerp(new Color(0.5f,0.5f,0.5f), new Color32(255,200,0,255), e.Perfect/3f));
			//probably don't need tho second check but w/e
			if(e != null && mPBCharacterIcons[e.Character] != null)
			{
				mPBCharacterIcons[e.Character].set_perfectness(e.Perfect);
				mPBCharacterIcons[e.Character].set_difficulty(e.Difficulty);
			}
		}
		
		//fetus
		mPBCharacterIcons[CharacterIndex.sFetus].set_difficulty(-1);
		//top secret
		mPBCharacterIcons[CharacterIndex.sGrave].SoftColor = new Color(1,1,1,0);
		mPBCharacterIcons[CharacterIndex.sOneHundred].SoftColor = new Color(1,1,1,0);
	}
	//characters == LEVEL will be behind the BB
	void position_pb_character_icons(int splitLevel, float vOffset = 0)
	{
		float padding = 300;
		float hPadding = 250;
		float splitHeight = 220;
		Vector3 baseOffset =  mBBBasePosition;
		
		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
			
			Vector3 position = Vector3.zero;
			float netWidth = (e.NumberInRow - 1)*padding;
			position.x = netWidth/2f - padding*e.Choice;
			int indexOffset = (splitLevel - e.LevelIndex);
			position.y = hPadding*indexOffset + vOffset; 
			if(indexOffset > 0) //past characters
				position.y += splitHeight/2;
			else if (indexOffset < 0) //future characters
				position.y -= splitHeight/2;
			mPBCharacterIcons[e].SoftPosition = baseOffset + position;
		}
		
	}
	
	public void add_cutscene_particle_stream(CharacterIndex aTarget, PopupTextObject aPopup, float duration, bool aPositive)
	{
		float delay = 0;
		Color useColor = (!aPositive) ? GameConstants.ParticleStreamEasy : GameConstants.ParticleStreamHard;
		if(mPBCharacterIcons[aTarget] != null)
		{
			TED.add_one_shot_event(
				delegate()
				{
					if(aPositive)
						aPopup.set_text_color(GameConstants.UiWhite,true);
					aPopup.set_background_color(useColor,true);
					mPBCharacterIcons[aTarget].set_background_color(useColor);
				},
			delay).then_one_shot(
				delegate()
				{
					if(aPositive)
						aPopup.set_text_color(GameConstants.UiRed,true);
					aPopup.set_background_color(new Color(0.5f,0.5f,0.5f),true);
					mPBCharacterIcons[aTarget].set_background_color(new Color(0.5f,0.5f,0.5f));
				},
			duration);
			add_timed_particle_stream(
                mFlatCamera.get_point(0.40f,0),
                mPBCharacterIcons[aTarget],
                duration,
                delay,
                useColor);
		}
	}
	
	public void add_timed_particle_stream(Vector3 aPosition, CharacterIconObject aTarget, float aDuration, float aDelay, Color aColor)
	{
		ParticleStreamObject pso = null;
		TED.add_one_shot_event(
			delegate()
			{
				aTarget.set_depth(mPB.Depth + 5); //adjust the depth so the stream shows over the right icons
				pso = new ParticleStreamObject(mPB.Depth + 7,aPosition);
                pso.HardColor = aColor;
				pso.HardPosition = aTarget.SoftPosition;
				pso.update(0);
				mElement.Add(pso);
			},
		aDelay).then_one_shot(
			delegate()
			{
				aTarget.set_depth(mPB.Depth + 1); //and then  reset it to what it should be
				mElement.Remove(pso);
				pso.destroy();
			},
		aDuration);
	}
	
	//TEXT
	//TODO get rid ofthe stupid yreloffset parameter...
	public PopupTextObject add_timed_text_bubble(string aMsg, float duration, float yRelOffset = 0)
	{
		duration = duration -1; //TODO Hack don't modify timing here
		PopupTextObject to = new PopupTextObject(aMsg,8);
		to.HardPosition = random_position();
		to.HardColor = GameConstants.UiWhiteTransparent;
		to.SoftColor = GameConstants.UiWhite;
		to.set_text_color(GameConstants.UiWhiteTransparent,true);
		to.set_text_color(GameConstants.UiRed);
		TimedEventDistributor.TimedEventChain chain = TED.add_event(
			delegate(float aTime)
			{
				//to.SoftPosition = mFlatCamera.get_point(0.40f,yRelOffset); //fly in
				to.HardPosition = mFlatCamera.get_point(0.40f,yRelOffset); //cut in
				mElement.Add(to);
				return true;
			},
        0).then(
			delegate(float aTime)
			{
				if(aTime > duration) 
					return true;
				if(DoSkipSingleThisFrame)
				{
					DoSkipSingleThisFrame = false;
					return true;
				}
				return false;
			},
		0).then_one_shot(
			delegate()
			{
				//cutout
				mElement.Remove(to);
				to.destroy();
				
				//to.fade_out();  fade out
			},
		0).then_one_shot(
			delegate()
			{
				//fadeout
				//mElement.Remove(to);
				//to.destroy();
			},
		2);
		return to;
	}
	
	//this gets called during CHOOSE so BB should be full sized
	//this gets called by NewGameManager
	public void begin_new_character(PerformanceStats aChar)
	{
		//BB
		mBBText.Text = FlatElementText.convert_to_multiline(aChar.Character.Description.Length > 20 ? 2 : 1 ,aChar.Character.Description + " (" + aChar.Character.Age.ToString() + ")");
		if(aChar.Character.LevelIndex != 0)
		{
			mBBMultiplierImage.set_new_texture(mManager.mNewRef.bbScoreMultiplier[aChar.Stats.Difficulty]);
			for(int i = 0; i < 4; i++)
				mBBPerfectStars[i].SoftColor = i <= aChar.Stats.Perfect ? new Color(0.5f,0.5f,0.5f,0.5f) : new Color(0.5f,0.5f,0.5f,0f);
		}
		if(mBBLastPerformanceGraph != null) //fade out the old graph
		{
			mBBLastPerformanceGraph.PerformanceGraph.SoftColor = new Color(0.5f,0.5f,0.5f,0);
			//mBBLastPerformanceGraph.PerformanceGraph.Enabled = false;
			//mElement.Remove(mBBLastPerformanceGraph.PerformanceGraph);
		}
		mBBLastPerformanceGraph = aChar;
		mElement.Add(mBBLastPerformanceGraph.PerformanceGraph);
		
		
		//TODO delete this gets set elsewhere now
		//mBBQuestionText.Text = "What will you be like at age " + aChar.Character.get_future_neighbor(0).Age;
		
		//PB
		position_pb_character_icons(aChar.Character.LevelIndex);
		//disable the other characters no that we have made a choice
		foreach(CharacterIndex e in aChar.Character.Neighbors)
		{
			if(mPBCharacterIcons[e] != null)
				mPBCharacterIcons[e].Enabled = false;
			//mPBCharacterIcons[e].destroy();
			//mElement.Remove(mPBCharacterIcons[e]);
			//mPBCharacterIcons[e]=null;
		}
	}
	
	
	public void set_for_PLAY()
	{
		set_bb_for_play();
	}
	
	
	//these are hacks to allow me to skip cutscenes
	QuTimer mLastCutsceneChain = null;
	System.Action mLastCutsceneCompleteCb = null;
	public void set_for_CUTSCENE(System.Action cutsceneCompleteCb, NUPD.ChangeSet aChanges)
	{
		//used for skipping cutscene
		/*
		TED.add_event(
			delegate(float aTime)
			{
				add_timed_text_bubble("CUTSCENE HERE",1);
				return true;
			},
        0).then_one_shot( //dummy 
			delegate(){cutsceneCompleteCb();},END_CUTSCENE_DELAY_TIME);
		return;*/
		
		float gStartCutsceneDelay = 2.5f;
		float gPerformanceText = 5f;
		float gCutsceneText = 5f;
		float gPreParticle = 1.5f;
		float gParticle = 2f;
		
		
		//this slows the game down a lot...
		set_bb_for_play(mPB.BoundingBox.height/2+205);
		position_pb_character_icons(mBBLastPerformanceGraph.Character.LevelIndex,mPB.BoundingBox.height/2+205);
		
		mLastCutsceneCompleteCb = delegate() {
			//this slows the ame down a lot
			set_bb_for_play();
			position_pb_character_icons(mBBLastPerformanceGraph.Character.LevelIndex,0);
			cutsceneCompleteCb();
			mLastCutsceneCompleteCb = null;
			mLastCutsceneChain = null;
		};
		
		string[] perfectPhrase = {"awful","mediocre","good", "perfect"};
		string[] performancePhrase = {"horribly","poorly","well", "excellently"};
		PopupTextObject introPo = null;
		TimedEventDistributor.TimedEventChain chain = TED.add_event(
			delegate(float aTime)
			{
				string text = "";
				if(mBBLastPerformanceGraph.Character == CharacterIndex.sFetus)
				{
					//DELETE this has been moved to text files..
					//text = "Prepare to be Born"; 
				}
				else
				{
					//TODO use color text here... In fact you should replace color text as yoru standard text object really...
					//text = aChanges.PerformanceDescription.Replace("<P>",perfectPhrase[mBBLastPerformanceGraph.Stats.Perfect]);
					string noCapsDescription = mBBLastPerformanceGraph.Character.Description.ToLower();
					if(mBBLastPerformanceGraph.Character.IsDescriptionAdjective)
						text = "You lived your life " + noCapsDescription + " " + performancePhrase[(int)Mathf.Clamp(mBBLastPerformanceGraph.Score*4,0,3)] + ".";
					else
						text = "You lived your life as a " + noCapsDescription + " " + performancePhrase[(int)Mathf.Clamp(mBBLastPerformanceGraph.Score*4,0,3)] + ".";
					introPo = add_timed_text_bubble(text,gPerformanceText);
				}
				return true;
			},
        gStartCutsceneDelay).then( 
			delegate(float aTime)
			{
				if(introPo != null && introPo.IsDestroyed)
					return true;
				if(!(mBBLastPerformanceGraph.Character == CharacterIndex.sFetus))
					if(aTime > gPerformanceText)
						return true;
					else return false;
				return true;
			},
		0);
		
		
		foreach(var e in aChanges.Changes)
		{
			//string changeMsg = Random.Range(0,3) == 0 ? PDStats.negative_sentences[(int)e][0] : PDStats.positive_sentences[(int)e][0];
			var changes = e;
            var diffChanges = e.Changes;
            string changeMsg = e.Description;
			PopupTextObject po = null;
			chain = chain.then(
				delegate(float aTime)
				{
					if(po == null)
						po = add_timed_text_bubble(changeMsg,gCutsceneText);
					if(po.IsDestroyed || aTime > gPreParticle)
					{
						return true;
					}
					return false;
				}
			,0).then(
				delegate(float aTime)
				{
					if(!po.IsDestroyed)
					{
						foreach(CharacterIndex cchar in CharacterIndex.sAllCharacters)
						{
							if(diffChanges[cchar] != 0){
								add_cutscene_particle_stream(cchar,po,gParticle,changes.is_positive());
								int nDiff = Mathf.Clamp(mManager.mGameManager.get_character_difficulty(cchar) + diffChanges[cchar], 0, 3);
								mManager.mGameManager.change_interface_pose(cchar,nDiff);
	                        	mPBCharacterIcons[cchar].set_difficulty(nDiff);
							}
						}
					}
					return true;
				}
			,0).then(
				delegate(float aTime)
				{
					if(po.IsDestroyed || aTime > gCutsceneText-gPreParticle)
					{	
						return true;
					}
					return false;
				}
			,0);
		}
		
		chain = chain.then_one_shot(delegate(){mLastCutsceneCompleteCb();},END_CUTSCENE_DELAY_TIME);
		
		mLastCutsceneChain = TED.LastEventKeyAdded;
	}
	
	public void set_for_CHOICE()
	{
		//TODO
		set_bb_for_choosing();
	}
	
	//returns amount of time this will take
	public TimedEventDistributor.TimedEventChain set_for_DEATH(CharacterIndex aChar)
	{
		float gTextTime = 5;
		
		TimedEventDistributor.TimedEventChain chain;
		
		if(aChar.LevelIndex == 7)
		{
			//80
			chain = TED.add_one_shot_event(
				delegate()
				{
					add_timed_text_bubble("It's time for you to die a natural death",gTextTime);
				},
	        0).then_one_shot( //dummy 
			delegate(){},gTextTime);
		}
		else if (aChar.LevelIndex == 8)
		{
			//100!!!
			chain = TED.add_one_shot_event(
				delegate()
				{
					add_timed_text_bubble("Congragulations",gTextTime);
				},
	        0).then_one_shot(
				delegate()
				{
					add_timed_text_bubble("Your life was may not have been perfect",gTextTime);
				},
			gTextTime).then_one_shot(
				delegate()
				{
					add_timed_text_bubble("but you lived 100 years",gTextTime);
				},
			gTextTime).then_one_shot( //dummy 
				delegate(){},gTextTime);
		}
		else
		{
			chain = TED.add_one_shot_event(
				delegate()
				{
					add_timed_text_bubble("You die at the age of " + aChar.Age,gTextTime);
				},
	        1).then_one_shot(
				delegate()
				{
					//TODO pink bar animations
				}
			,gTextTime).then_one_shot( //dummy 
				delegate(){},0);
		}
		
		return chain;
	}
	
	
	//delegates needed for skipping cleanly
	QuTimer mGraveChain = null;
	System.Action<bool> mGraveCompleteCb = null;
	public void set_for_GRAVE(List<PerformanceStats> aStats, System.Action graveCompleteCb)
	{
		//timing vars
		float gIntroText = 4.5f;
		float gCharacterText = 4.5f;
		float gPreGlory = 0f;
		float gGlory = 0f;
		float gPostGlory = 0f;
		float gPreScoreCount = 2.5f;
		float gScoreCount = 1.5f;
		float gPostScoreCount = 0.5f;
		float gRestart = 15;
		
		//remove the grave
		if(aStats.Last().Character.Age == 999)
			aStats.RemoveAt(aStats.Count-1);
		
		
		//fake it for testing...
		/*
		for(int i = 0; i < 8; i++)
		{
			if(aStats.Last().Character.Age < (new CharacterIndex(i,0)).Age)
			{
				aStats.Add(new PerformanceStats(new CharacterIndex(i,0)));
			}
		}
		*/
		
		
		//clear away BB and PB
		var smallColor = new Color(1,1,1,0);
		mBB.SoftColor = smallColor;
		mBBText.SoftColor = smallColor;
		mBBScoreFrame.SoftColor = smallColor;
		mBBScoreText.SoftColor = smallColor;
		aStats[0].PerformanceGraph.SoftColor = smallColor;
		mBBMultiplierImage.SoftColor = smallColor;
		for(int i = 0; i < 4; i++)
			mBBPerfectStars[i].SoftColor = smallColor;
		
		position_pb_character_icons(0,-3000);
		
		
		//this is all a hack to get the score to show up right...
		float scoreIncrementor = 0;
		FlatElementText finalScoreText = new FlatElementText(mManager.mNewRef.genericFont,100,"",10);
		//FlatElementImage perfectEngraving = new FlatElementImage(mManager.mNewRef.gravePerfectnessEngraving,10);
		FlatElementText perfectPercent = new FlatElementText(mManager.mNewRef.genericFont,100,"",11);
		//perfectPercent.Text = ((int)(100*aStats.Sum(e=>e.Stats.Perfect+1)/(float)(aStats.Count*3))).ToString() + "%";
		perfectPercent.Text = aStats.Last().Character.Age.ToString();
		
		//hack to put things into bg camera
		foreach (Renderer f in finalScoreText.PrimaryGameObject.GetComponentsInChildren<Renderer>())
                f.gameObject.layer = 4;
		foreach (Renderer f in perfectPercent.PrimaryGameObject.GetComponentsInChildren<Renderer>())
                f.gameObject.layer = 4;
		//foreach (Renderer f in perfectEngraving.PrimaryGameObject.GetComponentsInChildren<Renderer>()) f.gameObject.layer = 4;
		
		Vector3 graveCenter = mManager.mBackgroundManager.mBackgroundElements[0].SoftPosition + new Vector3(0, 50, 0);
		finalScoreText.SoftPosition = graveCenter + new Vector3(0,-250,0);
		//perfectEngraving.SoftPosition = graveCenter + new Vector3(35,250,0);
		perfectPercent.SoftPosition = graveCenter + new Vector3(24,180,0);
		mElement.Add(finalScoreText);
		//mElement.Add(perfectEngraving);
		mElement.Add(perfectPercent);
		
		List<Vector3> ghostPositions = new List<Vector3>();
		List<FlatElementImage> ghostElements = new List<FlatElementImage>();
		for(int i = 1; i < aStats.Count; i++)
		{
			Vector3 angelPosition = graveCenter + 
				(aStats.Count > 2 ? new Vector3(
				Mathf.Cos(Mathf.PI*((i-1)/(aStats.Count-2f))),
				Mathf.Sin(Mathf.PI*((i-1)/(aStats.Count-2f))),
				0) : new Vector3(0,1,0)) *600;
			ghostPositions.Add(angelPosition);
			
			var isp = (mManager.mCharacterBundleManager.get_image("ANGELS_"+aStats[i].Character.StringIdentifier));
			var ge = new FlatElementImage(isp.Image,9);
			ge.HardPosition = random_position()*3f;
			ge.HardScale = new Vector3(2.5f,2.5f,1);
			foreach (Renderer f in ge.PrimaryGameObject.GetComponentsInChildren<Renderer>())
                f.gameObject.layer = 4;
			//ge.HardColor = new Color(.5f,.5f,.5f,0);
			ghostElements.Add(ge);
			mElement.Add(ge);
		}
		
		TimedEventDistributor.TimedEventChain chain;
		
		chain = TED.add_one_shot_event(
			delegate()
			{
				add_timed_text_bubble("You rest here beneath the earth...",gIntroText,0.5f);
			},
        gIntroText).then_one_shot( //wait a little bit to let the fading finish
			delegate()
			{
				add_timed_text_bubble("Here is your life story:",gIntroText,0.5f);
			},
		gIntroText).wait (gIntroText);
		
		float startingPosition = mFlatCamera.get_point(0,1).y - aStats[0].PerformanceGraph.BoundingBox.height/2f - 10;
		float intervalSize = aStats[0].PerformanceGraph.BoundingBox.height + 5;
		float cioXOffset = mBB.SoftPosition.x + 370;
		float pgoXOffset = mBB.SoftPosition.x - 145;
		//make performance graphs come in one at a time from the bottom
		//starting at one means skipping fetus
		//going less than count means skipping grave
		for(int i = 1; i < aStats.Count; i++)
		{
			int it = i;
			PerformanceStats ps = aStats[i];
			//reposition the assosciated character icon and performance graph
			CharacterIconObject cio = mPBCharacterIcons[ps.Character];
			PerformanceGraphObject pgo = ps.PerformanceGraph;
			cio.HardPosition = new Vector3(cioXOffset,-2000,0);
			pgo.HardPosition = new Vector3(pgoXOffset,-2000,0);
			pgo.HardColor = new Color(0.5f,0.5f,0.5f,1);
			
			string[] perfectPhrase = {"awful","mediocre","good", "perfect"};
			string[] performancePhrase = {"a disaster","bad","good", "perfect"};
			
			PopupTextObject po = null;
			chain = chain.then_one_shot(
				
				//TODO add soft skipping in here
				delegate()
				{
				
					ghostElements[ps.Character.LevelIndex-1].SoftPosition = ghostPositions[ps.Character.LevelIndex-1];
					mManager.mMusicManager.play_sound_effect("graveAngel");	
				
					string text = "";
					//set the textt
					if(ps.Character.IsDescriptionAdjective)
						text += "Your life " + ps.Character.Description;
					else
						text += "Your life as a " + ps.Character.Description;
					
					text += " was " + performancePhrase[Mathf.Clamp((int)(Mathf.Sqrt(ps.Score)*4),0,3)];
					text += ".";
					po = add_timed_text_bubble(text,gCharacterText,0.5f);
				
					//move in stuff
					cio.SoftPosition = new Vector3(cioXOffset,startingPosition - (it-1) * intervalSize,0);
					pgo.SoftPosition = new Vector3(pgoXOffset,startingPosition - (it-1) * intervalSize,0);
				
					float netHeight = (it) * intervalSize;
					if(netHeight > mFlatCamera.Height - 10) //start scrolling
					{
						Vector3 scroll = -new Vector3(0,intervalSize,0);
						foreach(var e in aStats)
						{
							if(e.Character.Age <= ps.Character.Age)
							{
								mPBCharacterIcons[e.Character].SoftPosition = mPBCharacterIcons[e.Character].SoftPosition - scroll;
								e.PerformanceGraph.SoftPosition = e.PerformanceGraph.SoftPosition - scroll;
							}
						}
					}
				} 
			).then(
				delegate(float aTime)
				{
					aTime -= gPreScoreCount;
					if(aTime > 0)
					{
						float displayScore = scoreIncrementor + (aTime/gScoreCount)*ps.AdjustedScore;
						finalScoreText.Text = ""+(int)displayScore;
					}
					if(po.IsDestroyed || aTime >  gScoreCount + gPostScoreCount)
					{
						scoreIncrementor += ps.AdjustedScore;
						return true;
					}
					return false;
				},
			0);
			
			//TODO grave connections
			CharIndexContainerString connections;
			bool wasHard = ps.Stats.Difficulty > 1;
			if(wasHard)
				connections = mManager.mCharacterBundleManager.get_character_stat(ps.Character).CharacterInfo.HardConnections;
			else
				connections = mManager.mCharacterBundleManager.get_character_stat(ps.Character).CharacterInfo.EasyConnections;
			CharIndexContainerString realConnections = new CharIndexContainerString();
			for(int j = 1; j < aStats.Count; j++)
			{
				if(connections[aStats[j].Character] != null && connections[aStats[j].Character] != "")
					realConnections[aStats[j].Character] = connections[aStats[j].Character];
			}
			
			/*
			chain = chain.then_one_shot (
				delegate(float aTime)
				{
					
				//	add_cutscene_particle_stream(
				},
			0).then(
				delegate(float aTime)
				{
					//TODO soft skipping lul
					return aTime > 10;
				}
			);*/
		}
		
		
		//variables for credits animation..
		float lastTime = 0;
		FlatElementImage logo1 = null;
		FlatElementImage logo2 = null;
		List<FlatElementText> creditsText = new List<FlatElementText>();
		float scrollSpeed = 200;
			
		
		
		System.Action set_all_GRAVE_positions =  delegate()
		{
			for(int i = 1; i < aStats.Count; i++)
			{
				int it = i;
				PerformanceStats ps = aStats[i];
				CharacterIconObject cio = mPBCharacterIcons[ps.Character];
				PerformanceGraphObject pgo = ps.PerformanceGraph;
				
				//angels
				ghostElements[ps.Character.LevelIndex-1].SoftPosition = ghostPositions[ps.Character.LevelIndex-1];
				
				//right side elements
				cio.SoftPosition = new Vector3(cioXOffset,startingPosition - (it-1) * intervalSize,0);
				pgo.SoftPosition = new Vector3(pgoXOffset,startingPosition - (it-1) * intervalSize,0);
			}
		};
		
		mGraveCompleteCb = delegate( bool aSetPositions)
		{
			if(aSetPositions)
			{
				set_all_GRAVE_positions();
			}
			
			TED.add_one_shot_event(
				delegate()
				{
					add_timed_text_bubble("G A M E  O V E R",99999,0.5f);
				}
			,0).then_one_shot(
				delegate()
				{
					//TODO create credits text and nonsense
					int counter = 0;
					foreach(string e in GameConstants.credits.Reverse())
					{
						var text = new FlatElementText(mManager.mNewRef.genericFont,50,e,mPB.Depth +1);
						text.HardColor = new Color(1,1,1,1);
						text.HardPosition = mPB.SoftPosition + new Vector3(0,mFlatCamera.Height/2+450,0) + (new Vector3(0,70,0))*counter;
						creditsText.Add(text);
						mElement.Add(text);
						counter++;
					}
				
					float logoStartHeight = mFlatCamera.Height/2+450 + 70*counter + 500;
					logo1 = new FlatElementImage(mManager.mNewRef.gameLabLogo,mPB.Depth+1);
					logo2 = new FlatElementImage(mManager.mNewRef.filmAkademieLogoGrave,mPB.Depth+1);
					logo1.HardPosition = mPB.SoftPosition + new Vector3(0,logoStartHeight,0);
					logo2.HardPosition = mPB.SoftPosition + new Vector3(0,logoStartHeight + 700,0);
				
					mElement.Add(logo1);
					mElement.Add(logo2);
				
				}
			,0).then(
				delegate(float aTime)
				{
					
					//scroll contents down
					Vector3 scroll = -new Vector3(0,scrollSpeed*(aTime-lastTime),0);
					foreach(var e in aStats)
					{
						mPBCharacterIcons[e.Character].SoftPosition = mPBCharacterIcons[e.Character].SoftPosition + scroll;
						e.PerformanceGraph.SoftPosition = e.PerformanceGraph.SoftPosition + scroll;
					}
					foreach(FlatElementText e in creditsText)
					{
						e.SoftPosition = e.SoftPosition + scroll;
					}
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
				mGraveCompleteCb(false);
				mGraveCompleteCb = null;
				mGraveChain = null;
			}
		);
		
		mGraveChain = TED.LastEventKeyAdded;
		
	}
	
	
	
	
	
}
	