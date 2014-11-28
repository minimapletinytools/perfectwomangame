using UnityEngine;
using System.Collections.Generic;
using System.Linq;


//TODO you'll want to switch this over to the Unity pulgins eventually

public class MicrosoftZig : ZgInterface
{
    
    ZgManager mZig;
	XboneUsers mUsers;
	XboneKinect mKinect;
	XbonePLM mPLM;
	XboneStorage mStorage;
	XboneEvents mEvents; 

    RenderTexture mColorImageRT = null;

	bool Initialized { get; set; }
	
	public void initialize(ZgManager aZig)
	{
        mZig = aZig;
        mUsers = new XboneUsers ();
		mPLM = new XbonePLM ();
		mKinect = new XboneKinect ();
		mStorage = new XboneStorage ();
		mEvents = new XboneEvents (aZig.mManager);


        mUsers.Start ();
		mPLM.Start ();
		mKinect.Start ();
		mStorage.Start ();
		mEvents.Start();

        //solves some jit problems
        System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

		Initialized = true;
	}
	
	public void update()
	{
		mKinect.Update ();
        mEvents.Update();

        if(mKinect.DepthTexture != null)
            ManagerManager.Manager.mZigManager.DepthView.UpdateTexture(mKinect.DepthTexture);


        //defer system gestures
        var gm = mZig.mManager.mGameManager;
        bool defer = false;
        if (gm.GS == NewGameManager.GameState.NORMAL)
            if (gm.mModeNormalPlay.GS == ModeNormalPlay.NormalPlayGameState.CHOICE && !gm.mModeNormalPlay.Paused) //if choosing and not paused
                defer = true; 
        //TODO OTHER DEFER CONDITIONS
        if (defer)
            mKinect.DeferSystemGestures(1);


        //testcode
        if (KeyMan.GetKeyDown("LeftThumbstick"))
        {
            write_data(mZig.mManager.mMetaManager.UnlockManager.serialize(),"unlock");

        }
        if (KeyMan.GetKeyDown("RightThumbstick"))
        {
            read_data("unlock",delegate(byte[] obj) { mZig.mManager.mMetaManager.UnlockManager.deserialize(obj);});
        }
	}
	
	public bool has_user()
	{
		return mKinect.IsTracking;
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
            /*
            Material mat = new Material(ManagerManager.Manager.mReferences.mXB1ColorImageShader);
            mat.SetTexture("Main",mKinect.ColorTexture);
            mat.SetTexture("Label",mKinect.LabelTexture);

            if(mColorImageRT == null)
                mColorImageRT = new RenderTexture(mKinect.ColorTexture.width,mKinect.ColorTexture.height,0);

            var img = new ImageGameObjectUtility(mKinect.ColorTexture);
            img.PlaneObject.renderer.material = mat;

            var aCam = ManagerManager.Manager.mCameraManager.ForegroundCamera; //borrow a camera
            //TODO resize the camera
            RenderTexture.active = mColorImageRT;
            aCam.targetTexture = mColorImageRT;
            aCam.Render();
            RenderTexture.active = null;
            aCam.targetTexture = null;
            */
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
        aResponse += delegate(byte[] obj) {Debug.Log("read callback " + obj.Length);};
        mStorage.read_data (aName,aResponse);
    }
	
	public ZgDepth DepthImage{get{ return null; }}
	public ZgImage ColorImage{get{ return null; }}
	public ZgLabelMap LabelMap{get{ return null; }}
    public bool ReaderInitialized { get{ return mKinect.IsReaderConnected; } } //TODO maybe return something more useful...
	public bool IsMicrosoftKinectSDK { get{ return true; } }
}


