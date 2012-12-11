using UnityEngine;
using System.Collections;

public class MeterObject : FlatElementMultiBase {

    //TODO
    FlatElementImage mBack;
    FlatElementImage mFront;
    FlatElementImage mFill;

    FlatElementMultiBase.ElementOffset mFillOffset;

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
            mFill.mImage.PixelDimension = new Vector2(value * 512f, 40);
        }
    }
    public MeterObject(Texture2D aFront, Texture2D aBack, Color aFill, int aDepth)
    {
        SoftColor = aFill;

        
        mBack = new FlatElementImage(aBack, aDepth);
        mFill = new FlatElementImage(null, aDepth+1);
        mFront = new FlatElementImage(aFront, aDepth+2);
        mFillOffset = new FlatElementMultiBase.ElementOffset(mFill, new Vector3(0, 0, 0));

        mElements.Add(new FlatElementMultiBase.ElementOffset(mBack,new Vector3(0,0,0)));
        mElements.Add(mFillOffset);
        mElements.Add(new FlatElementMultiBase.ElementOffset(mFront, new Vector3(0, 0, 0)));

        PrimaryGameObject = create_primary_from_elements();

        SoftInterpolation = SoftInterpolation;
        Percentage = 0.0f;
        Depth = aDepth;
    }

    public override void update_parameters(float aDeltaTime)
    {
        //mFillOffset.Element.SoftInterpolation = 1.0f;
        mFillOffset.Element.HardPosition = SoftPosition + new Vector3((757 / 2f - 245) - Percentage * 512/2f, 0, 0);
        base.update_parameters(aDeltaTime);
    }


}
