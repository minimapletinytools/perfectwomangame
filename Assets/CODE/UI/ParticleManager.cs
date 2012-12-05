using UnityEngine;
using System.Collections.Generic;
public class ParticleManager : FakeMonoBehaviour {
    public ParticleManager(ManagerManager aManager) : base(aManager) { }


    Dictionary<ZigJointId, ParticleSystem> mParticles = new Dictionary<ZigJointId, ParticleSystem>();
    public override void Start()
    {
        
        mManager.mEventManager.character_setup_event += character_setup_listener;

        /*
        foreach (ZigJointId e in BodyManager.mRenderedJoints)
        {
            ParticleSystem ps = (new GameObject("genParticle" + e.ToString())).AddComponent<ParticleSystem>();
            mParticles[e] = ps;
            ps.renderer.material = new Material(mManager.mReferences.mDefaultCharacterShader);
            ps.renderer.material.mainTexture = mManager.mReferences.mDefaultParticleTexture;
            ps.emissionRate = 1000;
            ps.enableEmission = false;
            ps.startSpeed = 100;
            ps.startSize = 5;
            ps.startEnergy = 4;
            ps.startColor = new Color(1, 1, 1, 0);
        }
        */
    }
    public override void Update()
    {

    }


    public void character_setup_listener(CharacterTextureBehaviour aCharacter)
    {
        /*
        foreach (KeyValuePair<ZigJointId, ParticleSystem> e in mParticles)
        {
            e.Value.transform.position = mManager.mBodyManager.get_part_position(e.Key);
            e.Value.enableEmission = true;
        }
        */
    }   
}
