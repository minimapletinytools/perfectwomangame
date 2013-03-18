using UnityEngine;
using System.Collections.Generic;

public class MusicManager : FakeMonoBehaviour
{
	static float FADE_TIME = 5;
	AudioSource mMusicSource;
	AudioSource mFadingSource;
	
	
	Dictionary<string,AudioClip> mSoundEffects  = new Dictionary<string, AudioClip>();
	
	public TimedEventDistributor TED { get; private set; }
    public MusicManager(ManagerManager aManager)
        : base(aManager) 
    {
		TED = new TimedEventDistributor();
    }
	
	public override void Start()
	{
		mMusicSource = mManager.gameObject.AddComponent<AudioSource>();
		mFadingSource = mManager.gameObject.AddComponent<AudioSource>();
		
		
		mSoundEffects["transitionIn"] = mManager.mNewRef.transitionIn;
		mSoundEffects["transitionOut"] = mManager.mNewRef.transitionOut;
		mSoundEffects["choiceBlip"] = mManager.mNewRef.choiceBlip;
		mSoundEffects["choiceMade"] = mManager.mNewRef.choiceMade;
	}
	
	public override void Update()
	{
		TED.update(Time.deltaTime);
	}
	
	public AudioClip get_sound_clip(string aSound)
	{
		if(mSoundEffects.ContainsKey(aSound))
			return mSoundEffects[aSound];
		return null;
	}
	
	public void play_sound_effect(string aSound)
	{
		if(mSoundEffects.ContainsKey(aSound))
		{
			mMusicSource.PlayOneShot(mSoundEffects[aSound]);
		}
		else
			throw new UnityException("sound " + aSound + " not found");
	}
	
	public void fade_out()
	{
		TED.add_event(
			delegate(float time)
			{
				float l = time/FADE_TIME;
				mMusicSource.volume = 1-l;
				return l > 1;
			}
		);
	}
	
	public void fade_in()
	{
		TED.add_event(
			delegate(float time)
			{
				float l = time/FADE_TIME;
				mMusicSource.volume = l;
				return l > 1;
			}
		);
	}
	
	
	
	//MusicManager assumes responsibility for fading in the music for the next character
	//but not fading out the music from the last character
	public void character_changed_listener(CharacterLoader aCharacter)
	{
		mMusicSource.clip = aCharacter.Images.backgroundMusic;
		fade_in();
	}
	
	
}
