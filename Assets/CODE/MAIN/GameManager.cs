using UnityEngine;
using System.Collections;

public class GameManager : FakeMonoBehaviour
{
    public Camera mCamera;
    public GameManager(ManagerManager aManager) : base(aManager) { }
    public override void Start()
    {
        mCamera = mManager.gameObject.AddComponent<Camera>();
        mCamera.transform.position = new Vector3(0, 0, 10);
        mCamera.transform.LookAt(Vector3.zero, Vector3.up);
    }
    public override void Update()
    {

    }
}
