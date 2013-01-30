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

    //constants
    public const float LEVEL_TIME_TOTAL = 30;
    public const float SELECTION_THRESHOLD = 9;
    public const float GRADING_THRESHOLD = 10;
    public const float CHOOSING_PERCENTAGE_GROWTH_RATE = 0.15f;
    public const float CHOOSING_PERCENTAGE_DECLINE_RATE = 1f;

    //public const bool DEBUGGING = true;
    public const bool DEBUGGING = false;

    //
    public Camera mCamera;
    public AudioSource mSource;
    public TimedEventHandler mEvents = new TimedEventHandler();

    //pretend constants
    int[] mPerfectness = new int[29]{ 0, 
            3, 1, 0, 2, 
            0, 1, 3, 2, 
            3, 0, 2, 1, 
            1, 3, 0, 2,
            1, 0, 3, 2, 
            0, 3, 1, 2, 
            1, 0, 2, 3 };
    string[] mLevelToAge = new string[8] { "0", "05", "16", "27", "34", "45", "60", "80" };
    ProGrading.Pose[] mPossibleChoicePoses;
    ProGrading.Pose[] mDifficultyTargetPoses = null;




    //basic gam estate variables
    float mMinStartTime = 0;
    public bool Started
    { get; private set; }
    public bool User
    { get; private set; }


    //specific game state variables
    public int CurrentLevel
    { get; private set; }
    public int CurrentIndex
    { get; private set; }
    public float TotalScore
    { get; private set; }
    public float TimeRemaining
    { get; private set; }
    float[] mDifficulties = new float[29];
    public int[] PastChoices
    { get; private set; }
    public bool IsStateChoosing
    { get { return TimeRemaining <= 0; } }

    //local game state variabbles
    public float[] ChoosingPercentages
    { get; private set; }
    public float CurrentGrade
    { get; private set; }
    public ProGrading.Pose CurrentPose
    { get; private set; }
    public int NextContendingChoice
    { get; private set; }
    ProGrading.Pose[] mChoicePoses = new ProGrading.Pose[4] { null, null, null, null };

    //asset bundle loading nonsense
    public bool IsLoading
    { get; private set; }
    public AssetBundle CurrentAssetBundle
    { get; private set; }
    public void unload_current_asset_bundle()
    {
        if (CurrentAssetBundle != null)
            CurrentAssetBundle.Unload(true);
    }
    

    //choice and difficulty accessors
    public bool does_choice_exist(int index)
    {
        //return mManager.mReferences.mCharacters[get_choice_index(index, CurrentLevel+1)] != null;

        if(index <= 0)
            return true;
        return mDifficultyTargetPoses[index * 4 - 3] != null;

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


    
    public GameManager(ManagerManager aManager)
        : base(aManager) 
    {
        CurrentPose = null;
        CurrentLevel = -1;
        IsLoading = false;
        CurrentAssetBundle = null;
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

        //set just the background
        /*
        GameObject dummyChar = (GameObject)GameObject.Instantiate(mManager.mReferences.mCharacters[0]);
        mManager.mBackgroundManager.set_background(dummyChar.GetComponent<CharacterTextureBehaviour>());
        GameObject.Destroy(dummyChar);*/

        mPossibleChoicePoses = new ProGrading.Pose[mManager.mReferences.mPossiblePoses.Length];
        for (int i = 0; i < mPossibleChoicePoses.Length; i++)
        { mPossibleChoicePoses[i] = ProGrading.read_pose(mManager.mReferences.mPossiblePoses[i]); }

        mManager.mAssetLoader.load_poses("POSES");

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

            if (DEBUGGING) advance_scene(1);
            else advance_scene(LEVEL_TIME_TOTAL);
            //maybe less time for fetus???
            Started = true;
        }

        if (Started)
        {
            if (!IsLoading)
            {

                hack_keyboard_input();

                TimeRemaining -= Time.deltaTime;
               
                

                if (User)
                {
                    CurrentPose = ProGrading.snap_pose(mManager);
                }

                if (CurrentLevel != 0)
                {
                    if (!IsStateChoosing)
                    {
                        pose_grading();
                        mManager.mCameraManager.set_camera_effects(ProGrading.grade_to_perfect(CurrentGrade));
                        mManager.mInterfaceManager.set_perfect_time(ProGrading.grade_to_perfect(CurrentGrade), (LEVEL_TIME_TOTAL - TimeRemaining) / LEVEL_TIME_TOTAL);
                        if (CurrentLevel != 7) //no more choices to be made tee hee
                            adjust_difficulty();
                    }
                }
                else
                {
                    mManager.mCameraManager.set_camera_effects(0.3f);
                    mManager.mInterfaceManager.set_perfect_time(0, (LEVEL_TIME_TOTAL - TimeRemaining) / LEVEL_TIME_TOTAL);
                }

                //goto next scene
                if (TimeRemaining < 0)
                {
                    if(TimeRemaining > -999) //this is a hack, move me somewhere else
                    {
                        if (CurrentLevel < 7)//hack
                        {
                            mChoicePoses = get_random_possible_poses();
                            for (int i = 0; i < 4; i++)
                                if (!does_choice_exist(get_choice_index(i, CurrentLevel + 1)))
                                    mChoicePoses[i] = null;
                            mManager.mInterfaceManager.set_bottom_poses(mChoicePoses);
                            mManager.mInterfaceManager.mBlueBar.Depth = 2;
                        }
                        else
                        {
                            advance_scene(999999);
                        }
                    }
                    TimeRemaining = -999;
                    choice_grading();
                    //advance_scene(LEVEL_TIME_TOTAL);
                }
            }
        }
        
    }

    //used by update routine
    void pose_grading()
    {
        if (CurrentPose != null && mManager.mTransparentBodyManager.mFlat.mTargetPose != null)
        {
            CurrentGrade = ProGrading.grade_pose(CurrentPose, mManager.mTransparentBodyManager.mFlat.mTargetPose);
            mManager.mInterfaceManager.mGrade = CurrentGrade;
        }
        TotalScore += Time.deltaTime * ProGrading.grade_to_perfect(CurrentGrade) * 5f * (mPerfectness[CurrentIndex]+1);
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

    //TODO this is completely different now
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
                
                {
                    //TODO play particle effects and make difficulty change event
                    mChoicePoses = get_poses(CurrentLevel);
                    mManager.mInterfaceManager.set_bottom_poses(mChoicePoses);
                    mManager.mInterfaceManager.set_choice_difficulties();
                }
            }
        }
    }


    string construct_bundle_name(int level, int index)
    {
        return mLevelToAge[level] + "-" + (index+1);
    }
    void advance_scene(float aSceneTime)
    {
        CurrentLevel++;
        
        if (CurrentLevel < 8)
        {
            PastChoices[CurrentLevel] = NextContendingChoice;
            CurrentIndex = get_choice_index(NextContendingChoice, CurrentLevel);
            TimeRemaining = aSceneTime;
            IsLoading = true;
            mManager.mAssetLoader.load_character(construct_bundle_name(CurrentLevel, NextContendingChoice));
        }
        else
        {
            
            TimeRemaining = 9999;
            mManager.mAssetLoader.load_character("999");
            IsLoading = true;
        }
        
    }

    //used by advance_scene
    public void scene_loaded_callback(AssetBundle aBundle, string aBundleName) //TODO do not aBundleName
    {


        CharacterLoader loader = new CharacterLoader();
        //Debug.Log("loading character in CharacterLoader " + aBundleName);
        loader.complete_load_character(aBundle,aBundleName);
        
        if (aBundleName == "999")
        {
            mManager.mInterfaceManager.mScoreText.SoftPosition = mManager.mBackgroundManager.mBackgroundElements.mElements[0].Element.SoftPosition + new Vector3(0, -150, 0);
            mManager.mInterfaceManager.mScoreText.Depth = 101;
            foreach (Renderer f in mManager.mInterfaceManager.mScoreText.PrimaryGameObject.GetComponentsInChildren<Renderer>()) //hack to make it show in background camera...
                f.gameObject.layer = mManager.mBackgroundManager.mBackgroundLayer;

            mManager.mBackgroundManager.character_changed_listener(loader);
            set_music(loader.Images.backgroundMusic);
            mManager.mBodyManager.character_changed_listener(null);
            mManager.mTransparentBodyManager.character_changed_listener(null);

        }
        else
        {
            start_character(loader, CurrentIndex);

            //mEvents.add_event((new GameEvents.FocusCameraOnElementEvent(mManager.mInterfaceManager.mFlatCamera, mManager.mInterfaceManager.mBlueBar)).get_event(), Mathf.Clamp(TimeRemaining - CHOICE_TIME,0,Mathf.Infinity));
            mEvents.add_event((new GameEvents.FocusCameraOnElementEvent(mManager.mInterfaceManager.mFlatCamera, mManager.mInterfaceManager.mBlueBar)).get_event(), Mathf.Clamp(TimeRemaining, 0, Mathf.Infinity));
            //mEvents.add_event((new GameEvents.ResetElementScaleEvent(mManager.mInterfaceManager.mBlueBar)).get_event(), TimeRemaining);

            //figure out next poses
            NextContendingChoice = get_default_choice(CurrentLevel);
            if (CurrentLevel < 7)
                mChoicePoses = get_poses(CurrentLevel);
            mManager.mInterfaceManager.set_choice_difficulties();
            mManager.mInterfaceManager.set_question(CurrentLevel);
            mManager.mInterfaceManager.set_bottom_poses(mChoicePoses);    
        }

        //reset the interface 
        mEvents.add_event((new GameEvents.FadeInTopChoiceInInterfaceEvent(mManager.mInterfaceManager)).get_event(), 0.5f);
        mManager.mInterfaceManager.reset_camera();
        reset_choosing_percentages();
        mManager.mInterfaceManager.mBlueBar.SoftScale = Vector3.one;
        mManager.mInterfaceManager.set_choosing_percentages(ChoosingPercentages);
        mManager.mInterfaceManager.fade_out_choices();
        mManager.mInterfaceManager.set_choice(-1);
        mManager.mInterfaceManager.mBlueBar.Depth = 100;
        IsLoading = false;

        //unload old bundle //TODO I don't think this is working
        unload_current_asset_bundle();
        CurrentAssetBundle = aBundle;
    }

    void reset_choosing_percentages()
    {
        ChoosingPercentages = new float[4] { 0, 0, 0, 0 };
    }

    void start_character(CharacterLoader container, int index)
    {
        mManager.mEventManager.character_changed_event(container);

        set_music(container.Images.backgroundMusic);

        if (container.Name != "999")
        {
            //setup transparent body manager
            if (get_pose_at_index(index, get_difficulty(index)) != null)
            {
                mManager.mTransparentBodyManager.set_target_pose(get_pose_at_index(index, get_difficulty(index)));
                mManager.mTransparentBodyManager.mFlat.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.35f);
            }
            else if (CurrentLevel == 0)
            {
                mManager.mTransparentBodyManager.mFlat.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.0f);
            }
            else
                throw new UnityException("Missing Pose for index " + index + " difficulty " + get_difficulty(index));
        }
    }
         
    public void set_music(AudioClip music)
    {
        //TODO fading nonsense
        mSource.clip = music;
        mSource.loop = true;
        mSource.Play();
    }

    //pose related
    ProGrading.Pose[] get_poses(int level)
    {
        ProGrading.Pose[] r = new ProGrading.Pose[4];
        for (int i = 0; i < 4; i++)
        {
            int index = get_choice_index(i, level + 1);
            r[i] = mDifficultyTargetPoses[index * 4 - 3 + get_difficulty(index)];
        }
        return r;
    }
    ProGrading.Pose get_pose_at_index(int index, int difficulty)
    {
        if (mDifficultyTargetPoses == null)
            return null;
        if (index == 0)
            return null; //fetus has no target pose although there is a space for it...
        return mDifficultyTargetPoses[index*4 + difficulty - 3];
    }
    public void pose_bundle_loaded_callback(AssetBundle aBundle)
    {
        mDifficultyTargetPoses = new ProGrading.Pose[28 * 4 + 1];
        for (int i = 0; i < 28 * 4 + 1; i++)
            mDifficultyTargetPoses[i] = null;
        for (int i = 0; i < 28; i++)
        {
            string bundle = construct_bundle_name(get_level_from_choice_index(i + 1), i % 4);
            if (mManager.mAssetLoader.does_bundle_exist(bundle))
            {
                ProGrading.Pose firstNotNullPose = null;
                for (int j = 0; j < 4; j++)
                {
                    TextAsset poseText = (TextAsset)aBundle.Load(bundle + "-" + j, typeof(TextAsset));
                    if (poseText != null)
                    {
                        mDifficultyTargetPoses[1 + 4 * i + j] = ProGrading.read_pose(poseText);
                        if (firstNotNullPose == null)
                            firstNotNullPose = mDifficultyTargetPoses[1 + 4 * i + j];
                    }
                }
                for (int j = 0; j < 4; j++)
                {
                    if (mDifficultyTargetPoses[1 + 4 * i + j] == null)
                    {
                        if (firstNotNullPose == null)
                        {
                            if (j == 3)
                                mDifficultyTargetPoses[1 + 4 * i + j] = ProGrading.read_pose(mManager.mReferences.mDefaultTargetPoses[Random.Range(3, 11)]);
                            else
                                mDifficultyTargetPoses[1 + 4 * i + j] = ProGrading.read_pose(mManager.mReferences.mDefaultTargetPoses[j]);
                        }
                        else
                            mDifficultyTargetPoses[1 + 4 * i + j] = firstNotNullPose;
                    }
                }
            }
            else
            {
                //Debug.Log("no character found for " + bundle);
            }
        }
        
        aBundle.Unload(true); //don't need this anymore I don't ithnk...
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



    //hack nonsense
    public void hack_choice(int choice)
    {
        NextContendingChoice = choice;
        advance_scene(LEVEL_TIME_TOTAL);
    }

    public void hack_keyboard_input()
    {
        //keyboard input hack
        if (Input.GetKeyDown(KeyCode.Alpha0))
            TimeRemaining = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            hack_choice(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            hack_choice(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            hack_choice(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            hack_choice(3);

    }

   
}
