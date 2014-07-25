using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class MicrosoftZig : ZgInterface
{
    public static Dictionary<ZgJointId,Windows.Kinect.JointType> sJointTypeMap = new Dictionary<ZgJointId,Windows.Kinect.JointType>()
    {
        {ZgJointId.LeftShoulder,Windows.Kinect.JointType.ShoulderLeft},
        {ZgJointId.LeftElbow,Windows.Kinect.JointType.ElbowLeft},
        {ZgJointId.LeftHip,Windows.Kinect.JointType.HipLeft},
        {ZgJointId.LeftKnee,Windows.Kinect.JointType.KneeLeft},
        {ZgJointId.LeftAnkle,Windows.Kinect.JointType.AnkleLeft},
        {ZgJointId.RightShoulder,Windows.Kinect.JointType.ShoulderRight},
        {ZgJointId.RightElbow,Windows.Kinect.JointType.ElbowRight},
        {ZgJointId.RightHip,Windows.Kinect.JointType.HipRight},
        {ZgJointId.RightKnee,Windows.Kinect.JointType.KneeRight},
        {ZgJointId.RightAnkle,Windows.Kinect.JointType.AnkleRight},
        {ZgJointId.Neck,Windows.Kinect.JointType.Neck},
        {ZgJointId.Torso,Windows.Kinect.JointType.SpineMid},
        {ZgJointId.Waist,Windows.Kinect.JointType.SpineBase},
        {ZgJointId.Head,Windows.Kinect.JointType.Head},
        {ZgJointId.LeftHand,Windows.Kinect.JointType.HandLeft},
        {ZgJointId.RightHand,Windows.Kinect.JointType.HandRight}
    };
    private static Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }

    bool mIsTracking = false; //are we tracking
    ulong mTrackingId = 0; //body id being tracked
    BodySourceManager mSource = null;
    ZgManager mZig;
	public void initialize(ZgManager aZig)
	{
        mZig = aZig;
        mSource = ManagerManager.Manager.gameObject.AddComponent<BodySourceManager>();
        //var view = ManagerManager.Manager.gameObject.AddComponent<BodySourceView>();
        //view.BodySourceManager = mSource.gameObject;
	}
	
	public void update()
	{

        //TODO you will need to call this whenever a player does something like a T gesture 
        //or fuck just call it all the time when your in play mode...
        //Windows::Xbox::Input::InputManager::DeferSystemGestures().
		

        //TODO update depth and image texture 
        //NOTE consider doing image texture on an as needed basis

        //update tracking data
        Windows.Kinect.Body[] data = mSource.GetData();
        if (data == null)
        {
            mIsTracking = false;
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            }
        }

        if (trackedIds.Count > 0)
        {
            if (mIsTracking == true && !trackedIds.Contains(mTrackingId))
            {
                mTrackingId = trackedIds [0];
            }
            mIsTracking = true;
        } else
        {
            mIsTracking = false;
        }

        if(mIsTracking)
            ManagerManager.Manager.mDebugString2 = "tracking " + ((int)mTrackingId).ToString();
        else ManagerManager.Manager.mDebugString2 = "NOT tracking " + ((int)mTrackingId).ToString();


        foreach(var body in data)
        {
            if (mIsTracking && body.TrackingId == mTrackingId)
            {
                ManagerManager.Manager.mDebugString2 = "legit tracking " + ((int)mTrackingId).ToString();
                ZgTrackedUser tu = new ZgTrackedUser((int)mTrackingId);
                tu.SkeletonTracked = true;
                tu.PositionTracked = true;
                for(int i = 0; i < tu.Skeleton.Count(); i++)
                {
                    if(sJointTypeMap.Keys.Contains(tu.Skeleton[i].Id))
                    {
                        tu.Skeleton[i].Position = GetVector3FromJoint(body.Joints[sJointTypeMap[tu.Skeleton[i].Id]]);
                        tu.Skeleton[i].GoodPosition = true;

                    }
                }
                ManagerManager.Manager.mZigManager.Zig_UpdateUser(tu);
            }
        }
	}
	
	public bool has_user()
	{
		return mIsTracking;
	}
	
	public ZgInput ZgInput
	{
		get
		{
			
			return null;
		}
	}
}



/* Native plugin stuff
public class MicrosoftZig : ZigInterface
{
    [AOT.MonoPInvokeCallbackAttribute(typeof(Kinect.KinectPlugin.OnTrackedPlayerChangedNativeCallback))]
    public static void callback(int a1,ulong a2,bool a3)
    {
        Debug.Log("player track change received");
        ManagerManager.Manager.mDebugString2 = "CB: player id: " + a1 + " tracking id: " + a2 + " is tracked: " + a3;
    }

    ZigInput mZigInput = null;
    public void initialize(ZigManager aZig)
    {
        Debug.Log("initializing MIcrosoftZig");
        Kinect.SensorManager.Create();
        var sensor = Kinect.SensorManager.Inst;
        sensor.Open();
        Kinect.GestureManager.Create();
        var gest = Kinect.GestureManager.Inst.Initialize();

        Kinect.KinectPlugin.KinectGestures_SetOnTrackedPlayerChangedCallback(callback);
        Debug.Log("opened sensor and registered callback");

        mZigInput = aZig.mManager.gameObject.AddComponent<ZigInput>();
        mZigInput.UpdateCallback += update_cb;
        //this is important!, this is the only way to get output from ZigInput
        mZigInput.AddListener(ManagerManager.Manager.gameObject);

        //TODO initialize xbone kinect stuff
    }

    //TODO this function needs to be called
    public void update()
    {
        //TODO update ZigTrackedUser
        //TODO update ZigInput
    }
    
    public void update_cb(List<GameObject> aObj)
    {
        foreach (var e in aObj)
        {
            //e.SendMessage("Zig_Update",mZigInput);
        }
    }
    
    public bool has_user()
    {
        return false;
    }
    
    public ZigInput ZigInput
    {
        get
        {
            
            return null;
        }
    }
}*/
