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
	
	public override void Start()
    {
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
        mFlatCamera.update(Time.deltaTime);
        if (mCurrentBody != null)
            mCurrentBody.match_body_to_projection(mManager.mProjectionManager);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		
		TED.update(Time.deltaTime);
		
		
		//hacks
		if(Input.GetKeyDown(KeyCode.Alpha0) )
		{
			if(mLastCutsceneCompleteCb != null && mLastCutsceneChain != null)
			{
				TED.remove_event(mLastCutsceneChain);
				mLastCutsceneCompleteCb();
				mLastCutsceneChain = null;
				mLastCutsceneCompleteCb = null;
			}
		}
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
	
	//CHOOSING
	int BB_NUM_CHOICES = 3;
	List<NewChoiceObject> mBBChoices = new List<NewChoiceObject>();
	List<FlatBodyObject> mBBChoiceBodies = new List<FlatBodyObject>();
	FlatElementImage mBBChoosingBackground;
	ColorTextObject mBBQuestionText;
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
		mBBWarningText = new FlatElementText(mManager.mNewRef.genericFont,500,"WARNING",20);
		mBBWarningText.HardColor = new Color(0.5f,0.5f,0.5f,0);
		mBBText.HardPosition = random_position();
		mBBScoreFrame.HardPosition = random_position();
		mBBScoreText.HardPosition = random_position();
		mElement.Add(mBBText);
		mElement.Add(mBBScoreFrame);
		mElement.Add(mBBScoreText);
		mElement.Add(mBBWarningText);
		
		
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
		mBBQuestionText.HardPosition = mFlatCamera.get_point(0.5f,0.75f) + new Vector3(awkwardOffset-padding,0,0);
		mBBQuestionText.SoftInterpolation = 1;
		mBBMiniMan = new FlatBodyObject(miniMan,20);
		mBBMiniMan.HardScale = miniManScale;
		mBBMiniManBasePosition = mFlatCamera.get_point(0.5f, -0.7f) + new Vector3(awkwardOffset - padding,0,0);
		mBBMiniMan.HardPosition = mBBMiniManBasePosition;
		
		
		mElement.Add(mBBChoosingBackground);
		mElement.Add(mBBMiniMan);
		mElement.Add(mBBQuestionText);
		GameObject.Destroy(mMiniMan.gameObject);
		
	}
	
	
	//--------
	//related to updating play
	//--------
	//called by NewGameManager
	public void enable_warning_text(bool enable)
	{
		mBBWarningText.HardColor = (((int)(Time.time*8)) % 2 == 0) && enable ? new Color(0.7f,0.2f,0,0.5f) : new Color(0,0,0,0);
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
		*/
		
		foreach(FlatBodyObject e in mBBChoiceBodies)
			e.SoftColor = fullColor;
		foreach(NewChoiceObject e in mBBChoices)
			e.SoftColor = fullColor;
		mBBMiniMan.SoftColor = fullColor;
		mBBQuestionText.SoftColor = fullColor;
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
		mBBMiniMan.SoftColor = new Color(1,0.3f,0.2f);
        //mBB.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
	}
	
	//make sure begin_new_character is called before this
	//called by set_for_PLAY()
	void set_bb_for_play(float aBBOffset = 0)
	{
		float bottomVOffset = -50;
		//mBB.SoftScale = new Vector3(1,1,1);
		mBB.SoftPosition = mBBBasePosition + new Vector3(0,aBBOffset,0);
		mBBText.SoftPosition = mBB.SoftPosition + new Vector3(0,160,0);
		mBBScoreFrame.SoftPosition = mBB.SoftPosition + new Vector3(-350,bottomVOffset,0);
		mBBScoreText.SoftPosition = mBB.SoftPosition + new Vector3(-350,bottomVOffset-40,0);
		mBBLastPerformanceGraph.PerformanceGraph.SoftPosition = mBB.SoftPosition + new Vector3(150,bottomVOffset,0);
		mBBWarningText.HardPosition = mFlatCamera.Center;//mBB.SoftPosition + new Vector3(150,bottomVOffset-40,0);
		
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
	public void set_bb_decider_pose(ProGrading.Pose aPose)
	{
		mBBMiniMan.set_target_pose(aPose);
	}
	
	//called by ChoiceHelper
	public void set_bb_choice_poses(List<ProGrading.Pose> aPoses)
	{
		for(int i = 0; i < BB_NUM_CHOICES; i++)
		{
			mBBChoiceBodies[i].set_target_pose(aPoses[i]);
		}
	}
	public void set_bb_choice_perfectness(List<int> aDifficulties)
	{
		for(int i = 0; i < BB_NUM_CHOICES; i++)
		{
			mBBChoices[i].set_perfectness(aDifficulties[i]);
		}
	}
	//called by ChoiceHelper
	//this should really be called set_bb_choice_icons now
	public void set_bb_choice_bodies(CharacterIndex aIndex)
	{
		CharacterIndex index = new CharacterIndex(aIndex.Level+1,0);
		var all = index.NeighborsAndSelf;
		all.Add(index);
		for(int i = 0; i < 3; i++)
		{
			//mBBChoices[i].set_actual_character(mManager.mCharacterBundleManager.get_mini_character(all[i]));
			mBBChoices[i].Character = all[i];
			mBBChoices[i].set_actual_character(all[i]);
			mBBChoices[i].set_difficulty(mManager.mCharacterBundleManager.get_character_helper().Characters[all[i].Index].Difficulty);
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
			mBBMiniMan.SoftPosition = mBBMiniManBasePosition;
			mBBQuestionText.set_text(
				new string[]{("What will you be like at age " + mBBLastPerformanceGraph.Character.get_future_neighbor(0).Age) + "?"},
				new Color[]{new Color(0.5f,0.5f,0.5f,1)});
		}
		else{
			mBBMiniMan.SoftPosition = mBBChoiceBodies[aIndex].SoftPosition;
			var nChar = mBBLastPerformanceGraph.Character.get_future_neighbor(aIndex);
			var nCharDiff = mManager.mCharacterBundleManager.get_character_helper().Characters[nChar.Index];
			var diffPhrases = new string[]{" easy", " medium", " hard", " impossible"};
			var perfectPhrases = new string[]{" horrible", " passable", " perfect", " PERFECT"};
			var perfectColors = new Color[]{new Color32(200,173,27,255),new Color32(240,220,130,255),new Color32(253,238,0,255),new Color32(255,126,0,255)};
			var diffColors = new Color[]{new Color(0,0.8f,0,1), new Color(0.8f,0.8f,0,1), new Color(0.9f,0.4f,0,1), new Color(0.8f,0,0,1)};
			mBBQuestionText.set_text(
				//new string[]{("Will you be " + nChar.Description + "\nThat is a " can't do this because my multicolor font thing can't handle new line
				new string[]{("That is a "), 
					perfectPhrases[nCharDiff.Perfect], 
					Mathf.Abs(-nCharDiff.Difficulty - nCharDiff.Perfect) > 0 ? " but" : " and",
					diffPhrases[nCharDiff.Difficulty],
					" choice."},
				new Color[]{new Color(0.5f,0.5f,0.5f,1),
					diffColors[nCharDiff.Difficulty]/2f,
					new Color(0.5f,0.5f,0.5f,1),
					perfectColors[nCharDiff.Perfect]/2f,
					new Color(0.5f,0.5f,0.5f,1)});
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
	CharacterIconObject[] mPBCharacterIcons = new CharacterIconObject[31];
	
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
			mPBCharacterIcons[e.Index] = new CharacterIconObject(e,1);
			mPBCharacterIcons[e.Index].set_name(e.ShortName);
			mElement.Add(mPBCharacterIcons[e.Index]);
			
			
		}
		mElement.Add(mPB);
		
		set_pb_character_icon_colors(new List<CharacterStats>());
		
		position_pb_character_icons(0);
	}
	
	//OLD no longer need to set icon poses
	public void set_pb_character_icon_poses(List<KeyValuePair<CharacterIndex,ProGrading.Pose>> aChars)
	{
		
		foreach(var e in aChars)
		{
			//mPBCharacterIcons[e.Key.Index].mBody.set_target_pose(e.Value);
		}
	}
	
	
	
	public void set_pb_character_icon_colors(List<CharacterStats> aChars)
	{
		foreach(CharacterStats e in aChars)
		{
			//mPBCharacterIcons[e.Character.Index].set_background_color(Color.Lerp(new Color(0.5f,0.5f,0.5f), new Color32(255,200,0,255), e.Perfect/3f));
			mPBCharacterIcons[e.Character.Index].set_perfectness(e.Perfect);
			mPBCharacterIcons[e.Character.Index].set_difficulty(e.Difficulty);
		}
		
		//fetus
		mPBCharacterIcons[0].set_difficulty(-1);
		//top secret
		mPBCharacterIcons[29].SoftColor = new Color(1,1,1,0);
		mPBCharacterIcons[30].SoftColor = new Color(1,1,1,0);
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
			int indexOffset = (splitLevel - e.Level);
			position.y = hPadding*indexOffset + vOffset; 
			if(indexOffset > 0) //past characters
				position.y += splitHeight/2;
			else if (indexOffset < 0) //future characters
				position.y -= splitHeight/2;
			mPBCharacterIcons[e.Index].SoftPosition = baseOffset + position;
		}
		
	}
	
	public void add_cutscene_particle_stream(CharacterIndex aTarget, PopupTextObject aPopup, float duration, bool aPositive)
	{
		float delay = 0;
		Color useColor = (!aPositive) ? new Color(0.1f,0.7f,0.2f) : new Color(0.7f,0,0);
		if(mPBCharacterIcons[aTarget.Index] != null)
		{
			TED.add_one_shot_event(
				delegate()
				{
					//TODO proper color setting routines
					aPopup.set_background_color(useColor);
					mPBCharacterIcons[aTarget.Index].set_background_color(useColor);
				},
			delay).then_one_shot(
				delegate()
				{
					aPopup.set_background_color(new Color(0.5f,0.5f,0.5f));
					mPBCharacterIcons[aTarget.Index].set_background_color(new Color(0.5f,0.5f,0.5f));
				},
			duration);
			add_timed_particle_stream(
                mFlatCamera.get_point(0.40f,0),
                mPBCharacterIcons[aTarget.Index].SoftPosition,
                duration,
                delay,
                useColor);
		}
	}
	
	public void add_timed_particle_stream(Vector3 aPosition, Vector3 aTarget, float aDuration, float aDelay, Color aColor)
	{
		ParticleStreamObject pso = null;
		TED.add_one_shot_event(
			delegate()
			{
				pso = new ParticleStreamObject(3,aTarget);
                pso.HardColor = aColor;
				pso.HardPosition = aPosition;
				mElement.Add(pso);
			},
		aDelay).then_one_shot(
			delegate()
			{
				mElement.Remove(pso);
				pso.destroy();
			},
		aDuration);
	}
	
	//TEXT
	//TODO get rid ofthe stupid yreloffset parameter...
	public PopupTextObject add_timed_text_bubble(string aMsg, float duration, float yRelOffset = 0)
	{
		PopupTextObject to = new PopupTextObject(aMsg,6);
		to.HardPosition = random_position();
		to.set_text_color(new Color(0.1f,0.1f,0.5f,1));
		TimedEventDistributor.TimedEventChain chain = TED.add_event(
			delegate(float aTime)
			{
				to.SoftPosition = mFlatCamera.get_point(0.40f,yRelOffset);
				mElement.Add(to);
				return true;
			},
        0).then_one_shot(
			delegate()
			{
				//to.SoftPosition = random_position();
				to.SoftColor = new Color(1,1,1,0);
			},
		duration).then_one_shot(
			delegate()
			{
				mElement.Remove(to);
				to.destroy();
			},
		2);
		return to;
	}
	
	//this gets called during CHOOSE so BB should be full sized
	//this gets called by NewGameManager
	public void begin_new_character(PerformanceStats aChar)
	{
		//BB
		mBBText.Text = aChar.Character.FullName;
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
		position_pb_character_icons(aChar.Character.Level);
		//disable the other characters no that we have made a choice
		foreach(CharacterIndex e in aChar.Character.Neighbors)
		{
			if(mPBCharacterIcons[e.Index] != null)
				mPBCharacterIcons[e.Index].Enabled = false;
			//mPBCharacterIcons[e.Index].destroy();
			//mElement.Remove(mPBCharacterIcons[e.Index]);
			//mPBCharacterIcons[e.Index]=null;
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
		
		
		float gPerformanceText = 5f;
		float gCutsceneText = 6f;
		float gPreParticle = 1.5f;
		float gParticle = 3f;
		
		
		//this slows the game down a lot...
		set_bb_for_play(mPB.BoundingBox.height/2+205);
		position_pb_character_icons(mBBLastPerformanceGraph.Character.Level,mPB.BoundingBox.height/2+205);
		
		mLastCutsceneCompleteCb = delegate() {
			//this slows the ame down a lot
			set_bb_for_play();
			position_pb_character_icons(mBBLastPerformanceGraph.Character.Level,0);
			cutsceneCompleteCb();
			mLastCutsceneCompleteCb = null;
			mLastCutsceneChain = null;
		};
		
		string[] perfectPhrase = {"awful","mediocre","good", "perfect"};
		string[] performancePhrase = {"miserably","poorly","well", "excellently"};

		TimedEventDistributor.TimedEventChain chain = TED.add_event(
			delegate(float aTime)
			{
				string text = "";
				if(mBBLastPerformanceGraph.Character.Index == 0 || mBBLastPerformanceGraph.Character.Index == 29)
				{
					//DELETE this has been moved to text files..
					//text = "Prepare to be Born"; 
				}
				else
				{
					//TODO use color text here... In fact you should replace color text as yoru standard text object really...
					text = aChanges.PerformanceDescription.Replace("<P>",perfectPhrase[mBBLastPerformanceGraph.Stats.Perfect]);
					add_timed_text_bubble(text,gPerformanceText);
				}
				return true;
			},
        0).then( 
			delegate(float aTime)
			{
				if(!(mBBLastPerformanceGraph.Character.Index == 0 || mBBLastPerformanceGraph.Character.Index == 29))
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
					po = add_timed_text_bubble(changeMsg,gCutsceneText);
					return true;
				}
			,0).then(
				delegate(float aTime)
				{
					for(int i = 0; i < diffChanges.Length; i++)
					{
						if(diffChanges[i] != 0){
                        	var cchar = new CharacterIndex(i);
							add_cutscene_particle_stream(cchar,po,gParticle,changes.is_positive());
							int nDiff = Mathf.Clamp(mManager.mGameManager.get_character_difficulty(cchar) + diffChanges[i], 0, 3);
							mManager.mGameManager.change_interface_pose(cchar,nDiff);
                        	mPBCharacterIcons[cchar.Index].set_difficulty(nDiff);
						}
					}
					return true;
				}
			,gPreParticle).wait (gCutsceneText-gPreParticle);
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
		
		if(aChar.Level == 7)
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
		else if (aChar.Level == 8)
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
		
		/*
		//fake it for testing...
		for(int i = 0; i < 8; i++)
		{
			if(aStats.Last().Character.Age < (new CharacterIndex(i,0)).Age)
			{
				aStats.Add(new PerformanceStats(new CharacterIndex(i,0)));
			}
		}*/
		
		
		
		//clear away BB and PB
		var smallColor = new Color(1,1,1,0);
		mBB.SoftColor = smallColor;
		mBBText.SoftColor = smallColor;
		mBBScoreFrame.SoftColor = smallColor;
		mBBScoreText.SoftColor = smallColor;
		aStats[0].PerformanceGraph.SoftColor = smallColor;
		
		position_pb_character_icons(0,-3000);
		
		
		//this is all a hack to get the score to show up right...
		float scoreIncrementor = 0;
		FlatElementText finalScoreText = new FlatElementText(mManager.mNewRef.genericFont,150,"",10);
		foreach (Renderer f in finalScoreText.PrimaryGameObject.GetComponentsInChildren<Renderer>())
                f.gameObject.layer = 4;
		finalScoreText.SoftPosition = mManager.mBackgroundManager.mBackgroundElements.mElements[0].Element.SoftPosition + new Vector3(0, -200, 0);
		mElement.Add(finalScoreText);
		
		List<Vector3> ghostPositions = new List<Vector3>();
		for(int i = 1; i < aStats.Count; i++)
		{
			//TODO
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
			CharacterIconObject cio = mPBCharacterIcons[ps.Character.Index];
			PerformanceGraphObject pgo = ps.PerformanceGraph;
			cio.HardPosition = new Vector3(cioXOffset,-2000,0);
			pgo.HardPosition = new Vector3(pgoXOffset,-2000,0);
			pgo.HardColor = new Color(0.5f,0.5f,0.5f,1);
			
			string[] perfectPhrase = {"awful","mediocre","good", "perfect"};
			string[] performancePhrase = {"miserably","poorly","well", "excellently"};
			chain = chain.then_one_shot(
				delegate()
				{
					//set the text
					string text = "You lived your ";
					text += perfectPhrase[ps.Stats.Perfect];
					text += " life as a ";
					text += ps.Character.FullName;
					text += " " + performancePhrase[Mathf.Clamp((int)(Mathf.Sqrt(ps.Score)*4),0,3)];
					add_timed_text_bubble(text,gCharacterText,0.5f);
				
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
								mPBCharacterIcons[e.Character.Index].SoftPosition = mPBCharacterIcons[e.Character.Index].SoftPosition - scroll;
								e.PerformanceGraph.SoftPosition = e.PerformanceGraph.SoftPosition - scroll;
							}
						}
					}
				} 
			).then(
				delegate(float aTime)
				{
					float displayScore = scoreIncrementor + (aTime/gScoreCount)*ps.AdjustedScore;
					finalScoreText.Text = ""+(int)displayScore;
					if(aTime >  gScoreCount)
					{
						scoreIncrementor += ps.AdjustedScore;
						return true;
					}
					return false;
					
				},
			gPreScoreCount).then(
				delegate(float aTime)
				{
					//TODO render mini character with golry hoooooo sound
					//mManager.mMusicManager.play_sound_effect("graveAngel");
					return true;
				},
			gPostScoreCount);
		}
		
		//variables for credits animation..
		float lastTime = 0;
		FlatElementImage logo1 = null;
		FlatElementImage logo2 = null;
		List<FlatElementText> creditsText = new List<FlatElementText>();
		float scrollSpeed = 300;
		
		//finish it off...
		chain = chain.then_one_shot(
			delegate()
			{
				add_timed_text_bubble("G A M E  O V E R",99999,0.5f);
			}
		,0).then_one_shot(
			delegate()
			{
				//TODO create credits text and nonsense
				int counter = 0;
				foreach(string e in GameConstants.credits)
				{
					var text = new FlatElementText(mManager.mNewRef.genericFont,50,e,mPB.Depth +1);
					text.HardColor = new Color(1,1,1,1);
					text.HardPosition = mPB.SoftPosition + new Vector3(0,mFlatCamera.Height/2+400,0) + (new Vector3(0,150,0))*counter;
					creditsText.Add(text);
					mElement.Add(text);
					counter++;
				}
			}
		,0).then(
			delegate(float aTime)
			{
				
				//scroll contents down
				Vector3 scroll = -new Vector3(0,scrollSpeed*(aTime-lastTime),0);
				foreach(var e in aStats)
				{
					mPBCharacterIcons[e.Character.Index].SoftPosition = mPBCharacterIcons[e.Character.Index].SoftPosition + scroll;
					e.PerformanceGraph.SoftPosition = e.PerformanceGraph.SoftPosition + scroll;
				}
				foreach(FlatElementText e in creditsText)
				{
					e.SoftPosition = e.SoftPosition + scroll;
				}
				lastTime = aTime;
				if(Input.GetKeyDown(KeyCode.Alpha0))
					return true;
				return false;
			}
		,0).then_one_shot(
			graveCompleteCb
		,gRestart);
	}
	
	
	
	//GLORY
	public void add_glory_character(){}
	
	
	
	
	
}
	