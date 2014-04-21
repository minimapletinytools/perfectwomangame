using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

public class ModePerfectSimian
{
	
	
	NewGameManager NGM {get; set;}
	ManagerManager mManager {get; set;}
	
	FarseerSimian mSimian = null;
	
	
	public ModePerfectSimian(NewGameManager aNgm)
	{
		NGM = aNgm;
		mManager = aNgm.mManager;
	}
	
	public ModePerfectSimian()
	{
	}
	
	public void initialize() 
	{
		mManager.mAssetLoader.new_load_character("05-1",mManager.mCharacterBundleManager);
	}
	
	
	public void character_loaded()
	{
		Debug.Log("character loaded");
		FlatBodyObject flatbody = new FlatBodyObject(NGM.CurrentCharacterLoader, -1);
		flatbody.set_target_pose(NGM.CurrentPose,true);
		flatbody.HardPosition = Vector3.zero;
        flatbody.update(0);

		//needed to make debugviewerthing work
		var cam = mManager.gameObject.AddComponent<Camera>();
		cam.transform.position = mManager.mCameraManager.MainBodyCamera.transform.position;
		cam.transform.rotation = mManager.mCameraManager.MainBodyCamera.transform.rotation;
		cam.fieldOfView = mManager.mCameraManager.MainBodyCamera.fieldOfView;
		cam.isOrthoGraphic = mManager.mCameraManager.MainBodyCamera.isOrthoGraphic;
		cam.orthographicSize = mManager.mCameraManager.MainBodyCamera.orthographicSize;
		Debug.Log (mManager.mCameraManager.MainBodyCamera.orthographicSize);
		cam.depth = 9999999;
		cam.clearFlags = CameraClearFlags.SolidColor;
		cam.cullingMask = 0;

		mSimian = new FarseerSimian();
		mSimian.initialize(mManager.gameObject);
		mSimian.setup_with_body(flatbody);

		//this is stupid
		mSimian.add_environment(GameObject.FindObjectsOfType(typeof(Transform)).Where(e=>e.name.StartsWith("FS_")).Select(e=>((Transform)e).gameObject));
		
		mManager.mTransitionCameraManager.fade_in_with_sound();
	}
	
	public void update () 
	{
		//we store the desired position inside of mFlat??
		if(mSimian  != null)
		{
			mSimian.update(mManager.mProjectionManager);
		}


	}
}
