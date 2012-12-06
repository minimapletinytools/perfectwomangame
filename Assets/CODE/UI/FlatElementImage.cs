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

}
