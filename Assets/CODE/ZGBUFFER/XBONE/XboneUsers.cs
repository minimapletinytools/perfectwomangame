using UnityEngine;
using System.Collections;
#if UNITY_XBOXONE 
using Users;
#endif


public class XboneUsers {
    #if UNITY_XBOXONE 

    public int ActiveUserId{ get; private set; }
	public void Start () {
		UsersManager.Create();
		UsersManager.OnUsersChanged       += OnUsersChanged;
		UsersManager.OnUserSignIn         += OnUserSignIn;
		UsersManager.OnUserSignOut        += OnUserSignOut;
		UsersManager.OnSignOutStarted     += OnUserSignOutStarted;
		UsersManager.OnDisplayInfoChanged += OnUserDisplayInfoChange;

		if (!UsersManager.IsSomeoneSignedIn)
        {
            ActiveUserId = -1;
            UsersManager.RequestSignIn(Users.AccountPickerOptions.AllowGuests);
        }
        else
            ActiveUserId = UsersManager.Users [0].Id;

	}

	void OnUsersChanged(int id,bool wasAdded)
	{

	}
	
	void OnUserSignIn(int id)
	{
        if (ActiveUserId == -1 || ActiveUserId == id)
            ActiveUserId = id;
        else
        {
            //TODO tell the user that the game will restart unless they change back.. This involves pausing the game??
            //TODO or just restart the game?
        }
	}
	
	void OnUserSignOut(int id)
	{
        UsersManager.RequestSignIn (Users.AccountPickerOptions.AllowGuests);
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
