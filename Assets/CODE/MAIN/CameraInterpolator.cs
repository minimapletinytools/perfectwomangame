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

    public SpatialPosition TargetSpatialPosition
    {
        get;
        set;
    }

    public virtual float SoftInterpolation { get; set; }
    public CameraInterpolator(Camera c)
    {
        this.Camera = c;
        TargetFOV = c.fieldOfView;
        TargetOrthographicHeight = c.orthographicSize;
        TargetSpatialPosition = new SpatialPosition(c.transform);
        SoftInterpolation = 1f;
    }

    public void update(float aDeltaTime)
    {
        SpatialPosition nsp = SpatialPosition.interpolate_linear(new SpatialPosition(Camera.transform), TargetSpatialPosition, SoftInterpolation);
        Camera.transform.position = nsp.p;
        Camera.transform.rotation = nsp.r;

        Camera.orthographicSize = Camera.orthographicSize * (1 - SoftInterpolation) + TargetOrthographicHeight * SoftInterpolation;
        Camera.fieldOfView = Camera.fieldOfView * (1 - SoftInterpolation) + TargetFOV * SoftInterpolation;
    }


}
