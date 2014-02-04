using UnityEngine;
using System.Collections;

//this class integrates HikingPhysics and FlatBodyObject
//must use Z axis as axis of rotation
public class FlatHiking
{
	//this will read target positions as the desired positions
	//and will directly set the rotations of the contained thing
	FlatBodyObject mFlat;

	HikingPhysics mHiking;

	public FlatHiking()
	{
	}

	public void initialize()
	{
		//TODO setup HikingPhysics using mFlat as input
	}

	public void update()
	{
		//TODO updated desired parametrs for physics
		//TODO solve system
		//TODO pass updated parameters back to mFLat
	}

}
