using UnityEngine;
using System.Collections;

public class MetaManager : FakeMonoBehaviour {
	
	
	public UnlockManager UnlockManager {get; private set;}
	
	public MetaManager(ManagerManager aManager)
        : base(aManager) 
    {
		UnlockManager = new UnlockManager();
    }
	
	
	public override void Start () {
		
	}
	
	
	public override void Update () {
	
	}
	
}
