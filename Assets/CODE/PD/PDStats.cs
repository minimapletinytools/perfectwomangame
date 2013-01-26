using UnityEngine;
using System.Collections.Generic;

public class PDStats
{
    public enum Stats
    {
        EDUCATION = 0,
        EXPRESSION,
        FAMILY,
        HEALTH,
        MONEY,
        RESPECT,
        ROMANCE,
        WISDOM
    }

    public static Stats[] EnumerableStats = new Stats[] { Stats.EDUCATION, Stats.EXPRESSION, Stats.FAMILY, Stats.HEALTH, Stats.MONEY, Stats.RESPECT, Stats.ROMANCE, Stats.WISDOM };

    public static List<string[]> positive_sentences = new List<string[]>()
    {
        new string[]{"you became more educated"},
        new string[]{"you made an independent video game", "you made progress on your artistic endeavors"},
        new string[]{"you grew closer with your family", "you developed strong relationships"},
        new string[]{"you had time to exercise","you ate nutritiously"},
        new string[]{"you made a lot of money", "you made A LOT of money"},
        new string[]{"you earned the respect of your peers"},
        new string[]{"you had a fantastic sex life", "you were very satisfied romantically"},
        new string[]{"you became more cultured", "you gained life experiences"}
    };

    public static List<string[]> negative_sentences = new List<string[]>()
    {
        new string[]{"you forgot a lot of what you learned"},
        new string[]{"your creativity was stifled"},
        new string[]{"you grew apart from your family"},
        new string[]{"you had a stressful life", "your health declined"},
        new string[]{"you made some poor investments","you lost your savings"},
        new string[]{"you did shameful things"},
        new string[]{"you barely had sex", "you had a miserable sex life"},
        new string[]{"you did not learn from your mistakes"}
    };
}
