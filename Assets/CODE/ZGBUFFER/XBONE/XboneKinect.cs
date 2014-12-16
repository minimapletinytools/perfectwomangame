using UnityEngine;
using System.Collections.Generic;
using System.Linq;


#if UNITY_XBOXONE

using Kinect;

public class XboneKinect{
    /* reverse map...
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
		{ZgJointId.Neck,Kinect.JointType.Neck},
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
     */

    public static Dictionary<Kinect.JointType,ZgJointId> sJointTypeMap = new Dictionary<Kinect.JointType,ZgJointId>()
    {
        {Kinect.JointType.ShoulderLeft,ZgJointId.LeftShoulder},
        {Kinect.JointType.ElbowLeft,ZgJointId.LeftElbow},
        {Kinect.JointType.HipLeft,ZgJointId.LeftHip},
        {Kinect.JointType.KneeLeft,ZgJointId.LeftKnee},
        {Kinect.JointType.AnkleLeft,ZgJointId.LeftAnkle},
        {Kinect.JointType.ShoulderRight,ZgJointId.RightShoulder},
        {Kinect.JointType.ElbowRight,ZgJointId.RightElbow},
        {Kinect.JointType.HipRight,ZgJointId.RightHip},
        {Kinect.JointType.KneeRight,ZgJointId.RightKnee},
        {Kinect.JointType.AnkleRight,ZgJointId.RightAnkle},
        {Kinect.JointType.Neck,ZgJointId.Neck},
        {Kinect.JointType.SpineMid,ZgJointId.Torso},
        {Kinect.JointType.SpineBase,ZgJointId.Waist},
        {Kinect.JointType.Head,ZgJointId.Head},
        {Kinect.JointType.HandLeft,ZgJointId.LeftHand},
        {Kinect.JointType.HandRight,ZgJointId.RightHand}
    };
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.position.x * 10, -joint.position.y * 10, joint.position.z * 10);
    }

	


	public bool IsTracking{ get; private set; } //are we tracking
    public bool IsReaderConnected {get{return SensorManager.IsOpen();}}

	ulong mTrackingId = 0; //body id being tracked
    Texture2D mDepthTexture = null;
    Texture2D mColorTexture = null;
    Texture2D mLabelTexture = null;
    public Texture2D DepthTexture{ get{ return mDepthTexture; } private set{ mDepthTexture = value; }} 
    public Texture2D ColorTexture{ get{ return mColorTexture; } private set{ mColorTexture = value; }} 
    public Texture2D LabelTexture{ get{ return mLabelTexture; } private set{ mLabelTexture = value; }} 
	


    public void DeferSystemGestures(float aTime)
    {
        XboxOneInput.DeferSystemGestures(aTime);
    }


	public void Start()
	{
		SensorManager.Create ();
		if (!SensorManager.IsOpen())
        {
            //Debug.Log("Opening Kinect Sensor");
            SensorManager.Open();
        }
        if (SensorManager.IsOpen())
        {
            bool bf = SensorManager.bodyFrameReader.Open();
            bool bif = SensorManager.bodyIndexFrameReader.Open();
            bool cf = SensorManager.colorFrameReader.Open();
            bool df = SensorManager.depthFrameReader.Open();
            ManagerManager.Log("Kinect initialization status: " + bf + bif + cf + df);

        } else
        {
            Debug.Log("Kinect Sensor is not open");
        }
        CreateKinectImageTexture(ref mDepthTexture, SensorManager.depthFrameReader, TextureFormat.R16, "Depth");
        CreateKinectImageTexture(ref mColorTexture, SensorManager.colorFrameReader, TextureFormat.YUY2, "Color");
        CreateKinectImageTexture(ref mLabelTexture, SensorManager.bodyIndexFrameReader, TextureFormat.Alpha8, "Label");
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
		
		//if(IsTracking)
			//ManagerManager.Manager.mDebugString2 = "tracking " + ((int)mTrackingId).ToString();
		//else ManagerManager.Manager.mDebugString2 = "NOT tracking " + ((int)mTrackingId).ToString();
		
		
		foreach (var e in SensorManager.BodyList.list)
		{
			if (IsTracking && e.trackingId == mTrackingId)
			{
				//ManagerManager.Manager.mDebugString2 = "real tracking " + ((int)mTrackingId).ToString();
				ZgTrackedUser tu = new ZgTrackedUser((int)mTrackingId);
				tu.SkeletonTracked = true;
				tu.PositionTracked = true;
                var jointsSeg = e.joints;
                for(int i = jointsSeg.Offset; i < (jointsSeg.Offset + jointsSeg.Count); i++)
                {
                    var jp = jointsSeg.Array[i];
                    for(int j = 0; j < tu.Skeleton.Count(); j++)
                    {
                        if(sJointTypeMap.ContainsKey(jp.jointType) && sJointTypeMap[jp.jointType] == tu.Skeleton[j].Id)
                        {
                            tu.Skeleton[j].Position = GetVector3FromJoint(jp);
                            tu.Skeleton[j].GoodPosition = true;
                            //TODO rotation
                        }
                    }
                }
                /* this way does not seem to work for osme reason..
				for(int i = 0; i < tu.Skeleton.Count(); i++)
				{
					if(sJointTypeMap.Keys.Contains(tu.Skeleton[i].Id))
					{
                        try{
    						tu.Skeleton[i].Position = GetVector3FromJoint(e.joints.Array.First(f=>f.jointType == sJointTypeMap[tu.Skeleton[i].Id]));
    						tu.Skeleton[i].GoodPosition = true;
                        }catch{Debug.Log ("couldn't find joint " + tu.Skeleton[i].Id);}
						
					}
				}*/
				ManagerManager.Manager.mZigManager.Zig_UpdateUser(tu);
                break;
			}
		}

        //DEPTH/IMAGE/USE
        FrameDescription depthFrameDesc;
        bool depthFrameDescSuccess = SensorManager.depthFrameReader.GetFrameDescription(out depthFrameDesc);
        if (depthFrameDescSuccess)
        {
            //mDepthTexture = new Texture2D(depthFrameDesc.Width, depthFrameDesc.Height, TextureFormat.R16, false);
            //byte[] depthRawData = new byte[depthFrameDesc.BytesPerPixel * depthFrameDesc.LengthInPixels];
            depthFrameDescSuccess = SensorManager.depthFrameReader.AcquireLatestFrame(mDepthTexture.GetNativeTexturePtr());
        }

        FrameDescription colorFrameDesc;
        bool colorFrameDescSuccess = SensorManager.colorFrameReader.GetFrameDescription(out colorFrameDesc);
        if(colorFrameDescSuccess)
        {
            //byte[] colorRawData = new byte[colorFrameDesc.BytesPerPixel * colorFrameDesc.LengthInPixels];
            colorFrameDescSuccess = SensorManager.colorFrameReader.AcquireLatestFrame(mColorTexture.GetNativeTexturePtr());
        }

        FrameDescription labelFrameDesc;
        bool labelFrameDescSuccess = SensorManager.bodyIndexFrameReader.GetFrameDescription(out labelFrameDesc);
        if(labelFrameDescSuccess)
        {
            //byte[] labelRawData = new byte[labelFrameDesc.BytesPerPixel * labelFrameDesc.LengthInPixels];
            labelFrameDescSuccess = SensorManager.bodyIndexFrameReader.AcquireLatestFrame(mLabelTexture.GetNativeTexturePtr());
        }
        //ManagerManager.Manager.mZigManager.DepthView.up
	}

    void CreateKinectImageTexture(ref Texture2D texture, Kinect.IImageFrameReader reader, TextureFormat fmt, string nameForLog )
    {
        FrameDescription frameDesc = new FrameDescription();
        if (reader.GetFrameDescription(out frameDesc))
        {
            texture = new Texture2D(frameDesc.Width, frameDesc.Height, fmt, false);
            texture.filterMode = FilterMode.Bilinear;
            
            // Call Apply() so it's actually uploaded to the GPU
            texture.Apply();
        }
        else
        {
            ManagerManager.Log( "Failed to get " + nameForLog + " frame desc." );
        }
    }
}

#else

//TODO DELETE
//this is using the microsoft provided kinect plugin
public class XboneKinect{
    public Texture2D DepthTexture{ get; private set;}
    public Texture2D ColorTexture{ get; private set;}
    public Texture2D LabelTexture{ get; private set;}
    
    public bool IsReaderConnected
    {
        get{
            return false;
        }
    }
    public bool IsTracking{ get; private set; } //are we tracking

    public void DeferSystemGestures(float aTime)
    {
    }
    
    public void Start()
    {
    }
    
    public void Update()
    {
    }
}

#endif


//TODO DELETE this is using the old microsoft provided unity wrapper code

/*
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
}*/
