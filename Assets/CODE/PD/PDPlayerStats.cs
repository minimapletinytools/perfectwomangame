using UnityEngine;
using System.Collections.Generic;

public class PDPlayerstats 
{
    Dictionary<PDStats.Stats, float> mValues = new Dictionary<PDStats.Stats, float>()
    {
        {PDStats.Stats.EDUCATION,0},
        {PDStats.Stats.EXPRESSION,0},
        {PDStats.Stats.HEALTH,0},
        {PDStats.Stats.MONEY,0},
        {PDStats.Stats.RESPECT,0},
        {PDStats.Stats.ROMANCE,0},
        {PDStats.Stats.WISDOM,0}
    };

    public Dictionary<PDStats.Stats, float> Values { get { return mValues; } }


}
    