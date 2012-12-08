using UnityEngine;
using System.Collections;

public class DifficultyObject : FlatElementMultiBase
{
    public int Difficulty { get; private set; }


    public FlatElementImage[] mImageElements = new FlatElementImage[4];
    public DifficultyObject(Texture2D aTex, int aDepth)
    {
        for (int i = 0; i < 4; i++)
        {
            mImageElements[i] = new FlatElementImage(aTex, aDepth);
            mElements.Add(new FlatElementMultiBase.ElementOffset(mImageElements[i], new Vector3(0, i * 40, 0)));
        }

        Difficulty = 0;
    }

    public override void set()
    {
        
        for (int i = 0; i < 4; i++)
        {
            if (i <= Difficulty)
            {
                mImageElements[i].set();
            }
            else
            {
                mImageElements[i].HardPosition = (Random.insideUnitCircle.normalized * 3000);
            }
        }
    }

}
