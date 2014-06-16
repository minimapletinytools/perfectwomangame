using UnityEngine;
using System.Collections.Generic;
using System;

//XBONE stuff
//NOTE you will still need to remove the zig folder

#if !UNITY_STANDALONE

public class ZigDepth
{
    public short[] data;
    public int yres;
    public int xres;
    public ZigDepth (int x, int y)
    {
        this.xres = x;
        this.yres = y;
        this.data = new short[x * y];
    }
}

public class ZigLabelMap
{
    public short[] data;
    public int xres
    {get;private set;}
    public int yres
    {get;private set;}

    public ZigLabelMap (int x, int y)
    {
        this.xres = x;
        this.yres = y;
        this.data = new short[x * y];
    }
}
public class ZigImage
{
    public Color32[] data;
    public int xres
    {get;private set;}
    public int yres
    {get;private set;}

    public ZigImage (int x, int y)
    {
        this.xres = x;
        this.yres = y;
        this.data = new Color32[x * y];
    }
}


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
        this.Skeleton = new ZigInputJoint[Enum.GetValues (typeof(ZigJointId)).Length];
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
    
    public ZigInputJoint (ZigJointId id) : this (id, Vector3.zero, Quaternion.identity, false)
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





//dummy definitions of monobehaviour stuff that we need
public class ZigInput : MonoBehaviour
{
    List<GameObject> mListeners = new List<GameObject>();
    public static ZigDepth Depth {get; set;}
    public static ZigImage Image {get; set;}
    public static ZigLabelMap LabelMap {get; set;}
    public bool ReaderInited {get; set;}
    public bool kinectSDK = false;

    public Action<List<GameObject>> UpdateCallback { get; set; }

    public void AddListener(GameObject aOb)
    {
        mListeners.Add(aOb);
        //TODO what happens when remove game object???
    }

    void Update()
    {
        if (UpdateCallback != null)
            UpdateCallback(mListeners);
    }
}

public class ZigEngageSingleUser : MonoBehaviour
{
    public bool SkeletonTracked = true;
    public bool RaiseHand;
    public List<GameObject> EngagedUsers;
    public ZigTrackedUser engagedTrackedUser { get; private set; }
    public void Reset() {}

}

public class Zig : MonoBehaviour
{

}

#endif