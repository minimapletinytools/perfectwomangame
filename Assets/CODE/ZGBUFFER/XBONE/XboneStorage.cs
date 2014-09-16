using UnityEngine;
using System.Collections;
#if UNITY_XBOXONE 
using Storage;
#endif

public class XboneStorage 
{

#if UNITY_XBOXONE 

	ConnectedStorage mStorage = null;
	bool StorageCreated {get; set;}

	public void Start()
	{
		Storage.StorageManager.Create();
		var dummy = (new GameObject ("genDummy")).AddComponent<DummyBehaviour> ();
		dummy.StartCoroutine (save_thread (dummy.gameObject));
	}

	public void write_data(byte[] aData, string aName)
	{
		if (StorageCreated) {
			DataMap toSave = DataMap.Create();
			toSave.AddOrReplaceBuffer(aName,aData);
			mStorage.SubmitUpdatesAsync(toSave,null,delegate(ContainerContext Storage2, SubmitDataMapUpdatesAsyncOp op2) {
				bool ok = op2.Success && op2.Status == ConnectedStorageStatus.SUCCESS;
				//TODO confirm success??
			});
		}
	}
	public byte[] read_data(string aName)
	{
		if (StorageCreated) 
		{
			mStorage.GetAsync(new string[]{aName},
				delegate(ContainerContext storage, GetDataMapViewAsyncOp op, DataMapView view) {
					if(op.Success)
					{
						var data = view.GetBuffer(aName);
						//TODO do something with the data
					}
					else
					{
						//TODO
					}
				}
			);
		}
		return null;
	}

	IEnumerator save_thread(GameObject aDestroy)
	{
		while (!Storage.StorageManager.AmFullyInitialized ())
			yield return null;

		//TODO use user ID from user manager...
		ConnectedStorage.CreateAsync (-1, "main_save", 
			delegate(ConnectedStorage storage, CreateConnectedStorageOp op) {
				if(op.Success)
				{
					mStorage = storage;
					StorageCreated = true;
					Debug.Log ("storage created success");
				}
				else
				{
					//TODO
					Debug.Log ("storage create failed");
				}
			}
		);

		GameObject.Destroy (aDestroy);
	}

	
#else
	public void Start()
	{
	}
	public void write_data(byte[] aData, string aName)
	{
		
	}
	public byte[] read_data(string aName)
	{
		return null;
	}
#endif
}
