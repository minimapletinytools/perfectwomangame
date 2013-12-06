using UnityEngine;
using System.Collections;

public class ModeTesting
{
	
	NewGameManager NGM {get; set;}
	ManagerManager mManager {get; set;}
	
	public ModeTesting(NewGameManager aNgm)
	{
		NGM = aNgm;
		mManager = aNgm.mManager;
	}

	public void character_loaded()
	{
		mManager.mTransitionCameraManager.fade_in_with_sound();
	}
	
	
	public int mLastDiff = 0;
	public int mLastWrite = 0;
	public int mLastCutscene = 0;
	public int mLastPoseFolder = 0;
	public PerformanceType.PType mLastPoseMode = PerformanceType.PType.SLOW;
	public float mLastPoseSpeed = 5;
	
	public void update()
	{
		
		if(NGM.CurrentPose != null && mManager.mBodyManager.mFlat != null) //make sure a character is in fact loaded, this can apparently happen in testing scene.
		{
			mManager.mBodyManager.set_target_pose(NGM.CurrentPose);
		}
		
		//if we are annoyed by the pose..
		if(Input.GetKeyDown(KeyCode.Alpha9))
			NGM.CurrentPoseAnimation = null;
		
		if(NGM.CurrentPoseAnimation != null)
		{
			NGM.CurrentTargetPose = NGM.CurrentPoseAnimation.get_pose(Time.time);
			mManager.mTransparentBodyManager.set_target_pose(NGM.CurrentTargetPose);
			float grade = ProGrading.grade_pose(NGM.CurrentPose, NGM.CurrentTargetPose);
			grade = ProGrading.grade_to_perfect(grade);
			mManager.mCameraManager.set_camera_effects(grade);
		}
		
		if(mManager.mZigManager.is_reader_connected() != 2)
			mManager.mBodyManager.keyboard_update();
		
		if(Input.GetKeyDown(KeyCode.Space))
		{
			string folderPrefix = "";
			string output = "";
			if(mManager.mZigManager.is_reader_connected() != 2){
				output = NGM.CurrentCharacterIndex.StringIdentifier + "_man_" + mLastWrite;
				mManager.mBodyManager.write_pose(folderPrefix + output + ".txt",true);
			} else {
				output = NGM.CurrentCharacterIndex.StringIdentifier + "_kinect_" + mLastWrite;
				mManager.mBodyManager.write_pose(folderPrefix + output + ".txt",false);
			}
			mManager.take_screenshot(folderPrefix + output+".png",mManager.mCameraManager.MainBodyCamera);
			mLastWrite++;		
		}
		
		
		if(Input.GetKeyDown(KeyCode.Alpha5))
		{
			if(NGM.CurrentCharacterLoader.has_cutscene(mLastCutscene))
			{
				mManager.mBodyManager.transition_character_out();
				mManager.mTransparentBodyManager.transition_character_out();
				mManager.mBackgroundManager.load_cutscene(mLastCutscene,NGM.CurrentCharacterLoader);
				ManagerManager.Manager.mDebugString = "loaded cutscene " + mLastCutscene;
				mManager.mDebugString = "loaded cutscene " + mLastCutscene;
			}
			else mManager.mDebugString = "missing cutscene " + mLastCutscene;
			mLastCutscene = (mLastCutscene+1) % 5;
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha6))
		{
			string[] dirs = System.IO.Directory.GetDirectories("POSETEST");
			NGM.CurrentPoseAnimation = new PerformanceType(PoseAnimation.load_from_folder(dirs[mLastPoseFolder% dirs.Length]),new CharacterIndex(2,0));
			mManager.mDebugString = "pose folder: " + dirs[mLastPoseFolder% dirs.Length];
			mLastPoseFolder++;
			
			NGM.CurrentPoseAnimation.PT = mLastPoseMode;
			NGM.CurrentPoseAnimation.ChangeTime = mLastPoseSpeed;
		}
		
		if(Input.GetKeyDown(KeyCode.A) && NGM.CurrentPoseAnimation != null)
		{
			mLastPoseMode = (PerformanceType.PType)(((int)mLastPoseMode + 1)%(int)PerformanceType.PType.COUNT);
			ManagerManager.Manager.mDebugString = "pose mode is: " + mLastPoseMode.ToString();
			NGM.CurrentPoseAnimation.PT = mLastPoseMode;
		}
		if(Input.GetKey(KeyCode.S) && NGM.CurrentPoseAnimation != null)
		{
			mLastPoseSpeed = (mLastPoseSpeed + Time.deltaTime*2.5f)%10;
			ManagerManager.Manager.mDebugString = "pose time is: " + mLastPoseSpeed;
			NGM.CurrentPoseAnimation.ChangeTime = mLastPoseSpeed;
		}
		
			
		
		if(Input.GetKeyDown(KeyCode.Alpha8))
		{
			mManager.mDebugString = "set to diff " + ((++mLastDiff)%4);
			NGM.CurrentPoseAnimation = new PerformanceType(mManager.mCharacterBundleManager.get_pose(NGM.CurrentCharacterIndex,mLastDiff%4), new CharacterIndex(2,0)); //forces it to be switch
		}
		
		
		
		int choice = -1;
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			choice = 0;
		}
		else if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			choice = 1;
		}
		else if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			choice = 2;
		}
		else if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			choice = 3;
		}
		bool shift = Input.GetKey(KeyCode.LeftShift);
		
		if(choice != -1)
		{
			if(shift)
			{
				if(NGM.CurrentCharacterIndex.LevelIndex > 0)
					mManager.mAssetLoader.new_load_character(NGM.CurrentCharacterIndex.get_past_neighbor(choice).StringIdentifier,mManager.mCharacterBundleManager);
			}
			else
			{
				if(NGM.CurrentCharacterIndex.LevelIndex < 8)
					mManager.mAssetLoader.new_load_character(NGM.CurrentCharacterIndex.get_future_neighbor(choice).StringIdentifier,mManager.mCharacterBundleManager);
			}
		}
	}
}
