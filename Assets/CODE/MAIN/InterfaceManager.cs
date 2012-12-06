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
            e.update_parameters(Time.deltaTime);
            e.set();
        }
    }
    public void setup_elements()
    {
        MenuReferenceBehaviour refs = mManager.mMenuReferences;
        //setup flat elements
        mPinkBackground = new FlatElementImage(refs.pinkBackground, 0);
        mPinkBackground.SoftPosition = mFlatCamera.get_point(-0.5f, 0);

        mBlueBar = new FlatElementImage(refs.blueBar, 1);
        mBlueBar.SoftPosition = mFlatCamera.get_point(-0.5f, 0);
        

        mMeterBackground = new FlatElementImage(refs.meterBackground, 1);
        mMeterBackground.SoftPosition = mFlatCamera.get_point(-0.5f, 1) + new Vector3(0,-refs.meterBackground.height/2f,0);

        mTimeMeter = new MeterObject(refs.timeMeterFront, refs.timeMeterBack, Color.green,2);
        mTimeMeter.SoftPosition = mMeterBackground.SoftPosition + new Vector3(0, 60, 0);
        mPerfectMeter = new MeterObject(refs.perfectMeterFront, refs.perfectMeterBack, Color.yellow,2);
        mPerfectMeter.SoftPosition = mMeterBackground.SoftPosition + new Vector3(0, -60, 0);


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
