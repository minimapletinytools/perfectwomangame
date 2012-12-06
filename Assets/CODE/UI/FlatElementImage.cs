using UnityEngine;
using System.Collections;

public class FlatElementImage : FlatElementBase
{

    public FlatElementImage(Texture2D aTex, float aDepth)
    {
        Depth = aDepth;
        SoftColor = new Color(0.5f,0.5f,0.5f,0.5f);
        PrimaryGameObject = ImageGameObjectUtility.create(aTex);
    }

}
