using UnityEngine;
using System.Collections;

public class MetaManager : FakeMonoBehaviour {
	
	
	public UnlockManager UnlockManager {get; private set;}

    public bool SaveDataRead{ get; private set; } //true if we have loaded save data
    public bool IsReadingSaveData { get; private set; }
	
	public MetaManager(ManagerManager aManager)
        : base(aManager) 
    {
		UnlockManager = new UnlockManager(aManager);
    }
	
	
	public override void Start () {
        StartSaveThread();
	}

    public void StartSaveThread()
    {
        if (GameConstants.UNLOCK_ALL) //no nead to read anything if we are in unlock all mode
        {
            return;
        }

        if (!IsReadingSaveData)
        {
            var dummy = (new GameObject("genDummy")).AddComponent<DummyBehaviour>();
            dummy.StartCoroutine(read_unlock_when_ready(dummy.gameObject));
        }
    }

    IEnumerator read_unlock_when_ready( GameObject dummy )
    {
        IsReadingSaveData = true;
        //ManagerManager.Manager.mDebugString = "save, can start state: " + mManager.mZigManager.ZgInterface.can_start();

        while (!mManager.mZigManager.ZgInterface.can_start())
            yield return null;

        ManagerManager.Log("starting save thread");
        mManager.mZigManager.ZgInterface.read_data("unlock", 
           delegate(byte[] obj){
                ManagerManager.Log("received save data " + obj);

                if(obj != null){
                    ManagerManager.Log("length " + obj.Length);
                    UnlockManager.deserialize(obj);
                }

                SaveDataRead = true;

                if (ManagerManager.Manager.mTransitionCameraManager.IsInitialized)
                {
                    ManagerManager.Log("setting char transparencies");
                    ManagerManager.Manager.mTransitionCameraManager.set_start_screen_character_transparency();
                }

                IsReadingSaveData = false;
            }
        );
        GameObject.Destroy(dummy);
    }
	

	
	public override void Update () {
	
	}

	
}
