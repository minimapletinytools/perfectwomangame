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
    
    public Vector3 random_position()
    {
        //UGG piece of junk...
        return (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)).normalized * Random.Range(2000,20000);
    }
	
	
	//BLUE BAR
	FlatElementImage mBB;
	//PLAY
	FlatGraphElement mBBLastPerformanceGraph = null; //owned by Character
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
	FlatElementImage mBBChoiceBox;
	
	public void setup_bb()
	{
		var newRef = mManager.mNewRef;
		MenuReferenceBehaviour menuRef = mManager.mMenuReferences;
		var refs = mManager.mReferences;
		
		
		mBB = new FlatElementImage(mManager.mNewRef.bbBackground,8);
		mBB.HardPosition = random_position();
		mElement.Add(mBB);
		
		//BB small nonsense
		mBBText = new FlatElementText(mManager.mNewRef.genericFont,25,"",10);
		mBBScoreFrame = new FlatElementImage(mManager.mNewRef.bbScoreBackground,9);
		mBBScoreText  = new FlatElementText(mManager.mNewRef.genericFont,25,"0",10);
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
		mBBMiniMan = new FlatBodyObject(miniMan,10);
		mBBChoiceBox = new FlatElementImage(newRef.bbChoiceBox,15);
		for(int i = 0; i < BB_NUM_CHOICES; i++)
		{
			mBBChoices.Add(new NewChoiceObject(11));
			mBBChoiceBodies.Add(new FlatBodyObject(miniMan,11));
		}
		
		
	}
	public FlatGraphElement set_bb_graph(FlatGraphElement aGraph)
	{
		FlatGraphElement r = mBBLastPerformanceGraph;
		if(mBBLastPerformanceGraph != null)
		{
			mBBLastPerformanceGraph.SoftColor = new Color(1,1,1,0);
			//mBBLastPerformanceGraph.Enabled = false;
			//mElement.Remove(mBBLastPerformanceGraph);
		}
		mBBLastPerformanceGraph = aGraph;
		//TODO
		//mBBLastPerformanceGraph.SoftPosition = 
		
		return r;
	}
	
	public void set_bb_choice_poses(List<ProGrading.Pose> aPoses)
	{
		for(int i = 0; i < BB_NUM_CHOICES; i++)
		{
			mBBChoiceBodies[i].set_target_pose(aPoses[i]);
		}
	}
	public void set_bb_choice_bodies(List<CharacterLoader> aBodies)
	{
		for(int i = 0; i < BB_NUM_CHOICES; i++)
		{
			mBBChoices[i].set_actual_character(aBodies[i]);
		}
	}
	
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
	public void set_bb_full_size()
	{
		Vector2 baseSize = new Vector2(mBB.BoundingBox.width,mBB.BoundingBox.height);
		Vector2 desiredSize = new Vector2(mFlatCamera.Width+30,mFlatCamera.Height+30);
		mBB.set_scale(new Vector3(desiredSize.x/baseSize.x,desiredSize.y/baseSize.y,1));
		
		//TODO CHOICE contents
		
		fade_bb_contents(false);
	}
	
	//make sure set_bb_graph is called before this
	public void set_bb_small()
	{
		mBB.set_scale(new Vector3(1,1,1));
		
		mBB.SoftPosition = mFlatCamera.get_point(-0.5f, 0) + new Vector3(0,-150,0);
		mBBText.SoftPosition = mBB.SoftPosition + new Vector3(0,100,0);
		mBBScoreFrame.SoftPosition = mBB.SoftPosition + new Vector3(-250,-50,0);
		mBBScoreText.SoftPosition = mBB.SoftPosition + new Vector3(-250,-50,0);
		mBBLastPerformanceGraph.SoftPosition = mBB.SoftPosition + new Vector3(100,-50,0);
		
		fade_bb_contents(true);
	}
	
	public void update_bb_score(float aScore)
	{
		mBBScoreText.Text = ((int)aScore).ToString();
	}
	
	
	
	//PINK BAR
	
	
	//TEXT
	public void add_timed_text_bubble(string aMsg, float duration)
	{
		PopupTextObject to = new PopupTextObject(null,10);
		to.HardPosition = random_position();
		to.set_text_one_line(aMsg); //TODO more than one line??
		TimedEventDistributor.TimedEventChain chain = TED.add_event(
			delegate(float aTime)
			{
				//TODO set message
				to.SoftPosition = mFlatCamera.Center + new Vector3(-500,0,0);
				mElement.Add(to);
				return true;
			},
        0).then_one_shot(
			delegate()
			{
				to.SoftPosition = random_position();
			},
		duration).then_one_shot(
			delegate()
			{
				mElement.Remove(to);
			},
		2);
		
	}
	
	//GLORY
	public void add_glory_character(){}
	
	
	
	//TODO This needs to handle creating a new graphobject thingy maybe
	public void set_for_PLAY()
	{
		set_bb_small();
		//transition in BB contents
	}
	
	
	//TODO This should take list of changes as argument
	public void set_for_CUTSCENE(System.Action cutsceneCompleteCb)
	{
		
		
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
		//transition out BB contents
		//transition in CHOICE items
		
		int numberEntries = 4; //this means 3 choices + space for person
		
	}
	
	public void set_for_DEATH(List<FlatGraphElement> aGraphs)
	{
		//TODO
	}
}
