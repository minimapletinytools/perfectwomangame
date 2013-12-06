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

		Debug.Log(start);

		for(int i = 0; i < count; i++)
		{
			var img = ManagerManager.Manager.mCharacterBundleManager.get_image("ANGELS_"+aChars[i].StringIdentifier);
			mCharacters[i] = new FlatElementImage(img.Image,img.Data.Size,10);
			mCharacters[i].HardPosition = start + offset + step*i;
			mCharacters[i].SoftPosition = mCharacters[i].SoftPosition + new Vector3(0,300,0);

			Debug.Log(mCharacters[i].HardPosition);

			mBadges[i] = new FlatElementImage(ManagerManager.Manager.mNewRef.bbChoicePerfectIcons[aDiffs[i]],11);
			mBadges[i].HardColor = GameConstants.UiWhiteTransparent;

			mElement.Add(mCharacters[i]);
			mElement.Add(mBadges[i]);
		}

		float gTimeBeforeBadges = 1f;
		float gBadgeTime = 1f;
		float gTimeAfterBadges = 1f;

		//wait one second
		var chain = TED.add_event(
			delegate(float aTime) {
				return true;
			},
		gTimeBeforeBadges);

		for(int j = 0; j < count; j++)
		{
			int workingIndex = j;
			chain = chain.then_one_shot(
				delegate() {
					//appear the badge
					mBadges[workingIndex].SoftColor = GameConstants.UiWhite;
					mBadges[workingIndex].HardPosition = mCharacters[workingIndex].SoftPosition + badgeOffset;

					//pulsating scale animation
					mBadges[workingIndex].Events.add_event(
						delegate(FlatElementBase aBase, float aTime) 
						{
							aBase.mLocalScale = Vector3.one * (1+Mathf.Sin(aTime*6))*1.2f;
							if(aTime > 0.3f) 
								return true;
							return false;
						},
					0);

					//jiggle character
					mCharacters[workingIndex].Events.add_event(
						delegate(FlatElementBase aBase, float aTime) 
						{
							aBase.mLocalScale = Vector3.one * (1+Mathf.Sin(aTime*6))*1.2f;
							if(aTime > 0.3f) 
								return true;
							return false;
						},
					0);

					//play a sound
					ManagerManager.Manager.mMusicManager.play_sound_effect("badge_blip_TODO");
				},
			gBadgeTime);
		}

		//put character back
		chain = chain.then_one_shot(
			delegate() {
				for(int i = 0; i < count; i++)
				{
					mBadges[i].SoftColor = GameConstants.UiWhiteTransparent;
					mCharacters[i].SoftPosition = start + offset + step*i;
				}
			},
		gTimeAfterBadges);
	}
}
