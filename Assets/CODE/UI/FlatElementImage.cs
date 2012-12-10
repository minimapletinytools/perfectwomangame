using UnityEngine;
using System.Collections;

public class FlatElementImage : FlatElementBase
{
    public ImageGameObjectUtility mImage;
    public FlatElementImage(Texture2D aTex, int aDepth)
    {
        SoftColor = new Color(0.5f,0.5f,0.5f,0.5f);
        mImage = new ImageGameObjectUtility(aTex);
        PrimaryGameObject = mImage.ParentObject;
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
            Vector2 extents = mImage.PixelDimension;
            return new Rect(center.x - extents.x / 2.0f, center.y - extents.y / 2.0f, extents.x, extents.y);
        }
    }
}
