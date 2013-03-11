using UnityEngine;
using System.Collections;

public class NewChoiceObject : FlatElementMultiBase {
    public FlatElementImage mSquare;
	public FlatElementSpriteText mText;
	public DifficultyObject mPerfect;
    public FlatBodyObject mBody = null;
    public MeterImageObject mMeter = null;

    float mSide = 45;
    float mRightBot = -80;

    public NewChoiceObject(CharacterLoader aActualChar, int aDepth)
    {
		//TODO finish and reposition everything
		//mSquare = new FlatElementImage(, aDepth);
		//mText = new FlatElementSpriteText(,,"",aDepth+1)
        mPerfect = new DifficultyObject(ManagerManager.Manager.mMenuReferences.perfectnessStar, aDepth);
        //mMeter = new MeterImageObject(, MeterImageObject.FillStyle.DU, aDepth + 1);
        mMeter.Percentage = 0.0f;
        mBody = new FlatBodyObject(aActualChar, aDepth + 2);
        //mBody.set_target_pose(aPose);
		mElements.Add(new FlatElementMultiBase.ElementOffset(mSquare, new Vector3(0, 0, 0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mText, new Vector3(0, 0, 0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mPerfect, new Vector3(0,0,0)));
        mElements.Add(new FlatElementMultiBase.ElementOffset(mMeter, new Vector3(0,0,0)));
        mElements.Add(new FlatElementMultiBase.ElementOffset(mBody, new Vector3(0,0,0)));

        PrimaryGameObject = create_primary_from_elements();
    }

    public void set_pose(ProGrading.Pose aPose)
    {
        if (mBody != null)
            mBody.set_target_pose(aPose);
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
