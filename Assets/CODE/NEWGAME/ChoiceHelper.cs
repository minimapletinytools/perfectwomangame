using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class ChoiceHelper
{
	public const float SELECTION_THRESHOLD = 8;
    public const float CHOOSING_PERCENTAGE_GROWTH_RATE = 1/6f;
    public const float CHOOSING_PERCENTAGE_DECLINE_RATE = 0.7f;
	
	
    Pose[] mChoicePoses = new Pose[4] { null, null, null, null };
	Pose[] mPossibleChoicePoses;
	
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
		load_choice_poses();
		ChoosingPercentages = new float[4]{0,0,0,0};
	}
	
	public void load_choice_poses()
	{
		mPossibleChoicePoses = new Pose[ManagerManager.Manager.mReferences.mPossiblePoses.Length];
        for (int i = 0; i < mPossibleChoicePoses.Length; i++)
        { mPossibleChoicePoses[i] = ProGrading.read_pose(ManagerManager.Manager.mReferences.mPossiblePoses[i]); }
	}
	
	public void shuffle_and_set_choice_poses(NewInterfaceManager aInterface)
	{
		//reset the choosing percentages from last round
		ChoosingPercentages = new float[4]{0,0,0,0};
		mChoicePoses = get_random_possible_poses();
		aInterface.set_bb_choice_poses(mChoicePoses.ToList());
		
	}
	
	//returns choice
	public int update(NewInterfaceManager aInterface)
	{
		int minIndex = 0;
        float minGrade = 99999;
        for (int i = 0; i < 4; i++)
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
		//else if(Input.GetKey(KeyCode.Alpha4))
		//	NextContendingChoice = 3;
		
		if(NextContendingChoice != -1 && LastContendingChoice != NextContendingChoice)
		{
			ManagerManager.Manager.mMusicManager.play_sound_effect("choiceBlip");
		}
		
		aInterface.set_bb_choice(NextContendingChoice);
		
        for (int i = 0; i < 4; i++)
        {
            if (NextContendingChoice == i)
            {
                ChoosingPercentages[i] = Mathf.Clamp01(ChoosingPercentages[i] + growthRate * Time.deltaTime);
            }
            else
            {
                ChoosingPercentages[i] = Mathf.Clamp01(ChoosingPercentages[i] - CHOOSING_PERCENTAGE_DECLINE_RATE * Time.deltaTime);
            }
			aInterface.set_bb_choice_percentages(i,ChoosingPercentages[i]);
            if (ChoosingPercentages[i] == 1)
            {
                //choice is made!!!
				LastContendingChoice = -1;
				ManagerManager.Manager.mMusicManager.play_sound_effect("choiceMade");
				return NextContendingChoice;
            }
        }
		LastContendingChoice = NextContendingChoice;
        
		return -1;
	}
	
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
    Pose[] get_random_possible_poses()
    {
        Pose[] r = new Pose[4];
        Shuffle<Pose>(mPossibleChoicePoses);
        for (int i = 0; i < 4; i++)
            r[i] = mPossibleChoicePoses[i];
        return r;
    }
}
