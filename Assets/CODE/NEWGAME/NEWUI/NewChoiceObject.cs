using UnityEngine;
using System.Collections;

public class NewChoiceObject : FlatElementMultiBase {
    public FlatElementImage mSquare;
	public FlatElementText mText = null;
	//public DifficultyObject mPerfect;
	public FlatElementImage mDiffImage;
	
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
		var meterImage = ManagerManager.Manager.mCharacterBundleManager.get_image("SELECTION_BOX");

		var newRef = ManagerManager.Manager.mNewRef;
		//mSquare = new FlatElementImage(newRef.bbChoiceBox, aDepth);
		mSquare = new FlatElementImage(meterImage.Image,meterImage.Data.Size, aDepth);
		mSquare.HardColor = GameConstants.UiRed;
		mText = new FlatElementText(newRef.fatFont,48,"",aDepth +2);
		mDiffImage = new FlatElementImage(null,aDepth + 3);
		mDiffImage.HardScale = 1.33f * Vector3.one;
        //mPerfect = new DifficultyObject(ManagerManager.Manager.mNewRef.uiPerfectStar, aDepth);
		mIcon = new FlatElementImage(null,aDepth +2);
		mIcon.HardScale = 1.25f*Vector3.one;
        //mMeter = new MeterImageObject(newRef.bbChoiceBox, null,MeterImageObject.FillStyle.DU, aDepth + 1);
		mMeter = new MeterImageObject(meterImage.Image,meterImage.Data.Size,MeterImageObject.FillStyle.DU, aDepth + 1);
        mMeter.Percentage = 0.0f;
		set_perfectness(3);
        
        //mBody.set_target_pose(aPose);
		mElements.Add(new FlatElementMultiBase.ElementOffset(mSquare, new Vector3(0,0,0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mDiffImage, new Vector3(-mSquare.BoundingBox.width/2f+80,-mSquare.BoundingBox.height/2f+80,0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mText, new Vector3(0,0,0)));
		//mElements.Add(new FlatElementMultiBase.ElementOffset(mPerfect, new Vector3(-122,65,0)));
        mElements.Add(new FlatElementMultiBase.ElementOffset(mMeter, new Vector3(0,0,0)));
		
		mElements.Add(new FlatElementMultiBase.ElementOffset(mIcon, new Vector3(0,275,0)));
        
		mText.HardColor = GameConstants.uiWhite;
		if(aIndex != null)
			set_actual_character(aIndex.Value);

        PrimaryGameObject = create_primary_from_elements();
		Depth = aDepth; 
    }
	
	
	//public void set_actual_character(CharacterLoader aActualChar)
	public void set_actual_character(CharacterIndex aIndex)
	{ 
		/*
		 * TODO DELETE
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


		var charIcon = ManagerManager.Manager.mCharacterBundleManager.get_image("ICON_S_" + aIndex.StringIdentifier);
		if(charIcon.Image != null && charIcon.Data != null)
			mIcon.set_new_texture(charIcon.Image,charIcon.Data.Size);
		else 
			mIcon.set_new_texture(ManagerManager.Manager.mCharacterBundleManager.get_image("BOX_" + aIndex.StringIdentifier).Image,null);

		
		
		Character = aIndex;
	}
	
	CharacterIndex mCharacterIndex = new CharacterIndex(-1,0);
	public CharacterIndex Character
	{ 
		get{return mCharacterIndex;} 
		set{
			mCharacterIndex = value;
			if(mCharacterIndex.LevelIndex != -1)
			{
				mText.Text = FlatElementText.convert_to_multiline(mCharacterIndex.Description.Length > 34 ? 3 : 2,mCharacterIndex.Description.ToUpper());
				//mText.Text = mCharacterIndex.ShortName.ToUpper();
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
        //mPerfect.Difficulty = perfectness;
		//mPerfectImage.set_new_texture(ManagerManager.Manager.mNewRef.bbChoicePerfectIcons[perfectness]);
    }
	
	public void set_difficulty(int difficulty)
	{
		string[] labelNames = new string[]{
			"label_easy","label_normal","label_hard","label_extreme"
		};
		//mIcon.SoftColor = GameConstants.IconDifficultyColorsOverTwo[difficulty];
		var tex = ManagerManager.Manager.mCharacterBundleManager.get_image(labelNames[difficulty]);
		mDiffImage.set_new_texture(tex.Image,tex.Data.Size);
	}


	//TODO this function doe not work
	/*
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
            //mPerfect.SoftColor = value;
			
			//DELETE this is a stupid hack
			if(value.a != 0 && mText != null)
				mText.SoftColor = GameConstants.uiWhite;
			
			//this is also a stupid hack
			//if(mBody!=null)
			//	mBody.SoftColor = bodyColor;
			if(mIcon != null && value.a != 0)
				mIcon.SoftColor = bodyColor;
        }
    }*/
}
