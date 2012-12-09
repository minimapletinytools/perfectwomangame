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
    public GameManager mGameManager;

    public PrefabReferenceBehaviour mReferences;
    public MenuReferenceBehaviour mMenuReferences;

	void Awake () {
        mReferences = GetComponent<PrefabReferenceBehaviour>();
        mMenuReferences = GetComponent<MenuReferenceBehaviour>();

        if (sManager == null)
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
		if(mUpdateDelegates != null) 
			mUpdateDelegates();
	}
	
	void FixedUpdate() {
		if(mFixedUpdateDelegates != null) mFixedUpdateDelegates();
	}
}
