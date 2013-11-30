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
		
		for(int i = 0; i < count; i++)
		{
			//TODO
			var img = ManagerManager.Manager.mCharacterBundleManager.get_image(aChars[i].StringIdentifier);
			mCharacters[i] = new FlatElementImage(img.Image,img.Data.Size,10);
			mBadges[i] = new FlatElementImage(ManagerManager.Manager.mNewRef.bbChoicePerfectIcons[aDiffs[i]],11);

			
			//TODO create images
			//hide images
			//popup images
			//enable diff badges one at a time
			//mIcon.SoftColor = GameConstants.IconDifficultyColorsOverTwo[difficulty];
			
			//hide images
		}

		for(int i = 0; i < count; i++)
		{
			//TODO animate badges
			//need little pulsating scale animation for difficulty
			//mBadges[i].Events.add_event(
			//sound
			//maybe jiggle player a little
		}
	}
}
