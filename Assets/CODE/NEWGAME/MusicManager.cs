using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class MusicManager : FakeMonoBehaviour
{
	static float FADE_TIME = 5;
	static float QUICK_FADE_TIME = 0.2f; //for music to cutscene music
	static float CHOICE_FADE_TIME = 2;
	
	static float MAX_MUSIC_VOLUME = 0.8f;
	
	AudioSource mMusicSource;
	AudioSource mCutsceneSource; //TODO use me
	AudioSource mChoiceSource;
	AudioSource mFadingSource;
	
	public bool IsMusicSourcePlaying
	{ get { return mMusicSource.isPlaying; } }
	
	
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
		mCutsceneSource = mManager.gameObject.AddComponent<AudioSource>();
		mFadingSource = mManager.gameObject.AddComponent<AudioSource>();
		mChoiceSource = mManager.gameObject.AddComponent<AudioSource>();
		
		/*mSoundEffects["transitionIn"] = mManager.mNewRef.transitionIn;
		mSoundEffects["transitionOut"] = mManager.mNewRef.transitionOut;
		mSoundEffects["choiceBlip"] = mManager.mNewRef.choiceBlip;
		mSoundEffects["choiceMade"] = mManager.mNewRef.choiceMade;
		mSoundEffects["choiceMusic"] = mManager.mNewRef.choiceMusic;
		mSoundEffects["graveAngel"] = mManager.mNewRef.graveAngel;	*/
		
		foreach(var e in typeof(NewMenuReferenceBehaviour).GetFields())
		{
			Debug.Log (e.Name);
			Debug.Log (e.FieldType);
		}
		foreach(FieldInfo e in typeof(NewMenuReferenceBehaviour).GetFields().Where(e => e.FieldType == typeof(AudioClip)))
		{
			mSoundEffects[e.Name] = e.GetValue(mManager.mNewRef) as AudioClip;
		}
		
		//start screen music, probbaly shouldn't go here...
		mManager.mMusicManager.fade_in_extra_music("startMusic");
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
	
	public void play_sound_effect(string aSound, float aVolume = 1)
	{
		if(mSoundEffects.ContainsKey(aSound))
		{
			//TODO may need to create new sources to make sure they are not being used..
			mFadingSource.PlayOneShot(mSoundEffects[aSound],aVolume);
			
		}
		else
			throw new UnityException("sound " + aSound + " not found");
	}
	
	public void fade_out(float aFadeTime = -1)
	{
		if(aFadeTime == -1)
			aFadeTime = FADE_TIME;
		TED.add_event(
			delegate(float time)
			{
				float l = time/aFadeTime;
				mMusicSource.volume = (1-l)*MAX_MUSIC_VOLUME;
				return l > 1;
			}
		);
	}
	
	public void fade_in(float aFadeTime = -1)
	{
		if(aFadeTime == -1)
			aFadeTime = FADE_TIME;
		TED.add_event(
			delegate(float time)
			{
				float l = time/aFadeTime;
				mMusicSource.volume = (l)*MAX_MUSIC_VOLUME;
				return l > 1;
			}
		);
	}
	
	//TODO delete
	public void fade_in_cutscene_music(AudioClip aClip)
	{
		//fade out game music
		TED.add_event(
			delegate(float time)
			{
				float l = time/QUICK_FADE_TIME;
				mMusicSource.volume = (1-l)*MAX_MUSIC_VOLUME;
				return l > 1;
			}
		).then_one_shot( //switch to cutscene music
			delegate()
			{
				mMusicSource.clip = aClip;
				mMusicSource.volume = (0)*MAX_MUSIC_VOLUME;
				mMusicSource.loop = false;
				mMusicSource.Play();
			}
		).then( //fade it in
			delegate(float time)
			{
				float l = time/QUICK_FADE_TIME;
				mMusicSource.volume = (l)*MAX_MUSIC_VOLUME;
				return l > 1;
			}
		);
	}
	
	public void play_cutscene_music(AudioClip aClip)
	{
		Debug.Log ("playnig cutscene music " + aClip);
		
		mMusicSource.Stop();
		mMusicSource.clip = aClip;
		mMusicSource.loop = false;
		mMusicSource.volume = 1;
		mMusicSource.Play();
	}
	
	
	public void fade_in_extra_music(string aMusic)
	{
		mChoiceSource.clip = get_sound_clip("choiceMusic");
		mChoiceSource.volume = 0.01f;
		mChoiceSource.loop = true;
		mChoiceSource.Play();
		
		TED.add_event(
			delegate(float time)
			{
				float l = time/FADE_TIME;
				mChoiceSource.volume = (l)*MAX_MUSIC_VOLUME;
				return l > 1;
			}
		);
	}
	public void fade_out_extra_music()
	{
		TED.add_event(
			delegate(float time)
			{
				float l = time/FADE_TIME;
				mChoiceSource.volume = (1-l)*MAX_MUSIC_VOLUME;
				return l > 1;
			}
		).then_one_shot(
			delegate()
			{
				mChoiceSource.Stop();
			}
		);
	}
	
	//MusicManager assumes responsibility for fading in the music for the next character
	//but not fading out the music from the last character
	public void character_changed_listener(CharacterLoader aCharacter)
	{
		mMusicSource.clip = aCharacter.Images.backgroundMusic;
		mMusicSource.volume = 0;
		mMusicSource.loop = false;
		mMusicSource.Play();
		fade_in();
	}
	
	
}
