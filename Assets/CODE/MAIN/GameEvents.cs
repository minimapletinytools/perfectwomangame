using UnityEngine;
using System.Collections.Generic;

public class GameEvents {
	
	/*
    public class FadeEvent
    {
        //bool mIn;
        public FadeEvent(bool aIn)
        {
            //mIn = aIn;
        }

        bool call(float aTime)
        {
            return true;
        }

        public GameManager.GameEventDelegate get_event()
        {
            return call;
        }
    }

    public class ResetElementScaleEvent
    {
        FlatElementBase mElement;
        public ResetElementScaleEvent(FlatElementBase aElement)
        {
            mElement = aElement;
        }
        bool call(float aTime)
        {
            mElement.SoftScale = new Vector3(1, 1, 1);
            return true;
        }

        public GameManager.GameEventDelegate get_event()
        {
            return call;
        }
    }
    public class FocusCameraOnElementEvent
    {
        FlatCameraManager mCamera;
        FlatElementBase mElement;
        public FocusCameraOnElementEvent(FlatCameraManager aCamera, FlatElementBase aElement)
        {
            mElement = aElement;
            mCamera = aCamera;
        }

        bool call(float aTime)
        {
            mElement.SoftScale = new Vector3(1.2f, 1.2f, 1f);
            mCamera.focus_camera_on_element(mElement);
            mElement.SoftScale = new Vector3(1.25f, 1.25f, 1f);
            return true;
        }

        public GameManager.GameEventDelegate get_event()
        {
            return call;
        }

    }

    public class FadeInTopChoiceInInterfaceEvent
    {
        InterfaceManager mInterface;
        public FadeInTopChoiceInInterfaceEvent(InterfaceManager aInterface)
        {
            mInterface = aInterface;
        }

        bool call(float aTime)
        {
            mInterface.fade_in_choices();
            return true;
        }

        public GameManager.GameEventDelegate get_event()
        {
            return call;
        }

    }
    */
}
