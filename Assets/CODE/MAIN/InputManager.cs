using UnityEngine;
using System.Collections;

public class InputManager : FakeMonoBehaviour
{
    public class MultiMouseProfile
    {
        public System.Collections.Generic.Dictionary<uint, VectorSignal> mInputs = new System.Collections.Generic.Dictionary<uint, VectorSignal>();
        public void touch_down(uint id, Vector3 pos)
        {
            mInputs[id] = new VectorSignal();
            mInputs[id].add_absolute(pos, Time.time);
        }
        public VectorSignal touch_up(uint id)
        {
            if (!mInputs.ContainsKey(id))
                throw new UnityException("multimouse profile does not contain touch id " + id);
            VectorSignal r = mInputs[id];
            mInputs.Remove(id);
            return r;
        }
        public void touch_move(uint id, Vector3 pos)
        {
            if (!mInputs.ContainsKey(id))
                throw new UnityException("multimouse profile does not contain touch id " + id);
            mInputs[id].add_absolute(pos, Time.time);
        }

    }
    public class MouseProfile
    {
        public VectorSignal mPositions;
        public int mButton;
        public MouseProfile(int aButton = 0)
        {
            mButton = aButton;
            mPositions = new VectorSignal();//(to_relative(Input.mousePosition),Time.time);
        }

        public Vector3 get_last_mouse_position_relative() { return mPositions.get_last(); }
        public Vector3 get_last_mouse_position_absolute() { return to_absolute(mPositions.get_last()); }
        public Vector3 get_last_mouse_change_relative()
        {
            return mPositions.get_last_value_difference();
        }
        public static Vector3 to_absolute(Vector3 v)
        {
            Vector3 r = v;
            r.x *= Screen.width;
            r.y *= Screen.height;
            return r;
        }

        public static Vector3 to_relative(Vector3 v)
        {
            Vector3 r = v;
            r.x /= Screen.width;
            r.y /= Screen.height;
            return r;
        }
    };


    public class MouseEventHandler
    {
        public delegate bool MouseHandlerDelegate(MouseProfile mouse);
        public delegate void VoidMouseHandlerDelegate(MouseProfile mouse);
        public delegate void PinchHandlerDelegate(float change);
        public MouseHandlerDelegate mMousePressed;
        public VoidMouseHandlerDelegate mMouseMoved;
        public VoidMouseHandlerDelegate mMouseReleased;
        public PinchHandlerDelegate mPinch;

        public bool mMouseDown = false;
        public float mTimeDown = 0;
        public MouseEventHandler() { mMouseReleased += mouse_released; }
        public void mouse_pressed() { mMouseDown = true; mTimeDown = Time.time; }
        public float time_down() { return Time.time - mTimeDown; }
        public bool was_mouse_pressed() { return mMouseDown; }
        void mouse_released(MouseProfile mouse) { mMouseDown = false; }
    }

    MouseProfile mMouse = new MouseProfile();

    public InputManager(ManagerManager aManager) : base(aManager) { }

    public void handle_pinch(MouseEventHandler mHandler)
    {
        //TODO actually handle pinch
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        mHandler.mPinch(zoom);
    }

    public bool handle_mouse(MouseEventHandler mHandler)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (mHandler.mMousePressed(mMouse))
            {
                mHandler.mouse_pressed();
                return true;
            }
            return false;
        }
        if (!mHandler.was_mouse_pressed()) return false;
        if(mHandler.mMouseMoved != null)
            mHandler.mMouseMoved(mMouse);
        if (Input.GetMouseButtonUp(0))
            if(mHandler.mMouseReleased != null)
                mHandler.mMouseReleased(mMouse);
        return true;
    }
    public override void Start()
    {
    }
    public override void Update()
    {
        for (int i = 0; i < 1; i++)
        {
            if (Input.GetMouseButtonDown(i))
                mMouse = new MouseProfile(i);
            if (Input.GetMouseButton(i))
                mMouse.mPositions.add_absolute(MouseProfile.to_relative(Input.mousePosition), Time.time);

            if(!handle_mouse(mManager.mFlatViewManager.mMouseHandler))
                if (!handle_mouse(mManager.mThreeViewManager.mManipulationMouseHandler))
                    handle_mouse(mManager.mThreeViewManager.mCameraMouseHandler);
        }

        handle_pinch(mManager.mThreeViewManager.mCameraMouseHandler);
    }
}
