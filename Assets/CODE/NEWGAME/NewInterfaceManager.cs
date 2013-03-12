using UnityEngine;
using System.Collections.Generic;

public class NewInterfaceManager : FakeMonoBehaviour {
    public NewInterfaceManager(ManagerManager aManager) : base(aManager) { }

	
    public FlatCameraManager mFlatCamera;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	
    CharacterTextureBehaviour mMiniMan;
	FlatBodyObject mCurrentBody = null;

    
	
	public override void Start()
    {
        mFlatCamera = new FlatCameraManager(new Vector3(10000, 0, 0), 10);
        mMiniMan = ((GameObject)GameObject.Instantiate(ManagerManager.Manager.mMenuReferences.miniMan)).GetComponent<CharacterTextureBehaviour>();        
		
		/*
		var refs = mManager.mMenuReferences;
		FlatElementSpriteText spriteTex = new FlatElementSpriteText(refs.fontTex,20,"testmessage",10);
		spriteTex.SoftPosition = mFlatCamera.Center;
		spriteTex.SoftScale = new Vector3(0.5f,0.5f,0.5f);
		mElement.Add (spriteTex);*/
    }
    public override void Update()
    {
        mFlatCamera.update(Time.deltaTime);
        if (mCurrentBody != null)
            mCurrentBody.match_body_to_projection(mManager.mProjectionManager);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		
		
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
	public void update_bb_for_performance(float perfect, float time)
	{
	}
	
	
	
	//PINK BAR
	
	//TEXT
	
	//GLORY
	public void add_glory_character(){}
	
	
	
	public void set_for_PLAY()
	{
		//TODO transition BB back to its orig place
		//transition in BB contents
	}
	public void set_for_CUTSCENE()
	{
		//TODO rearange INTERFACE
		//TODO script cutscene 
	}
	public void set_for_choice()
	{
		//TODO
		//transition BB to full screen
		//transition out BB contents
		//transition in CHOICE items
	}
	
	
}
