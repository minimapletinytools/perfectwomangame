using UnityEngine;
using System.Collections.Generic;

public class FlatElementBase {

    
    public class TimedEventHandler
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

    GameObject mPrimaryGameObject;
    public GameObject PrimaryGameObject
    {
        get
        {
            return mPrimaryGameObject;
        }
        protected set
        {
            mPrimaryGameObject = value;
            mBaseScale = mPrimaryGameObject.transform.localScale;
        }
    }

    bool mEnabled = true;
    public virtual bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            if (mEnabled != value)
            {
                mEnabled = value;
                foreach (Renderer e in PrimaryGameObject.GetComponentsInChildren<Renderer>())
                    e.enabled = value;
            }
            
        }

    }

    public virtual float SoftInterpolation{get;set;}
    public TimedEventHandler Events { get; private set; }


    Vector3 mCurrentPosition;
    Vector3 mTargetPosition;
    public Vector3 mLocalPosition = Vector3.zero;
    public virtual Vector3 SoftPosition
    {
        get{ return mTargetPosition; }
        set{ mTargetPosition = value; }
    }
    public virtual Vector3 HardPosition
    {
        get { return mCurrentPosition; }
        set 
        { 
            mCurrentPosition = value; 
            mTargetPosition = value; 
        }
    }

    Vector3 mBaseScale;
    Vector3 mCurrentScale;
    Vector3 mTargetScale;
    public Vector3 mLocalScale = Vector3.one;
    public virtual Vector3 SoftScale
    {
        get { return mTargetScale; }
        set { mTargetScale = value; }
    }
    public virtual Vector3 HardScale
    {
        get { return mCurrentScale; }
        set { mCurrentScale = mTargetScale = value; }
    }

    Quaternion mCurrentRotation;
    Quaternion mTargetRotation;
    public Quaternion mLocalRotation = Quaternion.identity;
    public virtual float SoftFlatRotation
    {
        get { return mTargetRotation.flat_rotation(); }
        set { mTargetRotation = Quaternion.AngleAxis(value, Vector3.forward); }
    }
    public virtual float HardFlatRotation
    {
        get { return mCurrentRotation.flat_rotation(); }
        set { mCurrentRotation = mTargetRotation = Quaternion.AngleAxis(value, Vector3.forward); }
    }

    Color mCurrentColor;
    Color mTargetColor;
    public Color mLocalColor = new Color(0,0,0,0);
    public virtual Color SoftColor
    {
        get { return mTargetColor; }
        set { mTargetColor = value; }
    }
    public virtual Color HardColor
    {
        get { return mCurrentColor; }
        set { mCurrentColor = mTargetColor = value; }
    }

    public virtual Rect BoundingBox
    {
        get;
        private set;
    }

    protected int mDepth = 0;
    public virtual int Depth
    {
        get { return mDepth; }
        set
        {
            mDepth = value;
            foreach (Renderer e in PrimaryGameObject.GetComponentsInChildren<Renderer>())
                e.material.renderQueue = mDepth;
        }
    }

    public virtual void destroy()
    {
    }

    public FlatElementBase()
    {
        BoundingBox = new Rect(0, 0, 0, 0);
        SoftInterpolation = 0.3f;
        HardScale = Vector3.one;
        Events = new TimedEventHandler();
    }



    public virtual void set_position(Vector3 aPos)
    {
        if (PrimaryGameObject != null)
        {
            PrimaryGameObject.transform.position = aPos;
        }
    }
    public virtual void set_scale(Vector3 aScale)
    {
        if (PrimaryGameObject != null)
        {
            PrimaryGameObject.transform.localScale = aScale;
        }
    }
    public virtual void set_rotation(Quaternion aRot)
    {
        if (PrimaryGameObject != null)
        {
            PrimaryGameObject.transform.rotation = aRot;
        }
    }
    public virtual void set_color(Color aColor)
    {
        if (PrimaryGameObject != null)
        {
            foreach (Renderer e in PrimaryGameObject.GetComponentsInChildren<Renderer>())
            {
                try { e.material.SetColor("_TintColor", aColor); }
                catch { }
                try { e.material.color = aColor; }
                catch { }
            }
            /*
            Renderer rend = PrimaryGameObject.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                try { rend.material.SetColor("_TintColor", aColor); }
                catch { }
                try { rend.material.color = aColor; }
                catch { }
            }*/
        }
    }
    

    public void update(float aDeltaTime)
    {
        Events.update(aDeltaTime, this);
        update_parameters(aDeltaTime);
        set();
    }
    public virtual void update_parameters(float aDeltaTime)
    {
        mCurrentPosition = (1 - SoftInterpolation) * mCurrentPosition + SoftInterpolation * mTargetPosition;
        mCurrentRotation = Quaternion.Slerp(mCurrentRotation, mTargetRotation, SoftInterpolation);
        mCurrentScale = (1 - SoftInterpolation) * mCurrentScale + SoftInterpolation * mTargetScale;
        mCurrentColor = (1 - SoftInterpolation) * mCurrentColor + SoftInterpolation * mTargetColor;
    }

    public virtual void set()
    {
        set_position(mCurrentPosition + mLocalPosition);// + new Vector3(0,0,Depth));
        set_scale(mBaseScale.component_multiply(mCurrentScale).component_multiply(mLocalScale));
        set_rotation(mLocalRotation*mCurrentRotation);
        set_color(mCurrentColor + mLocalColor);
    }

}
