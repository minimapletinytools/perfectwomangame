using UnityEngine;
using System.Collections;


#if UNITY_XBOXONE 
using DataPlatform;
using Users;

public class XbonePLM
{
    ManagerManager mManager;
    
    public XbonePLM(ManagerManager aManager)
    {
        mManager = aManager;
    }

	public void Start()
	{
		if (XboxOneApplicationExecutionState.Terminated == XboxOnePLM.GetPreviousExecutionState ()) 
		{
			//TODO attemp to load
		}

		XboxOnePLM.OnSuspendingEvent += Suspending;
		XboxOnePLM.OnResourceAvailabilityChangedEvent += ResourceAvailabilityChangedEvent;
		XboxOnePLM.OnResumingEvent += Resuming;
		XboxOnePLM.OnActivationEvent += Activation;
	}

	void Suspending()
	{
		//TODO save
        ManagerManager.Log("SUSPENDING");
		XboxOnePLM.AmReadyToSuspendNow ();
	}

	void ResourceAvailabilityChangedEvent(bool aConstrained)
	{
		//TODO (also make sure the boolean is correct..
        ManagerManager.Log("RESOURCES CHANGED " + aConstrained);

        if (aConstrained)
            ManagerManager.Manager.GameEventDistributor("PAUSE",null);
        else if(aConstrained == false && !MicrosoftZig.Inst.mAll.IsSomeoneSignedIn)
            UsersManager.RequestSignIn(Users.AccountPickerOptions.AllowGuests);
        else
            ManagerManager.Manager.GameEventDistributor("RESUME", null);

        
	}

	void Resuming(double aTime)
	{
		//TODO check if user has changed
        ManagerManager.Log("RESUMING");
        if(!MicrosoftZig.Inst.mAll.IsSomeoneSignedIn)
            UsersManager.RequestSignIn(Users.AccountPickerOptions.AllowGuests);
    }

	void Activation(ActivatedEventArgs args)
	{
		//TODO do I need to return something here toget out of constrained mode???
        ManagerManager.Log("ACTIVATION");
	}
}
#else
    
public class XbonePLM
{
    public XbonePLM(ManagerManager aManager)
    {
    }
	public void Start()
	{
	}
}
#endif
