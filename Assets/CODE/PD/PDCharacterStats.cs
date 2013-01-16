using UnityEngine;
using System.Collections.Generic;

public class PDCharacterStats 
{
    Dictionary<PDStats.Stats, float> mDifficultyValues = new Dictionary<PDStats.Stats, float>()
    {
        {PDStats.Stats.EDUCATION,0},
        {PDStats.Stats.EXPRESSION,0},
        {PDStats.Stats.HEALTH,0},
        {PDStats.Stats.MONEY,0},
        {PDStats.Stats.RESPECT,0},
        {PDStats.Stats.ROMANCE,0},
        {PDStats.Stats.WISDOM,0}
    };

    public Dictionary<PDStats.Stats, float> DifficultyValues { get { return mDifficultyValues; } }

    public PDCharacterStats(float[] aValues)
    {
        mDifficultyValues[PDStats.Stats.EDUCATION] = aValues[0];
        mDifficultyValues[PDStats.Stats.EXPRESSION] = aValues[1];
        mDifficultyValues[PDStats.Stats.HEALTH] = aValues[2];
        mDifficultyValues[PDStats.Stats.MONEY] = aValues[3];
        mDifficultyValues[PDStats.Stats.RESPECT] = aValues[4];
        mDifficultyValues[PDStats.Stats.ROMANCE] = aValues[5];
        mDifficultyValues[PDStats.Stats.WISDOM] = aValues[6];
    }

    //public static 
}
