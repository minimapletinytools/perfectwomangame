using UnityEngine;
using System.Collections;

//TODO DELETE
public class CharacterIconObject : FlatElementMultiBase {
	FlatElementImage mBackground = null;
	DifficultyObject mDifficultyStars = null;
	FlatElementMultiBase.ElementOffset mText = null;
	FlatElementImage mIcon = null;
    //public FlatBodyObject mBody = null;
	float mBodyOffset = 0;
	
    public CharacterIconObject(CharacterIndex aIndex, int aDepth)
    {

		//NOTE these assets were removed since this class is no longer being used.
		//mBackground = new FlatElementImage(ManagerManager.Manager.mNewRef.pbCharacterIconBackground,aDepth);
		//mDifficultyStars = new DifficultyObject(ManagerManager.Manager.mNewRef.uiPerfectStar,aDepth+1);
		mIcon = new FlatElementImage(null,aDepth+3);
		if(aIndex.LevelIndex != -1)
		{
			var loaded = ManagerManager.Manager.mCharacterBundleManager.get_image("BOX_"+aIndex.StringIdentifier);
			mIcon.set_new_texture(loaded.Image,null);//loaded.Data.Size);
		}
			
		//TODO delete
		/*
		if(aIcon == null)
		{
			CharacterTextureBehaviour ctb = (GameObject.Instantiate(ManagerManager.Manager.mReferences.mMiniChar) as  GameObject).GetComponent<CharacterTextureBehaviour>();
			mBody =  new FlatBodyObject(ctb,aDepth);
			GameObject.Destroy(ctb.gameObject);
		}
		else
			mBody =  new FlatBodyObject(aIcon,aDepth+3);
		mBody.HardShader = ManagerManager.Manager.mReferences.mMiniCharacterShader;
		*/
		
		
		
		//mBodyOffset = mDifficultyStars.mImageElements[0].BoundingBox.width/2 + 5;
		//float starOffset = mBackground.BoundingBox.width/2+5;
		
		//float starOffset = 0;
		mBodyOffset = 0;
		//mElements.Add(new ElementOffset(mDifficultyStars, new Vector3(-starOffset,-70,0)));
		mElements.Add(new ElementOffset(mBackground, new Vector3(mBodyOffset,0,0)));
		mElements.Add (new ElementOffset(mIcon, new Vector3(mBodyOffset,20,0)));
		//mElements.Add(new ElementOffset(mBody, new Vector3(mBodyOffset,0,0)));
		
		
		PrimaryGameObject = create_primary_from_elements();
		Depth = aDepth;
		
		set_perfectness(0);
	}
	
	public void set_depth(int aDepth)
	{
		foreach (ElementOffset e in mElements)
		{
            e.Element.Depth = aDepth + (e.Element.Depth - Depth);
		}
		Depth = aDepth;
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
			aName = aName.ToUpper();
			mText = new ElementOffset(new FlatElementText(ManagerManager.Manager.mNewRef.genericFont,30,"",Depth +1),new Vector3(mBodyOffset,-90,0));
			(mText.Element as FlatElementText).HardColor = new Color(0,0,0,1);
			if(aName.Length > 9 && aName.Contains(" "))
			{
				(mText.Element as FlatElementText).Text = FlatElementText.convert_to_multiline(2,aName);
				mText.Position = mText.Position + new Vector3(0,15,0);
			}
			else
				(mText.Element as FlatElementText).Text = aName;
			mElements.Add(mText);
		}
	}
	/*
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
	}*/
	
	public void set_perfectness(int perfectness)
    {
        mDifficultyStars.Difficulty = perfectness;
    }
	
	public void set_background_color(Color aColor)
	{
		mBackground.SoftColor = aColor;
	}
	
	public void set_difficulty(int aDiff)
	{
		if(aDiff == -1)
			set_icon_color(new Color(0.25f,0.25f,0.25f,0.5f));
		else
			set_icon_color(GameConstants.IconDifficultyColorsOverTwo[aDiff]);
	}
	
	public void set_icon_color(Color aColor)
	{
		//mBody.SoftColor = aColor;
		mIcon.SoftColor = aColor;	
	}
}
