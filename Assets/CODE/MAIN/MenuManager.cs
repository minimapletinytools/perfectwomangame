using UnityEngine;
using System.Collections;

public class MenuManager : FakeMonoBehaviour {
    public MenuManager(ManagerManager aManager) : base(aManager) { }

    //constants
    const int mNumberBefore = 2;
    const int mNumberAfter = 3;
    const int mMinSpacing = 30; //pixels

    

    public Vector3 mCenter = new Vector3(9999, 0, 0);
    public Camera mCamera = null;

    public QuTimer mAnimateTimer = new QuTimer(0, 1);
    public override void Start()
    {
        mCamera = (new GameObject("genMenuCamera")).AddComponent<Camera>();
        mCamera.transform.position = mCenter + new Vector3(0, 0, 10);
        mCamera.isOrthoGraphic = true;
        mCamera.clearFlags = CameraClearFlags.Depth;
        //mCamera.orthographicSize  TODO
        mCamera.transform.LookAt(mCenter);


    }
    public override void Update()
    {

    }

}
