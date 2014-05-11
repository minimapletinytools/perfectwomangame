using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class ChoiceHelper
{
	public const float SELECTION_THRESHOLD = 8;
    public const float CHOOSING_PERCENTAGE_GROWTH_RATE = 1/6f;
    public const float CHOOSING_PERCENTAGE_DECLINE_RATE = 0.7f;
	
	
    Pose[] mChoicePoses = null;
	Pose[] mPossibleChoicePoses; //we randomly choose poses from here to populate mChoicePoses
	
	float[] ChoosingPercentages
    { get; set; }
	int NextContendingChoice
    { get; set; }
	int LastContendingChoice
	{ get; set; }
	public Pose CurrentPose
    { get; set; }
	
	public ChoiceHelper()
	{
		load_possible_choice_poses();
	}
	
	//
	public void load_possible_choice_poses()
	{
		mPossibleChoicePoses = new Pose[ManagerManager.Manager.mReferences.mPossiblePoses.Length];
        for (int i = 0; i < mPossibleChoicePoses.Length; i++)
        { mPossibleChoicePoses[i] = ProGrading.read_pose(ManagerManager.Manager.mReferences.mPossiblePoses[i]); }
	}

	public void shuffle_and_set_choice_poses(int aCount, ChoosingManager aChoosing)
	{
		//reset the choosing percentages from last round
		mChoicePoses = get_random_possible_poses(aCount);
		ChoosingPercentages = new float[aCount];
		for(int j = 0; j < ChoosingPercentages.Length; j++)
			ChoosingPercentages[j] = 0;
		aChoosing.set_bb_choice_poses(mChoicePoses.ToList());
		
	}
	
	//returns choice
	public int update(SetChoiceInterface aInterface)
	{
		if(mChoicePoses == null || mChoicePoses.Length == 0)
			throw new UnityException("problem with choice poses");
		
		int minIndex = 0;
        float minGrade = 99999;
        for (int i = 0; i < mChoicePoses.Length; i++) //TODO need sto be 4 eventually....
        {
            if (mChoicePoses[i] != null)
            {
				float grade = 9999999; //important that there are more 9s here than above!
				if(CurrentPose != null && mChoicePoses[i] != null)
					grade = ProGrading.grade_pose(CurrentPose, mChoicePoses[i]);
                if (grade < minGrade)
                {
                    minGrade = grade;
                    minIndex = i;
                }
            }
        }
		
        //Debug.Log(output);
        if (minGrade > SELECTION_THRESHOLD)
        {
            NextContendingChoice = -1;
        }
        else
        {
            NextContendingChoice = minIndex;
        }
	
		float growthRate = CHOOSING_PERCENTAGE_GROWTH_RATE;
		
		//hack choice testing
		if(Input.GetKey(KeyCode.Alpha1))
		{
			NextContendingChoice = 0;
			growthRate = 1;
		}
		else if(Input.GetKey(KeyCode.Alpha2))
		{
			NextContendingChoice = 1;
			growthRate = 1;
		}
		else if(Input.GetKey(KeyCode.Alpha3))
		{
			NextContendingChoice = 2;
			growthRate = 1;
		}
		else if(Input.GetKey(KeyCode.Alpha4))
		{
			NextContendingChoice = 3;
			growthRate = 1;
		}
		//else if(Input.GetKey(KeyCode.Alpha4))
		//	NextContendingChoice = 3;
		
		if(NextContendingChoice != -1 && LastContendingChoice != NextContendingChoice)
		{
			ManagerManager.Manager.mMusicManager.play_sound_effect("choiceBlip");
		}
		
		aInterface.set_choice(NextContendingChoice);
		
        for (int i = 0; i < mChoicePoses.Length; i++)
        {
            if (NextContendingChoice == i)
            {
                ChoosingPercentages[i] = Mathf.Clamp01(ChoosingPercentages[i] + growthRate * Time.deltaTime);
            }
            else
            {
                ChoosingPercentages[i] = Mathf.Clamp01(ChoosingPercentages[i] - CHOOSING_PERCENTAGE_DECLINE_RATE * Time.deltaTime);
            }
			aInterface.set_choice_percentages(i,ChoosingPercentages[i]);
            if (ChoosingPercentages[i] == 1)
            {
				for(int j = 0; j < ChoosingPercentages.Length; j++)
					ChoosingPercentages[j] = 0;
				LastContendingChoice = -1;
				ManagerManager.Manager.mMusicManager.play_sound_effect("choiceMade");
				return NextContendingChoice;
            }
        }
		LastContendingChoice = NextContendingChoice;
        
		return -1;
	}
	

    //TODO move to some util file instead
    public static void Shuffle<T>(T[] array)
    {
        for (int i = array.Length; i > 1; i--)
        {
            // Pick random element to swap.
            int j = Random.Range(0, i - 1); // 0 <= j <= i-1
            // Swap.
            T tmp = array[j];
            array[j] = array[i - 1];
            array[i - 1] = tmp;
        }
    }
    Pose[] get_random_possible_poses(int number)
    {
        Pose[] r = new Pose[number];
        Shuffle<Pose>(mPossibleChoicePoses);
        for (int i = 0; i < number; i++)
            r[i] = mPossibleChoicePoses[i];
        return r;
    }
}
