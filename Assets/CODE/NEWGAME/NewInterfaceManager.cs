using UnityEngine;
using System.Collections.Generic;

public class NewInterfaceManager : FakeMonoBehaviour {
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
        mMiniMan = ((GameObject)GameObject.Instantiate(ManagerManager.Manager.mMenuReferences.miniMan)).GetComponent<CharacterTextureBehaviour>();        
		
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
		//TODO if PLAY update graph
    }
    
    Vector3 random_position()
    {
        //UGG piece of junk...
        return (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)).normalized * Random.Range(2000,20000);
    }
	
	//BLUE BAR
	FlatElementImage mBB;
	//PLAY
	PerformanceGraphObject mBBLastPerformanceGraph = null; //owned by Character
	FlatElementImage mBBPerformanceGraphFrame;
	FlatElementText mBBText;
	FlatElementImage mBBScoreFrame;
	FlatElementText mBBScoreText;
	//CUTSCENE
	//???
	//CHOOSING
	int BB_NUM_CHOICES = 3;
	List<NewChoiceObject> mBBChoices = new List<NewChoiceObject>();
	List<FlatBodyObject> mBBChoiceBodies = new List<FlatBodyObject>();
	FlatBodyObject mBBMiniMan;
	Vector3 mBBMiniManBasePosition;
	FlatElementImage mBBChoiceBox;
	
	//called by NewGameManager
	public void setup_bb()
	{
		var newRef = mManager.mNewRef;
		MenuReferenceBehaviour menuRef = mManager.mMenuReferences;
		var refs = mManager.mReferences;
		
		mBB = new FlatElementImage(mManager.mNewRef.bbBackground,8);
		mBB.HardPosition = random_position();
		mElement.Add(mBB);
		
		//BB small nonsense
		mBBText = new FlatElementText(mManager.mNewRef.genericFont,300,"",10);
		mBBScoreFrame = new FlatElementImage(mManager.mNewRef.bbScoreBackground,9);
		mBBScoreText  = new FlatElementText(mManager.mNewRef.genericFont,300,"0",10);
		mBBPerformanceGraphFrame = new FlatElementImage(mManager.mNewRef.bbGraphBackground,9);
		mBBText.HardPosition = random_position();
		mBBScoreFrame.HardPosition = random_position();
		mBBScoreText.HardPosition = random_position();
		mBBPerformanceGraphFrame.HardPosition = random_position();
		mElement.Add(mBBText);
		mElement.Add(mBBScoreFrame);
		mElement.Add(mBBScoreText);
		mElement.Add(mBBPerformanceGraphFrame);
		
		
		
		//BB choice nonsense
		var miniMan = ((GameObject)GameObject.Instantiate(menuRef.miniMan)).GetComponent<CharacterTextureBehaviour>();
		Vector3 miniManScale = new Vector3(2,2,1);
		float padding = 600;
		float netWidth = (BB_NUM_CHOICES)*padding;
		for(int i = 0; i < BB_NUM_CHOICES; i++)
		{
			mBBChoices.Add(new NewChoiceObject(11));
			mBBChoiceBodies.Add(new FlatBodyObject(miniMan,12));
			float xOffset = netWidth/2 - padding*i;
			mBBChoices[i].HardPosition = mFlatCamera.get_point(0, 0) + new Vector3(xOffset,0,0);
			mBBChoiceBodies[i].HardShader = refs.mMiniCharacterShader;
			mBBChoiceBodies[i].HardPosition = mFlatCamera.get_point(0, 0) + new Vector3(xOffset,-180,0);
			mBBChoiceBodies[i].HardScale = miniManScale;
			mElement.Add(mBBChoices[i]);
			mElement.Add(mBBChoiceBodies[i]);
		}
		
		mBBMiniMan = new FlatBodyObject(miniMan,10);
		mBBMiniMan.HardScale = miniManScale;
		mBBChoiceBox = new FlatElementImage(newRef.bbChoiceFrame,15);
		mBBMiniManBasePosition = mFlatCamera.get_point(0, 0) + new Vector3(netWidth/2 - padding*3,0,0);
		mBBMiniMan.HardPosition = mBBMiniManBasePosition;
		mBBChoiceBox.HardPosition = mBBMiniMan.SoftPosition;
		
		mElement.Add(mBBMiniMan);
		mElement.Add(mBBChoiceBox);
		
		GameObject.Destroy(mMiniMan.gameObject);
	}
	
	//called by set_bb_small/full
	void fade_bb_contents(bool small)
	{
		Color smallColor = small ? new Color(1,1,1,1) : new Color(1,1,1,0);
		Color fullColor = !small ? new Color(1,1,1,1) : new Color(1,1,1,0);
	
		mBBText.SoftColor = smallColor;
		mBBScoreFrame.SoftColor = smallColor;
		mBBScoreText.SoftColor = smallColor;
		mBBLastPerformanceGraph.SoftColor = smallColor;
		
		foreach(FlatBodyObject e in mBBChoiceBodies)
			e.SoftColor = fullColor;
		foreach(NewChoiceObject e in mBBChoices)
			e.SoftColor = fullColor;
		mBBMiniMan.SoftColor = fullColor;
		mBBChoiceBox.SoftColor = fullColor;
	}
	
	//make sure choice contents are made first before calling this
	//called by set_for_CHOICE()
	public void set_bb_full_size()
	{
		Vector2 baseSize = new Vector2(mBB.BoundingBox.width,mBB.BoundingBox.height);
		Vector2 desiredSize = new Vector2(mFlatCamera.Width+200,mFlatCamera.Height+200);
		mBB.SoftScale = new Vector3(desiredSize.x/baseSize.x,desiredSize.y/baseSize.y,1);
		mBB.SoftPosition = mFlatCamera.get_point(0, 0);
		fade_bb_contents(false);
	}
	
	//make sure begin_new_character is called before this
	//called by set_for_PLAY()
	void set_bb_small()
	{
		mBB.set_scale(new Vector3(1,1,1));
		mBB.SoftPosition = mFlatCamera.get_point(-0.5f, 0) + new Vector3(0,-150,0);
		mBBText.SoftPosition = mBB.SoftPosition + new Vector3(0,100,0);
		mBBScoreFrame.SoftPosition = mBB.SoftPosition + new Vector3(-250,-50,0);
		mBBScoreText.SoftPosition = mBB.SoftPosition + new Vector3(-250,-50,0);
		mBBLastPerformanceGraph.SoftPosition = mBB.SoftPosition + new Vector3(100,-50,0);
		
		fade_bb_contents(true);
	}
	//called by NewGameManager
	public void update_bb_score(float aScore)
	{
		mBBScoreText.Text = ((int)aScore).ToString();
	}
	//called by NewGameManager
	public void set_bb_choice_poses(List<ProGrading.Pose> aPoses)
	{
		for(int i = 0; i < BB_NUM_CHOICES; i++)
		{
			mBBChoiceBodies[i].set_target_pose(aPoses[i]);
		}
	}
	//called by ChoiceHelper
	public void set_bb_choice_bodies(CharacterIndex aIndex)
	{
		CharacterIndex index = new CharacterIndex(aIndex.Level +1,0);
		var all = index.Neighbors;
		all.Add(index);
		for(int i = 0; i < all.Count; i++)
		{
			mBBChoices[i].set_actual_character(mManager.mCharacterBundleManager.get_mini_character(all[i]));
		}
	}
	//this is the character that is curretnly being selected
	//called by ChoiceHelper
	public void set_bb_choice(int aIndex)
	{
		if(aIndex == -1) //no choice
		{
			mBBMiniMan.SoftPosition = mBBMiniManBasePosition;
			mBBChoiceBox.SoftPosition = mBBMiniManBasePosition;
		}
		else{
			mBBMiniMan.SoftPosition = mBBChoiceBodies[aIndex].SoftPosition;
			mBBChoiceBox.SoftPosition = mBBChoices[aIndex].SoftPosition;
		}
	}
	//called by ChoiceHelper
	public void set_bb_choice_percentages(CharacterIndex aIndex, float aPercent)
	{
		int index = aIndex.Choice == 3 ? 2 : aIndex.Choice; //TODO delete this
		mBBChoices[index].Percentage = aPercent;
		mBBMiniMan.SoftPosition = mBBChoiceBodies[index].SoftPosition;
		mBBChoiceBox.HardPosition = new Vector3(mBBMiniMan.SoftPosition.x,mBBChoiceBox.HardPosition.y, mBBChoiceBox.HardPosition.z);
	}
	
	
	
	
	//PINK BAR
	FlatElementImage mPB;
	CharacterIconObject[] mPBCharacterIcons = new CharacterIconObject[31];
	
	//called by NewGameManager
	public void setup_pb()
	{
		var newRef = mManager.mNewRef;
		mPB = new FlatElementImage(newRef.pbBackground,0);
		mPB.HardPosition = random_position();
		mPB.SoftPosition = mFlatCamera.get_point(-0.5f, 0);
		
		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
			mPBCharacterIcons[e.Index] = new CharacterIconObject(
					mManager.mCharacterBundleManager.get_mini_character(e),5);
			mElement.Add(mPBCharacterIcons[e.Index]);
		}
		mElement.Add(mPB);
		
		position_pb_character_icons();
	}
	
	public void position_pb_character_icons()
	{
		float padding = 100;
		float hPadding = 150;
		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
			Vector3 baseOffset =  mPB.SoftPosition; //TODO overall offset
			Vector3 position = Vector3.zero;
			float netWidth = (e.NumberInRow - 1)*padding;
			position.x = netWidth/2f - padding*e.Choice;
			position.y = -hPadding*e.Level; // TODO make space for blue bar
			mPBCharacterIcons[e.Index].SoftPosition = baseOffset + position;
		}
		
	}
	
	
	//TODO positioning helpers
	
	//TEXT
	public void add_timed_text_bubble(string aMsg, float duration)
	{
		PopupTextObject to = new PopupTextObject(null,10);
		to.HardPosition = random_position();
		to.Text = aMsg; //TODO more than one line??
		TimedEventDistributor.TimedEventChain chain = TED.add_event(
			delegate(float aTime)
			{
				//TODO set message
				to.SoftPosition = mFlatCamera.Center + new Vector3(400,0,0);
				mElement.Add(to);
				return true;
			},
        0).then_one_shot(
			delegate()
			{
				//to.SoftPosition = random_position();
				to.SoftColor = new Color32(1,1,1,0);
				to.SoftInterpolation = 0.05f;
			},
		duration).then_one_shot(
			delegate()
			{
				mElement.Remove(to);
				to.destroy();
			},
		2);
		
	}
	
	//this gets called during CHOOSE so BB should be full sized
	//this gets called by NewGameManager
	public void begin_new_character(PerformanceStats aChar)
	{
		//BB
		mBBText.Text = "CHARACTER " + aChar.Character.StringIdentifier;
		
		PerformanceGraphObject aGraph = aChar.PerformanceGraph;
		if(mBBLastPerformanceGraph != null)
		{
			mBBLastPerformanceGraph.SoftColor = new Color(1,1,1,0);
			//mBBLastPerformanceGraph.Enabled = false;
			//mElement.Remove(mBBLastPerformanceGraph);
		}
		mBBLastPerformanceGraph = aGraph;
		mElement.Add(mBBLastPerformanceGraph);
		
		//PB
		//disable the other characters no that we have made a choice
		foreach(CharacterIndex e in aChar.Character.Neighbors)
		{
			mPBCharacterIcons[e.Index].Enabled = false;
			//mPBCharacterIcons[e.Index].destroy();
			//mElement.Remove(mPBCharacterIcons[e.Index]);
		}
	}
	
	
	public void set_for_PLAY()
	{
		set_bb_small();
	}
	
	
	//TODO This should take list of changes as argument
	public void set_for_CUTSCENE(System.Action cutsceneCompleteCb)
	{
		//used for skipping cutscene
		TED.add_event(
			delegate(float aTime)
			{
				add_timed_text_bubble("CUTSCENE HERE",1);
				return true;
			},
        0).then_one_shot( //dummy 
			delegate(){cutsceneCompleteCb();},1);
		return;
		
		
		
		
		//TODO rearange INTERFACE
			//shift BB up
			//shift PB contents up
		
		//TODO get actual message
		float firstTextTime = 3;
		TimedEventDistributor.TimedEventChain chain = TED.add_event(
			delegate(float aTime)
			{
				add_timed_text_bubble("CUTSCENE BEGIN",firstTextTime);
				return true;
			},
        0).then_one_shot( //dummy 
			delegate(){},firstTextTime);
		
		//TODO get actual cutscenes
		List<int> poo = new List<int>(){0,1,2,3}; //placeholder for cutscene
		for(int i = 0; i < poo.Count; i++)
		{
			float cutsceneTextTime = 5;
			chain = chain.then(
				delegate(float aTime)
				{
					//TODO set message
					add_timed_text_bubble("MESSAGE " + i,cutsceneTextTime);
					return true;
				},
			cutsceneTextTime/2f).then(
				delegate(float aTime)
				{
					//TODO particle effects
					add_timed_text_bubble("particles",1);
					return true;
				},
			cutsceneTextTime/2f).then(
				delegate(float aTime)
				{
					add_timed_text_bubble("noparts",1);
					return true;
				},
			0);
		}
		
		chain.then_one_shot(delegate(){cutsceneCompleteCb();});
	}
	
	public void set_for_CHOICE()
	{
		//TODO
		set_bb_full_size();
	}
	
	//returns amount of time this will take
	public TimedEventDistributor.TimedEventChain set_for_DEATH(CharacterIndex aChar)
	{
		TimedEventDistributor.TimedEventChain chain;
		float textTime = 3;
		if(aChar.Level == 7)
		{
			//80
			chain = TED.add_one_shot_event(
				delegate()
				{
					add_timed_text_bubble("It's time for you to\ndie a natural death",textTime);
				},
	        0).then_one_shot( //dummy 
			delegate(){},textTime);
		}
		else if (aChar.Level == 8)
		{
			//100!!!
			chain = TED.add_one_shot_event(
				delegate()
				{
					add_timed_text_bubble("Congragulations",textTime);
				},
	        0).then_one_shot(
				delegate()
				{
					add_timed_text_bubble("Your life was\nmay not have been\nperfect",textTime);
				},
			textTime).then_one_shot(
				delegate()
				{
					add_timed_text_bubble("but you lived\n100 years",textTime);
				},
			textTime).then_one_shot( //dummy 
				delegate(){},textTime);
		}
		else
		{
			chain = TED.add_one_shot_event(
				delegate()
				{
					add_timed_text_bubble("You die\nat the age of\n" + aChar.Age,textTime);
				},
	        1).then_one_shot(
				delegate()
				{
					//TODO pink bar animations
				}
			).then_one_shot( //dummy 
				delegate(){},0);
		}
		
		return chain;
	}
	public void set_for_GRAVE(List<FlatGraphElement> aGraphs)
	{
		//TODO
	}
	
	
	
	//GLORY
	public void add_glory_character(){}
	
	
	
	
	
}
