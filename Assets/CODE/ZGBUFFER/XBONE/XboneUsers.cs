using UnityEngine;
using System.Collections;
#if UNITY_XBOXONE 
using Users;
#endif


public class XboneUsers {


    #if UNITY_XBOXONE 
	public void Start () {
		UsersManager.Create();
		UsersManager.OnUsersChanged       += OnUsersChanged;
		UsersManager.OnUserSignIn         += OnUserSignIn;
		UsersManager.OnUserSignOut        += OnUserSignOut;
		UsersManager.OnSignOutStarted     += OnUserSignOutStarted;
		UsersManager.OnDisplayInfoChanged += OnUserDisplayInfoChange;

		//if (!Users.UsersManager.Inst.IsSomeoneSignedIn)
		//	Users.UsersManager.Inst.RequestSignIn (Users.AccountPickerOptions.AllowGuests);

	}

	void OnUsersChanged(int id,bool wasAdded)
	{

	}
	
	void OnUserSignIn(int id)
	{

	}
	
	void OnUserSignOut(int id)
	{

	}
	
	void OnUserSignOutStarted(int id, System.IntPtr deferred)
	{
		var deferral = new SignOutDeferral(deferred);
		var dummy = (new GameObject ("genDummy")).AddComponent<DummyBehaviour> ();
		dummy.StartCoroutine (deferral_thread (dummy.gameObject,deferral));
	}

	IEnumerator deferral_thread(GameObject aDestroy,SignOutDeferral aDef)
	{
		yield return null; 
		aDef.Complete ();
		GameObject.Destroy (aDestroy);
	}
	
	void OnUserDisplayInfoChange(int id)
	{

	}

#else
    public void Start () {
    }

#endif
}
