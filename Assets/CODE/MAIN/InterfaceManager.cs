using UnityEngine;
using System.Collections;

public class InterfaceManager : FakeMonoBehaviour {
    InterfaceBehaviour mBehaviour = null;
    public float mGrade;

    public FlatCameraManager mCameraManager;
    //TODO UI elements

    public InterfaceManager(ManagerManager aManager) : base(aManager) { }
    public override void Start() 
    {
        mBehaviour = mManager.gameObject.AddComponent<InterfaceBehaviour>();
        mBehaviour.mManager = this;

        mCameraManager = new FlatCameraManager(new Vector3(1000, 0, 0), 10);

    }
    public override void  Update()
    {
        
    }
}
