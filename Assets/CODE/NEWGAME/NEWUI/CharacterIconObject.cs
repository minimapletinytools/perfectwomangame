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
		
		
		
		if(aIcon == null)
		{
			CharacterTextureBehaviour ctb = (GameObject.Instantiate(ManagerManager.Manager.mReferences.mMiniChar) as  GameObject).GetComponent<CharacterTextureBehaviour>();
			mBody =  new FlatBodyObject(ctb,aDepth);
			GameObject.Destroy(ctb.gameObject);
		}
		else
			mBody =  new FlatBodyObject(aIcon,aDepth);
		
		mBody.HardShader = ManagerManager.Manager.mReferences.mMiniCharacterShader;
		
		//TODO position
		float bodyOffset = mDifficultyStars.mImageElements[0].BoundingBox.width/2 + 5;
		float starOffset = mBackground.BoundingBox.width/2+5;
		mElements.Add(new ElementOffset(mDifficultyStars, new Vector3(-starOffset,-70,0)));
		mElements.Add(new ElementOffset(mBackground, new Vector3(bodyOffset,0,0)));
		mElements.Add(new ElementOffset(mBody, new Vector3(bodyOffset,0,0)));
		
		PrimaryGameObject = create_primary_from_elements();
		
		Depth = aDepth;
	}
	public FlatBodyObject take_body()
	{
		FlatBodyObject r = reposses_element(mBody) as FlatBodyObject;
		mBody = null;
		return r;
	}
	public void return_body(FlatBodyObject aBody)
	{
		mBody = aBody;
		mElements.Add(new ElementOffset(aBody, new Vector3(0,0,0)));
	}
	public void set_pose(ProGrading.Pose aPose)
	{
		if(mBody != null)
			mBody.set_target_pose(aPose);
	}
	public void set_perfectness(int perfectness)
    {
        mDifficultyStars.Difficulty = perfectness;
    }
}
