using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
public class ManagerManager : MonoBehaviour{



    static ManagerManager sManager = null;
    public static ManagerManager Manager
    {
        get { return sManager; }
        private set { sManager = value; }
    }

    //debug nonsense
    public bool mRecordMode = false;
    //constants
    public static int MANIPULATOR_LAYER = 8;
	
	//delegate stuff
	public delegate void VoidDelegate();
	HashSet<FakeMonoBehaviour> mScripts = new HashSet<FakeMonoBehaviour>();
	VoidDelegate mStartDelegates = null;
	VoidDelegate mUpdateDelegates = null;
	VoidDelegate mFixedUpdateDelegates = null;

    public EventManager mEventManager;
    public InputManager mInputManager;
    public ZigManager mZigManager;
	public ProjectionManager mProjectionManager;
    public InterfaceManager mInterfaceManager;
    public BodyManager mBodyManager;
    public BodyManager mTransparentBodyManager;
    public BackgroundManager mBackgroundManager;
    public CameraManager mCameraManager;
    public ParticleManager mParticleManager;
    public AssetBundleLoader mAssetLoader;
    public GameManager mGameManager;
    

    public PrefabReferenceBehaviour mReferences;
    public MenuReferenceBehaviour mMenuReferences;

	void Awake () {

        Debug.Log("setting up managers");
        mReferences = GetComponent<PrefabReferenceBehaviour>();
        mMenuReferences = GetComponent<MenuReferenceBehaviour>();

        
        Manager = this;

        mEventManager = new EventManager(this);
        mInputManager = new InputManager(this);
		mZigManager = new ZigManager(this);
		mProjectionManager = new ProjectionManager(this);
        mInterfaceManager = new InterfaceManager(this);
        mBodyManager = new BodyManager(this);
        mTransparentBodyManager = new BodyManager(this);
        mBackgroundManager = new BackgroundManager(this);
        mCameraManager = new CameraManager(this);
        mParticleManager = new ParticleManager(this);
        mAssetLoader = new AssetBundleLoader(this);
        mGameManager = new GameManager(this);

		if(mStartDelegates != null) 
			mStartDelegates();

         
	}
	
	public void register_FakeMonoBehaviour(FakeMonoBehaviour aScript)
	{
		mScripts.Add(aScript);

		if(aScript.is_method_overridden("Start"))
			mStartDelegates += aScript.Start;
		if(aScript.is_method_overridden("Update"))
			mUpdateDelegates += aScript.Update;
		if(aScript.is_method_overridden("FixedUpdate"))
			mFixedUpdateDelegates += aScript.FixedUpdate;
	}
	
	public void deregister_FakeMonoBehaviour(FakeMonoBehaviour aScript)
	{
		mScripts.Add(aScript);
		if(aScript.is_method_overridden("Start"))
			mStartDelegates -= aScript.Start;
		if(aScript.is_method_overridden("Update"))
			mUpdateDelegates -= aScript.Update;
		if(aScript.is_method_overridden("FixedUpdate"))
			mFixedUpdateDelegates -= aScript.FixedUpdate;
	}
		
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        if (Input.GetKeyDown(KeyCode.R))
        {
            mGameManager.unload_current_asset_bundle();
            Application.LoadLevel("kinect_test");
        }
		if(mUpdateDelegates != null) 
			mUpdateDelegates();
	}



    //Screenshot nonsense
    //TODO move this nonsense into its own class
    static int sScreenShotNumber = 0;
    public static string ScreenShotName { get { return "char" + sScreenShotNumber; } }
    void take_screenshot(string filename, Camera cam)
    {
        
        int resWidth = 800;
        int resHeight = 450;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        cam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        CameraClearFlags ccf = cam.clearFlags;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 1);
        cam.DoClear();
        cam.clearFlags = ccf;
        cam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(filename, bytes);
        //Debug.Log(string.Format("Took screenshot to: {0}", filename));
        
    }
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Camera cam = mCameraManager.MainBodyCamera;
            string filename = "char_kinect_" + ScreenShotName + ".png";
            take_screenshot(filename, cam);
            cam = mCameraManager.TransparentBodyCamera;
            filename = "char_manual_" + ScreenShotName + ".png";
            take_screenshot(filename, cam);
            sScreenShotNumber++;
        }
    }
	
	void FixedUpdate() {
		if(mFixedUpdateDelegates != null) mFixedUpdateDelegates();
	}
}
