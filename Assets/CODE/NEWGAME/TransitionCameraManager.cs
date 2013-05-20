using UnityEngine;
using System.Collections.Generic;


//this class also handles initialization camera nonsense
public class TransitionCameraManager : FakeMonoBehaviour
{
	public static float FADE_TIME = 2.3f;
	//static float FADE_TIME = 0.2f;
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
    }
	
	public override void Start()
	{
		mFlatCamera = new FlatCameraManager(new Vector3(10000, 10000, 0), 10);
		mFlatCamera.Camera.depth = 101; //we want this on top always
		mFlatCamera.Camera.clearFlags = CameraClearFlags.SolidColor;
		mFlatCamera.Camera.backgroundColor = new Color(0.05f,0.05f,0.06f);
		mFlatCamera.fit_camera_to_screen(false);
		
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
		mDepthWarningText.HardPosition = mFlatCamera.get_point(1,-1) + new Vector3(-600,120,0);
		EnableDepthWarning = false;
		
		mElement.Add(mDepthImage);
		mElement.Add(mDepthWarningText);
	}
	
	public bool EnableDepthWarning{
		//TODO DepthImage as well eventually...
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
	
	
	
	public void start_configuration_display()
	{
		
		//fade in
		TED.add_event(fade_in,0);
		
		NewMenuReferenceBehaviour refs = mManager.mNewRef;
		mPWLogo = new FlatElementText(refs.genericFont,130,"P e r f e c t  W o m a n",1);
		mPWLogo.HardPosition = mFlatCamera.Center + new Vector3(0,250,0);
		//S c h ö n f e l d e r
		mPWCredits =  new FlatElementText(refs.genericFont,60,"A  G a m e  b y  P e t e r  L u  a n d  L e a  S c h o e n f e l d e r",1);
		mPWCredits.HardPosition = mFlatCamera.Center + new Vector3(0,0,0);
		
		mMessageText = new FlatElementText(refs.genericFont,60,"",1);
		mMessageText.HardPosition = mFlatCamera.Center + new Vector3(0,400,0);
		
		//TODO GL and FA logo
		mGLLogo = new FlatElementImage(refs.gameLabLogo,1);
		mFilmLogo = new FlatElementImage(refs.filmAkademieLogo,1);
		mGLLogo.HardPosition = mFlatCamera.get_point(0,-0.5f) + new Vector3(mGLLogo.BoundingBox.width/2 + 50,0,0);
		mFilmLogo.HardPosition = mFlatCamera.get_point(0,-0.5f) - new Vector3(mFilmLogo.BoundingBox.width/2 + 50,0,0);
		
		mElement.Add(mPWLogo);
		mElement.Add(mPWCredits);
		mElement.Add(mGLLogo);
		mElement.Add(mFilmLogo);
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
				if((aTime > 5 && mManager.mZigManager.has_user() && mManager.mCharacterBundleManager.is_initial_loaded()) ||
					Input.GetKeyDown(KeyCode.Alpha0) ||
					mForceStart){
					go_to_fetus(0); 
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
		//we assume things have faded already so we can just destroy
		mPWLogo.destroy();
		mPWCredits.destroy();
		mGLLogo.destroy();
		mFilmLogo.destroy();
		mFlatCamera.Camera.clearFlags = CameraClearFlags.Depth;
	}
	
	public bool go_to_fetus(float time)
	{
		//TODO if user is not roughly in the center of the screen, return false
		fade_out_with_sound(
			delegate()
			{
				mManager.mGameManager.initialize_fetus();
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
				float delay = FADE_TIME-mManager.mMusicManager.get_sound_clip("transitionOut").length;
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
		float l = (time/FADE_TIME);
		mSunShafts.sunShaftIntensity = (1-l)*MAX_FADE + l*0;
		return l>=1;
	}
	bool fade_out(float time)
	{
		float l = (time/FADE_TIME);
		mSunShafts.sunShaftIntensity = (1-l)*0 + l*MAX_FADE;
		return l>=1;
	}
}
