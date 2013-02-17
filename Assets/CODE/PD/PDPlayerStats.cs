using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PDPlayerstats 
{
    Dictionary<PDStats.Stats, float> mValues = new Dictionary<PDStats.Stats, float>()
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

    Dictionary<PDStats.Stats, float> mNextValues;

    public PDPlayerstats()
    {
        mNextValues = new Dictionary<PDStats.Stats, float>(mValues);
    }

    public Dictionary<PDStats.Stats, float> Values { get { return mValues; } }
    public Dictionary<PDStats.Stats, float> NextValues { get { return mNextValues; } }

    public int get_change(PDStats.Stats aStat)
    {
        float change = (NextValues[aStat] - Values[aStat]);
        //Debug.Log("change for stat " + aStat + " is " + NextValues[aStat] + " " + Values[aStat]);
        if (change > 0)
            return 1;
        else if (change < 0)
            return -1;
        else
            return 0;
    }

    public void change_stats(PDCharacterStats aStat, float aPerfect, PDStats.Stats aStatToChange)
    {
        //foreach(PDStats.Stats e in PDStats.EnumerableStats)
        PDStats.Stats e = aStatToChange;
        {
            PDCharacterStats.Adjustment adj = aStat.AdjustmentValues[e];
            if (aPerfect < adj.m)
            {
                float c = aStat.AdjustmentValues[e].l * (adj.m - aPerfect) / (adj.m);
                NextValues[e] += c;
                //Debug.Log("changed for stat " + e + " is " + c +  " for " + NextValues[e]);
            }
            if (aPerfect > adj.m)
            {
                float c = aStat.AdjustmentValues[e].r * (aPerfect - adj.m) / (1.0f - adj.m);
                NextValues[e] += c;
                //Debug.Log("changed for stat " + e + " is " + c + " for " + NextValues[e]);
            }
           
        }
    }

    public void set_stats()
    {
        foreach (var e in mNextValues)
        {
            mValues[e.Key] = e.Value;
        }
    }


	//negative is easy, positive is hard
    public float grade_absolute(PDCharacterStats aStat)
    {
        var mapDiffToChange = 
        //TODO
        //groups
        //both positive, both negative, opposite signs
        float r = 0;
		float sum = aStat.DifficultyValues.Values.Sum();
        foreach (var e in NextValues)
        {
            float cdv = aStat.DifficultyValues[e.Key];
            if (cdv == 0) //no effect when stat is zero
                continue;
			float pdv = e.Value;
			
			
            if (pdv < 0 && cdv < 0)
                if (pdv <= cdv)
                    r -= cdv / sum;
                else
                    r += 0;
            else if (pdv > 0 && cdv > 0)
                if (pdv >= cdv)
                    r -= cdv/sum;
                else
                    r += 0;
			else
				r += Mathf.Abs((pdv-cdv)/sum);
			return r;
        }

        return r;

        /* TODO delsete
        float r = 0;
        //grades against next values
        foreach (var e in NextValues)
        {
            r += e.Value * aStat.DifficultyValues[e.Key];
        }
        return r;*/
    }
    public int difficulty_absolute(PDCharacterStats aStat)
    {
        float g = grade_absolute(aStat);
		if(g <= -0.5f) 
			return 0;
		else if(g <= 0.25)
			return 1;
		else if(g < 1f)
			return 2;
		else
			return 3;
    }
    public int[] difficulty_absolute(PDCharacterStats[] aStats)
    {
        int[] r = new int[4] { 0, 1, 2, 3 };
        for (int i = 0; i < 4; i++)
            r[i] = difficulty_absolute(aStats[i]);
        return r;
    }
    public int[] difficulty_relative(PDCharacterStats[] aStats)
    {
        int[] r = new int[4]{0,1,2,3};
        float[] orig = new float[4];
        for (int i = 0; i < 4; i++)
            orig[i] = grade_absolute(aStats[i]);
        return r.OrderBy(x => orig[x]).ToArray(); 
    }
}
    