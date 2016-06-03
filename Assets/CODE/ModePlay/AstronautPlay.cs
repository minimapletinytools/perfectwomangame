using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AstronautPlay
{
    ModeNormalPlay mMode;
    //FarseerSimian mSimian;

    List<GameObject> mAsteroids = new List<GameObject>();
    Dictionary<ZgJointId,GameObject> mParts = new Dictionary<ZgJointId, GameObject>();

    Vector3 mStartingPos;

    public TimedEventDistributor TED { get; private set; }

    public AstronautPlay(ModeNormalPlay aMode)
    {
        mMode = aMode;
        TED = new TimedEventDistributor();
    }

    public void start_astro()
    {
        //TODO this wont work, you need to do manual rescaling...
        //GameConstants.SCALE = 1 / 200f;
        //mSimian = new FarseerSimian();
        //mSimian.initialize(mMode.NGM.mManager.gameObject);
        //mSimian.setup_with_body(mMode.NGM.mManager.mBodyManager.mFlat,false);
        //TODO add the asteroids
        //mSimian.add_environment(GameObject.FindObjectsOfType(typeof(Transform)).Where(e=>e.name.StartsWith("FS_")).Select(e=>((Transform)e).gameObject));

        ZgJointId[] colliders = new ZgJointId[]
        {
            //ZgJointId.Head,
            ZgJointId.LeftHand,
            ZgJointId.RightHand,
            ZgJointId.LeftAnkle,
            ZgJointId.RightAnkle
        };
        foreach (var e in colliders)
        {
            mParts[e] = new GameObject("genCollider"+e.ToString());
            mParts[e].AddComponent<Rigidbody>().isKinematic = true;
            mParts[e].AddComponent<SphereCollider>();
            mParts[e].transform.localScale = Vector3.one * (e == ZgJointId.Head ? 350:150);
            if(e == ZgJointId.Head)

            mParts[e].transform.position = mMode.NGM.mManager.mBodyManager.mFlat.mParts[ZgJointId.Head].transform.position;
        }

        mStartingPos = mMode.NGM.mManager.mBodyManager.mFlat.SoftPosition;
    }

    public void spawn_asteroid(Vector3 aPos, Vector3 aVel)
    {

        //pull the images from the character loader
        //I should have done this using dependency injection but who cares
        string[] astroNames = {"BG-1","BG-2","BG-3","BG-4","FG-1","FG-2"};
        astroNames.Shuffle();
        var sizing = ManagerManager.Manager.mGameManager.CurrentCharacterLoader.Sizes.find_static_element(astroNames[0]);
        var astroImage = ManagerManager.Manager.mGameManager.CurrentCharacterLoader.Images.staticElements [astroNames[0]];

        var ast = new ImageGameObjectUtility(astroImage,sizing.Size).ParentObject;
        foreach (Renderer e in ast.GetComponentsInChildren<Renderer>())
        {
            e.material.renderQueue = 5000;
            e.gameObject.layer = 1; //this is the mainbodycamera layer
        }
        ast.AddComponent<AstronautCollisionBehaviour>().Astronaut = this;
        ast.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ & RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationY;
        ast.AddComponent<SphereCollider>().radius = sizing.Size.y*4/9f;
        ast.transform.position = aPos;
        ast.GetComponent<Rigidbody>().velocity = aVel;
        ast.GetComponent<Rigidbody>().useGravity = false;

        mAsteroids.Add(ast);
    }

    public void finish_astro()
    {
        //mSimian.destroy();
        //mSimian = null;
        //GameConstants.SCALE = 1;


        //destroy the asteroids after 5000 seconds, I kind of like how they float into other screens but we don't want any memory licks either :o
        var copyAstro = mAsteroids;
        TED.add_one_shot_event(
            delegate(){
                foreach (var e in copyAstro)
                {
                    GameObject.Destroy(e);
                }
            }
        ,5000);
        mAsteroids.Clear();

        foreach (var e in mParts)
            GameObject.Destroy(e.Value);
        mParts.Clear();

    }

    Vector3 mMoveSpeed = new Vector3(0,0,0);
    public void ASTROCOLLISION(Vector3 aVel, Vector3 aPoint)
    {
        mMoveSpeed -= aVel/10f;
    }

    public void update_astro()
    {
        //mSimian.update(mMode.NGM.mManager.mProjectionManager);

        //floaty astronaut
        mMode.NGM.mManager.mBodyManager.mFlat.SoftPosition += mMoveSpeed * Time.deltaTime;
        mMoveSpeed = mMoveSpeed * 0.98f;
        

        foreach (var e in mParts)
        {
            var rb = e.Value.GetComponent<Rigidbody>();
            rb.MovePosition(mMode.NGM.mManager.mBodyManager.mFlat.mParts[e.Key].transform.position);
        }
        


        //generate asteroids
        if (Random.Range(0f, 1f) < Time.deltaTime / 2f) //about every 2 seconds
        {
            float rad = Random.Range(0,Mathf.PI*2);
            float rad2 = Random.Range(0,Mathf.PI*2);
            var pos = new Vector3(Mathf.Cos(rad),Mathf.Sin(rad)*9/16f,0)*2500;
            var vel = (-pos.normalized + new Vector3(Mathf.Cos(rad2),Mathf.Sin(rad2),0)*.15f)*Random.Range(200,300); //send it flying towards the center of the screen
            spawn_asteroid(pos,vel);
        }

    }
}
