using UnityEngine;
using System.Collections;

public class XboneStorage 
{
	public void Start()
	{
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
		//}

	}
}
