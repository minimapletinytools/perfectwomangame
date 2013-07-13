using UnityEngine;
using System.Collections.Generic;


public class GestureTracker 
{
	//assume evenly spaced though this may not always be the case..
	LinkedList<Vector3> LastPoints
	{get; set;}
	
	LinkedList<Vector3> TargetPoints
	{get; set;}
	
	public GestureTracker()
	{
		LastPoints = new LinkedList<Vector3>();
		TargetPoints = new LinkedList<Vector3>();
		
		//TODO finish
	}
	
	
	public float test()
	{
		//parametize last ponits and target points by arc length...
		//TODO
		
		return 0;
	}
}
