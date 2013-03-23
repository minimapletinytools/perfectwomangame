using UnityEngine;
using System.Collections;

public class FlatCameraManager{
    
    public Camera Camera{ get; private set; }
    public CameraInterpolator Interpolator
    { get; private set; }

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
                return Camera.orthographicSize * Camera.aspect * 2;
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
        Interpolator = new CameraInterpolator(this.Camera);
    }
	
    public void update(float aDeltaTime)
    {
		//chis is dumb. we don't need the camera interplotaor...
        Interpolator.update(aDeltaTime);
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
	
	//TODO rename these functions, make them call each other pfftt
	
	public static void fit_camera_to_screen(Camera aCam)
	{
		//comment out this function to disable black bars
		float desiredAspect = ManagerManager.FORCED_ASPECT_RATIO;
		float screenRatio = Screen.width / (float)Screen.height;
		Rect newRect = aCam.rect;
        if (desiredAspect > screenRatio) //match camera width to screen width
		{
			float yGive = screenRatio/desiredAspect; //desiredHeight to screenHeight
			newRect.y = (1-yGive)/2;
			newRect.height = yGive;
		}
        else
		{
			float xGive = desiredAspect/screenRatio; //screen width to camera width
			newRect.x = (1-xGive)/2;
			newRect.width = xGive;
		}
		aCam.rect = newRect;
		aCam.aspect = desiredAspect;
		aCam.orthographicSize = ManagerManager.DESIRED_SCENE_HEIGHT/2f;
	}
	
	public void fit_camera_to_screen(bool hard = true)
	{
		
		Interpolator.TargetOrthographicHeight = ManagerManager.DESIRED_SCENE_HEIGHT/2f;
		Camera.orthographicSize = ManagerManager.DESIRED_SCENE_HEIGHT/2f;
		
		//this is for black bars
		if(hard)
			fit_camera_to_screen(Camera);
	}
	
    public void focus_camera_on_element(FlatElementBase aElement)
    {
        Rect focus = aElement.BoundingBox;
             //TODO what if camera is not orthographic
        float texRatio = focus.width / (float)focus.height;
        float camRatio = this.Camera.aspect;
        if (camRatio > texRatio) //match width
            Interpolator.TargetOrthographicHeight = BodyManager.convert_units(focus.width / camRatio) / 2.0f;
        else
            Interpolator.TargetOrthographicHeight = BodyManager.convert_units(focus.height) / 2.0f;
        Vector3 position = aElement.HardPosition;
        position.z = Center.z + Distance;
        Interpolator.TargetSpatialPosition = new SpatialPosition(position, Camera.transform.rotation);
    }

    //other stuff
    //returns world point from coordinates relative to center of screen where screen is (-1,1)x(-1,1)
    public Vector3 get_point(float aX, float aY)
    {
        return Center + new Vector3(aX * Width / 2.0f, aY * Height / 2.0f, 0);
    }
}
