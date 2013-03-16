using UnityEngine;
using System.Collections;

public class CharacterIconObject : FlatElementMultiBase {
	FlatElementImage mBackground;
	DifficultyObject mDifficultyStars;
    public FlatBodyObject mBody = null;
	
    public CharacterIconObject(CharacterLoader aIcon, int aDepth)
    {
		mBackground = new FlatElementImage(ManagerManager.Manager.mNewRef.pbCharacterIconBackground,aDepth);
		mDifficultyStars = new DifficultyObject(ManagerManager.Manager.mNewRef.uiPerfectStar,aDepth+2);
		mBody = new FlatBodyObject(aIcon, aDepth+1);
		
		//TODO position
		mElements.Add(new ElementOffset(mBackground, new Vector3(0,0,0)));
		mElements.Add(new ElementOffset(mDifficultyStars, new Vector3(0,0,0)));
		mElements.Add(new ElementOffset(mBody, new Vector3(0,0,0)));
		
		Depth = aDepth;
	}
	public void set_perfectness(int perfectness)
    {
        mDifficultyStars.Difficulty = perfectness;
    }
}
