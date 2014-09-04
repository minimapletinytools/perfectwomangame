using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ModeTesting
{
	
	public NewGameManager NGM {get; set;}
	public ManagerManager mManager {get; set;}
    AuthoringGuiBehaviour Gui { get; set; }
	public ModeTesting(NewGameManager aNgm)
	{
		NGM = aNgm;
		mManager = aNgm.mManager;
	}
	public void character_loaded(){}
	
	



    public int mLastDiff = 0;
    public int mLastWrite = 0;
    public int mLastCutscene = 0;
    public int mLastPoseFolder = 0;
    public PerformanceType.PType mLastPoseMode = PerformanceType.PType.SLOW;
    public float mLastPoseSpeed = 5;

    PoseAnimation mCurrentPoseAnimation = null;

    //functions for AuthoringGuiBehaviour to call
    public void load_char_from_folder(CharacterIndex aChar, int aDiff)
    {
        var aFolder = aChar.StringIdentifier + "_" + aDiff;
        string[] dirs = System.IO.Directory.GetDirectories("POSETEST");
        string dir = System.IO.Directory.GetDirectories("POSETEST").FirstOrDefault(e => e == aFolder);
        if(dir != "")
            set_pose_animation(PoseAnimation.load_from_folder(dir), aDiff);
        //TODO else error message
       
    }
    public void load_char_default_poses(CharacterIndex aChar, int aDiff)
    {
        if(CharacterIndex.sAllCharacters.Contains(aChar) && aChar != CharacterIndex.sGrave && aChar != CharacterIndex.sFetus && aChar != CharacterIndex.sOneHundred)
            set_pose_animation(mManager.mCharacterBundleManager.get_pose(aChar,aDiff),aDiff);
        //TODO eles error message
    }
    public void set_pose_animation(PoseAnimation aAnim, int aDiff)
    {
        mCurrentPoseAnimation = aAnim;
        NGM.CurrentPoseAnimation = new PerformanceType(aAnim, new CharacterIndex(2,0)); //forces it to be switch
        NGM.CurrentPoseAnimation.set_change_time(GameConstants.difficultyToChangeTime[aDiff]);
        NGM.CurrentPoseAnimation.PT = mLastPoseMode;
        NGM.CurrentPoseAnimation.ChangeTime = mLastPoseSpeed;
    }
    public void write_poses_to_folder(CharacterIndex aChar, int aDiff)
    {
        mCurrentPoseAnimation.save_to_folder("POSETEST/" + aChar.StringIdentifier + "_" + aDiff,aChar.StringIdentifier + "_" + aDiff);
    }
    public void load_character(CharacterIndex aChar)
    {
        if(CharacterIndex.sAllCharacters.Contains(aChar) && aChar != CharacterIndex.sGrave)
            mManager.mAssetLoader.new_load_character(aChar.StringIdentifier,mManager.mCharacterBundleManager);
        //TODO else error message
    }



	
	
	public void update()
	{

        //create the gui
        if (Gui == null)
        {
            Gui = mManager.gameObject.AddComponent<AuthoringGuiBehaviour>();
            Gui.mTesting = this;
        }

        //update the character
		if(NGM.CurrentPose != null && mManager.mBodyManager.mFlat != null) //make sure a character is in fact loaded, this can apparently happen in testing scene.
			mManager.mBodyManager.set_target_pose(NGM.CurrentPose);
        else if(mManager.mZigManager.is_reader_connected() != 2) //via keyboard if there is no kinect
            mManager.mBodyManager.keyboard_update();
        if(NGM.CurrentPoseAnimation != null)
        {
            NGM.CurrentTargetPose = NGM.CurrentPoseAnimation.get_pose(Time.time);
            mManager.mTransparentBodyManager.set_target_pose(NGM.CurrentTargetPose);
            float grade = ProGrading.grade_pose(NGM.CurrentPose, NGM.CurrentTargetPose);
            grade = ProGrading.grade_to_perfect(grade);
            //TODO do something with grade
        }

		//if we are annoyed by the pose..
		if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if(NGM.CurrentPoseAnimation == null)
                NGM.CurrentPoseAnimation = new PerformanceType(mCurrentPoseAnimation,mLastPoseMode);
            else 
                NGM.CurrentPoseAnimation = null;
        }
		
		//TODO DELETE
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
		
		//switch to cutscene, note, this hides all the character stuff, not disable it
		if(Input.GetKeyDown(KeyCode.Alpha5))
		{
			if(NGM.CurrentCharacterLoader.has_cutscene(mLastCutscene))
			{
				mManager.mBodyManager.transition_character_out();
				mManager.mTransparentBodyManager.transition_character_out();
				mManager.mBackgroundManager.load_cutscene(mLastCutscene,NGM.CurrentCharacterLoader);

				//cutscene music
				if(mLastCutscene == 1)
				{
					mManager.mMusicManager.play_sound_effect("cutBad");
					mManager.mMusicManager.play_cutscene_music(NGM.CurrentCharacterLoader.Images.cutsceneMusic[0]);
				} else if(mLastCutscene == 0)
				{
					mManager.mMusicManager.play_sound_effect("cutGood");
					mManager.mMusicManager.play_cutscene_music(NGM.CurrentCharacterLoader.Images.cutsceneMusic[1]);
				}
				else if(mLastCutscene == 4)
				{
					mManager.mMusicManager.play_sound_effect("cutDie");
					mManager.mMusicManager.play_cutscene_music(NGM.CurrentCharacterLoader.Images.deathMusic);
				}


				ManagerManager.Manager.mDebugString = "loaded cutscene " + mLastCutscene;
				mManager.mDebugString = "loaded cutscene " + mLastCutscene;
			}
			else mManager.mDebugString = "missing cutscene " + mLastCutscene;
			mLastCutscene = (mLastCutscene+1) % 5;
		}
		
        //TODO DELETE
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
		
        //TODO DELETE
		if(Input.GetKeyDown(KeyCode.Alpha8))
		{
			mManager.mDebugString = "Loaded poses for difficulty " + ((++mLastDiff)%4);
			NGM.CurrentPoseAnimation = new PerformanceType(mManager.mCharacterBundleManager.get_pose(NGM.CurrentCharacterIndex,mLastDiff%4), new CharacterIndex(2,0)); //forces it to be switch
			NGM.CurrentPoseAnimation.set_change_time(GameConstants.difficultyToChangeTime[mLastDiff%4]);
		}
		
		
		//TODO DELETE
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
					if(!(NGM.CurrentCharacterIndex.LevelIndex == 1 && choice != 0)) //if we are level 1 make sure we don't go back to an invalid character
						mManager.mAssetLoader.new_load_character(NGM.CurrentCharacterIndex.get_past_neighbor(choice).StringIdentifier,mManager.mCharacterBundleManager);
			}
			else
			{
				if(NGM.CurrentCharacterIndex.LevelIndex < 7)
					mManager.mAssetLoader.new_load_character(NGM.CurrentCharacterIndex.get_future_neighbor(choice).StringIdentifier,mManager.mCharacterBundleManager);
			}
		}
	}
}
