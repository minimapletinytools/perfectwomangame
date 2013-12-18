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


	
	public void create_shine_over_character(FlatElementImage aHead,bool positive, float duration)
	{
		var shineImage = ManagerManager.Manager.mCharacterBundleManager.get_image("STREAM");
		FlatElementImage shine = new FlatElementImage(shineImage.Image,shineImage.Data.Size,3);
		Vector3 targetPos = aHead.SoftPosition + new Vector3(0,shine.BoundingBox.height/2-150,0);
		shine.HardPosition = targetPos + new Vector3(0,500,0);
		//shine.SoftPosition = targetPos;
		shine.HardColor = positive ? GameConstants.UiGreenTransparent : GameConstants.UiRedTransparent; 
		shine.HardColor = positive ? GameConstants.UiGreen : GameConstants.UiRed; 
		mElement.Add(shine); 
	
		var boxImage = ManagerManager.Manager.mCharacterBundleManager.get_image("CUTSCENE_BOX");
		FlatElementImage box = new FlatElementImage(boxImage.Image,boxImage.Data.Size,3);
		box.HardPosition = aHead.SoftPosition;
		box.HardColor = positive ? GameConstants.UiGreenTransparent : GameConstants.UiRedTransparent; 
		box.HardColor = positive ? GameConstants.UiGreen : GameConstants.UiRed; 
		mElement.Add(box);
	
		TED.add_one_shot_event(
			delegate() {
				shine.HardColor = GameConstants.UiWhiteTransparent;
			box.HardColor = GameConstants.UiWhiteTransparent;
			},
		duration).then_one_shot(
			delegate() {
				mElement.Remove(shine);
				mElement.Remove(box);
				shine.destroy();
				box.destroy();
			},
		3);
	}


	public void popup_character(CharacterIndex[] aChars, int[] aDiffs)
	{
		int count = aChars.Length;

		FlatElementImage[] mCharacters = null;
		FlatElementImage[] mBadges = null;
		FlatElementText[] mNames = null;
		mCharacters = new FlatElementImage[count];
		mBadges = new FlatElementImage[count];
		mNames = new FlatElementText[count];

		//var sizeImg = ManagerManager.Manager.mCharacterBundleManager.get_image("ICON_05-1");
		//TODO read from sizeImg
		float gIconHeight = 343;
		//float gIconWidth = 266;

		Vector3 step = new Vector3(-500,0,0);
		Vector3 start = NIM.mFlatCamera.get_point(0,-1) - new Vector3(0,gIconHeight/2,0);
		Vector3 offset = (count-1)*(-step)/2f;
		Vector3 badgeOffset = new Vector3(-150,180,0);
		Vector3 nameOffset = new Vector3(0,-180,0);



		for(int i = 0; i < count; i++)
		{
			//TODO delete all the fallback stuff
			CharacterBundleManager.ImageSizePair img = null;
			img = ManagerManager.Manager.mCharacterBundleManager.get_image("ICON_"+aChars[i].StringIdentifier);
			if(img == null || img.Data == null || img.Image == null)
				img = ManagerManager.Manager.mCharacterBundleManager.get_image("ANGELS_"+aChars[i].StringIdentifier);
			if(img == null || img.Data == null || img.Image == null)
				mCharacters[i] = new FlatElementImage(null,10);
			else
				mCharacters[i] = new FlatElementImage(img.Image,img.Data.Size,10);

			mCharacters[i].HardPosition = start + offset + step*i;
			mCharacters[i].SoftPosition = mCharacters[i].SoftPosition + new Vector3(0,gIconHeight + 50,0);
		
			mBadges[i] = new FlatElementImage(ManagerManager.Manager.mNewRef.bbChoicePerfectIcons[aDiffs[i]],11);
			mBadges[i].HardColor = GameConstants.UiWhiteTransparent;

			mNames[i] = new FlatElementText(
				ManagerManager.Manager.mNewRef.genericFont,
				60,
				ManagerManager.Manager.mGameManager.CharacterHelper.Characters[aChars[i]].CharacterInfo.ShortName.ToUpper(),
				11);
			mNames[i].HardPosition = mCharacters[i].HardPosition + nameOffset;
			mNames[i].SoftPosition = mCharacters[i].SoftPosition + nameOffset;

			mElement.Add(mCharacters[i]);
			mElement.Add(mBadges[i]);
			mElement.Add(mNames[i]);
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

					//Shineeee
					//TODO good/bad
					create_shine_over_character(mCharacters[workingIndex],true,gBadgeTime);

					//play a sound
					//TODO play diff sound for good or bad
					ManagerManager.Manager.mMusicManager.play_sound_effect("headPopupGood");
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
					mNames[i].SoftPosition = mCharacters[i].SoftPosition + nameOffset;
					//destroy them eventually.
					TED.add_one_shot_event(
						delegate()
					    {
							mElement.Remove(mCharacters[index]);
							mElement.Remove(mBadges[index]);
							mElement.Remove(mNames[index]);
							mBadges[index].destroy();
							mCharacters[index].destroy();
							mNames[index].destroy();
						},
					3);
				}
			},
		gTimeAfterBadges);
	}
}
