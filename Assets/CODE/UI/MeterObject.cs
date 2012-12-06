using UnityEngine;
using System.Collections;

public class MeterObject : FlatElementMultiBase {

    //TODO
    FlatElementImage mBack;
    FlatElementImage mFront;
    FlatElementImage mFill;

    float mPercentage;
    public float Percentage 
    {
        get
        {
            return mPercentage;
        }
        set
        {
            mPercentage = value;
            mFill.mImage.PixelDimension = new Vector2(value * 512f, 50);
        }
    }
    public MeterObject(Texture2D aFront, Texture2D aBack, Color aFill, int aDepth)
    {
        SoftColor = aFill;
        PrimaryGameObject = new GameObject("genFlatElementImageParent");

        
        mBack = new FlatElementImage(aBack, aDepth);
        mFront = new FlatElementImage(aFront, aDepth);
        mFill = new FlatElementImage(null, aDepth);

        mElements.Add(mBack);
        mElements.Add(mFill);
        mElements.Add(mFront);

        Percentage = 0.5f;

        Depth = aDepth;
    }


    public override void set_position(Vector3 aPos)
    {
        mFront.set_position(aPos);
        mBack.set_position(aPos);
        //TODO fix me
        mFill.set_position(aPos + new Vector3((757 / 2f - 245) - Percentage * 512f, 0, 0));
    }

    public override void set_color(Color aColor)
    {
        mFill.set_color(aColor);
    }
}
