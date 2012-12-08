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

    //elements
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();

    FlatElementImage mPinkBackground;
    FlatElementImage mBlueBar;
    FlatElementImage mMeterBackground;

    MeterObject mTimeMeter;
    MeterObject mPerfectMeter;
    FlatElementImage mScoreBackground;
    FlatElementText mScoreText;


    //blue bar elements


    //generic elements lol



    
    public override void Start()
    {
        mManager.mEventManager.character_changed_event += character_changed_listener;

        mBehaviour = mManager.gameObject.AddComponent<InterfaceBehaviour>();
        mBehaviour.mManager = this;
        mFlatCamera = new FlatCameraManager(new Vector3(10000, 0, 0), 10);


    }
    public override void Update()
    {
        foreach (FlatElementBase e in mElement)
        {
            e.update_parameters(Time.deltaTime);
            e.set();
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

        mBlueBar = new FlatElementImage(refs.blueBar, 1);
        mBlueBar.HardPosition = random_position();
        mBlueBar.SoftPosition = mFlatCamera.get_point(-0.5f, 0);
        

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



        mElement.Add(mPinkBackground);
        mElement.Add(mBlueBar);
        mElement.Add(mMeterBackground);
        mElement.Add(mTimeMeter);
        mElement.Add(mPerfectMeter);
        mElement.Add(mScoreBackground);
        mElement.Add(mScoreText);

        /*MiniManObject man = new MiniManObject();
        man.SoftPosition = mFlatCamera.get_point(0, 0);
        mElement.Add(man);*/

        mIsSetup = true;
    }

    public void character_changed_listener(CharacterTextureBehaviour aCharacter)
    {
        BackgroundManager.resize_camera_against_texture(mFlatCamera.Camera, aCharacter.background1);
        if(!mIsSetup)
            setup_elements();
    }
}
