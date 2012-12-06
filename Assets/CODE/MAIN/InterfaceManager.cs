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
            e.update(Time.deltaTime);
        }
    }
    public void setup_elements()
    {
        MenuReferenceBehaviour refs = mManager.mMenuReferences;
        //setup flat elements
        mPinkBackground = new FlatElementImage(refs.pinkBackground, 0);
        mPinkBackground.SoftPosition = mFlatCamera.get_point(-1f, 0);

        mBlueBar = new FlatElementImage(refs.blueBar, 0.001f);
        mBlueBar.SoftPosition = mFlatCamera.get_point(-1f, 0);
        

        mMeterBackground = new FlatElementImage(refs.meterBackground, 0.001f);
        mMeterBackground.SoftPosition = mFlatCamera.get_point(-1f, 1);

        mTimeMeter = new MeterObject(refs.timeMeterFront, refs.timeMeterBack, Color.black,0.002f);
        mTimeMeter.SoftPosition = mFlatCamera.get_point(-1f, 1.1f);
        mPerfectMeter = new MeterObject(refs.perfectMeterFront, refs.perfectMeterBack, Color.black,0.002f);
        mPerfectMeter.SoftPosition = mFlatCamera.get_point(-1f, 0.9f);


        mElement.Add(mPinkBackground);
        mElement.Add(mBlueBar);
        mElement.Add(mMeterBackground);
        mElement.Add(mTimeMeter);
        mElement.Add(mPerfectMeter);
        mIsSetup = true;
    }

    public void character_changed_listener(CharacterTextureBehaviour aCharacter)
    {
        BackgroundManager.resize_camera_against_texture(mFlatCamera.Camera, aCharacter.background1);
        if(!mIsSetup)
            setup_elements();
    }
}
