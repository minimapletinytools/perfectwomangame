using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Kinect;

#if UNITY_XBOXONE

public class XboneKinect{
	public static Dictionary<ZgJointId,Kinect.JointType> sJointTypeMap = new Dictionary<ZgJointId,Kinect.JointType>()
	{
		{ZgJointId.LeftShoulder,Kinect.JointType.ShoulderLeft},
		{ZgJointId.LeftElbow,Kinect.JointType.ElbowLeft},
		{ZgJointId.LeftHip,Kinect.JointType.HipLeft},
		{ZgJointId.LeftKnee,Kinect.JointType.KneeLeft},
		{ZgJointId.LeftAnkle,Kinect.JointType.AnkleLeft},
		{ZgJointId.RightShoulder,Kinect.JointType.ShoulderRight},
		{ZgJointId.RightElbow,Kinect.JointType.ElbowRight},
		{ZgJointId.RightHip,Kinect.JointType.HipRight},
		{ZgJointId.RightKnee,Kinect.JointType.KneeRight},
		{ZgJointId.RightAnkle,Kinect.JointType.AnkleRight},
		{ZgJointId.Neck,JointType.Neck},
		{ZgJointId.Torso,Kinect.JointType.SpineMid},
		{ZgJointId.Waist,Kinect.JointType.SpineBase},
		{ZgJointId.Head,Kinect.JointType.Head},
		{ZgJointId.LeftHand,Kinect.JointType.HandLeft},
		{ZgJointId.RightHand,Kinect.JointType.HandRight}
	};
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.position.x * 10, joint.position.y * 10, joint.position.z * 10);
    }

	
	public bool IsTracking{ get; private set; } //are we tracking
	ulong mTrackingId = 0; //body id being tracked
	

    public bool IsReaderConnected
    {
        get{
            return SensorManager.IsOpen();
        }
    }
	public void Start()
	{
		SensorManager.Create ();
		if (!SensorManager.IsOpen ())
			SensorManager.Open ();
	}
	
	public void Update()
	{
        //BODY
        bool bodyReadSuccess = SensorManager.bodyFrameReader.AcquireLatestFrame(JointCoordType.DepthSpace);
		List<ulong> trackedIds = new List<ulong>();

        foreach (var e in SensorManager.BodyList.list)
        {
            if(e.isTracked)
                trackedIds.Add(e.trackingId);
        }

		if (trackedIds.Count > 0)
		{
			if (!trackedIds.Contains(mTrackingId))
				mTrackingId = trackedIds [0];
			IsTracking = true;
		} else
		{
			IsTracking = false;
		}
		
		if(IsTracking)
			ManagerManager.Manager.mDebugString2 = "tracking " + ((int)mTrackingId).ToString();
		else ManagerManager.Manager.mDebugString2 = "NOT tracking " + ((int)mTrackingId).ToString();
		
		
		foreach (var e in SensorManager.BodyList.list)
		{
			if (IsTracking && e.trackingId == mTrackingId)
			{
				ManagerManager.Manager.mDebugString2 = "real tracking " + ((int)mTrackingId).ToString();
				ZgTrackedUser tu = new ZgTrackedUser((int)mTrackingId);
				tu.SkeletonTracked = true;
				tu.PositionTracked = true;
				for(int i = 0; i < tu.Skeleton.Count(); i++)
				{
					if(sJointTypeMap.Keys.Contains(tu.Skeleton[i].Id))
					{
                        try{
						tu.Skeleton[i].Position = GetVector3FromJoint(e.joints.Array.First(f=>f.jointType == sJointTypeMap[tu.Skeleton[i].Id]));
						tu.Skeleton[i].GoodPosition = true;
                        }catch{Debug.Log ("couldn't find joint " + tu.Skeleton[i].Id);}
						
					}
				}
				ManagerManager.Manager.mZigManager.Zig_UpdateUser(tu);
			}
		}

        //DEPTH/IMAGE/USER
        /*TODO
        FrameDescription depthFrameDesc;
        bool depthFrameDescSuccess = SensorManager.depthFrameReader.GetFrameDescription(out depthFrameDesc);
        if(depthFrameDescSuccess)
        {
            byte[] depthRawData = new byte[depthFrameDesc.BytesPerPixel * depthFrameDesc.LengthInPixels];
            SensorManager.depthFrameReader.AcquireLatestFrame(depthRawData);
            ZgDepth depthImage = new ZgDepth(depthFrameDesc.Width,depthFrameDesc.Height,null);
        }
        */
        //ManagerManager.Manager.mZigManager.DepthView.up
	}
}

#else

public class XboneKinect{
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
	
	
    public bool IsReaderConnected
    {
        get{
            return mSource.GetData() != null;
        }
    }
	public bool IsTracking{ get; private set; } //are we tracking
	ulong mTrackingId = 0; //body id being tracked
	BodySourceManager mSource = null;

	public void Start()
	{
		mSource = ManagerManager.Manager.gameObject.AddComponent<BodySourceManager>();
		//var view = ManagerManager.Manager.gameObject.AddComponent<BodySourceView>();
		//view.BodySourceManager = mSource.gameObject;
	}

	public void Update()
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
			IsTracking = false;
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
			if (IsTracking == true && !trackedIds.Contains(mTrackingId))
			{
				mTrackingId = trackedIds [0];
			}
			IsTracking = true;
		} else
		{
			IsTracking = false;
		}
		
		if(IsTracking)
			ManagerManager.Manager.mDebugString2 = "tracking " + ((int)mTrackingId).ToString();
		else ManagerManager.Manager.mDebugString2 = "NOT tracking " + ((int)mTrackingId).ToString();
		
		
		foreach(var body in data)
		{
			if (IsTracking && body.TrackingId == mTrackingId)
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
}

#endif
