using UnityEngine;
using System.Collections;
using System;
using System.Text;

//put me on a camera game object with gui 
public class PDTester : MonoBehaviour {

    public class PDInstance
    {
        public int Difficulty { get; set; }
        public int NextDifficulty { get; set; }
        public bool Changed { get { return Difficulty != NextDifficulty; } }
        public int Perfectness { get; set; }
    }


    PDManager mP;
    int mCurrentState = 0;
    PDCharacterStats[] mStats;
    PDInstance[] mInstances;
    PDPlayerstats mPlayer;


    TimedEventDistributor mEvents;

    void Start()
    {
        mP = new PDManager(null);
        mEvents = new TimedEventDistributor();
        mPlayer = new PDPlayerstats();
        mStats = PDCharacters.characters;
        mInstances = new PDInstance[28];
        for (int i = 0; i < 28; i++)
        {
         
            mInstances[i] = new PDInstance();
            
        }
        for (int i = 0; i < 7; i++)
        {
            int[] answer = mPlayer.difficulty_relative(new PDCharacterStats[] { mStats[4 * i + 0], mStats[4 * i + 1], mStats[4 * i + 2], mStats[4 * i + 3] });
            for (int j = 0; j < 4; j++)
            {
                mInstances[i * 4 + j].Difficulty = answer[j];
                mInstances[i * 4 + j].NextDifficulty = answer[j];
            }
        }
    }


    void Update()
    {
        mEvents.update(Time.deltaTime);
	}

    string get_display_string(int age, int group)
    {
        PDInstance ins = mInstances[age * 4 + group];
        if (ins.Changed)
            return random_string();
        return ins.Difficulty.ToString();
    }
    string random_string()
    {
        StringBuilder builder = new StringBuilder();
        System.Random random = new System.Random();
        char ch;
        for (int i = 0; i < 15; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
            builder.Append(ch);
        }

        return builder.ToString();
    }
    void OnGUI()
    {

        Vector2 GRID_OFFSET = new Vector2(100,250);
        Vector2 GRID_SIZE = new Vector2(50,50);
        Vector2 GRID_SPACING = new Vector2(75,75);
        for (int i = 0; i < 4; i++)
            if (GUI.Button(new Rect(100 + i * GRID_SPACING.x, 50, GRID_SIZE.x, GRID_SIZE.y), i.ToString()))
                advance(i);

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GUI.TextArea(new Rect(GRID_OFFSET.x + j * GRID_SPACING.x, GRID_OFFSET.y + i * GRID_SPACING.y, GRID_SIZE.x, GRID_SIZE.y), get_display_string(i,j));
            }
        }

        //poo poo
        //GUI.TextArea(new Rect(50, 150, Screen.width - 100, Screen.height - 250), "nonsense");
    }

    void advance(int choice)
    {

    }



    //for events
    void stop_flashing()
    {

    }

}
