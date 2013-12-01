using UnityEngine;
using System.Collections.Generic;

public class CharacterHeadPopupThingy 
{
	NewInterfaceManager NIM {get; set;}
	HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	public TimedEventDistributor TED { get; private set; }
	
	public CharacterHeadPopupThingy(NewInterfaceManager aNIM)
	{
		NIM = aNIM;
		TED = new TimedEventDistributor();
	}
	
	public void update()
	{
		foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		
		TED.update(Time.deltaTime);
	}
	
	FlatElementImage[] mCharacters = null;
	FlatElementImage[] mBadges = null;
	
	public void cleanup()
	{
		if(mCharacters != null)
			for(int i = 0; i < mCharacters.Length; i++)
				mCharacters[i].destroy();
		
		if(mBadges != null)
			for(int i = 0; i < mBadges.Length; i++)
				mBadges[i].destroy();
		
		mCharacters = null;
		mBadges = null;
	}
	public void popup_character(CharacterIndex[] aChars, int[] aDiffs)
	{
		int count = aChars.Length;
		
		cleanup();
		mCharacters = new FlatElementImage[count];
		mBadges = new FlatElementImage[count];

		Vector3 start = NIM.mFlatCamera.get_point(0,-1.2f);
		Vector3 step = new Vector3(-200,0,0);
		Vector3 offset = count*step/2f;
		Vector3 badgeOffset = new Vector3(-50,-100,0);

		for(int i = 0; i < count; i++)
		{

			var img = ManagerManager.Manager.mCharacterBundleManager.get_image(aChars[i].StringIdentifier);
			mCharacters[i] = new FlatElementImage(img.Image,img.Data.Size,10);
			mCharacters[i].HardPosition = start + offset + step*i;
			mBadges[i] = new FlatElementImage(ManagerManager.Manager.mNewRef.bbChoicePerfectIcons[aDiffs[i]],11);
			mBadges[i].HardColor = GameConstants.UiWhiteTransparent;

		}

		for(int i = 0; i < count; i++)
		{
			mBadges[i].SoftColor = GameConstants.UiWhite;
			//pulsating scale animation
			mBadges[i].Events.add_event(
				delegate(FlatElementBase aBase, float aTime) 
				{
					aBase.mLocalScale = Vector3.one * (1+Mathf.Sin(aTime*6))*1.2f;
					if(aTime > 0.3f) 
						return true;
					return false;
				},
			0);
			//pop out charcater
			mCharacters[i].SoftPosition = mCharacters[i].SoftPosition + new Vector3(0,300,0);
			//TODO sound
			//TODO maybe jiggle player a little
			//put character back
			mCharacters[i].SoftPosition = start + offset + step*i;
		}
	}
}
