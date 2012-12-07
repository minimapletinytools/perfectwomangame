using UnityEngine;
using System.Collections;

public class ChoiceObjectPair : FlatElementMultiBase {
    FlatElementImage mSquare;
    DifficultyObject mDifficultyStars;
    DifficultyObject mDifficultyBalls;
    public ChoiceObjectPair(Texture2D aLeftTex, int aDepth)
    {
        mSquare = new FlatElementImage(aLeftTex, aDepth);
        mDifficultyStars = new DifficultyObject(ManagerManager.Manager.mMenuReferences.perfectnessStar, aDepth);
        mDifficultyBalls = new DifficultyObject(ManagerManager.Manager.mMenuReferences.difficultyDot, aDepth);
        mElements.Add(mSquare);
        mElements.Add(mDifficultyStars);
        mElements.Add(mDifficultyBalls);
    }

    public override void set_position(Vector3 aPos)
    {
        mSquare.set_position(aPos + new Vector3(50, 0, 0));
        mDifficultyStars.set_position(aPos + new Vector3(-50, 0, 0));
        mDifficultyBalls.set_position(aPos + new Vector3(-50, 0, 0));
    }

    public override void set_color(Color aColor)
    {
        mDifficultyBalls.set_color(aColor);
    }
}
