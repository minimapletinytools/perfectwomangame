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
        //SO BAD
		GameConstants.SCALE = 1/200f;
		mManager.mAssetLoader.new_load_character("05-1",mManager.mCharacterBundleManager);
	}
	
	
	public void load_character(CharacterLoader aLoader)
	{
        NGM.mManager.mBackgroundManager.load_character(aLoader);

		FlatBodyObject flatbody = new FlatBodyObject(aLoader, -1);

		flatbody.set_target_pose(NGM.CurrentPose,true);
		flatbody.HardPosition = new Vector3(10,0,0);
        flatbody.update(0);
		flatbody.set_layer (1 << 1);

		flatbody.PrimaryGameObject.name = "SIMIANBODY";


		//needed to make debugviewerthing work
		var cam = mManager.gameObject.AddComponent<Camera>();
		cam.transform.position = mManager.mCameraManager.MainBodyCamera.transform.position;
		cam.transform.rotation = mManager.mCameraManager.MainBodyCamera.transform.rotation;
		cam.fieldOfView = mManager.mCameraManager.MainBodyCamera.fieldOfView;
		cam.isOrthoGraphic = mManager.mCameraManager.MainBodyCamera.isOrthoGraphic;
		cam.orthographicSize = mManager.mCameraManager.MainBodyCamera.orthographicSize;
		Debug.Log (mManager.mCameraManager.MainBodyCamera.orthographicSize);
		cam.depth = 9999999;
		cam.clearFlags = CameraClearFlags.Depth;
		cam.cullingMask = 0;

		mSimian = new FarseerSimian();
		mSimian.initialize(mManager.gameObject);
		mSimian.setup_with_body(flatbody);

		//this is stupid
		mSimian.add_environment(GameObject.FindObjectsOfType(typeof(Transform)).Where(e=>e.name.StartsWith("FS_")).Select(e=>((Transform)e).gameObject));
		
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
