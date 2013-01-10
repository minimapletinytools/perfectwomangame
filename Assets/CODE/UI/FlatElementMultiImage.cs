using UnityEngine;
using System.Collections;

public class FlatElementMultiImage : FlatElementMultiBase
{
    public FlatElementMultiImage(int aDepth)
    {
        Depth = aDepth;
    }

    public void add_image(Texture2D aTex, Vector3 aOffset, Vector2? aSize = null)
    {
        FlatElementImage image = new FlatElementImage(aTex,  aSize, Depth + mElements.Count);
        mElements.Add(new ElementOffset(image, aOffset));
        SoftPosition = SoftPosition;
    }
}
