using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NewInterfaceManager {
	static float END_CUTSCENE_DELAY_TIME = 1;
	
	
	ManagerManager mManager;
	ModeNormalPlay mModeNormalPlay;
    public NewInterfaceManager(ManagerManager aManager,ModeNormalPlay aPlay)
	{
		mManager = aManager;
		mModeNormalPlay = aPlay;
	}

	public TimedEventDistributor TED { get; private set; }
    public FlatCameraManager mFlatCamera;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();

	CharacterHeadPopupThingy mHeadPop;
	
    CharacterTextureBehaviour mMiniMan;
	FlatBodyObject mCurrentBody = null;

	
	public void initialize()
    {
		TED = new TimedEventDistributor();
        mFlatCamera = new FlatCameraManager(new Vector3(50000, 10000, 0), 9);
		mFlatCamera.fit_camera_to_screen();
		//mFlatCamera.fit_camera_to_game();
        mMiniMan = ((GameObject)GameObject.Instantiate(ManagerManager.Manager.mReferences.mMiniChar)).GetComponent<CharacterTextureBehaviour>();        
		//mMiniMan = //TODO something like this: mManager.mCharacterBundleManager.get_mini_character(new CharacterIndex(0,1));

		mHeadPop = new CharacterHeadPopupThingy(this);
    }
    public void Update()
    {
		
        mFlatCamera.update(Time.deltaTime);
        if (mCurrentBody != null)
            mCurrentBody.match_body_to_projection(mManager.mProjectionManager);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		
		TED.update(Time.deltaTime);
		
		mHeadPop.update();
    }
    
    Vector3 random_position()
    {
        //UGG piece of junk...
        return (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)).normalized * Random.Range(2000,20000);
    }

	FlatElementImage mBBNameTextFrame;
	FlatElementText mBBNameText;
	FlatElementImage mBBScoreFrame;
	FlatElementText mBBScoreText;
	FlatElementImage mBBMultiplierImage; //replace with difficulty image

	FlatElementText mBBWarningText = null;

	
	
	//called by NewGameManager
	public void setup_bb()
	{
		var newRef = mManager.mNewRef;
		var refs = mManager.mReferences;

		var nameFrame = mManager.mCharacterBundleManager.get_image("TEXTBOX-01");
		var scoreFrame = mManager.mCharacterBundleManager.get_image("TEXTBOX-02");

		
		//BB small nonsense
		mBBNameText = new FlatElementText(mManager.mNewRef.genericFont,60,"",11);
		mBBNameTextFrame = new FlatElementImage(nameFrame.Image,10);
		mBBScoreText = new FlatElementText(mManager.mNewRef.genericFont,60,"0",11);
		mBBScoreFrame = new FlatElementImage(scoreFrame.Image,10);
		mBBMultiplierImage = new FlatElementImage(null,15);

		//mBBWarningText = new FlatElementText(mManager.mNewRef.genericFont,150,"WARNING",12);
		mBBWarningText = new FlatElementText(mManager.mNewRef.genericFont,400,"WARNING",20);
		mBBWarningText.HardColor = new Color(0.5f,0.5f,0.5f,0);
		mBBWarningText.HardPosition = mFlatCamera.Center;


		mBBNameText.HardColor = GameConstants.UiPink;
		//mBBNameText.Alignment = TextAlignment.Left;
		//mBBNameText.Anchor = TextAnchor.MiddleLeft;
		mBBScoreText.HardColor = GameConstants.UiPink;
		mBBMultiplierImage.HardPosition = mFlatCamera.get_point(-1,1) + new Vector3(200,-200,0) 
			+ new Vector3(mBBMultiplierImage.BoundingBox.width, mBBMultiplierImage.BoundingBox.height,0)/2f; 

		mBBNameTextFrame.HardPosition = mBBMultiplierImage.HardPosition + new Vector3(mBBNameTextFrame.BoundingBox.width+40,170,0)/2;
		mBBNameText.HardPosition = mBBNameTextFrame.HardPosition;
		mBBScoreFrame.HardPosition = mBBMultiplierImage.HardPosition + new Vector3(mBBScoreFrame.BoundingBox.width+40,-110,0)/2;
		mBBScoreText.HardPosition = mBBScoreFrame.HardPosition;

		

		mElement.Add(mBBNameText);
		mElement.Add(mBBNameTextFrame);
		mElement.Add(mBBScoreFrame);
		mElement.Add(mBBScoreText);
		mElement.Add(mBBWarningText);
		mElement.Add(mBBMultiplierImage);
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
	
	public void set_popup_color_for_cutscene_particles(PopupTextObject aPopup, bool aPositive)
	{
		Color useColor = (!aPositive) ? GameConstants.ParticleStreamEasy : GameConstants.ParticleStreamHard;
		if(aPositive)
			aPopup.set_text_color(GameConstants.UiWhite,true);
		aPopup.set_background_color(useColor,true);
	}
	
	
	public void add_cutscene_particle_stream_new(CharacterIndex aTarget, PopupTextObject aPopup, float duration, bool aPositive)
	{
		//TODO keep or delete
		//pop little guys up from bottom
		//do a little bing with badges displaying new difficulty with sound effects
		//move little guys back down
	}


	//TEXT
	//TODO get rid ofthe stupid yreloffset parameter...
	public PopupTextObject add_timed_text_bubble(string aMsg, float duration, float yRelOffset = 0)
	{
		PopupTextObject to = new PopupTextObject(aMsg,30);
		to.HardPosition = random_position();
		to.HardColor = GameConstants.UiWhiteTransparent;
		to.SoftColor = GameConstants.UiWhite;
		to.set_text_color(GameConstants.UiWhiteTransparent,true);
		to.set_text_color(GameConstants.UiRed);
		to.set_background_color(GameConstants.UiWhiteTransparent,true);
		to.set_background_color(GameConstants.UiPopupBubble);
		TimedEventDistributor.TimedEventChain chain = TED.add_event(
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
	
	public System.Func<float,bool> skippable_text_bubble_event(string aText, float displayDur)
	{
		System.Func<float,bool> skip_del = null;
		PopupTextObject po = null;
		return delegate(float aTime){
			if(po == null)
			{
				po = add_timed_text_bubble(aText,displayDur);
				skip_del = PopupTextObject.skip(displayDur,po);
			}
			return skip_del(aTime);
		};
	}
	
	//this gets called during CHOOSE so BB should be full sized
	//this gets called by NewGameManager
	public void begin_new_character(PerformanceStats aChar)
	{
		mBBNameText.Text = aChar.Character.Description + " (" + aChar.Character.Age.ToString() + ")";//FlatElementText.convert_to_multiline(aChar.Character.Description.Length > 20 ? 2 : 1 ,aChar.Character.Description + " (" + aChar.Character.Age.ToString() + ")");
		if(aChar.Character.LevelIndex != 0)
		{
			string[] labelNames = new string[]{"label_easy_BIG","label_normal_BIG","label_hard_BIG","label_extreme_BIG"};
			var diffImage = mManager.mCharacterBundleManager.get_image(labelNames[aChar.Stats.Difficulty]);
			mBBMultiplierImage.set_new_texture(diffImage.Image,diffImage.Data.Size);
			//mBBMultiplierImage.set_new_texture(mManager.mNewRef.bbChoicePerfectIcons[aChar.Stats.Difficulty]);
		}
	}
	



	//these are hacks to allow me to skip cutscenes
	QuTimer mLastCutsceneChain = null;
	System.Action mLastCutsceneCompleteCb = null;

	public void skip_cutscene()
	{
		if(mLastCutsceneCompleteCb != null && mLastCutsceneChain != null)
		{
			TED.remove_event(mLastCutsceneChain);
			mLastCutsceneCompleteCb();
			mLastCutsceneChain = null;
			mLastCutsceneCompleteCb = null;
		}
	}

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
		
		float gStartCutsceneDelay = 4f;
		float gPerformanceText = 4f;
		float gCutsceneText = 5f;
		float gPreParticle = 1.5f;
		float gParticle = 2f;

		float gBubblePos = 0.2f;

		
		mLastCutsceneCompleteCb = delegate() {
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
				if(mModeNormalPlay.CurrentPerformanceStat.Character == CharacterIndex.sFetus)
				{
					//DELETE this has been moved to text files..
					//text = "Prepare to be Born"; 
				}
				else if(mModeNormalPlay.CurrentPerformanceStat.DeathTime == -1) //if we did not die this time
				{
					//TODO use color text here... In fact you should replace color text as yoru standard text object really...
					//text = aChanges.PerformanceDescription.Replace("<P>",perfectPhrase[mBBLastPerformanceGraph.Stats.Perfect]);
					/*string noCapsDescription = mBBLastPerformanceGraph.Character.Description.ToLower();
					if(mBBLastPerformanceGraph.Character.IsDescriptionAdjective)
						text = "You lived your life " + noCapsDescription + " " + performancePhrase[(int)Mathf.Clamp(mBBLastPerformanceGraph.Score*4,0,3)] + ".";
					else
						text = "You lived your life as a " + noCapsDescription + " " + performancePhrase[(int)Mathf.Clamp(mBBLastPerformanceGraph.Score*4,0,3)] + ".";*/
					introPo = add_timed_text_bubble(aChanges.PerformanceDescription.ToUpper(),gPerformanceText);
				}
				return true;
			},
        gStartCutsceneDelay).then( 
			delegate(float aTime)
			{
				if(introPo != null && introPo.IsDestroyed)
					return true;
				if(!(mModeNormalPlay.CurrentPerformanceStat.Character == CharacterIndex.sFetus))
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
					{
						po = add_timed_text_bubble(changeMsg,gCutsceneText,gBubblePos);
					
						//dumb stuff I need to make sure there was actually a change
						foreach(CharacterIndex cchar in CharacterIndex.sAllCharacters)
							if(diffChanges[cchar] != 0)
							{
								set_popup_color_for_cutscene_particles(po,changes.is_positive());
								break;
							}
					}
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
						List<CharacterIndex> aChangedChars = new List<CharacterIndex>();
						List<int> aDiffs = new List<int>();
						foreach(CharacterIndex cchar in CharacterIndex.sAllCharacters)
							if(diffChanges[cchar] != 0)
							{
								aChangedChars.Add(cchar);
								int nDiff = Mathf.Clamp(mManager.mGameManager.get_character_difficulty(cchar) + diffChanges[cchar], 0, 3);
								aDiffs.Add(nDiff);
							}
						mHeadPop.popup_character(aChangedChars.ToArray(),aDiffs.ToArray());
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
	
	
	//returns amount of time this will take
	public TimedEventDistributor.TimedEventChain set_for_DEATH(CharacterIndex aChar)
	{
		float gTextTime = 4;
		
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
}
	