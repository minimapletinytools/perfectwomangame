using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;

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
    string mPrompt = "hi";
    bool mFlashing = false;
    bool mChanging = false;
    float mPerfect = 0.5f;


    TimedEventDistributor mEvents;

    PDCharacterStats get_character()
    {
        return mStats[mCurrentState];
    }

    int get_index(int age, int group)
    {
        return age * 4 + group;
    }

    int get_age(int index)
    {
        return index / 4;
    }

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
        compute_next_difficulties();
        stop_flashing();
        mPlayer.set_stats();
        foreach (PDCharacterStats e in mStats)
        {
            e.generate_adjustment_values_from_difficulty_values();
        }
    }

    void compute_next_difficulties()
    {
        for (int i = 0; i < 7; i++)
        {
            int[] answer = mPlayer.difficulty_relative(new PDCharacterStats[] { mStats[4 * i + 0], mStats[4 * i + 1], mStats[4 * i + 2], mStats[4 * i + 3] });
            for (int j = 0; j < 4; j++)
            {
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
        PDInstance ins = mInstances[get_index(age,group)];
        if (ins.Changed && mFlashing)
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
        mPerfect = GUI.HorizontalSlider(new Rect(100, 100, 200, 20), mPerfect, 0, 1);
        GUI.TextField(new Rect(100, 150, 500, 50), mPrompt);

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
        if (mChanging)
            return;
        Debug.Log("advancing " + choice);
        compute_next_difficulties();
        mCurrentState = get_age(mCurrentState) + 1 + choice;
        
        TimedEventDistributor.TimedEventChain chain =  mEvents.add_event(
            delegate(float time)
            {
                mChanging = true;
                this.mPrompt = "your life was womp womp";
                if (time > 2)
                {
                    return true;
                }
                return false;
            },
            0);
        
        //TODO foreach stat
        foreach (PDStats.Stats e in PDStats.EnumerableStats)
        {
            PDStats.Stats loopStat = e;
            mPlayer.change_stats(get_character(), mPerfect, e);
            int change = mPlayer.get_change(e);
            if(change == 0) 
                continue;
            chain = chain.then(
                delegate(float time)
                {
                    //TODO should pick random out of list
                    this.mPrompt = change > 0 ? PDStats.positive_sentences[(int)loopStat][0] : PDStats.negative_sentences[(int)loopStat][0];
                    mPlayer.change_stats(get_character(), mPerfect, loopStat);
                    compute_next_difficulties();
                    return true;
                },
                0).then(
                delegate(float time)
                {
                    Debug.Log("flash on " + loopStat);
                    start_flashing();
                    return true;
                },
                0).then(
                delegate(float time)
                {
                    Debug.Log("stop flash on " + loopStat);
                    stop_flashing();
                    if(time > 1)
                        return true;
                    return false;
                },
                2);
        }
        chain.then_one_shot(
            delegate() 
            {
                this.mPrompt = "you are now age " + get_age(mCurrentState);
                mChanging = false;
                mPlayer.set_stats();
            });
        //this is a hack but we need to reset the NextStats in player so the delegates do what they need to do
         Dictionary<PDStats.Stats, float> keys = new Dictionary<PDStats.Stats,float>(mPlayer.NextValues);
        foreach(PDStats.Stats e in keys.Keys)
            mPlayer.NextValues[e] = mPlayer.Values[e];
    }



    
    //for events
    void start_flashing()
    {
        mFlashing = true;
    }
    void stop_flashing()
    {
        foreach (PDInstance e in mInstances)
        {
            e.Difficulty = e.NextDifficulty;
        }
        mFlashing = false;
    }

}
