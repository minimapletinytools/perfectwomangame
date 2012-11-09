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

	void Start () {
        if (sManager == null)
            Manager = this;

        mEventManager = new EventManager(this);
        mInputManager = new InputManager(this);
		mZigManager = new ZigManager(this);
        

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
