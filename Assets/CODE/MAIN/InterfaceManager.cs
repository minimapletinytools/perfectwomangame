using UnityEngine;
using System.Collections.Generic;

public class InterfaceManager : FakeMonoBehaviour {
    public InterfaceManager(ManagerManager aManager) : base(aManager) { }

    //constants
    const int mNumberBefore = 2;
    const int mNumberAfter = 3;
    const int mMinSpacing = 30; //pixels

    InterfaceBehaviour mBehaviour = null;
    public float mGrade;

    bool mIsSetup = false;


    public FlatCameraManager mFlatCamera;
    CharacterTextureBehaviour mMiniMan;

    //elements
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();

    FlatElementImage mGameBackground;
    FlatElementImage mPinkBackground;
    public FlatElementImage mBlueBar;
    FlatElementImage mMeterBackground;

    //meter elements
    MeterObject mTimeMeter;
    MeterObject mPerfectMeter;
    FlatElementImage mScoreBackground;
    public FlatElementText mScoreText;
    FlatElementImage mMultiplier;



    //blue bar elements
    ChoiceObjectPair[] mTopChoices = new ChoiceObjectPair[4];
    ChoiceObjectPair[] mBottomChoices = new ChoiceObjectPair[4];
    FlatBodyObject mCurrentBody = null;
    FlatElementImage mBigBadBox;
    FlatElementImage mQuestion;

    //nonsense
    FlatGraphElement mGraph;
    public Vector3 choice_offset(int y, int x, bool blue = false) //-1 is bottom, 0 is middel 1 is top
    {
        if (!blue)
            return new Vector3(x * (-250) + 375 - 50, y * 125, 0);
        else
            return choice_offset(y, x, false) + new Vector3(0, -50, 0);
    }

    //generic elements 
    ChoiceObjectPair[] mChoices = new ChoiceObjectPair[29];
    public Vector3 generic_offset(int i, int level)
    {
        float heightSpace = 250;
        float heightOffset = -125;
        float middleOffset = 40;
        int tier = (i - 1) / 4;
        Vector3 r;
        if (i == 0)
        {
            tier = -1;
            r = mBlueBar.SoftPosition + new Vector3(0, 0, 0) + new Vector3(0, (-tier-1) * heightSpace, 0) + new Vector3(0, level * heightSpace + heightOffset, 0);
            
        }
        else
            r = mBlueBar.SoftPosition + choice_offset(0, (i - 1) % 4) + new Vector3(0, (-tier-1) * heightSpace, 0) + new Vector3(0, level * heightSpace + heightOffset, 0);

        if (tier + 1 - level < 0)
            r += new Vector3(0, heightSpace + middleOffset, 0);
        //else r += new Vector3(0, heightSpace-middleOffset, 0);
        else r += new Vector3(0, - middleOffset, 0);
        return r;
    }



    
    public override void Start()
    {
        mManager.mEventManager.character_changed_event += character_changed_listener;
        mBehaviour = mManager.gameObject.AddComponent<InterfaceBehaviour>();
        mBehaviour.mManager = this;
        mFlatCamera = new FlatCameraManager(new Vector3(10000, 0, 0), 10);
        mMiniMan = ((GameObject)GameObject.Instantiate(ManagerManager.Manager.mMenuReferences.miniMan)).GetComponent<CharacterTextureBehaviour>();

        mGameBackground = new FlatElementImage(null, new Vector2(2880, 1800), 0); //TODO need image here???
        mGameBackground.HardPosition = mFlatCamera.get_point(0, 0);
        mGameBackground.Enabled = false;
        
    }
    public override void Update()
    {
        mFlatCamera.update(Time.deltaTime);
        if (mCurrentBody != null)
            mCurrentBody.match_body_to_projection(mManager.mProjectionManager);
        foreach (FlatElementBase e in mElement)
        {
            //e.mLocalColor = (new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)))*0.1f;
            e.update(Time.deltaTime);            
        }
    }
    
    public Vector3 random_position()
    {
        //UGG piece of junk...
        return (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)).normalized * Random.Range(2000,20000);
    }

    public void setup_elements()
    {
        MenuReferenceBehaviour refs = mManager.mMenuReferences;

        //setup flat elements
        mPinkBackground = new FlatElementImage(refs.pinkBackground, 0);
        mPinkBackground.HardPosition = random_position();
        mPinkBackground.SoftPosition = mFlatCamera.get_point(-0.5f, 0);

        mBlueBar = new FlatElementImage(refs.blueBar, 2);
        mBlueBar.HardPosition = random_position();
        mBlueBar.SoftPosition = mFlatCamera.get_point(-0.5f, 0) + new Vector3(0,-150,0);
        

        mMeterBackground = new FlatElementImage(refs.meterBackground, 1);
        mMeterBackground.HardPosition = random_position();
        mMeterBackground.SoftPosition = mFlatCamera.get_point(-0.5f, 1) + new Vector3(0,-refs.meterBackground.height/2f,0);

        mTimeMeter = new MeterObject(refs.timeMeterFront, refs.timeMeterBack, Color.green,2);
        mTimeMeter.HardPosition = random_position();
        mTimeMeter.SoftPosition = mMeterBackground.SoftPosition + new Vector3(100, 60, 0);

        mPerfectMeter = new MeterObject(refs.perfectMeterFront, refs.perfectMeterBack, Color.yellow,2);
        mPerfectMeter.HardPosition = random_position();
        mPerfectMeter.SoftPosition = mMeterBackground.SoftPosition + new Vector3(100, -60, 0);

        mScoreBackground = new FlatElementImage(refs.scoreBackground, 2);
        mScoreBackground.HardPosition = random_position();
        mScoreBackground.SoftPosition = mMeterBackground.SoftPosition + new Vector3(-400,0,0);

        mScoreText = new FlatElementText(refs.menuFont, 700, "0", 3);
        mScoreText.HardPosition = random_position();
        mScoreText.SoftPosition = mScoreBackground.SoftPosition + new Vector3(0,-35,0);
        mScoreText.SoftColor = new Color(1f, 1f, 1f, 1f);

        mMultiplier = new FlatElementImage(null, 4);
        mMultiplier.SoftPosition = mScoreBackground.SoftPosition + new Vector3(-250, -50, 0);

        for (int i = 0; i < 4; i++)
        {
            mTopChoices[i] = new ChoiceObjectPair(refs.emptyBox, 3);
            mTopChoices[i].HardPosition = random_position();
            mTopChoices[i].SoftPosition = mBlueBar.SoftPosition + choice_offset(1, i,true);
            mBottomChoices[i] = new ChoiceObjectPair(refs.emptyBox, mMiniMan, ProGrading.read_pose(refs.cheapPose), 3);
            //mBottomChoices[i].HardPosition = random_position();
            mBottomChoices[i].HardPosition = mBlueBar.SoftPosition + choice_offset(-1, i,true);
        }

        mBigBadBox = new FlatElementImage(refs.bigBadBox, 4);
        mBigBadBox.HardPosition = mBigBadBox.SoftPosition;
        mQuestion = new FlatElementImage(refs.questions[0], 4);
        mQuestion.SoftPosition = mBlueBar.SoftPosition + new Vector3(0, 240, 0);


        for (int i = 0; i < 29; i++)
        {
            mChoices[i] = new ChoiceObjectPair(refs.boxes[i],1);
            mChoices[i].HardPosition = random_position();
            mChoices[i].SoftPosition = generic_offset(i, 0);
        }

        mCurrentBody = new FlatBodyObject(mMiniMan, 4);
        mCurrentBody.SoftColor = new Color(0.6f, 0.3f, 0.3f, 0.2f);
        mCurrentBody.HardPosition = random_position();

        set_choice(-1);



        /*
        mGraph = new FlatGraphElement(400, 250, 6);
        for (int i = 0; i < 100; i++)
            mGraph.draw_point(Random.insideUnitCircle / 2f + new Vector2(0.5f, 0.5f), 10, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        mGraph.HardPosition = random_position();
        mGraph.SoftPosition = mBlueBar.SoftPosition + new Vector3(400, 0, 0);*/


        mElement.Add(mPinkBackground);
        mElement.Add(mBlueBar);
        mElement.Add(mMeterBackground);
        mElement.Add(mTimeMeter);
        mElement.Add(mPerfectMeter);
        mElement.Add(mScoreBackground);
        mElement.Add(mScoreText);
        mElement.Add(mMultiplier);
        for (int i = 0; i < 4; i++)
        {
            mElement.Add(mTopChoices[i]);
            mElement.Add(mBottomChoices[i]);
        }
        for (int i = 0; i < 29; i++)
            mElement.Add(mChoices[i]);
        mElement.Add(mBigBadBox);
        mElement.Add(mQuestion);
        mElement.Add(mCurrentBody);

        //mElement.Add(mGraph);

        mIsSetup = true;
    }


    public void set_choice(int index)
    {
        if (index == -1)
        {
            mBigBadBox.SoftPosition = new Vector3(-3000, 400, 0);
            //mCurrentBody.SoftPosition = new Vector3(-3000, 400, 0);
            mCurrentBody.SoftPosition = mBlueBar.SoftPosition;
        }
        else
        {
            mBigBadBox.SoftPosition = mBlueBar.SoftPosition + choice_offset(0, index, true) + new Vector3(48, 0, 0);
            mCurrentBody.SoftPosition = mBottomChoices[index].mBody.SoftPosition;
        }
    }

    public void set_choice_difficulties()
    {
        for (int i = 0; i < 29; i++)
        {
            int subIndex = (i - 1) % 4;
            int level = mManager.mGameManager.get_level_from_choice_index(i);
            mChoices[i].SoftPosition = generic_offset(i, mManager.mGameManager.CurrentLevel);
            mChoices[i].set_difficulty(mManager.mGameManager.get_difficulty(i));
            if (level <= mManager.mGameManager.CurrentLevel && i > 0 && mManager.mGameManager.PastChoices[level] != subIndex)
            {
                mChoices[i].Enabled = false;
            }
            if (level == mManager.mGameManager.CurrentLevel+1)
            {
                mBottomChoices[subIndex].set_difficulty(mManager.mGameManager.get_difficulty(i));
                mTopChoices[subIndex].set_perfectness(mManager.mGameManager.get_perfectness(i));
            }
        }
    }

    public void set_question(int level)
    {
        if (level < mManager.mMenuReferences.questions.Length) 
            mQuestion.mImage.set_new_texture(mManager.mMenuReferences.questions[level]);
    }

    public void set_choosing_percentages(float[] aPercentages)
    {
        for (int i = 0; i < 4; i++)
            mBottomChoices[i].mMeter.Percentage = aPercentages[i]; 
    }

    public void set_bottom_poses(ProGrading.Pose[] aPoses)
    {
        for (int i = 0; i < 4; i++)
        {
            if (aPoses[i] == null)
            {
                mBottomChoices[i].fade_pose(false);
            }
            else
            {
                mBottomChoices[i].fade_pose(true);
                mBottomChoices[i].set_pose(aPoses[i]);
            }
        }
    }

    public void fade_out_choices()
    {
        for (int i = 0; i < 4; i++)
        {
            mTopChoices[i].mSquare.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0f);
        }
    }

    public void fade_in_choices()
    {
        for (int i = 0; i < 4; i++)
        {
            int index = mManager.mGameManager.get_choice_index(i, mManager.mGameManager.CurrentLevel + 1);
            if (index < mManager.mMenuReferences.boxes.Length && index >= 0)
            {
                mTopChoices[i].mSquare.mImage.set_new_texture(mManager.mMenuReferences.boxes[index]);
            }
            mTopChoices[i].mSquare.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }   

    public void set_perfect_time(float perfect, float time)
    {
        mPerfectMeter.Percentage = perfect;
        mTimeMeter.Percentage = time;

        mScoreText.Text = ((int)(mManager.mGameManager.TotalScore)).ToString();
        if (perfect > 0.5f)
            mPerfectMeter.mLocalColor = (new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))) * 0.2f;
        else mPerfectMeter.mLocalColor = new Color(0, 0, 0, 0);
        if (perfect > 0.8f)
            mPerfectMeter.mLocalPosition = Random.insideUnitCircle * 10;
        else mPerfectMeter.mLocalPosition = Vector3.zero;
        if (perfect > 0.9f)
            mPerfectMeter.mLocalRotation = Quaternion.AngleAxis(Random.Range(-5f, 5f), Vector3.forward);
        else mPerfectMeter.mLocalRotation = Quaternion.identity;
    }


    public void reset_camera()
    {
        mFlatCamera.focus_camera_on_element(mGameBackground);
    }


    public void character_changed_listener(CharacterLoader aCharacter)
    {
        BackgroundManager.resize_camera(mFlatCamera.Camera, aCharacter.Sizes.mBackSize);
        if(!mIsSetup)
            setup_elements();
    }

    public void mini_bundle_loaded_listener(AssetBundle aBundle, string aBundleName)
    {


        CharacterLoader loader = new CharacterLoader();
        loader.complete_load_character(aBundle, aBundleName);
        //TODO
        //new FlatBodyObject(loader);

        //unload bundles??? nah
        
    }
}
