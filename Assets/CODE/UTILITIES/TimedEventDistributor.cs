using UnityEngine;
using System.Collections.Generic;

public class TimedEventDistributor 
{
	
	//TODO to make this even more awesome you should have a list of chains to follow!!
    public class TimedEventChain
    {
        TimedEventChain mFollow = null;
        float mTime = 0;
        System.Func<float, bool> mEvent = null;
        bool mDone = false;
        float mFollowTimer = 0;
        float mTimeDone = 0;
		
		
        TimedEventChain()
        {
        }
        public TimedEventChain then(System.Func<float, bool> aEvent, float aTime = 0)
        {
            mEvent = aEvent;
            mTime = aTime;
            mFollow = new TimedEventChain();
            return mFollow;
        }
        public TimedEventChain then_one_shot(System.Action aEvent, float aTime = 0)
        {
            return then(delegate(float time) { aEvent(); return true; }, aTime);
        }
		
		public TimedEventChain wait(float aTime)
        {
            return then(delegate(float time) { return true; }, aTime);
        }

        public bool call(float aTime = 0)
        {
            if (mEvent == null) //base case
                return true;

            if (!mDone) //do my action
            {
                mDone = mEvent(aTime);
                if (mDone)
                    mTimeDone = aTime;
            }

            if (mDone && mFollow != null) //if my action is done, do following action
            {
                mFollowTimer = aTime-mTimeDone; //increase time
                //Debug.Log(mFollowTimer + " " + mFollow.mTime);
                if (mFollowTimer > mFollow.mTime) //if it's time to do followup do it
                    return mFollow.call(mFollowTimer-mFollow.mTime); //return the results
                else 
                    return false; //not time yet
            }
            return mDone;
        }

        public static TimedEventChain first(TimedEventDistributor aEventDistributor, System.Func<float, bool> aEvent, float aTime = 0)
        {
            TimedEventChain f = new TimedEventChain();
            TimedEventChain r =  f.then(aEvent, aTime);
            aEventDistributor.add_event_raw(f.call, aTime);
            return r;
        }
    }

    /*
    public class Chainable
    {
        TimedEventDistributor mEventDistributor;
        float mFirstTime;
        float mFollowTime;
        System.Func<float,bool> mFirstEvent;
        System.Func<float, bool> mFollowEvent;

        public Chainable(TimedEventDistributor aEventDistributor, System.Func<float, bool> aEvent, float aTime)
        {
            mEventDistributor = aEventDistributor;
            mFirstEvent = aEvent;
            mFirstTime = aTime;

            mEventDistributor.add_event(
                delegate(float time)
                {
                    bool r = aEvent(time);
                    if (r)
                        mEventDistributor.add_event(mFollowEvent,);
                    return r;
                },aTime);
        }
        public Chainable then(System.Func<float,bool> aEvent, float aTime)
        {

            return this;
        }
        private void add_next()
        {
            mEventDistributor.add_event(mFollowEvent, mFollowTime);
        }
    };*/

    Dictionary<QuTimer, System.Func<float,bool>> mEvents = new Dictionary<QuTimer, System.Func<float,bool>>();
	public QuTimer LastEventKeyAdded
	{ get; private set; }

    public void update(float aDeltaTime)
    {
        Dictionary<QuTimer, System.Func<float, bool>> copy = new Dictionary<QuTimer, System.Func<float, bool>>(mEvents);
        foreach (KeyValuePair<QuTimer, System.Func<float,bool>> e in copy)
        {
            e.Key.update(aDeltaTime);
            if (e.Key.isExpired())
            {
                if (e.Value(e.Key.getTimeSinceExpired()))
                    mEvents.Remove(e.Key);
            }
        }
    }
	
	//note, this will hard kill any events so if that event has vital code, it may not be called
	public void remove_event(QuTimer aKey)
	{
		if(aKey != null)
			mEvents.Remove(aKey);
				
	}
    public void add_event_raw(System.Func<float, bool> aEvent, float aTime = 0)
    {
		LastEventKeyAdded = new QuTimer(0, aTime);
        mEvents.Add(LastEventKeyAdded, aEvent);
		
    }
    public TimedEventChain add_event(System.Func<float,bool> aEvent, float aTime = 0)
    {
        TimedEventChain r = TimedEventChain.first(this, aEvent, aTime);
        return r;
    }

    public TimedEventChain add_one_shot_event(System.Action aEvent, float aTime = 0)
    {
        return add_event(delegate(float time) { aEvent(); return true; }, aTime);
    }

	public TimedEventChain empty_chain() //actually has one frame delay I think lol
	{
		return add_event(delegate(float time) { return true; }, 0);
	}
    
}
