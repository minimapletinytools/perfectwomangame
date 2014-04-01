using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HikingRigidNode
{
	public float mass;
	public Vector3 relPosition;
	public Collider collider;
	public HikingRigidComponent[] parents;
}

public struct HikingSpatialPosition
{
	public Vector3 position;
	public Quaternion rotation;
}

public class HikingRigidComponent
{
	HikingRigidNode[] nodes;
	public HikingSpatialPosition startingPosition;
	public HikingSpatialPosition currentPosition;
	public HikingSpatialPosition desiredPosition;
	public Vector3 velocity;
	public Vector3 angularVelocity; //stored in euler angles per second
	public float mass; 

	public HikingRigidComponent(HikingRigidNode[] aNodes)
	{
		nodes = aNodes.ToArray(); //makes a copy :)... I hope
		compute_net_mass();
	}

	void compute_net_mass()
	{
		mass = nodes.Sum(e=>e.mass);
	}

	//TODO function to slove rigid system based on forces on RigidNodes
}

public class HikingRigidConnection
{
	public HikingRigidComponent A;
	public HikingRigidComponent B;
	public Quaternion DesiredRotation {get;set;} 
	public Quaternion Rotation {get{return B.currentPosition.rotation*Quaternion.Inverse(A.currentPosition.rotation);}} 
	//BONUS limits, spring constant
}

public static class HikingRigidFunctions
{

}

public class HikingPhysics
{
	//for cleanup purposes
	List<GameObject> mColliders = new List<GameObject>();
	List<HikingRigidComponent> mRigidComponents = new List<HikingRigidComponent>();

	public HikingPhysics()
	{
		
	}
	
	public void initialize()
	{

		//TODO create colliders and turn them off (because we manually resolve the collisions)
	}

	public void set_desired_rotations()
	{

	}
	
	public void update(float dt)
	{
		//updated starting positions
		foreach(var e in mRigidComponents)
		{
			e.startingPosition = e.currentPosition;
		}

		//TODO
		//decay velocities

		//TODO
		//resolve velocities???
		//could just sum all the velocities and weight them by forces and apply to COM
			//or do someting more sophisticated and combine it with the next step
			//the problem with this is that the system is key points of the system need to remain rigid
			//you would need some sort of solver to resolve velocities with updated positions..

		//attempt to resolve new positions
		update_desired_positions();

		//TODO update nodes with collision resolution


		//compute new velocities
		foreach(var e in mRigidComponents)
		{
			e.velocity = (e.currentPosition.position - e.startingPosition.position)/dt;
			e.angularVelocity = (e.currentPosition.rotation*Quaternion.Inverse(e.startingPosition.rotation)).eulerAngles/dt;
		}


	}

	//updates forces based on desired rotations
	public void update_desired_positions()
	{
		//foreach connection
			//foreach connected body
				//distribute torque based on angular mass
				//apply torque to connecting node and compute new desired spatial position
	}

	//updates given node without moving fixedComponent
	public void update_node(HikingRigidNode aNode, HikingRigidComponent fixedComponent, int depth, int maxDepth = 200)
	{
		//foreach connected body

		//TODO if we are OK, then just return

		//TODO max iteration stops here, output error message
		if(depth > maxDepth)
		{
			Debug.Log("reached max depth :(");
			return;
		}



		//updated
		//foreach 


		//recurse
		/*
		//TODO set this to nodes we actually want to recurse on
		var node = aNode;
		foreach(var e in node.parents)
		{
			if(e != fixedComponent)
			{
				update_node(node,e,depth+1);
			}
		}*/
	}

	//collides and attempts to resolve given component
	public bool collide_component(HikingRigidComponent aComponent)
	{
		bool r = false;
		//loop max iterations
			//foreach node
				//collide assuming linear motion
					//resolve node position linearly
					//resolve attached nodes using angular velocity and collision time
					//r = true;
		return r;
	}

	//TODO some function for outputing rotations
}
