using UnityEngine;
using System.Collections.Generic;


//this class also handles initialization camera nonsense
public class TransitionCameraManager : FakeMonoBehaviour
{
	//static float FADE_TIME = 2.3f;
	static float FADE_TIME = 0.2f;
	static float MAX_FADE = 30;
	
	
	//render to this guy someday eventually ha ha...
	public RenderTexture AllRenderTexture { get; private set; }
	
	public TimedEventDistributor TED { get; private set; }
	
	
    public FlatCameraManager mFlatCamera;
	SunShafts mSunShafts;
	AlternativeDepthViewer mADV = null;
	
	FlatElementImage mDepthImage;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	//configuration nonsense
	FlatElementText mPWLogo;
	FlatElementText mPWCredits;
	FlatElementImage mGLLogo;
	FlatElementImage mFilmLogo;
	
	
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
		mFlatCamera.Camera.backgroundColor = new Color(0.05f,0.05f,0.07f);
		mFlatCamera.fit_camera_to_screen();
		mFlatCamera.Interpolator.SoftInterpolation = 1f;
		mFlatCamera.update(0);
		
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
	}

    
    public override void Update()
    {
		//w/e
		//TODO do something with this.
		if(mADV == null)
			mADV = mManager.mZigManager.DepthView;
		//mDepthImage.mImage.set_new_texture(mADV.DepthTexture,new Vector2(mFlatCamera.Width,mFlatCamera.Height));
		
		
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
		
		//TODO GL and FA logo
		mGLLogo = new FlatElementImage(refs.gameLabLogo,1);
		mFilmLogo = new FlatElementImage(refs.filmAkademieLogo,1);
		mGLLogo.HardPosition = mFlatCamera.get_point(0,-0.5f) + new Vector3(mGLLogo.BoundingBox.width/2 + 50,0,0);
		mFilmLogo.HardPosition = mFlatCamera.get_point(0,-0.5f) - new Vector3(mFilmLogo.BoundingBox.width/2 + 50,0,0);
		
		mElement.Add(mPWLogo);
		mElement.Add(mPWCredits);
		mElement.Add(mGLLogo);
		mElement.Add(mFilmLogo);
		
		
		/*
		PerformanceGraphObject mGraph = new PerformanceGraphObject(10);
		mGraph.SoftPosition = mFlatCamera.Center;
		for(int i = 0; i < 500; i++)
		{
			mGraph.update_graph(Random.Range(0f,1f),Random.Range(0f,1f));
		}
		mElement.Add(mGraph);*/
		
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
		
		TED.add_event(
			delegate(float aTime){
				if(true)
					go_to_fetus(0);
				else if(Input.GetKeyDown(KeyCode.Alpha0))
					go_to_fetus(0);
				else if(aTime > 5 && 
					mManager.mZigManager.has_user() && 
					mManager.mCharacterBundleManager.is_initial_loaded()
				)
					go_to_fetus(0); 
				else return false;
				return true;
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
