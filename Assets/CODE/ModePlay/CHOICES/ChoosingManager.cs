using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ChoosingManager 
{
	ManagerManager mManager;
	//ModeNormalPlay mModeNormalPlay;
    public ChoosingManager(ManagerManager aManager, ModeNormalPlay aModeNormalPlay)
	{
		//mModeNormalPlay = aModeNormalPlay;
		mManager = aManager;
	}

    public FlatCameraManager mFlatCamera;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	
	int BB_NUM_CHOICES = 4;
	List<NewChoiceObject> mBBChoices = new List<NewChoiceObject>();
	List<FlatBodyObject> mBBChoiceBodies = new List<FlatBodyObject>();
	FlatElementImage mBBChoosingBackground;
	ColorTextObject mBBQuestionText;
    FlatElementText mBBQuestionTextPrefix;
	FlatElementImage mBBQuestionBubble;
	Vector3 mBBMiniManBasePositionOffset = new Vector3(0,-300,0);

	FlatBodyObject[] mBBMiniMans;

	//CharacterIndex[] mChoices = null;
	

	public void initialize()
	{
		mFlatCamera = new FlatCameraManager(new Vector3(24200,-3500,0),10);
		mFlatCamera.fit_camera_to_game();
		//mFlatCamera = mModeNormalPlay.mSunsetManager.mFlatCamera;
		

		var refs = mManager.mReferences;
		var newRef = mManager.mNewRef;
		//BB choice nonsense
		var miniMan = ((GameObject)GameObject.Instantiate(refs.mMiniChar)).GetComponent<CharacterTextureBehaviour>();
		//mMiniMan = //TODO something like this: mManager.mCharacterBundleManager.get_mini_character(new CharacterIndex(0,1));
		Vector3 miniManScale = (new Vector3(1,1,1))*1.5f;
		for(int i = 0; i < BB_NUM_CHOICES; i++)
		{
			mBBChoices.Add(new NewChoiceObject(11));
			mBBChoiceBodies.Add(new FlatBodyObject(miniMan,12));
			mBBChoiceBodies[i].HardShader = refs.mMiniCharacterShader;
			mBBChoiceBodies[i].HardScale = miniManScale;
			mElement.Add(mBBChoices[i]);
			mElement.Add(mBBChoiceBodies[i]);
		}
		
		mBBChoosingBackground = new FlatElementImage(null,mFlatCamera.Size,0);
		mBBChoosingBackground.Enabled = false;
		mBBChoosingBackground.HardPosition = mFlatCamera.get_point(Vector3.zero);
		mBBQuestionText = new ColorTextObject(10);
        mBBQuestionTextPrefix = new FlatElementText(newRef.genericFont, 100, "", 10);
		//mBBQuestionText.HardPosition = mFlatCamera.get_point(0,0.6f) + new Vector3(0,-75,0);
		mBBQuestionText.HardPosition = mFlatCamera.get_point(0,0.72f);
		mBBQuestionTextPrefix.HardPosition = mFlatCamera.get_point(0, 0.72f) + new Vector3(0,75, 0);
		mBBQuestionText.SoftInterpolation = 1;
        mBBQuestionTextPrefix.SoftInterpolation = 1;
		var bubbleImage = mManager.mCharacterBundleManager.get_image("SELECTION_BUBBLE");
		mBBQuestionBubble = new FlatElementImage(bubbleImage.Image,bubbleImage.Data.Size,1);
		mBBQuestionBubble.HardPosition = mFlatCamera.get_point(0,0.75f);
		mBBQuestionBubble.HardScale = new Vector3(1.3f,1.1f,1);
		mBBQuestionBubble.HardColor = GameConstants.UiPopupBubble;

		mBBMiniMans = new FlatBodyObject[BB_NUM_CHOICES];
		for(int i = 0; i < BB_NUM_CHOICES; i++)
		{
			mBBMiniMans[i] = new FlatBodyObject(miniMan,20);
			mBBMiniMans[i].HardScale = miniManScale;
			mBBMiniMans[i].HardColor = GameConstants.UiPurpleTransparent;
			mBBMiniMans[i].SoftInterpolation = .13f;
			mElement.Add(mBBMiniMans[i]);
		}
		
		
		
		mElement.Add(mBBChoosingBackground);

		mElement.Add(mBBQuestionText);
        mElement.Add(mBBQuestionTextPrefix);
		mElement.Add(mBBQuestionBubble);

		GameObject.Destroy(miniMan.gameObject);

		set_for_choosing();
	}

	//TODO DELETE/CONSOLIDATE
	//NOTE this function is no longer needed but we still use it to initialize choosing
	public void fade_choosing_contents(bool small = false)
	{
		
		//TODO read out of game constants please
		//Color smallColor = small ? new Color(0.5f,0.5f,0.5f,1) : new Color(0.5f,0.5f,0.5f,0);
		Color fullColor = !small ? new Color(0.5f,0.5f,0.5f,1) : new Color(0.5f,0.5f,0.5f,0);
		
		foreach(FlatBodyObject e in mBBChoiceBodies)
			e.SoftColor =  !small ? GameConstants.uiWhite : GameConstants.UiWhiteTransparent;
		//foreach(NewChoiceObject e in mBBChoices)
		//	e.SoftColor = fullColor;
		//foreach(var e in mBBChoices)
		//	e.SoftColor = fullColor;
		mBBQuestionText.SoftColor = fullColor;
        mBBQuestionTextPrefix.SoftColor = fullColor*GameConstants.UiRed*2;
		mBBChoosingBackground.SoftColor = fullColor*(new Color(0.6f,0.6f,1))*1;//0.2f;
	}

	//TODO DELETE/CONSOLIDATE
	public void set_for_choosing()
	{
		fade_choosing_contents(false);
		Color fullColor = new Color(0.5f,0.5f,0.5f,1);
		mBBChoosingBackground.SoftColor = fullColor*(new Color(0.6f,0.6f,1))*1;//0.2f;
	}

	public void update()
	{
		mFlatCamera.update(Time.deltaTime);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		
		//TODO should render mFlatCamera to a render texture
	}


	//--------
	//related to setting for CHOOSING
	//--------
	//called by NewGameManager
	public void set_bb_decider_pose(Pose aPose)
	{
		for(int i = 0; i < mBBMiniMans.Length; i++)
			mBBMiniMans[i].set_target_pose(aPose);
	}
	
	//called by ChoiceHelper
	public void set_bb_choice_poses(List<Pose> aPoses)
	{
		for(int i = 0; i < aPoses.Count; i++)
		{
			mBBChoiceBodies[i].set_target_pose(aPoses[i]);
		}
	}
	
	public void set_bb_choices(CharacterIndex[] aChoices)
	{
		//mChoices = aChoices;

        //JIT/AOT hard fix. I think aChoices is always passed into here in order anyways so it's not a problem
        var ch = aChoices;//aChoices.OrderBy(e => e.Choice).ToArray();
		int len = ch.Count();
		
		float padding = 700;
		float netWidth = (len)*padding;
		
		for (int i = len; i < mBBChoices.Count; i++)
        {
            mBBChoices [i].Enabled = false;
            mBBMiniMans[i].Enabled = false;
            mBBChoiceBodies[i].Enabled = false;
        }
		for(int i = 0; i < len; i++)
		{
			
			mBBChoices[i].Enabled = true;
            mBBMiniMans[i].Enabled = true;
            mBBChoiceBodies[i].Enabled = true;

			mBBChoices[i].set_actual_character(ch[i]);
			mBBChoices[i].set_difficulty(mManager.mCharacterBundleManager.get_character_helper().Characters[ch[i]].Difficulty);
			
            var xOffset = netWidth/2 - padding/2 - padding*i;
			mBBChoices[i].HardPosition = mFlatCamera.get_point(0, -0.16f) + new Vector3(xOffset,0,0);
			mBBChoiceBodies[i].HardPosition = mFlatCamera.get_point(0, -0.16f) + new Vector3(xOffset,-270,0);
			mBBMiniMans[i].HardPosition = mBBChoiceBodies[i].HardPosition + mBBMiniManBasePositionOffset;
			mBBMiniMans[i].HardColor = GameConstants.UiPurpleTransparent;
		}
	}


	//this is the character that is curretnly being selected
	//called by ChoiceHelper
	public void set_bb_choice(int aIndex)
	{
		//reset choosing state
		for(int i = 0; i < mBBMiniMans.Length; i++)
		{
			mBBMiniMans[i].SoftPosition = mBBChoiceBodies[i].HardPosition + mBBMiniManBasePositionOffset;
			mBBMiniMans[i].SoftColor = GameConstants.UiPurpleTransparent;
		}


		if(aIndex == -1) //no choice
		{
			//mBBQuestionTextPrefix.Text = "Choose your perfect life";
			mBBQuestionText.set_text(
                new string[]{(GameStrings.GetString("CM1",mManager.mGameManager.CurrentCharacterIndex.get_future_neighbor(0).Age.ToString()))},
			new Color[]{GameConstants.UiRed});
		}
		else{
			mBBMiniMans[aIndex].SoftPosition = mBBChoiceBodies[aIndex].HardPosition;
			mBBMiniMans[aIndex].SoftColor = GameConstants.UiMiniMan;

			var nChar = mManager.mGameManager.CurrentCharacterIndex.get_future_neighbor(aIndex);
			var nCharDiff = mManager.mCharacterBundleManager.get_character_helper().Characters[nChar];
            var diffPhrases = new string[]
            {
                GameStrings.GetString("CMdiffphrase1"),
                GameStrings.GetString("CMdiffphrase2"),
                GameStrings.GetString("CMdiffphrase3"),
                GameStrings.GetString("CMdiffphrase4")
            };
			//var perfectPhrases = new string[]{" horrible", " passable", " perfect", " PERFECT"};
			//var perfectColors = new Color[]{GameConstants.uiYellow, GameConstants.uio
			//var diffColors = new Color[]{new Color(0,0.8f,0,1), new Color(0.8f,0.8f,0,1), new Color(0.9f,0.4f,0,1), new Color(0.8f,0,0,1)};
			//mBBQuestionTextPrefix.Text = "That is a";
			mBBQuestionText.set_text(
				new string[]{
                GameStrings.GetString("CM2"),
				//perfectPhrases[nCharDiff.Perfect], 
				//Mathf.Abs((3-nCharDiff.Difficulty) - nCharDiff.Perfect) > 1 ? " but" : " and",
				diffPhrases[nCharDiff.Difficulty],
                GameStrings.GetString("CM3")},
			new Color[]{
				//GameConstants.UiPink,
				//diffColors[nCharDiff.Difficulty]/2f,
				GameConstants.UiRed,
				//GameConstants.UiPink,
				//perfectColors[nCharDiff.Perfect]/2f,
				GameConstants.UiRed,
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
