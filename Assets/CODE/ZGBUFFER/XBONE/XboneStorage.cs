using UnityEngine;
using System.Collections;
#if UNITY_XBOXONE 
using Storage;
#endif

public class XboneStorage 
{

#if UNITY_XBOXONE 

	ConnectedStorage mStorage = null;
    public bool StorageCreated {get; private set;}
    public bool IsStorageFail { get; private set; } 
    public bool IsWriting{get; private set;}

    public void InitializeUserStorage()
    {
        var dummy = (new GameObject("genDummy")).AddComponent<DummyBehaviour>();
        dummy.StartCoroutine(save_thread(dummy.gameObject));
    }

	public void write_data(byte[] aData, string aName)
	{
		if (StorageCreated && !IsWriting)
        {
            //Debug.Log("trying to write data " + aData.Length);
            IsWriting = true;
            DataMap toSave = DataMap.Create();
            toSave.AddOrReplaceBuffer(aName, aData);
            mStorage.SubmitUpdatesAsync(toSave, null, delegate(ContainerContext Storage2, SubmitDataMapUpdatesAsyncOp op2)
            {

                bool ok = op2.Success && op2.Status == ConnectedStorageStatus.SUCCESS;

                ManagerManager.Log("write data success " + ok + " " + op2.Success + " " + op2.Status);
                //TODO confirm success??

                IsWriting = false;
            });
        } else
        {
            ManagerManager.Log("error, could not write " + StorageCreated + " " + IsWriting);
        }
	}
	public void read_data(string aName, System.Action<byte[]> aResponse)
	{
		if (StorageCreated) 
		{
			mStorage.GetAsync(new string[]{aName},
				delegate(ContainerContext storage, GetDataMapViewAsyncOp op, DataMapView view) {
					if(op.Success)
					{
						var data = view.GetBuffer(aName);
                        aResponse(data);
					}
					else
					{
                        aResponse(null);
					}
				}
			);
		} else 
        {
            ManagerManager.Log("error trying to read data before storage is created");
            aResponse(null);
        }
	}

	IEnumerator save_thread(GameObject aDestroy)
	{
		while (!Storage.StorageManager.AmFullyInitialized () || !MicrosoftZig.Inst.has_user())
			yield return null;

		ConnectedStorage.CreateAsync (MicrosoftZig.Inst.mAll.ActiveUser.Id, "main_save", 
			delegate(ConnectedStorage storage, CreateConnectedStorageOp op) {
				if(op.Success)
				{
                    StorageCreated = true;
                    IsStorageFail = false;
					mStorage = storage;
					ManagerManager.Log ("storage created success");
				}
				else
				{
                    IsStorageFail = true;
                    ManagerManager.Log ("storage create failed");
				}
			}
		);

		GameObject.Destroy (aDestroy);
	}

	
#else
    public bool StorageCreated {get; private set;}
    public bool IsStorageFail { get; private set; } 
    public bool IsWriting{get; private set;}

	public void write_data(byte[] aData, string aName)
	{
		
	}
    public void read_data(string aName, System.Action<byte[]> aResponse)
  	{

	}
#endif
}
