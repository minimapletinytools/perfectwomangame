using UnityEngine;
using System.Collections;

public class CharacterIconObject : FlatElementMultiBase {
	FlatElementImage mBackground;
	DifficultyObject mDifficultyStars;
	FlatElementMultiBase.ElementOffset mText = null;
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
			mBody =  new FlatBodyObject(aIcon,aDepth+3);
		
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
	public void set_name(string aName)
	{
		//TODO complete hack, fix this..
		if(mText != null)
		{
			mElements.Remove(mText);
			mText.Element.destroy();
			mText = null;
		}
		if(aName != "")
		{
			mText = new ElementOffset(new FlatElementText(ManagerManager.Manager.mNewRef.genericFont,30,"",Depth +2),new Vector3(mBodyOffset,-90,0));
			(mText.Element as FlatElementText).HardColor = new Color(0,0,0,1);
			(mText.Element as FlatElementText).Text = aName;
			mElements.Add(mText);
		}
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
	
	
	
	public void set_background_color(Color aColor)
	{
		mBackground.SoftColor = aColor;
	}
	
	public static Color[] sDiffColorMapping = new Color[]{new Color(0,0.8f,0,1), new Color(0.8f,0.8f,0,1), new Color(0.9f,0.4f,0,1), new Color(0.8f,0,0,1)};
	public void set_difficulty(int aDiff)
	{
		set_body_color(sDiffColorMapping[aDiff]);
	}
	public void set_body_color(Color aColor)
	{
		mBody.SoftColor = aColor;
	}
}
