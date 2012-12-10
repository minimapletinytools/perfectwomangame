using UnityEngine;
using System.Collections;

public class CameraInterpolator 
{
    public Camera Camera
    {
        get;
        private set;
    }
    public float TargetFOV
    {
        get;
        set;
    }
    public float TargetOrthographicHeight
    {
        get;
        set;
    }
    public CameraInterpolator(Camera c)
    {
        this.Camera = c;
        TargetFOV = c.fov;
        TargetOrthographicHeight = c.orthographicSize;
    }


}
