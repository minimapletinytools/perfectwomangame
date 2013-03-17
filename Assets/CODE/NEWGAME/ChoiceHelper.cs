using UnityEngine;
using System.Collections;

public class ChoiceHelper
{
	public const float SELECTION_THRESHOLD = 9;
    public const float CHOOSING_PERCENTAGE_GROWTH_RATE = 0.15f;
    public const float CHOOSING_PERCENTAGE_DECLINE_RATE = 1f;
	
	public float[] ChoosingPercentages
    { get; private set; }
	public int NextContendingChoice
    { get; private set; }
    public ProGrading.Pose[] mChoicePoses = new ProGrading.Pose[4] { null, null, null, null };
	ProGrading.Pose[] mPossibleChoicePoses;
	public ProGrading.Pose CurrentPose
    { get; private set; }
	
	public ChoiceHelper()
	{
		load_choice_poses();
		
		ChoosingPercentages = new float[4];
	}
	
	public void load_choice_poses()
	{
		mPossibleChoicePoses = new ProGrading.Pose[ManagerManager.Manager.mReferences.mPossiblePoses.Length];
        for (int i = 0; i < mPossibleChoicePoses.Length; i++)
        { mPossibleChoicePoses[i] = ProGrading.read_pose(ManagerManager.Manager.mReferences.mPossiblePoses[i]); }
	}
	
	//returns choice
	public int update(NewInterfaceManager aInterface)
	{
		int minIndex = 0;
        float minGrade = Mathf.Infinity;
        for (int i = 0; i < 4; i++)
        {
            if (mChoicePoses[i] != null)
            {
                float grade = ProGrading.grade_pose(CurrentPose, mChoicePoses[i]);
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
            NextContendingChoice = -1;//get_default_choice(CurrentLevel);
            //TODO NO CHOICE
			//update BB
			//aInterface.
        }
        else
        {
            NextContendingChoice = minIndex;
            //TODO made a choice
			//update BB
			//aInterface
        }

        for (int i = 0; i < 4; i++)
        {
            if (NextContendingChoice == i)
            {
                ChoosingPercentages[i] = Mathf.Clamp01(ChoosingPercentages[i] + CHOOSING_PERCENTAGE_GROWTH_RATE * Time.deltaTime);
            }
            else
            {
                ChoosingPercentages[i] = Mathf.Clamp01(ChoosingPercentages[i] - CHOOSING_PERCENTAGE_DECLINE_RATE * Time.deltaTime);
            }
            if (ChoosingPercentages[i] == 1)
            {
                //TODO choice is made!!!
				return NextContendingChoice;
            }
        }
		return -1;
	}
	
	//move this stuff elsewhere poo poo
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
    ProGrading.Pose[] get_random_possible_poses()
    {
        ProGrading.Pose[] r = new ProGrading.Pose[4];
        Shuffle<ProGrading.Pose>(mPossibleChoicePoses);
        for (int i = 0; i < 4; i++)
            r[i] = mPossibleChoicePoses[i];
        return r;
    }
}
