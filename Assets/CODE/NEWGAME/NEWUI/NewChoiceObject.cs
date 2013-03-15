using UnityEngine;
using System.Collections;

public class NewChoiceObject : FlatElementMultiBase {
    public FlatElementImage mSquare;
	public FlatElementSpriteText mText;
	public DifficultyObject mPerfect;
	FlatElementMultiBase.ElementOffset mBodyElementOffset = null;
    public MeterImageObject mMeter = null;

    float mSide = 45;
    float mRightBot = -80;
	
	public NewChoiceObject(int aDepth)
	{
		initialize(null, aDepth);
	}
    void initialize(CharacterLoader aActualChar, int aDepth)
    {
		//TODO finish and reposition everything
		//mSquare = new FlatElementImage(, aDepth);
		//mText = new FlatElementSpriteText(,,"",aDepth+1)
        mPerfect = new DifficultyObject(ManagerManager.Manager.mMenuReferences.perfectnessStar, aDepth);
        //mMeter = new MeterImageObject(, MeterImageObject.FillStyle.DU, aDepth + 1);
        //mMeter.Percentage = 0.0f;
        
        //mBody.set_target_pose(aPose);
		//mElements.Add(new FlatElementMultiBase.ElementOffset(mSquare, new Vector3(0, 0, 0)));
		//mElements.Add(new FlatElementMultiBase.ElementOffset(mText, new Vector3(0, 0, 0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mPerfect, new Vector3(0,0,0)));
        //mElements.Add(new FlatElementMultiBase.ElementOffset(mMeter, new Vector3(0,0,0)));
        
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
		mBodyElementOffset = new FlatElementMultiBase.ElementOffset(new FlatBodyObject(aActualChar, Depth + 2), new Vector3(0,0,0));
		mElements.Add(mBodyElementOffset);
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
