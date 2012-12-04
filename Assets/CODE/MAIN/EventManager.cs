using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class EventManager : FakeMonoBehaviour {
	public EventManager(ManagerManager aManager) : base(aManager) {}
	
	public delegate void VoidDelegate();
    public delegate void CharacterDelegate(CharacterTextureBehaviour character);
	LinkedList<KeyValuePair<QuTimer,VoidDelegate> > mTimedEvents = new LinkedList<KeyValuePair<QuTimer, VoidDelegate> >();

    //other events here
    public CharacterDelegate character_changed_event;
    public CharacterDelegate character_setup_event;

	public override void Update()
	{
		foreach(KeyValuePair<QuTimer,VoidDelegate> e in mTimedEvents)
		{
			e.Key.update(Time.deltaTime);
			if(e.Key.isExpired())
			{
				e.Value();
				mTimedEvents.Remove(e);
			}
		}
	}
	
	public void add_timed_event(float countdown, VoidDelegate toTrigger)
	{
		mTimedEvents.AddLast(new KeyValuePair<QuTimer,VoidDelegate>(new QuTimer(0,countdown), toTrigger));
	}
	public void add_delegate_to_last_timed_event(VoidDelegate toTrigger)
	{
		mTimedEvents.Last.Value = new KeyValuePair<QuTimer,VoidDelegate>(mTimedEvents.Last.Value.Key,mTimedEvents.Last.Value.Value + toTrigger);
	}
}
