using UnityEngine;
using System.Collections;

public class XboneUsers {


	public void Start () {
		Users.UsersManager.Create();
		
		
		//access with Users.UsersManager.Inst.Users;
		//listen to event Users.UsersManager.Inst.OnUserSignOut 
		//if (!Users.UsersManager.Inst.IsSomeoneSignedIn)
		//	Users.UsersManager.Inst.RequestSignIn (Users.AccountPickerOptions.AllowGuests);

	}
}
