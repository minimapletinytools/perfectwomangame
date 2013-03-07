using UnityEngine;
using System.Collections.Generic;


//this class also handles initialization camera nonsense
public class TransitionCameraManager : FakeMonoBehaviour
{
	
	//render to this guy someday eventually ha ha...
	public RenderTexture AllRenderTexture { get; private set; }
	
	public TimedEventDistributor TED { get; private set; }
	
	
    public FlatCameraManager mFlatCamera;
	SunShafts mSunShafts;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	
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

	}

    
    public override void Update()
    {
        TED.update(Time.deltaTime);

        
	}
	
	public void fade(System.Action aFadeCompleteCb)
	{
		float fadeTime = 3;
		float maxFade = 40;
		TimedEventDistributor.TimedEventChain chain = TED.add_event(
			delegate(float time)
            {
				float l = (time/fadeTime);
				mSunShafts.sunShaftIntensity = (1-l)*0 + l*maxFade;
				return l>=1;
            },
        0).then_one_shot(
			delegate()
			{
				aFadeCompleteCb();
			}
		).then(
			delegate(float time)
			{
				float l = (time/fadeTime);
				mSunShafts.sunShaftIntensity = (1-l)*maxFade + l*0;
				return l>=1;
			},
		0);
	}
	
    
}
