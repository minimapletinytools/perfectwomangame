using UnityEngine;
using System.Collections.Generic;

public class FlatElementBase {

    
    class TimedEventHandler
    {
        LinkedList<KeyValuePair<QuTimer, FlatElementAnimations.ElementAnimationDelegate>> mTimedEvents = new LinkedList<KeyValuePair<QuTimer, FlatElementAnimations.ElementAnimationDelegate>>();
        public void update(float aDeltaTime, FlatElementBase aElement)
        {
            foreach (KeyValuePair<QuTimer, FlatElementAnimations.ElementAnimationDelegate> e in mTimedEvents)
            {
                e.Key.update(aDeltaTime);
                if (e.Key.isExpired())
                {
                    if (e.Value(aElement, aDeltaTime))
                        mTimedEvents.Remove(e);
                }
            }
        }
        public void add_event(FlatElementAnimations.ElementAnimationDelegate aEvent, float aTime)
        {
            mTimedEvents.AddLast(new KeyValuePair<QuTimer,FlatElementAnimations.ElementAnimationDelegate>(new QuTimer(0,aTime),aEvent));
        }
    }

    public GameObject PrimaryGameObject
    {
        get;
        protected set;
    }

    public float SoftInterpolation{get;set;}
    //public TimedEventHandler Events { get; set; }

    Vector3 mCurrentPosition;
    Vector3 mTargetPosition;
    public Vector3 mLocalPosition = Vector3.zero;
    public Vector3 SoftPosition
    {
        get{ return mTargetPosition; }
        set{ mTargetPosition = value; }
    }
    public Vector3 HardPosition
    {
        get { return mCurrentPosition; }
        set { mCurrentPosition = mTargetPosition = value; }
    }

    Quaternion mCurrentRotation;
    Quaternion mTargetRotation;
    public Quaternion mLocalRotation = Quaternion.identity;
    public float SoftFlatRotation
    {
        get { return mTargetRotation.flat_rotation(); }
        set { mTargetRotation = Quaternion.AngleAxis(value, Vector3.forward); }
    }
    public float HardFlatRotation
    {
        get { return mCurrentRotation.flat_rotation(); }
        set { mCurrentRotation = mTargetRotation = Quaternion.AngleAxis(value, Vector3.forward); }
    }

    Color mCurrentColor;
    Color mTargetColor;
    public Color mLocalColor = new Color(0,0,0,0);
    public Color SoftColor
    {
        get { return mTargetColor; }
        set { mTargetColor = value; }
    }
    public Color HardColor
    {
        get { return mCurrentColor; }
        set { mCurrentColor = mTargetColor = value; }
    }

    public virtual Rect BoundingBox
    {
        get;
        private set;
    }

    public FlatElementBase()
    {
        BoundingBox = new Rect(0, 0, 0, 0);
        SoftInterpolation = 0.5f;
        //Events = new TimedEventHandler();
    }






    protected virtual void set_position(Vector3 aPos)
    {
        if (PrimaryGameObject != null)
        {
            PrimaryGameObject.transform.position = aPos;
        }
    }
    protected virtual void set_rotation(Quaternion aRot)
    {
        if (PrimaryGameObject != null)
        {
            PrimaryGameObject.transform.rotation = aRot;
        }
    }
    protected virtual void set_color(Color aColor)
    {
        if (PrimaryGameObject != null && PrimaryGameObject.renderer != null && PrimaryGameObject.renderer.material != null)
        {
            PrimaryGameObject.renderer.material.color = aColor;
        }
    }



    public virtual void update(float aDeltaTime)
    {
        mCurrentPosition = (1 - SoftInterpolation) * mCurrentPosition + SoftInterpolation * mTargetPosition;
        set_position(mCurrentPosition + mLocalPosition);
        mCurrentRotation = Quaternion.Slerp(mCurrentRotation, mTargetRotation, SoftInterpolation);
        set_rotation(mLocalRotation*mCurrentRotation);
        mCurrentColor = (1 - SoftInterpolation) * mCurrentColor + SoftInterpolation * mTargetColor;
        set_color(mCurrentColor);

        //TODO
    }

}
