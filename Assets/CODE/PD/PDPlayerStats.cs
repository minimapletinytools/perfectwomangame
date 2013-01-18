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

    public Dictionary<PDStats.Stats, float> Values { get { return mValues; } }



    

    public float grade_absolute(PDCharacterStats aStat)
    {
        //TODO 
        return 0;
    }
    public int difficulty_absolute(PDCharacterStats aStat)
    {
        //TODO 
        return 0;
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
        return r.OrderBy(x => orig[x]).ToArray(); //TODO is this correct?
    }
}
    