using UnityEngine;
using System.Collections.Generic;

//XBONE stuff
//NOTE you will still need to remove the zig folder

#if !UNITY_STANDALONE
public enum ZigJointId
{
    None,
    Head,
    Neck,
    Torso,
    Waist,
    LeftCollar,
    LeftShoulder,
    LeftElbow,
    LeftWrist,
    LeftHand,
    LeftFingertip,
    RightCollar,
    RightShoulder,
    RightElbow,
    RightWrist,
    RightHand,
    RightFingertip,
    LeftHip,
    LeftKnee,
    LeftAnkle,
    LeftFoot,
    RightHip,
    RightKnee,
    RightAnkle,
    RightFoot
}

public class ZigTrackedUser
{
    public int Id
    {
        get;
        private set;
    }
    
    public Vector3 Position
    {
        get;
        private set;
    }
    
    public bool PositionTracked
    {
        get;
        private set;
    }
    
    public ZigInputJoint[] Skeleton
    {
        get;
        private set;
    }
    
    public bool SkeletonTracked
    {
        get;
        private set;
    }

    public ZigTrackedUser (int aId)
    {
        Id = aId;
        this.Skeleton = new ZigInputJoint[Enum.GetValues (typeof(ZigJointId)).get_Length ()];
        for (int i = 0; i < this.Skeleton.Length; i++)
        {
            this.Skeleton [i] = new ZigInputJoint ((ZigJointId)i);
        }
    }
}

public class ZigInputJoint
{
    public bool GoodRotation;
    
    public bool Inferred;
    
    public bool GoodPosition;
    
    public Vector3 Position;
    
    public Quaternion Rotation;

    public ZigJointId Id
    {
        get;
        private set;
    }

    public ZigInputJoint (ZigJointId id, Vector3 position, Quaternion rotation, bool inferred)
    {
        this.Id = id;
        this.Position = position;
        this.Rotation = rotation;
        this.Inferred = inferred;
    }
    
    public ZigInputJoint (ZigJointId id) : this (id, Vector3.get_zero (), Quaternion.get_identity (), false)
    {
        this.GoodPosition = false;
        this.GoodRotation = false;
    }
}

//TODO need to expand this with kinect2.0 resolutions
public enum ZigResolution
{
    QQVGA_160x120,
    QVGA_320x240,
    VGA_640x480,
}

public class ResolutionData
{
    protected ResolutionData(int width, int height)
    {
        Width = width;
        Height = height;
    }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public static ResolutionData FromZigResolution(ZigResolution res)
    {
        switch (res) {
            default: //fallthrough - default to QQVGA
            case ZigResolution.QQVGA_160x120:
                return new ResolutionData(160, 120);
            case ZigResolution.QVGA_320x240:
                return new ResolutionData(320, 240);
            case ZigResolution.VGA_640x480:
                return new ResolutionData(640, 480);
        }
        
    }
}

#endif