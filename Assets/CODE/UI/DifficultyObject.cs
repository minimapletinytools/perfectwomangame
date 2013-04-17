using UnityEngine;
using System.Collections;

public class DifficultyObject : FlatElementMultiBase
{
    int mDifficulty;
    public int Difficulty 
    {
        get { return mDifficulty; } 
        set
        {
            mDifficulty = value;

            /* use this if you ever fix blending
            for (int i = 0; i < 4; i++)
            {
                if (i <= Difficulty)
                {
                    mImageElements[i].SoftColor = new Color(0.5f, 0.5f, 0.5f, 1);
                }
                else
                {
                    mImageElements[i].SoftColor = new Color(0.5f, 0.5f, 0.5f, 0);
                }
            }*/
        } 
    }


    public FlatElementImage[] mImageElements = new FlatElementImage[4];
    public Vector3[] mNaughtyPositions = new Vector3[4];
    public DifficultyObject(Texture2D aTex, int aDepth)
    {
        PrimaryGameObject = new GameObject("genDifficultyParent");
        for (int i = 0; i < 4; i++)
        {
            mImageElements[i] = new FlatElementImage(aTex, aDepth);
            mElements.Add(new FlatElementMultiBase.ElementOffset(mImageElements[i], new Vector3(0, i * 47, 0)));
            mImageElements[i].PrimaryGameObject.transform.parent = PrimaryGameObject.transform;
            mNaughtyPositions[i] = (Random.insideUnitCircle.normalized * 3000);
        }
        Difficulty = 0;
    }


    
    

    public override void set()
    {

        base.SoftPosition = base.SoftPosition;
        base.SoftInterpolation = base.SoftInterpolation;
        for (int i = 0; i < 4; i++)
        {
            if (i <= Difficulty)
            { 
                mImageElements[i].set();
            }
            else
            {
                //TODO probably should not call every frame poo poo....
                mImageElements[i].SoftPosition = mNaughtyPositions[i];
                mImageElements[i].SoftInterpolation = 0.01f;
                mImageElements[i].set();
            }
        }
    }
    
}
