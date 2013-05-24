using UnityEngine;
using System.Collections;

public class NewChoiceObject : FlatElementMultiBase {
    public FlatElementImage mSquare;
	public FlatElementText mText = null;
	public DifficultyObject mPerfect;
	public FlatElementImage mPerfectImage;
	
    public MeterImageObject mMeter = null;
	//FlatElementMultiBase.ElementOffset mBodyElementOffset = null;
	//FlatBodyObject mBody = null;
	FlatElementImage mIcon = null;

    float mSide = 45;
    float mRightBot = -80;
	
	public float Percentage
	{
		get{return mMeter.Percentage;}
		set
		{
			mMeter.Percentage = value;
		}
	}
	public NewChoiceObject(int aDepth)
	{
		initialize(null, aDepth);
	}
    //void initialize(CharacterLoader aActualChar, int aDepth)
	void initialize(CharacterIndex? aIndex, int aDepth)
    {
		var newRef = ManagerManager.Manager.mNewRef;
		//TODO finish and reposition everything
		mSquare = new FlatElementImage(newRef.bbChoiceBox, aDepth);
		mText = new FlatElementText(newRef.genericFont,30,"",aDepth +1);
		mPerfectImage = new FlatElementImage(null,aDepth +2);
        mPerfect = new DifficultyObject(ManagerManager.Manager.mNewRef.uiPerfectStar, aDepth);
		mIcon = new FlatElementImage(null,aDepth +2);
		mIcon.HardScale = 1f*Vector3.one;
        mMeter = new MeterImageObject(newRef.bbChoiceBox, MeterImageObject.FillStyle.DU, aDepth + 1);
        mMeter.Percentage = 0.0f;
		set_perfectness(3);
        
        //mBody.set_target_pose(aPose);
		mElements.Add(new FlatElementMultiBase.ElementOffset(mSquare, new Vector3(0,0,0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mPerfectImage, new Vector3(-mSquare.BoundingBox.width/2,mSquare.BoundingBox.height/2,0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mText, new Vector3(0,20,0)));
		//mElements.Add(new FlatElementMultiBase.ElementOffset(mPerfect, new Vector3(-122,65,0)));
        mElements.Add(new FlatElementMultiBase.ElementOffset(mMeter, new Vector3(0,0,0)));
		
		mElements.Add(new FlatElementMultiBase.ElementOffset(mIcon, new Vector3(0,175,0)));
        
		mText.SoftColor = new Color(0,0,0,1);
		if(aIndex != null)
			set_actual_character(aIndex.Value);

        PrimaryGameObject = create_primary_from_elements();
		Depth = aDepth; 
    }
	
	
	//public void set_actual_character(CharacterLoader aActualChar)
	public void set_actual_character(CharacterIndex aIndex)
	{ 
		/*
		//remove the old one
		if(mBodyElementOffset != null)
		{
			mElements.Remove(mBodyElementOffset);
			mBodyElementOffset.Element.destroy();
		}
		//add the new one
		mBody = new FlatBodyObject(aActualChar, Depth + 2);
		mBodyElementOffset = new FlatElementMultiBase.ElementOffset(mBody, new Vector3(0,0,0));
		mElements.Add(mBodyElementOffset);*/
		
		
		mIcon.set_new_texture(ManagerManager.Manager.mCharacterBundleManager.get_image("BOX_" + aIndex.StringIdentifier).Image,null);
		
		
		Character = aIndex;
	}
	
	CharacterIndex mCharacterIndex = new CharacterIndex(-1);
	public CharacterIndex Character
	{ 
		get{return mCharacterIndex;} 
		set{
			mCharacterIndex = value;
			if(mCharacterIndex.Index != -1)
			{
				mText.Text = FlatElementText.convert_to_multiline(2,mCharacterIndex.Description);
			}
			else mText.Text = "";
		}
	}
	
	/*
	public FlatBodyObject take_body()
	{
		FlatBodyObject r = reposses_element(mBody) as FlatBodyObject;
		mBody = null;
		return r;
	}
	public void return_body(FlatBodyObject aBody)
	{
		mBody = aBody;
		mBody.Depth = Depth + 3;
		
		//hack
		mBody.HardScale = new Vector3(2.5f,2.5f,2.5f);
		mElements.Add(new ElementOffset(aBody, new Vector3(0,220,0)));
		SoftPosition = SoftPosition;
		mBody.PrimaryGameObject.transform.parent = PrimaryGameObject.transform;
	}
	
    public void set_pose(ProGrading.Pose aPose)
    {
        if (mBodyElementOffset != null)
            (mBodyElementOffset.Element as FlatBodyObject).set_target_pose(aPose);
    }*/

    public void set_perfectness(int perfectness)
    {
        mPerfect.Difficulty = perfectness;
		mPerfectImage.set_new_texture(ManagerManager.Manager.mNewRef.bbChoicePerfectIcons[perfectness]);
    }
	
	public void set_difficulty(int difficulty)
	{
		mIcon.SoftColor = GameConstants.IconDifficultyColorsOverTwo[difficulty];
	}
	
    public override Color SoftColor
    {
        get
        {
            return base.SoftColor;
        }
        set
        {
			
			//this is also a stupid hack
			Color bodyColor = new Color(0,0,0,0);
			//if(mBody!=null)
			//	bodyColor = mBody.SoftColor;
			if(mIcon != null)
				bodyColor = mIcon.SoftColor;
			
			base.SoftColor = value;
            mPerfect.SoftColor = value;
			
			//DELETE this is a stupid hack
			if(value.a != 0 && mText != null)
				mText.SoftColor = new Color(0,0,0,1);
			
			//this is also a stupid hack
			//if(mBody!=null)
			//	mBody.SoftColor = bodyColor;
			if(mIcon != null && value.a != 0)
				mIcon.SoftColor = bodyColor;
        }
    }

    public override Color HardColor
    {
        get
        {
            return base.HardColor;
        }
        set
        {
			base.HardColor = value;
            //mDifficultyBalls.HardColor = value;
        }
    }
}
