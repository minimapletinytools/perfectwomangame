using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AstronautPlay
{
    ModeNormalPlay mMode;

    FarseerSimian mSimian;

    public AstronautPlay(ModeNormalPlay aMode)
    {
        mMode = aMode;
    }

    public void start_astro()
    {
        GameConstants.SCALE = 1 / 200f;

        mSimian = new FarseerSimian();
        mSimian.initialize(mMode.NGM.mManager.gameObject);
        mSimian.setup_with_body(mMode.NGM.mManager.mBodyManager.mFlat,false);
        
        //TODO add the asteroids
        //mSimian.add_environment(GameObject.FindObjectsOfType(typeof(Transform)).Where(e=>e.name.StartsWith("FS_")).Select(e=>((Transform)e).gameObject));
    }

    public void finish_astro()
    {
        mSimian.destroy();
        mSimian = null;
        GameConstants.SCALE = 1;
    }

    public void update_astro()
    {
        mSimian.update(mMode.NGM.mManager.mProjectionManager);
    }
}
