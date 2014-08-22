using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class AuthoringGuiBehaviour : MonoBehaviour {

    public ModeTesting mTesting;


    string charText = "0 1";
    int saveDiff = 0;
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
        output += "CHAR: " + mTesting.NGM.CurrentCharacterIndex.FullName;
        output += "\nMODE: " + ((mTesting.NGM.CurrentPose != null) ? "KINECT" : "MANUAL");
        //TODO
        output += "\nPLAYING: " + "TODO";
        //TODO other stuff
        GUIStyle style = new GUIStyle();
        GUI.TextArea(new Rect(padding, leftTop, infoSize,infoSize), output, style);
        leftTop += infoSize + padding;

        //save difficulty
        if (GUI.Button(new Rect(10, leftTop, shortButWidth, butHeight), "DIFF " + saveDiff))
            saveDiff = (saveDiff + 1) % 4;
        leftTop += butHeight + padding;
        if (GUI.Button(new Rect(10, leftTop, shortButWidth, butHeight), "SAVE"))
        {
            //TODO
        }
        leftTop += butHeight + padding;
        //TODO  SPEEd/MODE/GRADING
        //if (GUI.Button(new Rect(0, leftTop, 150, 100), "SPEED"))



        //TODO button display
        charText = GUI.TextArea(new Rect(Screen.width - longButWidth - padding*2 - 50, rightTop, 50, butHeight), charText);
        if (GUI.Button(new Rect(Screen.width - longButWidth - padding, rightTop, longButWidth, butHeight), "Choose Character"))
        {
            int[] split = charText.Split(' ').Select(e=>int.Parse(e)).ToArray();
            if(split.Count() > 2)
            {
                CharacterIndex next = new CharacterIndex(split[0],split[1]);
                if(CharacterIndex.sAllCharacters.Contains(next) && next.LevelIndex < 9) //if it's a real character and not the grave
                {
                    //TODO 
                    //mTesting.
                }
            }
        }
        rightTop += butHeight + padding;

        if (GUI.Button(new Rect(Screen.width - longButWidth - padding, rightTop, longButWidth, butHeight), "Load Default Poses"))
        {
            //TODO
        }

        rightTop += butHeight + padding;

        if (GUI.Button(new Rect(Screen.width - longButWidth - padding, rightTop, longButWidth, butHeight), "Load Working Poses"))
        {
            //TODO
        }

        rightTop += butHeight + padding;

        //TODO go through each pose
        //foreach()
        {
        }

        if (GUI.Button(new Rect(Screen.width - shortButWidth - padding, rightTop, shortButWidth, butHeight), "NEW POSE"))
        {
            //TODO
        }


    }


}
