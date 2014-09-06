using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class AuthoringGuiBehaviour : MonoBehaviour {

    public ModeTesting mTesting;

    public string ErrorMessage{get;set;}

    string charText = "0 1";
    int saveDiff = 0;
    public bool useKinect = false;
    public void OnGUI()
    {
        int butHeight = 30;
        int longButWidth = 200;
        int shortButWidth = 100;
        int padding = 10;
        int infoSize = 300;


        float leftTop = padding;
        float rightTop = padding;

        string output = "";
        output += "CHAR: " + mTesting.NGM.CurrentCharacterIndex.StringIdentifier;
        //output += "\nMODE: " + ((mTesting.NGM.CurrentPose != null) ? "KINECT" : "MANUAL");
        //TODO
        //output += "\nPLAYING: " + "TODO";
        if (ErrorMessage != "")
            output += "\n" + ErrorMessage;
        //TODO other stuff
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        GUI.TextArea(new Rect(padding, leftTop, infoSize,infoSize), output, style);
        leftTop += infoSize + padding;

        //save difficulty
        if (GUI.Button(new Rect(10, leftTop, shortButWidth, butHeight), "MAN: " + !useKinect))
            useKinect = !useKinect;
        leftTop += butHeight + padding;
        if (GUI.Button(new Rect(10, leftTop, shortButWidth, butHeight), "DIFF " + saveDiff))
            saveDiff = (saveDiff + 1) % 4;
        leftTop += butHeight + padding;
        if (GUI.Button(new Rect(10, leftTop, longButWidth, butHeight), "WRITE TO FILE"))
        {
            mTesting.write_poses_to_folder(mTesting.NGM.CurrentCharacterIndex,saveDiff);
        }
        leftTop += butHeight + padding;
        //TODO  SPEEd/MODE/GRADING
        //if (GUI.Button(new Rect(0, leftTop, 150, 100), "SPEED"))


        charText = GUI.TextArea(new Rect(Screen.width - longButWidth - padding*2 - 50, rightTop, 50, butHeight), charText);
        if (GUI.Button(new Rect(Screen.width - longButWidth - padding, rightTop, longButWidth, butHeight), "Choose Character"))
        {
            try{
                int[] split = charText.Split(' ').Select(e=>int.Parse(e)).ToArray();
                if(split.Count() == 2)
                {
                    CharacterIndex next = new CharacterIndex(split[0],split[1]);
                    mTesting.load_character(next);
                }
            }
            catch{ErrorMessage = "ERROR: character choice is not formatted correctly";}
        }
        rightTop += butHeight + padding;

        if (GUI.Button(new Rect(Screen.width - longButWidth - padding, rightTop, longButWidth, butHeight), "Load Default Poses"))
        {
            mTesting.load_char_default_poses(mTesting.NGM.CurrentCharacterIndex,saveDiff);
        }

        rightTop += butHeight + padding;

        if (GUI.Button(new Rect(Screen.width - longButWidth - padding, rightTop, longButWidth, butHeight), "Load Saved Poses"))
        {
            mTesting.load_char_from_folder(mTesting.NGM.CurrentCharacterIndex,saveDiff);
        }

        rightTop += butHeight + padding;


        var rightShortX = Screen.width - shortButWidth - padding;
        if (mTesting.mCurrentPoseAnimation != null)
        {
            for(int i = 0; i < mTesting.mCurrentPoseAnimation.poses.Count; i++)
            {
                if(i == mTesting.mCurrentPoseIndex)
                {
                    GUI.Label(new Rect(rightShortX, rightTop, shortButWidth, butHeight), "POSE: " + i);
                    if(GUI.Button(new Rect(rightShortX - shortButWidth*2 - padding*2,rightTop,shortButWidth,butHeight),"DELETE"))
                    {
                        mTesting.mCurrentPoseAnimation.poses.RemoveAt(mTesting.mCurrentPoseIndex);
                        if(mTesting.mCurrentPoseAnimation.poses.Count > 0)
                            mTesting.set_pose_index(Mathf.Clamp(mTesting.mCurrentPoseIndex-1,0,9999999));
                        else
                        {
                            mTesting.mCurrentPoseAnimation = null;
                            mTesting.mCurrentPoseIndex = 0;
                            break;
                        }
                    }
                    if(GUI.Button(new Rect(rightShortX - shortButWidth - padding,rightTop,shortButWidth,butHeight),"SAVE"))
                        mTesting.mCurrentPoseAnimation.poses[mTesting.mCurrentPoseIndex] = mTesting.NGM.mManager.mBodyManager.get_current_pose();
                }
                else if (GUI.Button(new Rect(rightShortX, rightTop, shortButWidth, butHeight), "POSE: " + i))
                    mTesting.set_pose_index(i);
                rightTop += butHeight + padding;
            }
        }
        if (GUI.Button(new Rect(rightShortX, rightTop, shortButWidth, butHeight), "NEW POSE"))
        {
            if(mTesting.mCurrentPoseAnimation == null)
                mTesting.mCurrentPoseAnimation = new PoseAnimation();
            mTesting.mCurrentPoseAnimation.poses.Add(mTesting.NGM.mManager.mBodyManager.get_current_pose());
            mTesting.set_pose_index(mTesting.mCurrentPoseAnimation.poses.Count -1);
        }


    }


}
