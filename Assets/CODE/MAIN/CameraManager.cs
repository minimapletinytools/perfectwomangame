using UnityEngine;
using System.Collections;

public class CameraManager : FakeMonoBehaviour {
    public CameraManager(ManagerManager aManager) : base(aManager) { }

    public Camera MainBodyCamera { get; private set; } //layer 1
    public BloomAndLensFlares MainBodyCameraBloomEffect { get; private set; }
    public Camera TransparentBodyCamera { get; private set; } //layer 2
    public EdgeDetectEffect TransparentBodyCameraEdgeEffect { get; private set; }
    public Camera BackgroundCamera { get; private set; } //layer 3
    public Camera ForegroundCamera { get; private set; } //layer 4

    GameObject ImageEffects { get; set; }

    public Camera[] AllCameras { get { return new Camera[] { MainBodyCamera,TransparentBodyCamera,BackgroundCamera,ForegroundCamera};} }
	public override void Start () 
    {
		Camera wipeCamera = (new GameObject("genWipeCamera")).AddComponent<Camera>();
		wipeCamera.clearFlags = CameraClearFlags.SolidColor;
		wipeCamera.backgroundColor = new Color(0.05f,0.05f,0.06f);
		//wipeCamera.backgroundColor = new Color(1,1,1);
		wipeCamera.depth = -99;
		
        ImageEffects = (GameObject)GameObject.Instantiate(mManager.mReferences.mImageEffectsPrefabs);

        MainBodyCamera = (new GameObject("genMainCamera")).AddComponent<Camera>();
        MainBodyCamera.cullingMask = 1 << 1;
        MainBodyCamera.depth = 4;
        MainBodyCamera.clearFlags = CameraClearFlags.Depth;
        mManager.mBodyManager.set_layer(1);

        
        TransparentBodyCamera = (new GameObject("genTransparentCamera")).AddComponent<Camera>();
        TransparentBodyCamera.cullingMask = 1 << 2;
        TransparentBodyCamera.depth = 3;
        TransparentBodyCamera.clearFlags = CameraClearFlags.Depth;
        TransparentBodyCameraEdgeEffect = TransparentBodyCamera.gameObject.AddComponent<EdgeDetectEffect>();
        mManager.mTransparentBodyManager.set_layer(2);

        BackgroundCamera = (new GameObject("genBackgroundCamera")).AddComponent<Camera>();
        BackgroundCamera.cullingMask = 1 << 3;
        BackgroundCamera.depth = 2;
        BackgroundCamera.clearFlags = CameraClearFlags.Depth;
        mManager.mBackgroundManager.set_background_layer(3);

        ForegroundCamera = (new GameObject("genForegroundCamera")).AddComponent<Camera>();
        ForegroundCamera.cullingMask = 1 << 4;
        ForegroundCamera.depth = 5;
        ForegroundCamera.clearFlags = CameraClearFlags.Depth;
        mManager.mBackgroundManager.set_foreground_layer(4);
		
		
		
		
        MainBodyCameraBloomEffect = MainBodyCamera.gameObject.AddComponent<BloomAndLensFlares>();
		//MainBodyCameraBloomEffect = BackgroundCamera.gameObject.AddComponent<BloomAndLensFlares>();
        BloomAndLensFlares templateBloom = ImageEffects.GetComponent<BloomAndLensFlares>();
        MainBodyCameraBloomEffect.addBrightStuffOneOneShader = templateBloom.addBrightStuffOneOneShader;
        MainBodyCameraBloomEffect.brightPassFilterShader = templateBloom.brightPassFilterShader;
        MainBodyCameraBloomEffect.hollywoodFlaresShader = templateBloom.hollywoodFlaresShader;
        MainBodyCameraBloomEffect.lensFlareShader = templateBloom.lensFlareShader;
        MainBodyCameraBloomEffect.screenBlendShader = templateBloom.screenBlendShader;
        MainBodyCameraBloomEffect.separableBlurShader = templateBloom.separableBlurShader;
        MainBodyCameraBloomEffect.vignetteShader = templateBloom.vignetteShader;
        MainBodyCameraBloomEffect.bloomIntensity = 0;
		
		
        
        //TODO need to do render textures for this to work properly...
        foreach (Camera c in AllCameras)
        {
            c.transform.position = new Vector3(0, 0, 10);
            c.transform.LookAt(Vector3.zero, Vector3.up);
            c.isOrthoGraphic = true;
			
			FlatCameraManager.fit_camera_to_screen(c);
        }
	}
	
	
	public static float MAX_BLOOM_INTENSITY = 6;
	public float mBloomIntensity = 0;
    public void set_camera_effects(float perfect, bool hard = false)
    {
        mBloomIntensity = perfect;
		if(hard)
			MainBodyCameraBloomEffect.bloomIntensity = perfect;
    }
    public override void Update()
    {
		float interp = 0.1f;
		MainBodyCameraBloomEffect.bloomIntensity = MainBodyCameraBloomEffect.bloomIntensity*(1-interp) + mBloomIntensity * MAX_BLOOM_INTENSITY * interp;
	}
}
