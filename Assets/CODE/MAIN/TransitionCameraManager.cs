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

	//static float MAX_FADE = 30;
	

    public bool IsInitialized {get; private set;}
	public TimedEventDistributor TED { get; private set; }
	
	
    public FlatCameraManager mFlatCamera;
	
	
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	//configuration nonsense
	FlatElementText mPWLogo;
	FlatElementText mPWCredits;
	FlatElementImage mPWLogoImage;
	FlatElementImage mGLLogo;
	FlatElementImage mFilmLogo;
	FlatElementText mMessageText;

    //kinect required
    FlatElementImage mKinectRequiredImage;

    //restart menu stuff
    FlatElementImage mMenuBackground;
    FlatElementImage mMenuSelector;
    FlatElementText mResume;
    FlatElementText mRestart;

	
	//DepthWarning nonsense
	FlatElementText mDepthWarningText;
	FlatElementImage mDepthImage;
    FlatElementImage mColorImage; //for testing
	

    //camera stuff
    FlatElementImage mRTImage;
    FlatCameraManager mRTCamera;
	
    public TransitionCameraManager(ManagerManager aManager)
        : base(aManager) 
    {
		TED = new TimedEventDistributor();
    }
	
	public override void Start()
	{
        mManager.GameEventDistributor += game_event_listener;

        //setup flat camera
		mFlatCamera = new FlatCameraManager(new Vector3(10000, 10000, 0), 9);
		mFlatCamera.Camera.depth = 101; //we want this on top always
		//mFlatCamera.Camera.clearFlags = CameraClearFlags.SolidColor;
		mFlatCamera.Camera.clearFlags = CameraClearFlags.Depth;
		mFlatCamera.Camera.backgroundColor = new Color32(37,37,37,255);
		mFlatCamera.fit_camera_to_screen(true);
		

        //setup RT camera
        mRTCamera = new FlatCameraManager(new Vector3(10000, -6000, 0), 10);
        mRTCamera.set_render_texture_mode(true);
        mRTCamera.Camera.GetComponent<Camera>().name = "TCMCAMERA";
        mRTCamera.fit_camera_to_game();
        mRTImage = new FlatElementImage(mRTCamera.RT, 1);
        mRTImage.PrimaryGameObject.name = "TCMIMAGE";
        mRTImage.HardScale = Vector3.one * mFlatCamera.Width / mRTImage.mImage.PixelDimension.x;
        ModeNormalPlay.slide_image(mFlatCamera, null, mRTImage, false,true);
        mElement.Add(mRTImage);
        
        //this is seen by main flat camera and not RT camera FYI
        initialize_depth_warning();

		mManager.mAssetLoader.new_load_asset_bundle("START",delegate(AssetBundle aBundle){start_screen_loaded_callback(aBundle,"START");});
	}
	

	public void initialize_depth_warning()
	{
		mDepthImage = new FlatElementImage(null,new Vector2(160,120),100);
        mDepthImage.HardScale = Vector3.one * 2;
        //mDepthImage.HardPosition = mFlatCamera.get_point(1, -1) + new Vector3(-10 - mDepthImage.BoundingBox.width / 4, 10 + mDepthImage.BoundingBox.height / 4, 0) * mFlatCamera.screen_pixel_to_camera_pixel_ratio();
        mDepthImage.HardPosition = mFlatCamera.get_point(1.5f, -1) + new Vector3(-10 - mDepthImage.BoundingBox.width / 4, 10 + mDepthImage.BoundingBox.height / 4, 0) * mFlatCamera.screen_pixel_to_camera_pixel_ratio();
        mDepthImage.HardShader = mManager.mReferences.mXB1DepthImageShader;
        mDepthWarningText = new FlatElementText(mManager.mNewRef.genericFont,40,GameStrings.GetString("TCMkinect1"),100);
		mDepthWarningText.HardColor = new Color(1,1,1,0);	
		mDepthWarningText.Alignment = TextAlignment.Left;
		mDepthWarningText.Anchor = TextAnchor.MiddleLeft;
		mDepthWarningText.HardPosition = mFlatCamera.get_point(1,-1) + new Vector3(-280,110,0) * mFlatCamera.screen_pixel_to_camera_pixel_ratio();
		mDepthWarningText.ColorInterpolationMaxLimit = 10f;
		mDepthWarningText.ColorInterpolationMinLimit = 2f;
		//mDepthWarningText.Alignment = TextAlignment.Left;
		EnableDepthWarning = false;
		
		mElement.Add(mDepthImage);
		mElement.Add(mDepthWarningText);
	}
	
	public bool EnableDepthWarning{
		set{

            //disabling depth warning for XB1 version
            //TODO fix depth warning eventually...
            if(GameConstants.XB1)
                value = false;

   			if(value){
                //WHY DIVIDE BY 4 AND NOT 2??? I DONT KNOW
                mDepthImage.SoftPosition = mFlatCamera.get_point(1,-1) + new Vector3(-50 - mDepthImage.BoundingBox.width / 4, 50 + mDepthImage.BoundingBox.height / 4, 0) * mFlatCamera.screen_pixel_to_camera_pixel_ratio();
                mDepthWarningText.SoftColor = GameConstants.UiWhite;
			} else {
                mDepthImage.SoftPosition = mFlatCamera.get_point(1,-1) + new Vector3(400 - mDepthImage.BoundingBox.width / 4, 50 + mDepthImage.BoundingBox.height / 4, 0) * mFlatCamera.screen_pixel_to_camera_pixel_ratio();
                mDepthWarningText.SoftColor = GameConstants.UiWhiteTransparent;
			}
		}
	}
					
	
	
	
    
    public override void Update()
    {
		mFlatCamera.update(Time.deltaTime);
		foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);          
        TED.update(Time.deltaTime);

        ModeNormalPlay.draw_render_texture(mRTCamera);


        if (!GameConstants.XB1)
        {
            if (mManager.mZigManager.DepthView.DepthTexture != null)
                mDepthImage.set_new_texture(mManager.mZigManager.DepthView.DepthTexture, new Vector2(160, 120));
        }


        //TEST CODE FOR XB1 DEPTH/IMAGE DISPLAY CAN DELETE
            //mDepthImage.set_new_texture((mManager.mZigManager.ZgInterface as MicrosoftZig).mKinect.DepthTexture,new Vector2(160,120));
        //if(mManager.mZigManager.ImageView.imageTexture != null)
            //mColorImage.set_new_texture((mManager.mZigManager.ZgInterface as MicrosoftZig).mColorImageRT,new Vector2(300,300));

        /*Material mat2 = new Material(ManagerManager.Manager.mReferences.mXB1ClearShader);
        mat2.SetTexture("_MainTex",(mManager.mZigManager.ZgInterface as MicrosoftZig).mKinect.LabelTexture);
        mDepthImage.PrimaryGameObject.GetComponentInChildren<Renderer>().material = mat2;

        Material mat = new Material(ManagerManager.Manager.mReferences.mXB1KinectImageMaskingShader);
        mat.SetTexture("_MainTex",(mManager.mZigManager.ZgInterface as MicrosoftZig).mKinect.ColorTexture);
        mat.SetTexture("_AlphaTex",(mManager.mZigManager.ZgInterface as MicrosoftZig).mKinect.LabelTexture);
        mColorImage.PrimaryGameObject.GetComponentInChildren<Renderer>().material = mat;*/
	}

    public void show_menu()
    {

    }

    public void update_menu()
    {

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
            try
            {
                r = new FlatElementImage(mLoader.Images.staticElements[aName], sizing.Size, aDepth);
            }
            catch(System.Exception e)
            {
                Debug.Log("messed up on " + aName);
                throw e;
            }

		}
		else 
		{
			var backSize = mLoader.Sizes.mBackSize;
			sizing = new CharacterData.ImageSizeOffsetAnimationData();
			sizing.Offset = new Vector2(0,0);
			sizing.Size = backSize;
			r = new FlatElementImage(mLoader.Images.background1,backSize,aDepth);
		}
        r.HardPosition = mRTCamera.get_point(Vector3.zero) + sizing.Offset;
		return r;
	}


    public void set_start_screen_character_transparency()
    {

        foreach (var e in mElement)
        {
            if (e.PrimaryGameObject.name.StartsWith("genImageObject_BG-"))
            {
                int chara = System.Convert.ToInt32(e.PrimaryGameObject.name.Substring(18));
                if (mManager.mMetaManager.UnlockManager.is_unlocked(sCharOrderList[chara - 2]) != 1)
                {
                    e.HardShader = mManager.mReferences.mTransparentCharacaterShader;
                    e.HardColor = //new Color(0.75f,0,0,1);
                        //actually can't read this data yet because it's not loaded fast enough I guess...
                        //mManager.mCharacterBundleManager.get_character_stat(sCharOrderList[i-2]).CharacterInfo.CharacterOutlineColor;
                        //GameConstants.StartScreenLockedGray;
                        GameConstants.UiWhiteTransparent; //completely dissapear :O
                }
                else
                {
                    e.HardShader = mManager.mReferences.mDefaultCharacterShader;
                    e.HardColor = GameConstants.UiWhite;
                }
            }
        }
    }


	CharacterLoader mLoader = null;
	AssetBundle mBundle = null;
	public void start_screen_loaded_callback(AssetBundle aBundle, string aBundleName)
	{
		mBundle = aBundle;

		CharacterLoader loader = new CharacterLoader();
        loader.complete_load_character(aBundle,aBundleName);
		mLoader = loader;


        //create background
		mElement.Add(construct_flat_image("BACKGROUND",0));
		mElement.Add(construct_flat_image("FG-1",30));
		for(int i = 2; i < 30; i++)
		{
			var img = construct_flat_image("BG-"+i,30-i);
			mElement.Add(img);
		}
        set_start_screen_character_transparency();

        
        NewMenuReferenceBehaviour refs = mManager.mNewRef;

        //kinect required image
        if (!GameConstants.ALLOW_NO_KINECT && mManager.mZigManager.is_reader_connected() == 0)
        {
            //TODO put up a KINECT REQUIRED message
            mKinectRequiredImage = construct_flat_image("FG-1", 1000);
            mKinectRequiredImage.SoftPosition += new Vector3(0, -400, 0);
            mElement.Add(mKinectRequiredImage);
        }


        //mMenuBackground;
        //mMenuSelector;
        //mResume;
        //mRestart;



        //????
        mMessageText = new FlatElementText(refs.genericFont, 60, "", 1);
        mMessageText.HardPosition = mRTCamera.get_point(Vector3.zero) + new Vector3(0, 400, 0);
        mElement.Add(mMessageText);

        start_configuration_display();

        IsInitialized = true;

	}

    public void reload()
    {

        //note this will glitch if you try and reset inside of start screen
        ModeNormalPlay.slide_image(mFlatCamera, null, mRTImage, false);
        set_start_screen_character_transparency();
        start_configuration_display();
    }
	
	public void start_configuration_display()
    {
        EnableDepthWarning = false;
		
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
				if( can_start(aTime)){

					go_to_play(); 
					return true;
				}	
				if(dState == 1)
                    mMessageText.Text = GameStrings.GetString("TCMkinect2");
				else if(dState == 2)
                    mMessageText.Text = GameStrings.GetString("TCMkinect3");
				else
					mMessageText.Text = "";
				return false;
			}
		);
		
	}

    bool can_start(float aTime)
    {

        /*
        mManager.mDebugString = (GameConstants.ALLOW_NO_KINECT + " " + mManager.mZigManager.is_reader_connected() + " " +
                                 mManager.mCharacterBundleManager.is_initial_loaded() + " " +
                                 mManager.mZigManager.ZgInterface.can_start() + " " +
                                 (GameConstants.UNLOCK_ALL || mManager.mMetaManager.SaveDataRead) + " " +
                                 aTime + " " +
                                 (mManager.mZigManager.has_user()) + " " +
                                 (KeyMan.GetKey("HardSkip") || KeyMan.GetKey("SoftSkip"))  + " " +
                                 GameConstants.FORCE_START);
        */

        if (!GameConstants.ALLOW_NO_KINECT && mManager.mZigManager.is_reader_connected() == 0)
            return false;

        return 
            mManager.mCharacterBundleManager.is_initial_loaded() &&
            mManager.mZigManager.ZgInterface.can_start() &&
            (GameConstants.UNLOCK_ALL || mManager.mMetaManager.SaveDataRead) && //we don't care about save data if we are in unlock all mode
            ((aTime > 3.5f && mManager.mZigManager.has_user()) ||
            KeyMan.GetKey("HardSkip") || KeyMan.GetKey("SoftSkip") ||
            GameConstants.FORCE_START);
    }
	

	public bool go_to_play()
	{
		mManager.mMusicManager.fade_out_extra_music();
		
		//cheap hack 
        var choices = mManager.mMetaManager.UnlockManager.get_unlocked_characters_at_level(1);
		CharacterIndex charIndex = Input.GetKeyDown(KeyCode.Alpha9) ? choices[Random.Range(0,choices.Count)] : CharacterIndex.sFetus;
		if(Input.GetKeyDown(KeyCode.Alpha9) )
		{
			GameConstants.numberRetries = 0;
			GameConstants.transitionToChoiceDelayTime = 0;
			GameConstants.preplayTime = 0;
		}

        mManager.mGameManager.start_game(charIndex);

		return true;
	}

    public void you_are_playing_as(string aName)
    {
        aName = System.Text.RegularExpressions.Regex.Replace(aName, @"[^\u0020-\u007E]", "[]");
        NewMenuReferenceBehaviour refs = mManager.mNewRef;
        var title = construct_flat_image("START_PLAYER", 100);
        var text = new FlatElementText(refs.genericFont, 50, aName, 101);
        title.HardPosition = mFlatCamera.get_point(.73f, -.80f);
        text.HardPosition = title.HardPosition - new Vector3(0,44,0);
        title.HardColor = GameConstants.UiWhiteTransparent;
        text.HardColor = GameConstants.UiBlueTransparent;
        title.SoftColor = GameConstants.UiWhite;
        text.SoftColor = GameConstants.UiBlue;
        mElement.Add(title);
        mElement.Add(text);
        TED.add_one_shot_event(
            delegate()
            {
                title.SoftColor = text.SoftColor = GameConstants.UiWhiteTransparent;
            },
        5).then_one_shot(
            delegate()
            {
                title.destroy();
                text.destroy();
                mElement.Remove(title);
                mElement.Remove(text);
            },
        5);
    }


    void game_event_listener(string name, object[] args)
    {
        if (name == "NEW CHARACTER")
        {
            //if ((CharacterIndex)args[0] == CharacterIndex.sFetus)
            {
                ModeNormalPlay.slide_image(mFlatCamera, mRTImage, null, false);
            }
        }
    }
}

