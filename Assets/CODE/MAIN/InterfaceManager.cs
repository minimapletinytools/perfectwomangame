using UnityEngine;
using System.Collections;

public class InterfaceManager : FakeMonoBehaviour {
    InterfaceBehaviour mBehaviour = null;

    public float mGrade;
    public InterfaceManager(ManagerManager aManager) : base(aManager) { }
    public override void Start() 
    {
        mBehaviour = mManager.gameObject.AddComponent<InterfaceBehaviour>();
        mBehaviour.mManager = this;
    }
    public override void  Update()
    {
        
    }
}
