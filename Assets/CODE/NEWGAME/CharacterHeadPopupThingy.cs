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


	public void popup_character(CharacterIndex[] aChars, int[] aDiffs)
	{
		int count = aChars.Length;

		FlatElementImage[] mCharacters = null;
		FlatElementImage[] mBadges = null;
		mCharacters = new FlatElementImage[count];
		mBadges = new FlatElementImage[count];


		//var sizeImg = ManagerManager.Manager.mCharacterBundleManager.get_image("ICON_05-1");
		//TODO read from sizeImg
		float gIconHeight = 343;
		float gIconWidth = 266;

		Vector3 step = new Vector3(-500,0,0);
		Vector3 start = NIM.mFlatCamera.get_point(0,-1) - new Vector3(0,gIconHeight/2,0);
		Vector3 offset = (count-1)*(-step)/2f;
		Vector3 badgeOffset = new Vector3(-75,150,0);



		for(int i = 0; i < count; i++)
		{
			CharacterBundleManager.ImageSizePair img = null;
			img = ManagerManager.Manager.mCharacterBundleManager.get_image("ICON_"+aChars[i].StringIdentifier);
			if(img == null || img.Data == null || img.Image == null)
				img = ManagerManager.Manager.mCharacterBundleManager.get_image("ANGELS_"+aChars[i].StringIdentifier);

			mCharacters[i] = new FlatElementImage(img.Image,img.Data.Size,10);
			mCharacters[i].HardPosition = start + offset + step*i;
			mCharacters[i].SoftPosition = mCharacters[i].SoftPosition + new Vector3(0,gIconHeight + 100,0);

			//Debug.Log(mCharacters[i].HardPosition);

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
							aBase.mLocalScale = Vector3.one * (1+Mathf.Sin(aTime*6)*0.3f);
							if(aTime > 0.3f) 
								return true;
							return false;
						},
					0);

					//jiggle character
					mCharacters[workingIndex].Events.add_event(
						delegate(FlatElementBase aBase, float aTime) 
						{
							aBase.mLocalScale = Vector3.one * (1+Mathf.Sin(aTime*6)*0.3f);
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

		//TODO I think this crashes if you skip through th ecutscenes...	
		//put character back
		chain = chain.then_one_shot(
			delegate() {
				for(int i = 0; i < count; i++)
				{
					int index = i;
					mBadges[i].SoftColor = GameConstants.UiWhiteTransparent;
					mCharacters[i].SoftPosition = start + offset + step*i + new Vector3(0,-100,0); //move down a little more to compensate for scale change
					//destroy them eventually.
					TED.add_one_shot_event(
						delegate()
					    {
							mBadges[index].destroy();
							mCharacters[index].destroy();
						},
					3);
				}
			},
		gTimeAfterBadges);
	}
}
