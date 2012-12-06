using UnityEngine;
using System.Collections;

public class FlatCameraManager{
    
    public Camera Camera{ get; private set; }
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
        get { return Camera.isOrthoGraphic; }
        private set { Camera.isOrthoGraphic = true; }
    }
    public float Width
    {
        get
        {
            if (IsOrthographic)
                return Camera.orthographicSize * Camera.aspect;
            else
                return Distance * Mathf.Tan(Camera.fov / 2.0f);
        }
    }
    public float Height
    {
        get { return Width / Camera.aspect; }
    }
    public FlatCameraManager(Vector3 aCenter, float aDistance)
    {
        Center = aCenter;
        Distance = aDistance;
        create_camera();
        IsOrthographic = true;
    }

    //for setting camera
    void create_camera()
    {
        Camera = (new GameObject("genFlatCamera")).AddComponent<Camera>();
        Camera.transform.position = Center + Vector3.forward * Distance;
        Camera.transform.LookAt(Center, Vector3.up);
        Camera.nearClipPlane = 0.1f;
        Camera.farClipPlane = 1000;
        Camera.depth = 100;
        Camera.clearFlags = CameraClearFlags.Depth;
    }


    //other stuff
    //returns world point from coordinates relative to center of screen where screen is (-1,1)x(-1,1)
    public Vector3 get_point(float aX, float aY)
    {
        return Center + new Vector3(aX * Width / 2.0f, aY * Height / 2.0f, 0);
    }
}
