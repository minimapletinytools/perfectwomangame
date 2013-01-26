using UnityEngine;
using System.Collections.Generic;

public class TimedEventDistributor 
{

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
        public TimedEventChain then(System.Func<float, bool> aEvent, float aTime)
        {
            mEvent = aEvent;
            mTime = aTime;
            mFollow = new TimedEventChain();
            return mFollow;
        }
        public TimedEventChain then_one_shot(System.Action aEvent)
        {
            return then(delegate(float time) { aEvent(); return true; }, 0);
        }

        public bool call(float aTime)
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

        public static TimedEventChain first(TimedEventDistributor aEventDistributor, System.Func<float, bool> aEvent, float aTime)
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

    public void update(float aDeltaTime)
    {
        Dictionary<QuTimer, System.Func<float, bool>> copy = new Dictionary<QuTimer, System.Func<float, bool>>(mEvents);
        foreach (KeyValuePair<QuTimer, System.Func<float,bool>> e in copy)
        {
            e.Key.update(aDeltaTime);
            if (e.Key.isExpired())
            {
                if (e.Value(e.Key.getTImeSinceExpired()))
                    mEvents.Remove(e.Key);
            }
        }
    }

    public void add_event_raw(System.Func<float, bool> aEvent, float aTime)
    {
        mEvents.Add(new QuTimer(0, aTime), aEvent);
    }
    public TimedEventChain add_event(System.Func<float,bool> aEvent, float aTime)
    {
        TimedEventChain r = TimedEventChain.first(this, aEvent, aTime);
        return r;
    }

    public TimedEventChain add_one_shot_event(System.Action aEvent, float aTime)
    {
        return add_event(delegate(float time) { aEvent(); return true; }, aTime);
    }
    
}
