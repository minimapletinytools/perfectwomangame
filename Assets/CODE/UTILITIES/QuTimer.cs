using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuTimer{
    private HashSet<float> mTriggerList = new HashSet<float>();
    private float mStart,mCurrent,mTarget;
    //private bool mPaused = false;
    public QuTimer(float aStart = 0, float aTarget = 0)
    {
        mStart = aStart;
        mCurrent = aStart;
        mTarget = aTarget;
    }
    public float getCurrent()
    {
        return mCurrent;
    }
	public void update(float delta)
	{
		mCurrent+=delta;
	}
    public void reset()
	{
		mCurrent = mStart;
        mTriggerList.Clear();
	}

	//TODO: change this to "isIntializied"
    public bool isInitialized()
	{
		return mTarget != mStart;
	}
	
	public void deinitialize()
	{
		mTarget = mStart;
        mTriggerList.Clear();
	}
	
    public void setTarget(float aTarget)
	{
		mTarget = aTarget;
	}
    public void setTargetAndReset(float aTarget)
	{
		setTarget(aTarget);
		reset();
	}
    public void expire()
	{
		mCurrent = mTarget;
	}
	//timer expires when mCurrent is >= mTarget
    public bool isExpired()
	{
		return mCurrent >= mTarget;
	}
    //this function returns true the first time it is called when aTime <= mCurrent and false after that. Resets on reset
    public bool hasTriggered(float aTime)
    {
        if (aTime <= mCurrent && !mTriggerList.Contains(aTime))
        {
            mTriggerList.Add(aTime);
            return true;
        }
        return false;
    }
    public float getTimeSinceStart()
	{
		return mCurrent-mStart;
	}
    public float getTImeSinceExpired()
    {
        return mCurrent - mTarget;
    }
    public float getLinear()
	{
		if(isExpired())
			return 1;
		return (float)(mCurrent-mStart)/(float)(mTarget-mStart);
	}
    public float getLinear(float l, float r)
	{
		return getLinear()*(r-l)+l;
	}
    public float getSquareRoot()
	{
		return Mathf.Sqrt(getLinear());
	}
	public float getOverShot()
	{
		return Mathf.Sin(getSquareRoot()*(Mathf.PI*0.5f+0.15f));
	}
    public float getSquare()
	{
		float r = getLinear();
		return r*r;
	}
    public float getCubed(float l = 0, float r = 1)
	{
		float v = getLinear();
		return v*v*v*(r-l)+l;
	}
	//0 and 0 and 1, max at 0.5
    public float getUpsidedownParabola()
	{
		float x = getLinear();
		return (float)(-(x-0.5)*(x-0.5) + 0.25)*4;
	}
	//1 at 0 and 1, 0 at 0.5
    public float getParabola(float l = 0, float r = 1)
	{
		return (1-getUpsidedownParabola())*(r-l)+l;
	}
	
}
