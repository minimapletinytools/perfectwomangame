using UnityEngine;
using System.Collections;

//position is source, need to use separate function to set target
public class ParticleStreamObject  : FlatElementBase {
	ParticleSystem mParticles;
	GameObject mParticleGameObject;
	public Vector3 Target { get; set; }
    public ParticleStreamObject(int aDepth, Vector3 aTarget)
    {
		mParticleGameObject = GameObject.Instantiate(ManagerManager.Manager.mNewRef.uiParticlePrefab) as GameObject;
		PrimaryGameObject = new GameObject("genParticleStreamParent");
		mParticles = mParticleGameObject.GetComponent<ParticleSystem>();
		mParticleGameObject.transform.parent = PrimaryGameObject.transform;
		Target = aTarget;
		Depth = aDepth;
		
		
    }
	
	
	public override void set_position (Vector3 aPos)
	{
		Vector3 targetDir = Target-aPos;
		mParticleGameObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward,targetDir); //particles come out in the positive Z axis by default...
		//mParticles.startLifetime = targetDir.magnitude / mParticles.startSpeed;
        mParticles.time = targetDir.magnitude / mParticles.startSpeed;
		base.set_position(aPos);
	}
	
	public override void set_rotation(Quaternion aRot)
	{
		//target determines rotation
	}
}

