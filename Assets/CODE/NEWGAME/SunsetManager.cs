using UnityEngine;
using System.Collections.Generic;

public class SunsetManager 
{
	ManagerManager mManager;
    public SunsetManager(ManagerManager aManager)
	{
		mManager = aManager;
	}

	public TimedEventDistributor TED { get; private set; }
    public FlatCameraManager mFlatCamera;
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	
	
	
	public void initialize()
	{
	}
	
	public void update()
	{
		mFlatCamera.update(Time.deltaTime);
        foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		
		TED.update(Time.deltaTime);
	}
	
	FlatElementImage mBackground = null;
	FlatElementImage mSun = null;
	List<FlatElementImage> mCharacters = new List<FlatElementImage>();
}
