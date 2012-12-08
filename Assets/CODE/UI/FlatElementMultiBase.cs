using UnityEngine;
using System.Collections.Generic;

public class FlatElementMultiBase : FlatElementBase
{

    public class ElementOffset
    {
        public FlatElementBase Element{get; private set;}
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public ElementOffset(FlatElementBase element, Vector3 offset)
        {
            Element = element;
            Position = offset;
            Rotation = Quaternion.identity;
        }
    }
    protected List<ElementOffset> mElements = new List<ElementOffset>();

    public override void destroy()
    {
        foreach (ElementOffset e in mElements)
            e.Element.destroy();
        mElements.Clear();
    }
    public override int Depth
    {
        get { return mDepth; }
        set
        {
            mDepth = value;
            int i = 0;
            foreach (ElementOffset e in mElements)
                e.Element.Depth = value + i++;
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
            foreach (ElementOffset e in mElements)
            {
                e.Element.SoftInterpolation = value*i;
                i *= 0.8f;
            }
        }
    }

    public override Vector3 SoftPosition
    {
        get { return base.SoftPosition; }
        set 
        { 
            base.SoftPosition = value;
            foreach (ElementOffset e in mElements)
                e.Element.SoftPosition = value + e.Position;
        }
    }
    public override Vector3 HardPosition
    {
        get { return base.HardPosition; }
        set
        {
            base.HardPosition = value;
            foreach (ElementOffset e in mElements)
                e.Element.HardPosition = value + e.Position;
        }
    }
    public override float SoftFlatRotation
    {
        get { return base.SoftFlatRotation; }
        set { 
            base.SoftFlatRotation = value;
            foreach (ElementOffset e in mElements)
                e.Element.SoftFlatRotation = e.Rotation.flat_rotation()*value;
        }
    }
    public override float HardFlatRotation
    {
        get { return base.HardFlatRotation; }
        set { 
            base.HardFlatRotation = value;
            foreach (ElementOffset e in mElements)
                e.Element.HardFlatRotation = e.Rotation.flat_rotation() * value;
        }
    }

    public override Color SoftColor
    {
        get { return base.SoftColor; }
        set { 
            base.SoftColor = value;
            foreach (ElementOffset e in mElements)
                e.Element.SoftColor = value;
        }
    }
    public override Color HardColor
    {
        get { return HardColor; }
        set { 
            base.HardColor = value;
            foreach (ElementOffset e in mElements)
                e.Element.HardColor = value;
        }
    }





    public override void update_parameters(float aDeltaTime)
    {
        foreach (ElementOffset e in mElements)
            e.Element.update_parameters(aDeltaTime);
    }

    public override void set()
    {
        foreach (ElementOffset e in mElements)
            e.Element.set();
    }
}
