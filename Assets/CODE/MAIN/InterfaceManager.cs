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


    FlatCameraManager mFlatCamera;
    CharacterTextureBehaviour mMiniMan;

    //elements
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();

    FlatElementImage mPinkBackground;
    FlatElementImage mBlueBar;
    FlatElementImage mMeterBackground;

    //meter elements
    MeterObject mTimeMeter;
    MeterObject mPerfectMeter;
    FlatElementImage mScoreBackground;
    FlatElementText mScoreText;
    FlatElementImage mMultiplier;



    //blue bar elements
    ChoiceObjectPair[] mTopChoices = new ChoiceObjectPair[4];
    ChoiceObjectPair[] mBottomChoices = new ChoiceObjectPair[4];
    FlatElementImage mBigBadBox;
    FlatElementImage mQuestion;

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
            r = mBlueBar.SoftPosition + new Vector3(0, 0, 0) + new Vector3(0, level * heightSpace + heightOffset, 0);
            tier = -1;
        }
        else
            r = mBlueBar.SoftPosition + choice_offset(0, (i - 1) % 4) + new Vector3(0, -tier * heightSpace, 0) + new Vector3(0, level * heightSpace + heightOffset, 0);

        if (tier + 1 - level < 0)
            r += new Vector3(0, heightSpace + middleOffset, 0);
        else r += new Vector3(0, -middleOffset, 0);
        return r;
    }



    
    public override void Start()
    {
        mManager.mEventManager.character_changed_event += character_changed_listener;
        mBehaviour = mManager.gameObject.AddComponent<InterfaceBehaviour>();
        mBehaviour.mManager = this;
        mFlatCamera = new FlatCameraManager(new Vector3(10000, 0, 0), 10);
        mMiniMan = ((GameObject)GameObject.Instantiate(ManagerManager.Manager.mMenuReferences.miniMan)).GetComponent<CharacterTextureBehaviour>();
        
    }
    public override void Update()
    {
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

        mScoreText = new FlatElementText(refs.menuFont, 50, "999", 3);
        mScoreText.HardPosition = random_position();
        mScoreText.SoftPosition = mScoreBackground.SoftPosition;

        mMultiplier = new FlatElementImage(null, 4);
        mMultiplier.SoftPosition = mScoreBackground.SoftPosition + new Vector3(-250, -50, 0);



        for (int i = 0; i < 4; i++)
        {
            mTopChoices[i] = new ChoiceObjectPair(refs.emptyBox, 3);
            mTopChoices[i].HardPosition = random_position();
            mTopChoices[i].SoftPosition = mBlueBar.SoftPosition + choice_offset(1, i,true);
            mBottomChoices[i] = new ChoiceObjectPair(refs.emptyBox, mMiniMan, ProGrading.read_pose(refs.cheapPose), 3);
            mBottomChoices[i].HardPosition = random_position();
            mBottomChoices[i].SoftPosition = mBlueBar.SoftPosition + choice_offset(-1, i,true);
        }
        mBigBadBox = new FlatElementImage(refs.bigBadBox, 4);
        mBigBadBox.HardPosition = mBlueBar.SoftPosition + choice_offset(0, 0,true) + new Vector3(48, 0, 0);
        mQuestion = new FlatElementImage(refs.questions[0], 4);
        mQuestion.SoftPosition = mBlueBar.SoftPosition + new Vector3(0, 140, 0);


        for (int i = 0; i < 29; i++)
        {
            mChoices[i] = new ChoiceObjectPair(refs.boxes[i],1);
            mChoices[i].HardPosition = random_position();
            mChoices[i].SoftPosition = generic_offset(i, 1);
        }



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


        mIsSetup = true;
    }

    public void character_changed_listener(CharacterTextureBehaviour aCharacter)
    {
        BackgroundManager.resize_camera_against_texture(mFlatCamera.Camera, aCharacter.background1);
        if(!mIsSetup)
            setup_elements();
    }
}
