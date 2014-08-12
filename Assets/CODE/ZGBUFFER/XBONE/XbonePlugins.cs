using UnityEngine;
using System.Collections;

//this class handles all plugins other than the kinect
public class XbonePlugins
{

	public XbonePlugins()
	{

	}

	public void initialize_plugins()
	{
		//TODO	 

		Users.UsersManager.Create();
		//access with Users.UsersManager.Inst.Users;
		//listen to event Users.UsersManager.Inst.OnUserSignOut 
		//if (!Users.UsersManager.Inst.IsSomeoneSignedIn)
		//	Users.UsersManager.Inst.RequestSignIn (Users.AccountPickerOptions.AllowGuests);

		Storage.StorageManager.Create();
		//Storage.StorageManager.AmFullyInitialized ();
		//note this creates a ConnectedStorage Ojbect, container string is legacy, it's the 'active container'
		//TODO add callbakc
		//var createStorageOp = Storage.ConnectedStorage.CreateAsync (Users.UsersManager.Inst.Users [0].Id, "main_save", null);
		//if (createStorageOp.IsComplete) { //actually just do this in the callback instead
			//var container = createStorageOp.Storage.OpenOrCreateContainer ("main_save");
			//TODO
			//container.ReadAsync
			//TODO update the Datamap in the container...
		}

		//for rich presence???
		//var rta = DataPlatform.RTAManager.CreateAsync (Users.UsersManager.Inst.Users [0].Id, null);



	}

	public void on_all_plugins_initialized()
	{
		//TODO handle plugin callbacks
	}

	public void update()
	{
		//TODO
	}



}
