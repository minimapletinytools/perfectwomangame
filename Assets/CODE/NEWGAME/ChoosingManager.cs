using UnityEngine;
using System.Collections.Generic;

public class ChoosingManager 
{
	ManagerManager mManager;
    public ChoosingManager(ManagerManager aManager)
	{
		mManager = aManager;
	}

	public TimedEventDistributor TED { get; private set; }
    public FlatCameraManager mFlatCamera;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	
	int BB_NUM_CHOICES = 3;
	List<NewChoiceObject> mBBChoices = new List<NewChoiceObject>();
	List<FlatBodyObject> mBBChoiceBodies = new List<FlatBodyObject>();
	FlatElementImage mBBChoosingBackground;
	ColorTextObject mBBQuestionText;
    FlatElementText mBBQuestionTextPrefix;
	FlatBodyObject mBBMiniMan;
	Vector3 mBBMiniManBasePosition;
	
	
	public void initialize()
	{
		
		TED = new TimedEventDistributor();
		
		mFlatCamera = new FlatCameraManager(new Vector3(24234,-3535,0),10);
		mFlatCamera.fit_camera_to_game();
		
		
		
		var refs = mManager.mReferences;
		var newRef = mManager.mNewRef;
		//BB choice nonsense
		var miniMan = ((GameObject)GameObject.Instantiate(refs.mMiniChar)).GetComponent<CharacterTextureBehaviour>();
		//mMiniMan = //TODO something like this: mManager.mCharacterBundleManager.get_mini_character(new CharacterIndex(0,1));
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
		//???DELETE???GameObject.Destroy(mMiniMan.gameObject);
	}
	
	//TODO we don't need this function if we are going to do slide transition...
	//DELETE
	public void fade_choosing_contents(bool small)
	{
		mBBMiniMan.SoftColor = GameConstants.UiMiniMan;
		
		//TODO read out of game constants please
		Color smallColor = small ? new Color(0.5f,0.5f,0.5f,1) : new Color(0.5f,0.5f,0.5f,0);
		Color fullColor = !small ? new Color(0.5f,0.5f,0.5f,1) : new Color(0.5f,0.5f,0.5f,0);
		
		foreach(FlatBodyObject e in mBBChoiceBodies)
			e.SoftColor = fullColor;
		foreach(NewChoiceObject e in mBBChoices)
			e.SoftColor = fullColor;
		mBBMiniMan.SoftColor = fullColor;
		foreach(var e in mBBChoices)
			e.SoftColor = fullColor;
		mBBQuestionText.SoftColor = fullColor;
        mBBQuestionTextPrefix.SoftColor = fullColor*GameConstants.UiRed*2;
		
		//mBBChoosingBackground.SoftColor = fullColor*(new Color(0.6f,0.6f,1))*1;//0.2f;
	}
	
	public void set_for_choosing()
	{
		fade_choosing_contents(false);
		mBBMiniMan.SoftColor = GameConstants.UiMiniMan;
		Color fullColor = new Color(0.5f,0.5f,0.5f,1);
		mBBChoosingBackground.SoftColor = fullColor*(new Color(0.6f,0.6f,1))*1;//0.2f;
	}
	
	void set_for_play()
	{
		fade_choosing_contents(true);
		Color fullColor = new Color(0.5f,0.5f,0.5f,0);
		mBBChoosingBackground.SoftColor = fullColor*(new Color(0.6f,0.6f,1))*1;
	}
	
	
	public void update()
	{
		mFlatCamera.update(Time.deltaTime);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		
		TED.update(Time.deltaTime);
		
		
		//TODO should render mFlatCamera to a render texture
	}
	
	FlatElementImage mBackground = null;
	FlatElementImage mSun = null;
	List<FlatElementImage> mCharacters = new List<FlatElementImage>();
}
