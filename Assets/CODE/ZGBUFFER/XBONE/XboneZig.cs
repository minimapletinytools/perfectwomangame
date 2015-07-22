using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class MicrosoftZig : ZgInterface
{
    
    ZgManager mZig;

    XboneAll mAll;
	public XboneKinect mKinect;
	XbonePLM mPLM;
	XboneStorage mStorage;
	public XboneEvents mEvents; 
    XboneUnityLogPlugin mLog;

    public RenderTexture mColorImageRT = null;

	bool Initialized { get; set; }
	
	public void initialize(ZgManager aZig)
	{
        mZig = aZig;
        mAll = new XboneAll();
        mLog = new XboneUnityLogPlugin();
        mPLM = new XbonePLM (aZig.mManager);
		mKinect = new XboneKinect ();
		mStorage = new XboneStorage ();
		mEvents = new XboneEvents (aZig.mManager);

        mAll.Start();
        mLog.Start();
		mPLM.Start ();
		mKinect.Start ();
		mStorage.Start ();
		mEvents.Start();


        //TODO DELETE solves nothing
        //solves some jit problems
        //System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

		Initialized = true;
	}

    int initCounter = 0;
	public void update()
	{
        mAll.Update();
		mKinect.Update ();
        mEvents.Update();

        if(mKinect.DepthTexture != null)
            ManagerManager.Manager.mZigManager.DepthView.UpdateTexture(mKinect.DepthTexture);

        //if (mKinect.ColorTexture != null)take_color_image();


        //defer system gestures
        var gm = mZig.mManager.mGameManager;
        bool defer = false;
        if (gm.GS == NewGameManager.GameState.NORMAL)
        {
            //defer if in choice or play but not paused
            if(!gm.mModeNormalPlay.Paused)
            {
                if (gm.mModeNormalPlay.GS == ModeNormalPlay.NormalPlayGameState.CHOICE)
                    defer = true; 
                if (gm.mModeNormalPlay.GS == ModeNormalPlay.NormalPlayGameState.PLAY)
                    defer = true;
            }
        }
        //TODO OTHER DEFER CONDITIONS
        if (defer)
            mKinect.DeferSystemGestures(1);


        //testcode
        if (KeyMan.GetKeyDown("LeftThumbstick"))
        {
            mLog.UnityLog("test save data");
            write_data(mZig.mManager.mMetaManager.UnlockManager.serialize(),"unlock");

        }
        if (KeyMan.GetKeyDown("RightThumbstick"))
        {
            //mEvents.SendDeathEvent();
            //ManagerManager.Log("Sent fake death event");

            //take_color_image();
            mLog.UnityLog("test read data");
            read_data("unlock",delegate(byte[] obj) { mZig.mManager.mMetaManager.UnlockManager.deserialize(obj);});
        }

        if (initCounter == 3)
            mZig.mManager.GameEventDistributor("OTHER_PLATFORM_INITIALIZE", null);
        initCounter++;
	}
	
	public bool has_user()
	{
		return mKinect.IsTracking && mAll.IsSomeoneSignedIn;
	}

	//TODO should check for users
	public bool can_start()
	{
		if(Initialized && (mStorage.StorageCreated || mStorage.IsStorageFail))
			return true;
		return false;
	}

    public Texture2D take_color_image()
    {
        if(ManagerManager.Manager.mZigManager.is_reader_connected() == 2)
        {
            //TODO write the shader and test
            ManagerManager.Log("taking color image");

            Material mat = new Material(ManagerManager.Manager.mReferences.mXB1KinectImageMaskingShader);
            mat.SetTexture("_MainTex",mKinect.ColorTexture);
            mat.SetTexture("_AlphaTex",mKinect.LabelTexture);
            //mat.SetTexture("_MainTex",mKinect.LabelTexture);

            if(mColorImageRT == null)
                mColorImageRT = new RenderTexture(mKinect.ColorTexture.width,mKinect.ColorTexture.height,0);

            var img = new ImageGameObjectUtility(mKinect.ColorTexture);
            img.PlaneObject.GetComponent<Renderer>().material = mat;

                                                                                                                                           
            Camera cam = ManagerManager.Manager.gameObject.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = img.BaseDimension.y;
            img.PlaneObject.transform.position = cam.transform.position + cam.transform.forward*10;
            cam.transform.LookAt(img.PlaneObject.transform.position);
            //TODO resize the camera
            RenderTexture.active = mColorImageRT;


            cam.targetTexture = mColorImageRT;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.blue;
            cam.Render();
            //Texture2D copyTex = new Texture2D(mColorImageRT.width,mColorImageRT.height);
            //copyTex.ReadPixels(new Rect(0,0,mColorImageRT.width,mColorImageRT.height),0,0);
            //copyTex.Apply();
            RenderTexture.active = null;
            cam.targetTexture = null;
            GameObject.Destroy(cam);
            img.destroy();


            return ManagerManager.Manager.mZigManager.ImageView.UpdateTexture(mKinect.ColorTexture,mKinect.LabelTexture);
        }
        return null;
    }

    public void write_data(byte[] aData, string aName)
    {
        mStorage.write_data (aData, aName);
    }
    public void read_data(string aName, System.Action<byte[]> aResponse)
    {
        aResponse += delegate(byte[] obj) {ManagerManager.Log("read callback " + (obj == null ? " FAIL" : obj.Length.ToString()));};
        mStorage.read_data (aName,aResponse);
    }
	
	public ZgDepth DepthImage{get{ return null; }}
	public ZgImage ColorImage{get{ return null; }}
	public ZgLabelMap LabelMap{get{ return null; }}
    public bool ReaderInitialized { get{ return mKinect.IsReaderConnected; } } //TODO maybe return something more useful...
	public bool IsMicrosoftKinectSDK { get{ return true; } }
}


