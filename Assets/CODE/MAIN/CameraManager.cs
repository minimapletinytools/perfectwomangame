using UnityEngine;
using System.Collections;

public class CameraManager : FakeMonoBehaviour {
    public CameraManager(ManagerManager aManager) : base(aManager) { }

    public Camera MainBodyCamera { get; private set; } //layer 1
    public BloomAndLensFlares MainBodyCameraBloomEffect { get; private set; }
    public Camera TransparentBodyCamera { get; private set; } //layer 2
    public EdgeDetectEffect TransparentBodyCameraEdgeEffect { get; private set; }
    public Camera BackgroundCamera { get; private set; } //layer 3

    GameObject ImageEffects { get; set; }

    public Camera[] AllCameras { get { return new Camera[] { MainBodyCamera,TransparentBodyCamera,BackgroundCamera};} }
	public override void Start () 
    {
        ImageEffects = (GameObject)GameObject.Instantiate(mManager.mReferences.mImageEffectsPrefabs);

        MainBodyCamera = (new GameObject("genMainCamera")).AddComponent<Camera>();
        MainBodyCamera.cullingMask = 1 << 1;
        MainBodyCamera.depth = 3;
        MainBodyCamera.clearFlags = CameraClearFlags.Depth;
        //MainBodyCameraBloomEffect = MainBodyCamera.gameObject.AddComponent<BloomAndLensFlares>();
        //MainBodyCameraBloomEffect.addBrightStuffOneOneShader = Blo
        mManager.mBodyManager.set_layer(1);

        
        TransparentBodyCamera = (new GameObject("genTransparentCamera")).AddComponent<Camera>();
        TransparentBodyCamera.cullingMask = 1 << 2;
        TransparentBodyCamera.depth = 2;
        TransparentBodyCamera.clearFlags = CameraClearFlags.Depth;
        TransparentBodyCameraEdgeEffect = TransparentBodyCamera.gameObject.AddComponent<EdgeDetectEffect>();
        mManager.mTransparentBodyManager.set_layer(2);

        BackgroundCamera = (new GameObject("genBackgroundCamera")).AddComponent<Camera>();
        BackgroundCamera.cullingMask = 1 << 3;
        BackgroundCamera.depth = 1;
        BackgroundCamera.clearFlags = CameraClearFlags.Depth;
        mManager.mBackgroundManager.set_background_layer(3);
        
        //TODO need to do render textures for this to work properly...

        foreach (Camera c in AllCameras)
        {
            c.transform.position = new Vector3(0, 0, 10);
            c.transform.LookAt(Vector3.zero, Vector3.up);
            c.isOrthoGraphic = true;
        }
	}
	
    public override void Update()
    {
	}
}
