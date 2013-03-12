using UnityEngine;
using System.Collections.Generic;

public class NewInterfaceManager : FakeMonoBehaviour {
    public NewInterfaceManager(ManagerManager aManager) : base(aManager) { }

	
    public FlatCameraManager mFlatCamera;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	
	
	//
    CharacterTextureBehaviour mMiniMan;
	FlatBodyObject mCurrentBody = null;

    
	
	public override void Start()
    {
        mFlatCamera = new FlatCameraManager(new Vector3(10000, 0, 0), 10);
        mMiniMan = ((GameObject)GameObject.Instantiate(ManagerManager.Manager.mMenuReferences.miniMan)).GetComponent<CharacterTextureBehaviour>();        
    }
    public override void Update()
    {
        mFlatCamera.update(Time.deltaTime);
        if (mCurrentBody != null)
            mCurrentBody.match_body_to_projection(mManager.mProjectionManager);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);            
    }
    
    public Vector3 random_position()
    {
        //UGG piece of junk...
        return (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)).normalized * Random.Range(2000,20000);
    }
}
