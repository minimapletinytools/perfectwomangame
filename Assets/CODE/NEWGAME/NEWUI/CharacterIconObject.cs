using UnityEngine;
using System.Collections;

public class CharacterIconObject : FlatElementMultiBase {
	FlatElementImage mBackground;
	DifficultyObject mDifficultyStars;
    public FlatBodyObject mBody = null;
	float mBodyOffset = 0;
	
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
		mBodyOffset = mDifficultyStars.mImageElements[0].BoundingBox.width/2 + 5;
		float starOffset = mBackground.BoundingBox.width/2+5;
		mElements.Add(new ElementOffset(mDifficultyStars, new Vector3(-starOffset,-70,0)));
		mElements.Add(new ElementOffset(mBackground, new Vector3(mBodyOffset,0,0)));
		mElements.Add(new ElementOffset(mBody, new Vector3(mBodyOffset,0,0)));
		
		PrimaryGameObject = create_primary_from_elements();
		Depth = aDepth;
		
		set_perfectness(0);
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
		mBody.Depth = Depth + 3;
		
		//hack
		mBody.HardScale = new Vector3(1,1,1);
		mElements.Add(new ElementOffset(aBody, new Vector3(mBodyOffset,0,0)));
		SoftPosition = SoftPosition;
		mBody.PrimaryGameObject.transform.parent = PrimaryGameObject.transform;
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
	
	
	public void set_color(Color aColor)
	{
		//this is dumb, but it works in our situation...
		if(SoftColor.a == 0)
			base.set_color(aColor);
	}
	
	public void set_background_color(Color aColor)
	{
		mBackground.SoftColor = aColor;
	}
	
	public void set_body_color(Color aColor)
	{
		mBody.SoftColor = aColor;
	}
}
