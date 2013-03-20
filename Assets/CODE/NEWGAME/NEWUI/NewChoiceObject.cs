using UnityEngine;
using System.Collections;

public class NewChoiceObject : FlatElementMultiBase {
    public FlatElementImage mSquare;
	public FlatElementText mText = null;
	public DifficultyObject mPerfect;
	FlatElementMultiBase.ElementOffset mBodyElementOffset = null;
    public MeterImageObject mMeter = null;
	FlatBodyObject mBody = null;

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
    void initialize(CharacterLoader aActualChar, int aDepth)
    {
		var newRef = ManagerManager.Manager.mNewRef;
		//TODO finish and reposition everything
		mSquare = new FlatElementImage(newRef.bbChoiceBox, aDepth);
		mText = new FlatElementText(newRef.genericFont,40,"meow",aDepth +1);
		
        mPerfect = new DifficultyObject(ManagerManager.Manager.mNewRef.uiPerfectStar, aDepth);
        mMeter = new MeterImageObject(newRef.bbChoiceBox, MeterImageObject.FillStyle.DU, aDepth + 1);
        mMeter.Percentage = 0.0f;
		set_perfectness(3);
        
        //mBody.set_target_pose(aPose);
		mElements.Add(new FlatElementMultiBase.ElementOffset(mSquare, new Vector3(0,0,0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mText, new Vector3(0,20,0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mPerfect, new Vector3(-173,65,0)));
        mElements.Add(new FlatElementMultiBase.ElementOffset(mMeter, new Vector3(0,0,0)));
        
		if(aActualChar != null)
			set_actual_character(aActualChar);

        PrimaryGameObject = create_primary_from_elements();
		Depth = aDepth;
    }
	
	
	public void set_actual_character(CharacterLoader aActualChar)
	{ 
		//remove the old one
		if(mBodyElementOffset != null)
		{
			mElements.Remove(mBodyElementOffset);
			mBodyElementOffset.Element.destroy();
		}
		//add the new one
		mBody = new FlatBodyObject(aActualChar, Depth + 2);
		mBodyElementOffset = new FlatElementMultiBase.ElementOffset(mBody, new Vector3(0,0,0));
		mElements.Add(mBodyElementOffset);
		
		mText.Text = aActualChar.Name; //TODO actual name
	}
	
	public FlatBodyObject take_body()
	{
		FlatBodyObject r = reposses_element(mBody) as FlatBodyObject;
		mBody = null;
		return r;
	}
	public void return_body(FlatBodyObject aBody)
	{
		mBody = aBody;
		mElements.Add(new ElementOffset(aBody, new Vector3(0,0,0)));
	}
	
    public void set_pose(ProGrading.Pose aPose)
    {
        if (mBodyElementOffset != null)
            (mBodyElementOffset.Element as FlatBodyObject).set_target_pose(aPose);
    }

    public void set_perfectness(int perfectness)
    {
        mPerfect.Difficulty = perfectness;
    }
	
	//TODO decide how we want color changes to behave
    public override Color SoftColor
    {
        get
        {
            return base.SoftColor;
        }
        set
        {
			base.SoftColor = value;
            //mDifficultyBalls.SoftColor = value;
			
			//DELETE this is a stupid hack
			if(value.a != 0 && mText != null)
				mText.SoftColor = new Color(0,0,0,1);
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
