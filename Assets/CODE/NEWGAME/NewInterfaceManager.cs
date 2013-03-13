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
		
		
		var refs = mManager.mMenuReferences;
		//FlatElementText text = new FlatElementText(mManager.mNewRef.genericFontPrefab,50,"aeuaeuoe",10);
		FlatElementText text = new FlatElementText(refs.menuFont,50,"aeuaeuoe",10);
		text.SoftPosition = mFlatCamera.Center;
		text.SoftScale = new Vector3(0.5f,0.5f,0.5f);
		mElement.Add (text);
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
	List<FlatGraphElement> mBBPerformanceGraphs = new List<FlatGraphElement>(); //maps age index to graph, null means no graph (e.g. fetus)
	FlatElementImage mBBPerformanceGraphFrame;
	FlatElementSpriteText mBBText;
	FlatElementSpriteText mBBScoreFrame;
	FlatElementSpriteText mBBScoreText;
	//CUTSCENE
	//???
	//CHOOSING
	List<NewChoiceObject> mBBChoices = new List<NewChoiceObject>();
	List<FlatBodyObject> mBBChoiceBodies = new List<FlatBodyObject>();
	FlatBodyObject mBBMiniMan;
	FlatElementImage mBBChoiceBox;
	public void set_bb_full_size()
	{
		Vector2 baseSize = new Vector2(mBB.BoundingBox.width,mBB.BoundingBox.height);
		Vector2 desiredSize = new Vector2(mFlatCamera.Width+30,mFlatCamera.Height+30);
		mBB.set_scale(new Vector3(desiredSize.x/baseSize.x,desiredSize.y/baseSize.y,1));
	}
	public void set_bb_small()
	{
		mBB.set_scale(new Vector3(1,1,1));
	}
	
	public void update_bb_for_performance(float perfect, float time)
	{
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
	
	
}
