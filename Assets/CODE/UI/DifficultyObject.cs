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
        } 
    }


    public FlatElementImage[] mImageElements = new FlatElementImage[4];
    public DifficultyObject(Texture2D aTex, int aDepth)
    {
        PrimaryGameObject = new GameObject("genDifficultyParent");
        for (int i = 0; i < 4; i++)
        {
            mImageElements[i] = new FlatElementImage(aTex, aDepth);
            mElements.Add(new FlatElementMultiBase.ElementOffset(mImageElements[i], new Vector3(0, i * 40, 0)));
            mImageElements[i].PrimaryGameObject.transform.parent = PrimaryGameObject.transform;
        }
        


        Difficulty = 0;
    }


    
    
    public override void  update_parameters(float aDeltaTime)
    {
        
        for (int i = 0; i < 4; i++)
        {
            if (i <= Difficulty)
            {
                base.SoftPosition = base.SoftPosition;
            }
            else
            {
                //TODO probably should not call every frame poo poo....
                mImageElements[i].HardPosition = (Random.insideUnitCircle.normalized * 3000);
            }
        }
        base.update_parameters(aDeltaTime);
    }

    public override void set()
    {

        for (int i = 0; i < 4; i++)
        {
            if (i <= Difficulty)
            {
                base.SoftPosition = base.SoftPosition;
                mImageElements[i].set();
            }
            else
            {
                //TODO probably should not call every frame poo poo....
                mImageElements[i].HardPosition = (Random.insideUnitCircle.normalized * 3000);
            }
        }
    }
    
}
