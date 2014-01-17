using UnityEngine;
using System.Collections.Generic;


//this class also handles initialization camera nonsense
public class TransitionCameraManager : FakeMonoBehaviour
{
	static List<CharacterIndex> sCharOrderList = new List<CharacterIndex>()
	{
		new CharacterIndex("60-4"),
		new CharacterIndex("60-3"),
		new CharacterIndex("27-4"),
		new CharacterIndex("85-1"),
		new CharacterIndex("60-1"),
		new CharacterIndex("85-4"),
		new CharacterIndex("05-1"),
		new CharacterIndex("05-2"),
		new CharacterIndex("34-4"),
		new CharacterIndex("05-3"),
		new CharacterIndex("05-4"),
		new CharacterIndex("16-2"),
		new CharacterIndex("45-3"),
		new CharacterIndex("16-4"),
		new CharacterIndex("34-1"),
		new CharacterIndex("27-1"),
		new CharacterIndex("27-3"),
		new CharacterIndex("34-2"),
		new CharacterIndex("34-3"),
		new CharacterIndex("45-1"),
		new CharacterIndex("45-2"),
		new CharacterIndex("45-4"),
		new CharacterIndex("60-2"),
		new CharacterIndex("85-2"),
		new CharacterIndex("85-3"),
		new CharacterIndex("27-2"),
		new CharacterIndex("16-1"),
		new CharacterIndex("16-3")
	};

	static float MAX_FADE = 30;
	
	
	//render to this guy someday eventually ha ha...
	public RenderTexture AllRenderTexture { get; private set; }
	
	public TimedEventDistributor TED { get; private set; }
	
	
	public bool mForceStart = false;
    public FlatCameraManager mFlatCamera;
	SunShafts mSunShafts;
	
	
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	//configuration nonsense
	FlatElementText mPWLogo;
	FlatElementText mPWCredits;
	FlatElementImage mPWLogoImage;
	FlatElementImage mGLLogo;
	FlatElementImage mFilmLogo;
	FlatElementText mMessageText;
	
	//DepthWarning nonsense
	FlatElementText mDepthWarningText;
	FlatElementImage mDepthImage;
	
	
    public TransitionCameraManager(ManagerManager aManager)
        : base(aManager) 
    {
		AllRenderTexture = new RenderTexture(Screen.width,Screen.height,16); 
		TED = new TimedEventDistributor();
		mManager.mAssetLoader.new_load_asset_bundle("START",delegate(AssetBundle aBundle){start_screen_loaded_callback(aBundle,"START");});
    }
	
	public override void Start()
	{
		mFlatCamera = new FlatCameraManager(new Vector3(10000, 10000, 0), 10);
		mFlatCamera.Camera.depth = 101; //we want this on top always
		//mFlatCamera.Camera.clearFlags = CameraClearFlags.SolidColor;
		mFlatCamera.Camera.clearFlags = CameraClearFlags.Depth;
		mFlatCamera.Camera.backgroundColor = new Color32(37,37,37,255);
		mFlatCamera.fit_camera_to_screen(true);
		
        SunShafts shafts = ((GameObject)GameObject.Instantiate(mManager.mReferences.mImageEffectsPrefabs)).GetComponent<SunShafts>();
		
		mSunShafts = mFlatCamera.Camera.gameObject.AddComponent<SunShafts>();
		mSunShafts.maxRadius = shafts.maxRadius;
		mSunShafts.radialBlurIterations = shafts.radialBlurIterations;
		mSunShafts.resolution = shafts.resolution;
		mSunShafts.screenBlendMode = shafts.screenBlendMode;
		mSunShafts.simpleClearShader = shafts.simpleClearShader;
		mSunShafts.sunColor = shafts.sunColor;
		mSunShafts.sunShaftBlurRadius = shafts.sunShaftBlurRadius;
		mSunShafts.sunShaftIntensity = shafts.sunShaftIntensity;
		mSunShafts.sunShaftsShader = shafts.sunShaftsShader;
		mSunShafts.useDepthTexture = shafts.useDepthTexture;
		mSunShafts.useSkyBoxAlpha = shafts.useSkyBoxAlpha;
		//???mSunShafts.sunTransform = shafts.sunTransform;
		
		
		
		//mDepthImage = new FlatElementImage(null,0); 
		//mElement.Add(mDepthImage);
		
		start_configuration_display();
		initialize_depth_warning();
		
		
	}
	
	
	public void initialize_depth_warning()
	{
		mDepthImage = new FlatElementImage(null,100);
		mDepthWarningText = new FlatElementText(mManager.mNewRef.genericFont,40,"Make sure you are\n in frame and no body\n parts are covered",100);
		mDepthWarningText.HardColor = new Color(1,1,1,0);	
		mDepthWarningText.HardPosition = mFlatCamera.get_point(1,-1) + new Vector3(-710,200,0);
		//mDepthWarningText.Alignment = TextAlignment.Left;
		EnableDepthWarning = false;
		
		mElement.Add(mDepthImage);
		mElement.Add(mDepthWarningText);
	}
	
	public bool EnableDepthWarning{
		set{
			if(value){
				mDepthWarningText.SoftColor = new Color(1,1,1,1);
			} else {
				mDepthWarningText.SoftColor = new Color(1,1,1,0);
			}
		}
	}
					
	
	
	
    
    public override void Update()
    {
		
		mFlatCamera.update(Time.deltaTime);
		foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);            
		
		
		
        TED.update(Time.deltaTime);
		
		
        
	}



	FlatElementImage construct_flat_image(string aName, int aDepth)
	{
		if(mLoader == null)
			throw new UnityException("start screen bundle null");


		CharacterData.ImageSizeOffsetAnimationData sizing;
		FlatElementImage r;
		if(aName != "BACKGROUND") //this is stupid and you should make background part of the dump..
		{
			sizing = mLoader.Sizes.find_static_element(aName);
			r = new FlatElementImage(mLoader.Images.staticElements[aName],sizing.Size,aDepth);
		}
		else 
		{
			var backSize = mLoader.Sizes.mBackSize;
			sizing = new CharacterData.ImageSizeOffsetAnimationData();
			sizing.Offset = new Vector2(0,0);
			sizing.Size = backSize;
			r = new FlatElementImage(mLoader.Images.background1,backSize,aDepth);
		}
		r.HardPosition = mFlatCamera.Center + sizing.Offset;
		return r;
	}



	CharacterLoader mLoader = null;
	AssetBundle mBundle = null;
	public void start_screen_loaded_callback(AssetBundle aBundle, string aBundleName)
	{
		mBundle = aBundle;

		
		//TED.add_event(fade_in,0);
		
		CharacterLoader loader = new CharacterLoader();
        loader.complete_load_character(aBundle,aBundleName);
		mLoader = loader;

		//mManager.mBackgroundManager.character_changed_listener(loader);
		//mManager.mMusicManager.character_changed_listener(loader);



		mElement.Add(construct_flat_image("BACKGROUND",0));
		mElement.Add(construct_flat_image("FG-1",30));

		for(int i = 2; i < 30; i++)
		{
			var img = construct_flat_image("BG-"+i,30-i);
			if(mManager.mMetaManager.UnlockManager.is_unlocked(sCharOrderList[i-2]) != 1)
			{
				img.HardShader = mManager.mReferences.mTransparentCharacaterShader;
				img.HardColor = //new Color(0.75f,0,0,1);
					//actually can't read this data yet because it's not loaded fast enough I guess...
					mManager.mCharacterBundleManager.get_character_stat(sCharOrderList[i-2]).CharacterInfo.CharacterOutlineColor;

			}

			mElement.Add(img);
			
		}


		
		//TODO draw silhouette of locked characters
		//TODO enable unlocked characters in background manager

	}
	
	public void start_configuration_display()
	{
		
		
		//fade in
		TED.add_event(fade_in,0);
		
		NewMenuReferenceBehaviour refs = mManager.mNewRef;
		//mPWLogo = new FlatElementText(refs.genericFont,130,"P e r f e c t  W o m a n",1);
		//mPWLogo.HardPosition = mFlatCamera.Center + new Vector3(0,250,0);
		//mPWCredits =  new FlatElementText(refs.genericFont,60,"A  G a m e  b y  P e t e r  L u  a n d  L e a  S c h \u00F6 e n f e l d e r",1);
		//mPWCredits.HardPosition = mFlatCamera.Center + new Vector3(0,0,0);
		//mElement.Add(mPWLogo);
		//mElement.Add(mPWCredits);
		
		//logos
		mPWLogoImage = new FlatElementImage(refs.perfectWomanLogo,1);
		mGLLogo = new FlatElementImage(refs.gameLabLogo,1);
		mFilmLogo = new FlatElementImage(refs.filmAkademieLogo,1);
		mPWLogoImage.HardPosition = mFlatCamera.Center + new Vector3(0,200,0);
		mGLLogo.HardPosition = mFlatCamera.get_point(0,-0.5f) + new Vector3(mGLLogo.BoundingBox.width/2 + 50,0,0);
		mFilmLogo.HardPosition = mFlatCamera.get_point(0,-0.5f) - new Vector3(mFilmLogo.BoundingBox.width/2 + 50,0,0);

		/*mPWLogo.Enabled = false;
		mGLLogo.Enabled = false;
		mFilmLogo.Enabled = false;*/

		
		
		mMessageText = new FlatElementText(refs.genericFont,60,"",1);
		mMessageText.HardPosition = mFlatCamera.Center + new Vector3(0,400,0);

		//TODO delete all this stuffeouou
		/*mElement.Add(mPWLogoImage);
		mElement.Add(mGLLogo);
		mElement.Add(mFilmLogo);*/

		mElement.Add(mMessageText);
		
	
		
		
		
		//display logo
		//if no kinect is found
			//display no kinect found nonsesnse
			//mManager.mZigManager.is_reader_connected()
		//if kinect is found and five seconds elapsed, fade in depth image
		//if user is not found prompt stand in front of the kinect so the knicet sees all of you
		//if 3 seconds elapsed and user is found, 1 sec GOOD, center in camera
		//if 3 second elapsed and user is near center, 1 sec GOOD, make a t pose
		//if 3 seconds elapesed and user is in tpose, 1 sec GOOD, begin fadeout
			//on fadeoutcb, move depth image to lower left corner	
		//mManager.mZigManager.DepthView.set_full(true);
		
		
		int dState = 0; //0-started, 1-kinect not found
		TED.add_event(
			delegate(float aTime){
			
				/*
				if(!mManager.mZigManager.is_reader_connected())
					dState = 1;
				else if(mManager.mZigManager.has_user() && false) //TODO test if user is in frame
					dState = 4;
				else if(mManager.mZigManager.has_user()) //TODO test if user is found
					dState = 3;
				else
					dState = 2;
					*/
				if( mManager.mCharacterBundleManager.is_initial_loaded() &&
			   		((aTime > 5 && mManager.mZigManager.has_user()) ||
					Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Alpha9) ||
					mForceStart)){
					go_to_play(); 
					return true;
				}	
				if(dState == 1)
					mMessageText.Text = "Kinect not found";
				else if(dState == 2)
					mMessageText.Text = "Center yourself in the screen";
				else
					mMessageText.Text = "";
				return false;
			}
		);
		
	}
	
	public void destroy_configuration_display()
	{

		foreach(var e in mElement)
			e.destroy();
		//we assume things have faded already so we can just destroy
		//mPWLogo.destroy();
		//mPWCredits.destroy();
		mPWLogoImage.destroy();
		mGLLogo.destroy();
		mFilmLogo.destroy();
		mFlatCamera.Camera.clearFlags = CameraClearFlags.Depth;

		//because it got destoryed at the step above
		initialize_depth_warning();

		mBundle.Unload(true);
		mBundle = null;
	}
	
	public bool go_to_play()
	{
		mManager.mMusicManager.fade_out_extra_music();
		
		//cheap hack 
		CharacterIndex charIndex = Input.GetKeyDown(KeyCode.Alpha9) ? new CharacterIndex(1,Random.Range(0,4)) : CharacterIndex.sFetus;
		if(Input.GetKeyDown(KeyCode.Alpha9) )
		{
			GameConstants.fadingTime = 0.1f;
			GameConstants.numberRetries = 0;
		}
		
		//TODO if user is not roughly in the center of the screen, return false
		fade_out_with_sound(
			delegate()
			{
				mManager.mGameManager.start_game(charIndex);
				destroy_configuration_display();
			}
		);
		return true;
	}
	public void fade_in_with_sound()
	{
		TED.add_one_shot_event(
			delegate(){ 
				mManager.mMusicManager.play_sound_effect("transitionIn"); 
			}
		).then(
			fade_in,
		0);
	}
	public void fade_out_with_sound(System.Action aFadeCompleteCb)
	{
			
		TED.add_one_shot_event(
			delegate(){ 
				float delay = GameConstants.fadingTime-mManager.mMusicManager.get_sound_clip("transitionOut").length;
				if(delay < 0) delay = 0; 
				TED.add_one_shot_event( //too bad we don't have awesome branching event chain
					delegate(){ mManager.mMusicManager.play_sound_effect("transitionOut"); },
				delay);
			}
		).then(
			fade_out,
        0).then_one_shot(
			delegate(){ aFadeCompleteCb(); }
		);
	}
	
    bool fade_in(float time)
	{
		float l = (time/GameConstants.fadingTime);
		mSunShafts.sunShaftIntensity = (1-l)*MAX_FADE + l*0;
		return l>=1;
	}
	bool fade_out(float time)
	{
		float l = (time/GameConstants.fadingTime);
		mSunShafts.sunShaftIntensity = (1-l)*0 + l*MAX_FADE;
		return l>=1;
	}
}
