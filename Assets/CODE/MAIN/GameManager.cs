using UnityEngine;
using System.Collections.Generic;

public class GameManager : FakeMonoBehaviour
{
    public delegate bool GameEventDelegate(float time);
    public class TimedEventHandler
    {
        Dictionary<QuTimer, GameEventDelegate> mTimedEvents = new Dictionary<QuTimer, GameEventDelegate>();
        public void update(float aDeltaTime)
        {
            List<KeyValuePair<QuTimer, GameEventDelegate>> removal = new List<KeyValuePair<QuTimer, GameEventDelegate>>();
            foreach (KeyValuePair<QuTimer, GameEventDelegate> e in mTimedEvents)
            {
                e.Key.update(aDeltaTime);
                if (e.Key.isExpired())
                {
                    if (e.Value(aDeltaTime))
                        removal.Add(e);
                }
            }
            foreach (KeyValuePair<QuTimer, GameEventDelegate> e in removal)
                mTimedEvents.Remove(e.Key);
        }
        public void add_event(GameEventDelegate aEvent, float aTime)
        {
            mTimedEvents.Add(new QuTimer(0, aTime), aEvent);
        }
    }

    public const float LEVEL_TIME_TOTAL = 10;
    public const float SELECTION_THRESHOLD = 17;
    //public const float CHOICE_TIME = 10; //irrelevent
    public const float CHOOSING_PERCENTAGE_GROWTH_RATE = 0.15f;
    public const float CHOOSING_PERCENTAGE_DECLINE_RATE = 1f;

    public Camera mCamera;
    public AudioSource mSource;
    public int CurrentLevel
    { get; private set; }
    public int CurrentIndex
    { get; private set; }
    public float TotalScore
    { get; private set; }
    public float TimeRemaining
    { get; private set; }
    public float CurrentGrade
    { get; private set; }
    public bool IsLoading
    { get; private set; }

    public float[] ChoosingPercentages
    { get; private set; }


    public ProGrading.Pose CurrentPose
    { get; private set; }
    public int NextContendingChoice
    { get; private set; }

    float mMinStartTime = 0;
    public bool Started
    { get; private set; }
    public bool User
    { get; private set; }

    float[] mDifficulties = new float[29];
    public int[] PastChoices
    { get; private set; }

    int[] mPerfectness = new int[29]{ 0, 
            0, 1, 2, 3, 
            3, 2, 1, 0, 
            0, 1, 2, 3, 
            3, 2, 1, 0,
            0, 1, 2, 3, 
            3, 2, 1, 0, 
            2, 1, 3, 0 };

    ProGrading.Pose[] mPossiblePoses;

    ProGrading.Pose[] mChoicePoses = new ProGrading.Pose[4]{null,null,null,null};
    
    public TimedEventHandler mEvents = new TimedEventHandler();



    //choice and difficulty accessors
    public bool does_choice_exist(int index)
    {
        return mManager.mReferences.mCharacters[get_choice_index(index, CurrentLevel+1)] != null;
    }
    public int get_choice_index(int index, int level)
    {
        if (level == 0) return 0;
        if (level == 8) return -1;
        return index + (level-1) * 4 + 1;
    }
    public int get_level_from_choice_index(int index)
    {
        return (index + 3) / 4;
    }
    public int get_default_choice(int level)
    {
        if (level == 8)
            return -1;
        int r = -1;
        float minDifficulty = Mathf.Infinity;
        for (int i = 0; i < 4; i++)
        {
            if (minDifficulty > mDifficulties[i] && does_choice_exist(i)) //we check if the pose actually exists or not
            {
                minDifficulty = mDifficulties[i];
                r = i;
            }
        }
        return r;
    }
    public int get_difficulty(int index)
    {
        //TODO maybe some thing more complicated lol
        return (int)Mathf.Clamp(mDifficulties[index],0,3);
    }
    public int get_perfectness(int index)
    {
        return mPerfectness[index];
    }


    void reset_choosing_percentages()
    {
        ChoosingPercentages = new float[4] { 0, 0, 0, 0 };
    }
    public GameManager(ManagerManager aManager)
        : base(aManager) 
    {
        CurrentPose = null;
        CurrentLevel = -1;
        IsLoading = false;
        NextContendingChoice = 0; //this means fetus
        TotalScore = 0;
        TimeRemaining = LEVEL_TIME_TOTAL;
        for (int i = 0; i < 29; i++)
            mDifficulties[i] = 0;
        PastChoices = new int[8]{0,-1,-1,-1,-1,-1,-1,-1};
        reset_choosing_percentages();
        
    }
    public override void Start()
    {
        mManager.gameObject.AddComponent<AudioListener>();
        mSource = mManager.gameObject.AddComponent<AudioSource>();
        mManager.mEventManager.character_changed_event += character_changed_listener;

        //set just the background
        GameObject dummyChar = (GameObject)GameObject.Instantiate(mManager.mReferences.mCharacters[0]);
        mManager.mBackgroundManager.set_background(dummyChar.GetComponent<CharacterTextureBehaviour>());
        GameObject.Destroy(dummyChar);

        ProGrading.Pose[] mPossiblePoses = new ProGrading.Pose[mManager.mReferences.mPossiblePoses.Length];
        for (int i = 0; i < mPossiblePoses.Length; i++)
        { mPossiblePoses[i] = ProGrading.read_pose(mManager.mReferences.mPossiblePoses[i]); }

        mManager.mAssetLoader.load_character("0.unity3d");

    }
    public override void Update()
    {
        User = (mManager.mZigManager.has_user());

        mEvents.update(Time.deltaTime);

        //TODO indicate to player to start the game
        if (!Started && !User)
        {
        }
        else if (User)
        {
        }

        if (!Started && User && Time.timeSinceLevelLoad > mMinStartTime)
        {   

            advance_scene(5);
            //maybe less time for fetus???
            Started = true;
        }

        if (Started)
        {
            if (!IsLoading)
            {
                TimeRemaining -= Time.deltaTime;

                if (User)
                {
                    CurrentPose = ProGrading.snap_pose(mManager);
                }

                if (CurrentLevel != 0)
                {
                    pose_grading();
                    mManager.mCameraManager.set_camera_effects(ProGrading.grade_to_perfect(CurrentGrade));
                    mManager.mInterfaceManager.set_perfect_time(ProGrading.grade_to_perfect(CurrentGrade), (LEVEL_TIME_TOTAL - TimeRemaining) / LEVEL_TIME_TOTAL);
                    adjust_difficulty();
                }
                else
                {
                    mManager.mCameraManager.set_camera_effects(0.3f);
                    mManager.mInterfaceManager.set_perfect_time(0, (LEVEL_TIME_TOTAL - TimeRemaining) / LEVEL_TIME_TOTAL);
                }

                //goto next scene
                if (TimeRemaining < 0)
                {
                    TimeRemaining = 0;
                    choice_grading();
                    //advance_scene(LEVEL_TIME_TOTAL);
                }
            }
        }
        
    }


    void pose_grading()
    {
        if (CurrentPose != null && mManager.mTransparentBodyManager.mFlat.mTargetPose != null)
        {
            CurrentGrade = ProGrading.grade_pose(CurrentPose, mManager.mTransparentBodyManager.mFlat.mTargetPose);
            mManager.mInterfaceManager.mGrade = CurrentGrade;
        }
        TotalScore += Time.deltaTime * ProGrading.grade_to_perfect(CurrentGrade) * 10f;
    }

    void choice_grading()
    {
        if (CurrentPose != null)
        {
            //grade for next choice
            int minIndex = 0;
            float minGrade = Mathf.Infinity;
            string output = "";
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
                    output += grade + " ";
                }
            }
            //Debug.Log(output);
            if (minGrade > SELECTION_THRESHOLD)
            {
                NextContendingChoice = -1;//get_default_choice(CurrentLevel);
                //mManager.mInterfaceManager.set_choice(-1);
                mManager.mInterfaceManager.set_choice(NextContendingChoice);
            }
            else
            {
                NextContendingChoice = minIndex;
                mManager.mInterfaceManager.set_choice(NextContendingChoice);
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
                    advance_scene(LEVEL_TIME_TOTAL);
                }
            }
            mManager.mInterfaceManager.set_choosing_percentages(ChoosingPercentages);
        }
    }

    void adjust_difficulty()
    {
        for (int i = 0; i < 29; i++)
        {
            float prevDifficulty = get_difficulty(i);
            mDifficulties[i] +=
                CharacterDifficulties.difficulties[CurrentIndex].values[i] *
                ProGrading.grade_to_perfect(CurrentGrade) *
                Time.deltaTime / LEVEL_TIME_TOTAL * 20; // TODO should not be 100

            if (get_difficulty(i) != prevDifficulty)
            {
                //TODO play particle effects and make difficulty change event
                mChoicePoses = get_poses(CurrentLevel);
                mManager.mInterfaceManager.set_bottom_poses(mChoicePoses);
                mManager.mInterfaceManager.set_choice_difficulties();
            }
        }
    }

    void scene_loaded_callback(CharacterTextureBehaviour aChar)
    {
        CurrentLevel++;
        PastChoices[CurrentLevel] = NextContendingChoice;
        CurrentIndex = get_choice_index(NextContendingChoice, CurrentLevel);


        if (NextContendingChoice == -1)
        {
            //TODO you lose??
        }
        else
        {
            start_character(CurrentIndex);
            
            //mEvents.add_event((new GameEvents.FocusCameraOnElementEvent(mManager.mInterfaceManager.mFlatCamera, mManager.mInterfaceManager.mBlueBar)).get_event(), Mathf.Clamp(TimeRemaining - CHOICE_TIME,0,Mathf.Infinity));
            mEvents.add_event((new GameEvents.FocusCameraOnElementEvent(mManager.mInterfaceManager.mFlatCamera, mManager.mInterfaceManager.mBlueBar)).get_event(), Mathf.Clamp(TimeRemaining, 0, Mathf.Infinity));
            //mEvents.add_event((new GameEvents.ResetElementScaleEvent(mManager.mInterfaceManager.mBlueBar)).get_event(), TimeRemaining);

            //figure out next poses
            NextContendingChoice = get_default_choice(CurrentLevel);
            mChoicePoses = get_poses(CurrentLevel);
            mManager.mInterfaceManager.set_choice_difficulties();
            mManager.mInterfaceManager.set_question(CurrentLevel);
            mManager.mInterfaceManager.set_bottom_poses(mChoicePoses);
        }

        //reset the interface camera
        mEvents.add_event((new GameEvents.ResetElementScaleEvent(mManager.mInterfaceManager.mBlueBar)).get_event(), 0);
        mManager.mInterfaceManager.reset_camera();
        //reset choosing percentages
        reset_choosing_percentages();
        mManager.mInterfaceManager.set_choice(-1);
    }

    void advance_scene(float aSceneTime)
    {
        CurrentLevel++;
        PastChoices[CurrentLevel] = NextContendingChoice;
        CurrentIndex = get_choice_index(NextContendingChoice, CurrentLevel);
        TimeRemaining = aSceneTime;
        mManager.mAssetLoader.load_character(CurrentLevel + "-" + CurrentIndex + ".unity3d");
        //TODO loading bar
    }
    void start_character(int index)
    {
        GameObject demoChar = (GameObject)GameObject.Instantiate(mManager.mReferences.mCharacters[index]);
        mManager.mEventManager.character_changed_event(demoChar.GetComponent<CharacterTextureBehaviour>());
        mManager.mEventManager.character_setup_event(demoChar.GetComponent<CharacterTextureBehaviour>());
        GameObject.Destroy(demoChar);

        //set transparent target pose
        //TODO needs to read based on previous choice
        if (mChoicePoses[NextContendingChoice] != null)
        {
            mManager.mTransparentBodyManager.set_target_pose(mChoicePoses[NextContendingChoice]);
            mManager.mTransparentBodyManager.mFlat.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        }
        else
        {
            mManager.mTransparentBodyManager.mFlat.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.0f);
        }
    }
    public void character_changed_listener(CharacterTextureBehaviour aCharacter)
    {
        mSource.clip = aCharacter.backgroundMusic;
        mSource.loop = true;
        mSource.Play();
    }

    ProGrading.Pose[] get_poses(int level)
    {
        ProGrading.Pose[] r = new ProGrading.Pose[4];
        for(int i = 0; i < 4; i++)
        {
            if(mManager.mReferences.mCharacters[get_choice_index(i,level+1)] != null)
            {
                GameObject demoChar = (GameObject)GameObject.Instantiate(mManager.mReferences.mCharacters[get_choice_index(i,level+1)]);
                r[i] = ProGrading.read_pose(demoChar.GetComponent<CharacterTextureBehaviour>().properPoses[get_difficulty(get_choice_index(i,level+1))]);
                GameObject.Destroy(demoChar);
            }
            else r[i] = null;
        }
        return r;
    }


    //move this stuff elsewhere poo poo
    public static void Shuffle<T>(T[] array)
    {
        for (int i = array.Length; i > 1; i--)
        {
            // Pick random element to swap.
            int j = Random.Range(0,i-1); // 0 <= j <= i-1
            // Swap.
            T tmp = array[j];
            array[j] = array[i - 1];
            array[i - 1] = tmp;
        }
    }
    ProGrading.Pose[] get_random_possible_poses()
    {
        ProGrading.Pose[] r = new ProGrading.Pose[4];
        Shuffle<ProGrading.Pose>(mPossiblePoses);
        for (int i = 0; i < 4; i++)
            r[i] = mPossiblePoses[i];
        return r;
    }
}
