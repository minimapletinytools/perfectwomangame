using UnityEngine;
using System.Collections;

public class ModePerfectSimian
{
	
	
	
	
	NewGameManager NGM {get; set;}
	ManagerManager mManager {get; set;}
	
	PhysicsFlatBodyObject mFlat = null;
	
	
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
		Physics.gravity = new Vector3(0,-10,0);
	}
	
	
	public void character_loaded()
	{
		Debug.Log("character loaded");
		FlatBodyObject flatbody = new FlatBodyObject(NGM.CurrentCharacterLoader, -1);
		flatbody.HardPosition = Vector3.zero;
		
		
        flatbody.update(0);
		
		mFlat = new PhysicsFlatBodyObject(flatbody);
		mFlat.setup_body_with_physics();
		
		mManager.mTransitionCameraManager.fade_in_with_sound();
	}
	
	public void update () 
	{
		if(mFlat  != null)
		{
			mFlat.set_target_pose(NGM.CurrentPose);
			mFlat.update();
		}
	}
}
