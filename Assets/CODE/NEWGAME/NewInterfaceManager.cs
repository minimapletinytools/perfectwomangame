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
		
		
		var refs = mManager.mMenuReferences;
		FlatElementSpriteText spriteTex = new FlatElementSpriteText(refs.fontTex,20,"testmessage",10);
		spriteTex.SoftPosition = mFlatCamera.Center;
		spriteTex.SoftScale = new Vector3(0.5f,0.5f,0.5f);
		mElement.Add (spriteTex);
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
	List<FlatGraphElement> mPerformanceGraphs = new List<FlatGraphElement>(); //maps age index to graph, null means no graph (e.g. fetus)
	FlatElementImage mPerformanceGraphFrame;
	FlatElementSpriteText mBBText;
	FlatElementSpriteText mBBScoreFrame;
	FlatElementSpriteText mBBScoreText;
	//CUTSCENE
	//???
	//CHOOSING
	
	
	
	
	//PINK BAR
	
	//TEXT
	
	//GLORY
	public void add_glory_character(){}
	
}
