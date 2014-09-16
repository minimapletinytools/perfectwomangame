using UnityEngine;
using System.Collections;

//TODO DELETE
//position is source, need to use separate function to set target
public class ParticleStreamObject  : FlatElementBase {
	ParticleSystem mParticles;
	GameObject mParticleGameObject = null;
	public Vector3 Target { get; set; }
    public ParticleStreamObject(int aDepth, Vector3 aTarget)
    {
		//NOTE these assets were removed since this class is no longer being used.
		//mParticleGameObject = GameObject.Instantiate(ManagerManager.Manager.mNewRef.uiParticlePrefab) as GameObject;
		PrimaryGameObject = new GameObject("genParticleStreamParent");
		mParticles = mParticleGameObject.GetComponent<ParticleSystem>();
		mParticleGameObject.transform.parent = PrimaryGameObject.transform;
		Target = aTarget;
		Depth = aDepth;
		
		
    }
	
	
    public override int Depth
    {
        get { return base.Depth; }
        set
        {
			base.Depth = value;
			mParticleGameObject.GetComponent<ParticleSystemRenderer>().material.renderQueue = value;
        }
    }

    public override void set_color(Color aColor)
    {
        mParticles.startColor = aColor;
        base.set_color(aColor);
    }
	
	public override void set_position (Vector3 aPos)
	{
		Vector3 targetDir = Target-aPos;
		mParticleGameObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward,targetDir); //particles come out in the positive Z axis by default...
		mParticles.startLifetime = targetDir.magnitude / mParticles.startSpeed;
        //mParticles.time = targetDir.magnitude / mParticles.startSpeed;
		base.set_position(aPos);
	}
	
	public override void set_rotation(Quaternion aRot)
	{
		//target determines rotation so we don't want tod anything here.
	}
}

