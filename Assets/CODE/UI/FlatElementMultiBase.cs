using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FlatElementMultiBase : FlatElementBase
{

    public class ElementOffset
    {
        public FlatElementBase Element{get; private set;}
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Quaternion Rotation { get; set; }
        public ElementOffset(FlatElementBase element, Vector3 offset)
        {
            Element = element;
            Position = offset;
            Scale = Vector3.one;
            Rotation = Quaternion.identity;
        }
    }
    public List<ElementOffset> mElements = new List<ElementOffset>();
	
	//returns null if element did not exist.
	public FlatElementBase reposses_element(FlatElementBase aRemove)
	{
		for(int i = 0; i < mElements.Count; i++)
			if(mElements[i].Element == aRemove)
			{
				mElements.RemoveAt(i);
				return aRemove;
			}
		return null;
	}
	
	public void destroy_element(FlatElementBase aRemove)
	{
		FlatElementBase d = reposses_element(aRemove);
		if(d != null)
			d.destroy();
	}
	
    protected GameObject create_primary_from_elements()
    {
        GameObject r = new GameObject("genMultiFlatParent");
        foreach (ElementOffset e in mElements)
            e.Element.PrimaryGameObject.transform.parent = r.transform;
        return r;
    }
    public override void destroy()
    {
        foreach (ElementOffset e in mElements)
            e.Element.destroy();
        mElements.Clear();
		try{GameObject.Destroy(PrimaryGameObject);} //probbaly don't need the try catch here
		catch{}
    }
	
    public override int Depth
    {
        get { return mDepth; }
        set
        {
			//TODO you need to change vereything else lol...
			//loolololol
            /*int i = 0;
            foreach (ElementOffset e in mElements)
			{
                e.Element.Depth = value + i++;// + (e.Element.Depth - mDepth);
			}*/
			mDepth = value;
        }
    }
	
	public override Rect BoundingBox
    {
		get{
			if(mElements.Count == 0)
				return new Rect(0,0,0,0);
			Rect r = mElements[0].Element.BoundingBox;
			foreach(ElementOffset e in mElements)
				r = r.union(e.Element.BoundingBox);
			return r;
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
				i *= 1;//0.8f;
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
    public override Vector3 SoftScale
    {
        get { return base.SoftScale; }
        set 
        {
            base.SoftScale = value;
            foreach (ElementOffset e in mElements)
                e.Element.SoftScale = value.component_multiply(e.Scale);
        }
    }
    public override Vector3 HardScale
    {
        get { return base.HardScale; }
        set
        {
            base.HardScale = value;
            foreach (ElementOffset e in mElements)
                e.Element.HardScale = value.component_multiply(e.Scale);
        }
    }
    public override float SoftFlatRotation
    {
        get { return base.SoftFlatRotation; }
        set { 
            base.SoftFlatRotation = value;
            foreach (ElementOffset e in mElements)
                e.Element.SoftFlatRotation = e.Rotation.flat_rotation() * value;
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
            //foreach (ElementOffset e in mElements)
            //    e.Element.SoftColor = value;
        }
    }
    public override Color HardColor
    {
        
        get { return base.HardColor; }
        set { 
            base.HardColor = value;
            //foreach (ElementOffset e in mElements)
            //    e.Element.HardColor = value;
        }
    }



    bool mEnabled = true;
    public override bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            if (mEnabled != value)
            {
                foreach (var e in mElements)
                {
                    e.Element.Enabled = value;
                }
                mEnabled = value;
            }
        }
    }
	
	


    public override void update_parameters(float aDeltaTime)
    {
		base.update_parameters(aDeltaTime);
        foreach (ElementOffset e in mElements)
            e.Element.update_parameters(aDeltaTime);
    }
	
	
    //TODO do your fancy blending version...
    public virtual new void set_color(Color aColor)
    {
		//TODO add or multiply mLocalcolor??
		foreach (ElementOffset e in mElements)
			e.Element.set_color(aColor*(e.Element.HardColor)*2); 
    }
    

    public override void set()
    {
        foreach (ElementOffset e in mElements)
			e.Element.set();
		//this will override color changes from above
		//TODO add or multiply mLocalcolor??
		set_color(HardColor);
    }
}
