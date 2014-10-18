using UnityEngine;
using System.Collections.Generic;
using System.Linq;


//TODO you'll want to switch this over to the Unity pulgins eventually

public class MicrosoftZig : ZgInterface
{
    
    ZgManager mZig;
	XboneUsers mUsers;
	XboneKinect mKinect;
	XbonePLM mPLM;
	XboneStorage mStorage;
	XboneEvents mEvents;

	bool Initialized { get; set; }
	
	public void initialize(ZgManager aZig)
	{
        mZig = aZig;
		mPLM = new XbonePLM ();
		mKinect = new XboneKinect ();
		mUsers = new XboneUsers ();
		mStorage = new XboneStorage ();
		mEvents = new XboneEvents ();


		mPLM.Start ();
		mKinect.Start ();
		mUsers.Start ();
		mStorage.Start ();
		mEvents.Start();

		Initialized = true;
	}
	
	public void update()
	{

		mKinect.Update ();
	}
	
	public bool has_user()
	{
		return mKinect.IsTracking;
	}

	//TODO should check for users
	public bool can_start()
	{
		if(Initialized)
			return true;
		return false;
	}



	//NOTE calling these assumes mStorage has been properly initialized already... 
	void write_data(byte[] aData, string aName){mStorage.write_data (aData, aName);}
	byte[] read_data(string aName){return mStorage.read_data (aName);}
	
	public ZgDepth DepthImage{get{ return null; }}
	public ZgImage ColorImage{get{ return null; }}
	public ZgLabelMap LabelMap{get{ return null; }}
    public bool ReaderInitialized { get{ return mKinect.IsReaderConnected; } } //TODO maybe return something more useful...
	public bool IsMicrosoftKinectSDK { get{ return true; } }
}


