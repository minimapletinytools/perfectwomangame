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
		mManager.mZigManager.ZgInterface.read_data("unlock", 
            delegate(byte[] obj){
                if(obj != null){
                    UnlockManager.deserialize(obj);
                }
                SaveDataRead = true;
            }
        );
	}
	

	
	public override void Update () {
	
	}

	
}
