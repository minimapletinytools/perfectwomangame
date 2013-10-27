using UnityEngine;
using System.Collections.Generic;

public class PDCharacterStats 
{
    public class Adjustment
    {
        public float l,r,m;
        public Adjustment() { l = r = m = 0; }
        public Adjustment(float al, float ar, float am) { l = al; r = ar; m = am; }
        public Adjustment(float ar) { l = 0; r = ar; m = 0; }
    }
	
    Dictionary<PDStats.Stats, float> mDifficultyValues = new Dictionary<PDStats.Stats, float>()
    {
        {PDStats.Stats.EDUCATION,0},
        {PDStats.Stats.EXPRESSION,0},
        {PDStats.Stats.FAMILY,0},
        {PDStats.Stats.HEALTH,0},
        {PDStats.Stats.MONEY,0},
        {PDStats.Stats.RESPECT,0},
        {PDStats.Stats.ROMANCE,0},
        {PDStats.Stats.WISDOM,0}
    };

    Dictionary<PDStats.Stats, Adjustment> mAdjustmentValues = new Dictionary<PDStats.Stats, Adjustment>()
    {
        {PDStats.Stats.EDUCATION, new Adjustment(0,0,0)},
        {PDStats.Stats.EXPRESSION,new Adjustment(0,0,0)},
        {PDStats.Stats.FAMILY,new Adjustment(0,0,0)},
        {PDStats.Stats.HEALTH,new Adjustment(0,0,0)},
        {PDStats.Stats.MONEY,new Adjustment(0,0,0)},
        {PDStats.Stats.RESPECT,new Adjustment(0,0,0)},
        {PDStats.Stats.ROMANCE,new Adjustment(0,0,0)},
        {PDStats.Stats.WISDOM,new Adjustment(0,0,0)}

    };


    public Dictionary<PDStats.Stats, Adjustment> AdjustmentValues { get { return mAdjustmentValues; } }
    public Dictionary<PDStats.Stats, float> DifficultyValues { get { return mDifficultyValues; } }

    public PDCharacterStats(float[] aValues = null)
    {
        if (aValues != null)
        {
            mDifficultyValues[PDStats.Stats.EDUCATION] = aValues[0];
            mDifficultyValues[PDStats.Stats.EXPRESSION] = aValues[1];
            mDifficultyValues[PDStats.Stats.FAMILY] = aValues[2];
            mDifficultyValues[PDStats.Stats.HEALTH] = aValues[3];
            mDifficultyValues[PDStats.Stats.MONEY] = aValues[4];
            mDifficultyValues[PDStats.Stats.RESPECT] = aValues[5];
            mDifficultyValues[PDStats.Stats.ROMANCE] = aValues[6];
            mDifficultyValues[PDStats.Stats.WISDOM] = aValues[7];
        }

            ADJUST_EDUCATION = new PDCharacterStats.Adjustment(0);
            ADJUST_EXPRESSION = new PDCharacterStats.Adjustment(0);
            ADJUST_FAMILY = new PDCharacterStats.Adjustment(0);
            ADJUST_HEALTH = new PDCharacterStats.Adjustment(0);
            ADJUST_MONEY = new PDCharacterStats.Adjustment(0);
            ADJUST_RESPECT = new PDCharacterStats.Adjustment(0);
            ADJUST_ROMANCE = new PDCharacterStats.Adjustment(0);
            ADJUST_WISDOM = new PDCharacterStats.Adjustment(0);
    }

    public float EDUCATION { get { return mDifficultyValues[PDStats.Stats.EDUCATION]; } set { mDifficultyValues[PDStats.Stats.EDUCATION] = value; } }
    public float EXPRESSION { get { return mDifficultyValues[PDStats.Stats.EXPRESSION]; } set { mDifficultyValues[PDStats.Stats.EXPRESSION] = value; } }
    public float FAMILY { get { return mDifficultyValues[PDStats.Stats.FAMILY]; } set { mDifficultyValues[PDStats.Stats.FAMILY] = value; } }
    public float HEALTH { get { return mDifficultyValues[PDStats.Stats.HEALTH]; } set { mDifficultyValues[PDStats.Stats.HEALTH] = value; } }
    public float MONEY { get { return mDifficultyValues[PDStats.Stats.MONEY]; } set { mDifficultyValues[PDStats.Stats.MONEY] = value; } }
    public float RESPECT { get { return mDifficultyValues[PDStats.Stats.RESPECT]; } set { mDifficultyValues[PDStats.Stats.RESPECT] = value; } }
    public float ROMANCE { get { return mDifficultyValues[PDStats.Stats.ROMANCE]; } set { mDifficultyValues[PDStats.Stats.ROMANCE] = value; } }
    public float WISDOM { get { return mDifficultyValues[PDStats.Stats.WISDOM]; } set { mDifficultyValues[PDStats.Stats.WISDOM] = value; } }


    public Adjustment ADJUST_EDUCATION { get { return mAdjustmentValues[PDStats.Stats.EDUCATION]; } set { mAdjustmentValues[PDStats.Stats.EDUCATION] = value; } }
    public Adjustment ADJUST_EXPRESSION { get { return mAdjustmentValues[PDStats.Stats.EXPRESSION]; } set { mAdjustmentValues[PDStats.Stats.EXPRESSION] = value; } }
    public Adjustment ADJUST_FAMILY { get { return mAdjustmentValues[PDStats.Stats.FAMILY]; } set { mAdjustmentValues[PDStats.Stats.FAMILY] = value; } }
    public Adjustment ADJUST_HEALTH { get { return mAdjustmentValues[PDStats.Stats.HEALTH]; } set { mAdjustmentValues[PDStats.Stats.HEALTH] = value; } }
    public Adjustment ADJUST_MONEY { get { return mAdjustmentValues[PDStats.Stats.MONEY]; } set { mAdjustmentValues[PDStats.Stats.MONEY] = value; } }
    public Adjustment ADJUST_RESPECT { get { return mAdjustmentValues[PDStats.Stats.RESPECT]; } set { mAdjustmentValues[PDStats.Stats.RESPECT] = value; } }
    public Adjustment ADJUST_ROMANCE { get { return mAdjustmentValues[PDStats.Stats.ROMANCE]; } set { mAdjustmentValues[PDStats.Stats.ROMANCE] = value; } }
    public Adjustment ADJUST_WISDOM { get { return mAdjustmentValues[PDStats.Stats.WISDOM]; } set { mAdjustmentValues[PDStats.Stats.WISDOM] = value; } }

    public string Title { get; set; }

    public void generate_adjustment_values_from_difficulty_values()
    {
        foreach (KeyValuePair<PDStats.Stats, float> e in mDifficultyValues)
        {
            mAdjustmentValues[e.Key] = new Adjustment(0, e.Value, 0);
        }
    }

    public void randomize_all_stats()
    {

        var keys = new List<PDStats.Stats>(mAdjustmentValues.Keys);
        foreach (var e in keys)
        {
            if (Random.Range(0f, 1f) < 0.5f)
                mAdjustmentValues[e] = new Adjustment(0, Random.Range(0f, 5f), 0);
            else
                mAdjustmentValues[e] = new Adjustment();
        }

        foreach (var e in keys)
        {
            if (Random.Range(0f, 1f) < 0.5f)
                mDifficultyValues[e] = Random.Range(0f, 5f);
            else
                mDifficultyValues[e] = 0;
        }

    }
}
