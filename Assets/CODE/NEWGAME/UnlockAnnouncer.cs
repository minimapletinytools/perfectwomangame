using UnityEngine;
using System.Collections.Generic;

//handles all animations when announcing whats been unlocked...
public class UnlockAnnouncer 
{
	public TimedEventDistributor TED { get; private set; }
    HashSet<FlatElementBase> mElement = new HashSet<FlatElementBase>();
	
	NewInterfaceManager NIM {get; set;}
	public UnlockAnnouncer(NewInterfaceManager aNIM)
	{
		NIM = aNIM;
		TED = new TimedEventDistributor();
	}
	
	
	public void update()
	{
		foreach (FlatElementBase e in mElement)
            e.update(Time.deltaTime);       
		
		TED.update(Time.deltaTime);
	}
	
	public void announce_unlock()
	{
	}
	
}
