using UnityEngine;
using System.Collections;

public class FlatElementMultiImage : FlatElementMultiBase
{
    public FlatElementMultiImage(int aDepth)
    {
        Depth = aDepth;
    }
	

    public ElementOffset add_image(Texture2D aTex, Vector3 aOffset, Vector2? aSize = null)
    {
        FlatElementImage image = new FlatElementImage(aTex,  aSize, Depth + mElements.Count);
		ElementOffset r = new ElementOffset(image, aOffset);
        mElements.Add(r);
        SoftPosition = SoftPosition;
		return r;
		
    }
}
