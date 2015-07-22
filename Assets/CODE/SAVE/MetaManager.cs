using UnityEngine;
using System.Collections;

public class MetaManager : FakeMonoBehaviour {
	
	
	public UnlockManager UnlockManager {get; private set;}

    public bool SaveDataRead{ get; private set; } //true if we have loaded save data
	
	public MetaManager(ManagerManager aManager)
        : base(aManager) 
    {
		UnlockManager = new UnlockManager(aManager);
    }
	
	
	public override void Start () {

        var dummy = (new GameObject("genDummy")).AddComponent<DummyBehaviour>();
        dummy.StartCoroutine(read_unlock_when_ready(dummy.gameObject));

		
	}

    IEnumerator read_unlock_when_ready( GameObject dummy )
    {
        while (!mManager.mZigManager.ZgInterface.can_start())
            yield return null;

        mManager.mZigManager.ZgInterface.read_data("unlock", 
           delegate(byte[] obj){
            ManagerManager.Log("received save data " + obj);
                if(obj != null){
                    ManagerManager.Log("length " + obj.Length);
                    UnlockManager.deserialize(obj);
                }
                SaveDataRead = true;
            }
        );
        GameObject.Destroy(dummy);
    }
	

	
	public override void Update () {
	
	}

	
}
