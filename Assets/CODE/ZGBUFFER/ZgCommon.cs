using UnityEngine;
using System.Collections;
using System;


//TODO need new resolutions for kinect2.0
public enum ZgResolution
{
    QQVGA_160x120,
    QVGA_320x240,
    VGA_640x480,
}

public class ZgResolutionData
{
    protected ZgResolutionData(int width, int height)
    {
        Width = width;
        Height = height;
    }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public static ZgResolutionData FromZgResolution(ZgResolution res)
    {
        switch (res) {
            default: //fallthrough - default to QQVGA
            case ZgResolution.QQVGA_160x120:
                return new ZgResolutionData(160, 120);
            case ZgResolution.QVGA_320x240:
                return new ZgResolutionData(320, 240);
            case ZgResolution.VGA_640x480:
                return new ZgResolutionData(640, 480);
        }
        
    }
}



public class ZgDepth
{
    public short[] data = null;
    public int yres;
    public int xres;
    Texture2D tex = null;
    public ZgDepth (int x, int y, short[] aData = null)
    {
        this.xres = x;
        this.yres = y;
		if (aData == null)
			this.data = new short[x * y];
		else
			this.data = aData;
    }
    public ZgDepth(Texture2D aTex)
    {
        tex = aTex;
    }
}

public class ZgLabelMap
{
    public short[] data;
    public int xres;
    public int yres;
    Texture2D tex = null;
    
    public ZgLabelMap (int x, int y, short[] aData)
    {
        this.xres = x;
        this.yres = y;
		if (aData == null)
			this.data = new short[x * y];
		else
			this.data = aData;
    }

    public ZgLabelMap(Texture2D aTex)
    {
        tex = aTex;
    }
}
public class ZgImage
{
    public Color32[] data = null;
    public int xres;
    public int yres;
    Texture2D tex = null;
    public ZgImage (int x, int y, Color32[] aData)
    {
        this.xres = x;
        this.yres = y;
		if (aData == null)
			this.data = new Color32[x * y];
		else
			this.data = aData;
    }
    public ZgImage(Texture2D aTex)
    {
        tex = aTex;
    }
}


public class ZgTrackedUser
{
    public int Id
    {
        get;
        private set;
    }
    
    public Vector3 Position
    {
        get;
        set;
    }
    
    public bool PositionTracked
    {
        get;
        set;
    }
    
    public ZgInputJoint[] Skeleton
    {
        get;
        set;
    }
    
    public bool SkeletonTracked
    {
        get;
        set;
    }
    
    public ZgTrackedUser (int aId)
    {
        Id = aId;
		Skeleton = null;
		SkeletonTracked = false;
		PositionTracked = false;

        this.Skeleton = new ZgInputJoint[Enum.GetValues (typeof(ZgJointId)).Length];
        for (int i = 0; i < this.Skeleton.Length; i++)
        {
            this.Skeleton [i] = new ZgInputJoint ((ZgJointId)i);
        }
    }
}

public class ZgInput
{
    public static ZgDepth Depth {get; set;}
    public static ZgImage Image {get; set;}
    public static ZgLabelMap LabelMap {get; set;}
    public bool ReaderInited {get; set;}
    public bool kinectSDK = false;
}


public enum ZgJointId
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


public class ZgInputJoint
{
    public bool GoodRotation;
    
    public bool Inferred;
    
    public bool GoodPosition;
    
    public Vector3 Position;
    
    public Quaternion Rotation;
    
    public ZgJointId Id
    {
        get;
        private set;
    }
    
    public ZgInputJoint (ZgJointId id, Vector3 position, Quaternion rotation, bool inferred)
    {
        this.Id = id;
        this.Position = position;
        this.Rotation = rotation;
        this.Inferred = inferred;
    }
    
    public ZgInputJoint (ZgJointId id) : this (id, Vector3.zero, Quaternion.identity, false)
    {
        this.GoodPosition = false;
        this.GoodRotation = false;
    }
}