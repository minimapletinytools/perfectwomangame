using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

		GameObject.Destroy(miniMan.gameObject);

		set_for_choosing();
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
		mBBChoosingBackground.SoftColor = fullColor*(new Color(0.6f,0.6f,1))*1;//0.2f;
	}
	
	public void set_for_choosing()
	{
		fade_choosing_contents(false);
		mBBMiniMan.SoftColor = GameConstants.UiMiniMan;
		Color fullColor = new Color(0.5f,0.5f,0.5f,1);
		mBBChoosingBackground.SoftColor = fullColor*(new Color(0.6f,0.6f,1))*1;//0.2f;
	}

	public void update()
	{
		mFlatCamera.update(Time.deltaTime);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		
		TED.update(Time.deltaTime);
		
		
		//TODO should render mFlatCamera to a render texture
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
	
	public void set_bb_choices(CharacterIndex[] aChoices)
	{
		var ch = aChoices.OrderBy(e => e.Choice).ToArray();
		int len = ch.Count();
		
		float padding = 400;
		float netWidth = (len)*padding;
		float awkwardOffset = netWidth/2 - padding/1.35f;
		
		for(int i = len; i < mBBChoices.Count; i++)
			mBBChoices[i].Enabled = false;
		for(int i = 0; i < len; i++)
		{
			
			mBBChoices[i].Enabled = true;
			mBBChoices[i].set_actual_character(ch[i]);
			mBBChoices[i].set_difficulty(mManager.mCharacterBundleManager.get_character_helper().Characters[ch[i]].Difficulty);
			
			
			float xOffset = awkwardOffset - padding*i;
			mBBChoices[i].HardPosition = mFlatCamera.get_point(0.5f, 0) + new Vector3(xOffset,0,0);
			mBBChoiceBodies[i].HardPosition = mFlatCamera.get_point(0.5f, 0) + new Vector3(xOffset,-195,0);
		}
	}
	
	//TODO DELETE this is replaced by the function above
	//called by ChoiceHelper
	//this should really be called set_bb_choice_icons now
	public void set_bb_choice_bodies(CharacterIndex aIndex)
	{
		CharacterIndex index = new CharacterIndex(aIndex.LevelIndex+1,0);
		var all = index.NeighborsAndSelf;
		all.RemoveAt(3);
		set_bb_choices(all.ToArray());
		
		
		/* TODO DELETE
		all.Add(index);
		for(int i = 0; i < 3; i++)
		{
			//mBBChoices[i].set_actual_character(mManager.mCharacterBundleManager.get_mini_character(all[i]));
			//TODO DELETE this is set in the following function mBBChoices[i].Character = all[i];
			mBBChoices[i].set_actual_character(all[i]);
			mBBChoices[i].set_difficulty(mManager.mCharacterBundleManager.get_character_helper().Characters[all[i]].Difficulty);

			
			//OLD when using mini char icons we took from PB
			//mBBChoices[i].return_body(mPBCharacterIcons[all[i].Index].take_body()); //make sure to return the body
		}*/
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
				new string[]{("at age " + mManager.mGameManager.CurrentCharacterIndex.get_future_neighbor(0).Age) + "?"},
			new Color[]{GameConstants.UiRed});
		}
		else{
			mBBMiniMan.SoftColor = GameConstants.UiMiniMan;
			mBBMiniMan.SoftPosition = mBBChoiceBodies[aIndex].SoftPosition;
			var nChar = mManager.mGameManager.CurrentCharacterIndex.get_future_neighbor(aIndex);
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
				//diffColors[nCharDiff.Difficulty]/2f,
				GameConstants.UiRed,
				//GameConstants.UiPink,
				//perfectColors[nCharDiff.Perfect]/2f,
				GameConstants.UiRed});
		}
	}
	//called by ChoiceHelper
	public void set_bb_choice_percentages(int aIndex, float aPercent)
	{	
		int index = aIndex;
		mBBChoices[index].Percentage = aPercent;
	}
}
