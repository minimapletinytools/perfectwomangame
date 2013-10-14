using UnityEngine;
using System.Collections;

public interface SetChoiceInterface 
{
	void set_choice(int aChoice);
	void set_choice_percentages(int aChoice, float aPercentage);
}

public class SetPlayChoice : SetChoiceInterface
{
	NewInterfaceManager NIM {get; set;}
	public SetPlayChoice(NewInterfaceManager aNim)
	{
		NIM = aNim;
	}
	public void set_choice(int aChoice)
	{
		NIM.set_bb_choice(aChoice);
	}
	public void set_choice_percentages(int aChoice, float aPercentage)
	{
		NIM.set_bb_choice_percentages(aChoice,aPercentage);
	}
}
