using UnityEngine;
using System.Collections.Generic;

public class TimedEventDistributor 
{

    public class TimedEventChain
    {
        TimedEventDistributor mEventDistributor;
        TimedEventChain mFollow = null;
        float mTime = -1;
        System.Func<float, bool> mEvent = null;
        TimedEventChain(TimedEventDistributor aEventDistributor)
        {
            mEventDistributor = aEventDistributor;
        }
        public TimedEventChain then(System.Func<float, bool> aEvent, float aTime)
        {
            mEvent = aEvent;
            mTime = aTime;
            mFollow = new TimedEventChain(mEventDistributor);
            return mFollow;
        }
        public TimedEventChain then_one_shot(System.Action aEvent)
        {
            mEvent = delegate(float time) { aEvent(); return true; };
            mTime = 0;
            mFollow = new TimedEventChain(mEventDistributor);
            return mFollow;
        }

        public bool call(float aTime)
        {
            bool r = mEvent(aTime);
            if (r && mFollow != null && mFollow.mEvent != null)
                mEventDistributor.add_event(mFollow.call, mFollow.mTime);
            return r;
        }

        public static TimedEventChain first(TimedEventDistributor aEventDistributor, System.Func<float, bool> aEvent, float aTime)
        {
            TimedEventChain f = new TimedEventChain(aEventDistributor);
            return f.then(aEvent, aTime);
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
        List<KeyValuePair<QuTimer, System.Func<float,bool>>> removal = new List<KeyValuePair<QuTimer, System.Func<float,bool>>>();
        foreach (KeyValuePair<QuTimer, System.Func<float,bool>> e in mEvents)
        {
            e.Key.update(aDeltaTime);
            if (e.Key.isExpired())
            {
                if (e.Value(aDeltaTime))
                    removal.Add(e);
            }
        }
        foreach (KeyValuePair<QuTimer, System.Func<float,bool>> e in removal)
            mEvents.Remove(e.Key);
    }
    public TimedEventChain add_event(System.Func<float,bool> aEvent, float aTime)
    {
        TimedEventChain r = TimedEventChain.first(this, aEvent, aTime);
        mEvents.Add(new QuTimer(0, aTime), r.call);
        return r;
    }

    public TimedEventChain add_one_shot_event(System.Action aEvent, float aTime)
    {
        return add_event(delegate(float time) { aEvent(); return true; }, aTime);
    }
    
}
