using UnityEngine;
using System.Collections;

public interface SetChoiceInterface 
{
	void set_choice(int aChoice);
	void set_choice_percentages(int aChoice, float aPercentage);
}

public class SetPlayChoice : SetChoiceInterface
{
	ChoosingManager CM {get; set;}
	public SetPlayChoice(ChoosingManager aCM)
	{
		CM = aCM;
	}
	public void set_choice(int aChoice)
	{
		CM.set_bb_choice(aChoice);
	}
	public void set_choice_percentages(int aChoice, float aPercentage)
	{
		CM.set_bb_choice_percentages(aChoice,aPercentage);
	}
}

public class SetChallengeChoice : SetChoiceInterface
{
    ChallengeMenu CM {get; set;}
    public SetChallengeChoice(ChallengeMenu aMenu)
    {
        CM = aMenu;
    }
    public void set_choice(int aChoice)
    {
        //TODO
        //CM.set_bb_choice(aChoice);
    }
    public void set_choice_percentages(int aChoice, float aPercentage)
    {
        //TODO
        //CM.set_bb_choice_percentages(aChoice,aPercentage);
    }
}
