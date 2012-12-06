using UnityEngine;
using System.Collections;

public class FlatCameraManager{
    
    Camera mCamera = null;
    public Vector3 Center
    {
        get;
        private set;
    }
    public float Distance 
    { 
        get; 
        private set; 
    }
    public bool IsOrthographic
    { 
        get;
        private set;
    }
    public float Width
    {
        get
        {
            if (IsOrthographic)
                return mCamera.orthographicSize * mCamera.aspect;
            else
                return Distance * Mathf.Tan(mCamera.fov / 2.0f);
        }
    }
    public float Height
    {
        get { return Width / mCamera.aspect; }
    }
    public FlatCameraManager(Vector3 aCenter, float aDistance)
    {
        Center = aCenter;
        Distance = aDistance;
        IsOrthographic = false;
        create_camera();
    }

    //for setting camera
    void create_camera()
    {
        mCamera = (new GameObject("genFlatCamera")).AddComponent<Camera>();
        mCamera.transform.position = Center + Vector3.forward * Distance;
        mCamera.transform.LookAt(Center, Vector3.up);
        mCamera.nearClipPlane = 0.1f;
        mCamera.farClipPlane = 1000;
    }


    //other stuff
    //returns world point from coordinates relative to center of screen where screen is (-1,1)x(-1,1)
    public Vector3 get_point(float aX, float aY)
    {
        return Center + new Vector3(aX * Width / 2.0f, aY * Height / 2.0f, 0);
    }
}
