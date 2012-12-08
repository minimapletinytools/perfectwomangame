using UnityEngine;
using System.Collections;

public class ChoiceObjectPair : FlatElementMultiBase {
    FlatElementImage mSquare;
    DifficultyObject mDifficultyStars;
    DifficultyObject mDifficultyBalls;
    FlatBodyObject mBody;
    public ChoiceObjectPair(Texture2D aLeftTex, int aDepth)
    {
        mSquare = new FlatElementImage(aLeftTex, aDepth);
        mDifficultyStars = new DifficultyObject(ManagerManager.Manager.mMenuReferences.perfectnessStar, aDepth);
        mDifficultyBalls = new DifficultyObject(ManagerManager.Manager.mMenuReferences.difficultyDot, aDepth);
        mElements.Add(new FlatElementMultiBase.ElementOffset(mSquare, new Vector3(50, 0, 0)));
        mElements.Add(new FlatElementMultiBase.ElementOffset(mDifficultyStars,new Vector3(-50,-50,0)));
        mElements.Add(new FlatElementMultiBase.ElementOffset(mDifficultyBalls,new Vector3(-50,-50,0)));

        mDifficultyBalls.Enabled = false;
    }

    public ChoiceObjectPair(Texture2D aLeftTex, CharacterTextureBehaviour aChar, ProGrading.Pose aPose, int aDepth)
    {
        mSquare = new FlatElementImage(aLeftTex, aDepth);
        mDifficultyStars = new DifficultyObject(ManagerManager.Manager.mMenuReferences.perfectnessStar, aDepth);
        mDifficultyBalls = new DifficultyObject(ManagerManager.Manager.mMenuReferences.difficultyDot, aDepth);
        mBody = new FlatBodyObject(aChar,aDepth+1);
        mBody.set_target_pose(aPose);
        mElements.Add(new FlatElementMultiBase.ElementOffset(mSquare, new Vector3(50, 0, 0)));
        mElements.Add(new FlatElementMultiBase.ElementOffset(mDifficultyStars, new Vector3(-50, 0, 0)));
        mElements.Add(new FlatElementMultiBase.ElementOffset(mDifficultyBalls, new Vector3(-50, 0, 0)));
        mElements.Add(new FlatElementMultiBase.ElementOffset(mBody, new Vector3(50, 0, 0)));
    }

    public override Color SoftColor
    {
        get
        {
            return mDifficultyBalls.SoftColor;
        }
        set
        {
            mDifficultyBalls.SoftColor = value;
        }
    }

    public override Color HardColor
    {
        get
        {
            return mDifficultyBalls.HardColor;
        }
        set
        {
            mDifficultyBalls.HardColor = value;
        }
    }

    public override void update_parameters(float aDeltaTime)
    {
        //mBody.
        base.update_parameters(aDeltaTime);
    }
}
