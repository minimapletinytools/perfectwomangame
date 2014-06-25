using UnityEngine;
using System.Collections;

public class ChallengeMenu 
{
    enum ChoiceMode{
        NONE,
        AGE,
        CHAR,
        DIFF,
        MODE
    }

    ChoiceMode mMode = ChoiceMode.NONE;
    ModeChallenge mManager;
    ChoiceHelper mChoiceHelper;

    public ChallengeMenu(ModeChallenge aManager)
    {
        mManager = aManager;
    }

    public void initialize()
    {
        mChoiceHelper = new ChoiceHelper();
    }

    public void set_age_select()
    {
        mMode = ChoiceMode.AGE;
    }

    public void set_char_select()
    {
        mMode = ChoiceMode.CHAR;
    }
}
