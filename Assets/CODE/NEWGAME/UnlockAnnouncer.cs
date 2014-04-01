using UnityEngine;
using System.Collections.Generic;


public class FlatUnlockIcon : FlatElementMultiBase{
	FlatElementImage background;
	FlatElementImage icon;
	FlatElementText name;

	public FlatUnlockIcon(CharacterIndex aChar, bool big, int aDepth)
	{
		var bgImage = ManagerManager.Manager.mCharacterBundleManager.get_image(big?"UNLOCKABLES_BOX_BIG" : "UNLOCKABLES_BOX_SMALL");
		var charIcon = ManagerManager.Manager.mCharacterBundleManager.get_image("ICON_"+aChar.StringIdentifier);

		background = new FlatElementImage(bgImage.Image,bgImage.Data.Size,aDepth);
		icon = new FlatElementImage(charIcon.Image,charIcon.Data.Size,aDepth+1);
		name = new FlatElementText(ManagerManager.Manager.mNewRef.genericFont,100,FlatElementText.convert_to_multiline(2,aChar.ShortName.ToUpper()),aDepth + 1);

		mElements.Add(new FlatElementMultiBase.ElementOffset(background, new Vector3(0,0,0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(icon, new Vector3(0,70,0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(name, new Vector3(0,-30,0)));

		PrimaryGameObject = create_primary_from_elements();
		Depth = aDepth; 
	}
}

public class FlatUnlockBadge : FlatElementMultiBase
{
	FlatElementText text1;
	FlatElementText text2;
	FlatElementText text3;

	FlatElementImage background;

	FlatUnlockIcon mainIcon;
	FlatUnlockIcon[] contributors;

	
	public FlatUnlockBadge(CharacterIndex aChar, int aDepth)
	{
		var bgImage = ManagerManager.Manager.mCharacterBundleManager.get_image("UNLOCKABLES_PLATE");
		background = new FlatElementImage(bgImage.Image,bgImage.Data.Size,aDepth);
		mainIcon = new FlatUnlockIcon(aChar,true,aDepth+1);

		//TODO construct contributors
		contributors = new FlatUnlockIcon[0];


		mElements.Add(new FlatElementMultiBase.ElementOffset(background, new Vector3(0,0,0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mainIcon, new Vector3(0,70,0)));
		Vector3 step = new Vector3(-200,0,0);
		Vector3 start = (contributors.Length-1)*(-step)/2f;
		for(int i = 0; i < contributors.Length; i++)
		{
			FlatUnlockIcon e = contributors[i];
			mElements.Add(new FlatElementMultiBase.ElementOffset(e, start + step*i));
		}
		
		PrimaryGameObject = create_primary_from_elements();
		Depth = aDepth; 
	}
}
//handles all animations when announcing whats been unlocked...
public class UnlockAnnouncer 
{
	public TimedEventDistributor TED { get; private set; }
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();

	SunsetManager mSunset;
	public UnlockAnnouncer(SunsetManager aSunset)
	{
		mSunset = aSunset;
		TED = new TimedEventDistributor();
	}

	public bool IsAnnouncing{
		get{
			return TED.has_event();
		}
	}

	
	public void update()
	{
		foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		TED.update(Time.deltaTime);
	}

	//TODO add text argument
	public void announce_unlock(CharacterIndex aChar)
	{
		float gDisplayTime = 5;

		FlatUnlockBadge badge = new FlatUnlockBadge(aChar,30);
		badge.HardPosition = mSunset.mFlatCamera.Center + new Vector3(0,2000,0);
		badge.SoftPosition = mSunset.mFlatCamera.Center;
		mElement.Add(badge);

		TED.add_one_shot_event(
			delegate() {
				badge.SoftPosition = badge.SoftPosition + new Vector3(0,-2000,0);
			}
		,gDisplayTime).then_one_shot(
			delegate(){
				mElement.Remove(badge);
				badge.destroy();
			}
		,1.5f);
	}
}
