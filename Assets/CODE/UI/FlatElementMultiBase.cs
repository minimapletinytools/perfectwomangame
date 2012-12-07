using UnityEngine;
using System.Collections.Generic;

public class FlatElementMultiBase : FlatElementBase
{
    protected List<FlatElementBase> mElements = new List<FlatElementBase>();

    public override int Depth
    {
        get { return mDepth; }
        set
        {
            mDepth = value;
            int i = 0;
            foreach (FlatElementBase e in mElements)
                e.Depth = value + i++;
        }
    }

    public override float SoftInterpolation
    {
        get
        {
            return base.SoftInterpolation;
        }
        set
        {
            base.SoftInterpolation = value;
            float i = 1;
            foreach (FlatElementBase e in mElements)
            {
                e.SoftInterpolation = value*i;
                i *= 0.2f;
            }
        }


    }
    public override void update_parameters(float aDeltaTime)
    {

        foreach (FlatElementBase e in mElements)
        {
            //e.HardPosition = base.HardPosition;
            e.SoftPosition = base.SoftPosition;
            //e.HardFlatRotation = base.HardFlatRotation;
            e.SoftFlatRotation = base.SoftFlatRotation;
            //e.HardColor = base.HardColor;
            e.SoftColor = base.SoftColor;

            e.update_parameters(aDeltaTime);
        }
    }
    
    public override void set_position(Vector3 aPos)
    {
        foreach (FlatElementBase e in mElements)
            e.set_position(aPos);
    }
    public override void set_rotation(Quaternion aRot)
    {
        foreach (FlatElementBase e in mElements)
            e.set_rotation(aRot);
    }
    public override void set_color(Color aColor)
    {
        foreach (FlatElementBase e in mElements)
            e.set_color(aColor);
    }


}
