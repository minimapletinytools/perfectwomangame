using UnityEngine;
using System.Collections.Generic;

public class TimedEventDistributor 
{

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
    public void add_event(System.Func<float,bool> aEvent, float aTime)
    {
        mEvents.Add(new QuTimer(0, aTime), aEvent);
    }

    public void add_one_shot_event(System.Action aEvent, float aTime)
    {
        add_event(delegate(float time) { aEvent(); return true; }, aTime);
    }
    
}
