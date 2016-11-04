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
		Vector3 targetPos = aHead.SoftPosition + new Vector3(0,shine.BoundingBox.height/2-360,0) + new Vector3(0,500,0);
		Vector3 storePosition = targetPos + new Vector3((NIM.mFlatCamera.get_point(Vector3.zero).x-targetPos.x)*0.2f,shine.BoundingBox.height - 100,0);
		shine.HardPosition = storePosition;
		shine.SoftPosition = targetPos;
		shine.HardColor = positive ? GameConstants.UiYellowTransparent : GameConstants.UiRedTransparent; 
		shine.HardColor = positive ? GameConstants.UiYellow : GameConstants.UiRed; 
		shine.PositionInterpolationMinLimit = 250;
		shine.SoftInterpolation = 0.15f;
		mElement.Add(shine); 


	
		/*
		var boxImage = ManagerManager.Manager.mCharacterBundleManager.get_image("CUTSCENE_BOX");
		FlatElementImage box = new FlatElementImage(boxImage.Image,boxImage.Data.Size,3);
		box.HardPosition = aHead.SoftPosition;
		box.HardColor = positive ? GameConstants.UiGreenTransparent : GameConstants.UiRedTransparent; 
		box.HardColor = positive ? GameConstants.UiGreen : GameConstants.UiRed; 
		mElement.Add(box);*/
	
		TED.add_one_shot_event(
			delegate() {
				shine.SoftPosition = storePosition;
				//shine.HardColor = GameConstants.UiWhiteTransparent;
				//box.HardColor = GameConstants.UiWhiteTransparent;
			},
		duration).then_one_shot(
			delegate() {
				mElement.Remove(shine);
				//mElement.Remove(box);
				shine.destroy();
				//box.destroy();
			},
		0.7f);
	}


	public void popup_character(CharacterIndex[] aChars, int[] aDiffs, int[] aOldDiffs, bool isGreen)
	{
		
		int count = aChars.Length;
		float gTimeBeforeBadges = .7f;
		float scaleTime = Mathf.Sqrt (Mathf.Sqrt(1/(float)count));
		float gBadgeTime = scaleTime * .3f;
		float gTimeAfterBadges = 1.0f;



		FlatElementImage[] mCharacters = null;
		FlatElementImage[] mBadges = null;
		FlatElementImage[] mBackgrounds = null;
		FlatElementText[] mNames = null;
		mCharacters = new FlatElementImage[count];
		mBadges = new FlatElementImage[count];
		mBackgrounds = new FlatElementImage[count];
		mNames = new FlatElementText[count];

		//var sizeImg = ManagerManager.Manager.mCharacterBundleManager.get_image("ICON_05-1");
		//TODO read from sizeImg
		float gIconHeight = 343;
		//float gIconWidth = 266;

		Vector3 step = new Vector3(-500,0,0);
		Vector3 start = NIM.mFlatCamera.get_point(0,-1) - new Vector3(0,gIconHeight/2,0);
		Vector3 offset = (count-1)*(-step)/2f;
		Vector3 badgeOffset = new Vector3(-150,165,0);
		Vector3 nameOffset = new Vector3(0,-180,0);

		CharacterBundleManager.ImageSizePair[] badgeImages = new CharacterBundleManager.ImageSizePair[]{
			ManagerManager.Manager.mCharacterBundleManager.get_image("label_easy"),
			ManagerManager.Manager.mCharacterBundleManager.get_image("label_normal"),
			ManagerManager.Manager.mCharacterBundleManager.get_image("label_hard"),
			ManagerManager.Manager.mCharacterBundleManager.get_image("label_extreme")
		};


		
		var chain = TED.add_event(
			delegate(float aTime) {
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
					mCharacters[i].SoftPosition = mCharacters[i].SoftPosition + new Vector3(0,gIconHeight + 43,0);
					mCharacters[i].HardScale = Vector3.one*0.8f;
				
					mBadges[i] = new FlatElementImage(badgeImages[aOldDiffs[i]].Image,badgeImages[aOldDiffs[i]].Data.Size,11);
					//mBadges[i].HardColor = GameConstants.UiWhiteTransparent;
					mBadges[i].HardColor = GameConstants.UiWhite;
					mBadges[i].HardPosition = mCharacters[i].HardPosition + badgeOffset;
					mBadges[i].SoftPosition = mCharacters[i].SoftPosition + badgeOffset;
					mBadges[i].HardScale = Vector3.one*0.9f;

					var boxImage = ManagerManager.Manager.mCharacterBundleManager.get_image("CUTSCENE_BOX");
					FlatElementImage box = new FlatElementImage(boxImage.Image,boxImage.Data.Size,3);
					box.HardPosition = mCharacters[i].HardPosition;
					box.SoftPosition = mCharacters[i].SoftPosition;
					box.HardColor = isGreen ? GameConstants.UiYellowTransparent : GameConstants.UiRedTransparent; 
					box.HardColor = isGreen ? GameConstants.UiYellow : GameConstants.UiRed; 
					mBackgrounds[i] = box;

					var text = ManagerManager.Manager.mGameManager.CharacterHelper.Characters[aChars[i]].CharacterInfo.ShortName.ToUpper();
					mNames[i] = new FlatElementText(
						ManagerManager.Manager.mNewRef.genericFont,
						text.Length > 10 ? 42 : 57,
						text,
						11);
					mNames[i].HardPosition = mCharacters[i].HardPosition + nameOffset;
					mNames[i].SoftPosition = mCharacters[i].SoftPosition + nameOffset;
					mNames[i].HardColor = (!isGreen) ? GameConstants.UiWhite : GameConstants.UiRed; 

					//set speeds..
					mCharacters[i].PositionInterpolationMinLimit = 250;
					mCharacters[i].SoftInterpolation = 0.15f;
					mBadges[i].PositionInterpolationMinLimit = 250;
					mBadges[i].SoftInterpolation = 0.15f;
					mNames[i].PositionInterpolationMinLimit = 250;
					mNames[i].SoftInterpolation = 0.15f;
					mBackgrounds[i].PositionInterpolationMinLimit = 250;
					mBackgrounds[i].SoftInterpolation = 0.15f;

					mElement.Add(mCharacters[i]);
					mElement.Add(mBadges[i]);
					mElement.Add(mBackgrounds[i]);
					mElement.Add(mNames[i]);

					create_shine_over_character(mCharacters[i],isGreen,gBadgeTime*(count)+gTimeAfterBadges+0.5f);
				}
				

				return true;
			},
		gTimeBeforeBadges);


		chain = chain.wait(0.5f);

		for(int j = 0; j < count; j++)
		{
			int workingIndex = j;
			chain = chain.then_one_shot(
				delegate() {

					//TODO DELETE
					//mBadges[workingIndex].destroy();
					//Vector3 pos = mBadges[workingIndex].HardPosition;
					//mBadges[workingIndex] = new FlatElementImage(badgeImages[aDiffs[workingIndex]].Image,badgeImages[aDiffs[workingIndex]].Data.Size,11);
					//mBadges[workingIndex].HardPosition = pos;
					//mElement.Add(mBadges[workingIndex]);
					mBadges[workingIndex].set_new_texture(badgeImages[aDiffs[workingIndex]].Image,badgeImages[aDiffs[workingIndex]].Data.Size);

					//pulsating scale animation
					mBadges[workingIndex].Events.add_event(
						delegate(FlatElementBase aBase, float aTime) 
						{
							aBase.mLocalScale = Vector3.one * (1+Mathf.Sin(aTime*12)*0.3f);
							if(aTime > 0.225f) 
								return true;
							return false;
						},
					0);

					/*
					mCharacters[workingIndex].Events.add_event(
						delegate(FlatElementBase aBase, float aTime) 
						{
							aBase.mLocalScale = Vector3.one * (1+Mathf.Sin(aTime*6)*0.1f);
							if(aTime > 0.3f) 
								return true;
							return false;
						},
					0);*/

					//play a sound
					if(isGreen)
						ManagerManager.Manager.mMusicManager.play_sound_effect("headPopupGood");
					else
						ManagerManager.Manager.mMusicManager.play_sound_effect("headPopupBad");
				},
			gBadgeTime);
		}


		chain = chain.then_one_shot(
			delegate() {
				for(int i = 0; i < count; i++)
				{
					int index = i;
					//mBadges[i].SoftColor = GameConstants.UiWhiteTransparent;
					mCharacters[i].SoftPosition = start + offset + step*i + new Vector3(0,-200,0); //move down a little more to compensate for scale change
					mNames[i].SoftPosition = mCharacters[i].SoftPosition + nameOffset;
					mBackgrounds[i].SoftPosition = mCharacters[i].SoftPosition;
					mBadges[i].SoftPosition = mCharacters[i].SoftPosition + badgeOffset;
					mBadges[i].SoftColor = GameConstants.UiWhiteTransparent;

					//destroy them eventually.
					TED.add_one_shot_event(
						delegate()
					    {
							mElement.Remove(mCharacters[index]);
							mElement.Remove(mBadges[index]);
							mElement.Remove(mBackgrounds[index]);
							mElement.Remove(mNames[index]);
							mBadges[index].destroy();
							mCharacters[index].destroy();
							mBackgrounds[index].destroy();
							mNames[index].destroy();
						},
					3);
				}
			},
		gTimeAfterBadges);
	}
}
