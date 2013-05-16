using UnityEngine;
using System.Collections;

public class FlatElementImage : FlatElementBase
{
    public ImageGameObjectUtility mImage;


    //TODO DELETE this version
    public FlatElementImage(Texture aTex, int aDepth)
    {
        initialize(aTex, null, aDepth);
    }

    public FlatElementImage(Texture aTex, System.Nullable<Vector2> aSize, int aDepth)
    {
        initialize(aTex, aSize, aDepth);
    }

    public void initialize(Texture aTex, System.Nullable<Vector2> aSize, int aDepth)
    {
        SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        mImage = new ImageGameObjectUtility(aTex, aSize);
        PrimaryGameObject = mImage.ParentObject;
		if(aTex != null)
			PrimaryGameObject.name = "genImageObject_"+aTex.name;
        Depth = aDepth;
    }

    public override void destroy()
    {
        mImage.destroy();
    }
    public override Rect BoundingBox
    {
        get
        {
            Vector2 center = new Vector2(PrimaryGameObject.transform.position.x,PrimaryGameObject.transform.position.y);
            Vector2 extents = mImage.BaseDimension;
            extents.x *= SoftScale.x;
            extents.y *= SoftScale.y;
            return new Rect(center.x - extents.x / 2.0f, center.y - extents.y / 2.0f, extents.x, extents.y);
        }
    }
	
	public void set_new_texture(Texture aTex, System.Nullable<Vector2> aSize = null)
    {

		mImage.set_new_texture(aTex,aSize);
    }
}
