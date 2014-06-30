using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//the callback behaviour in this cas e
public class ZigFuZig : ZigInterface
{
    
    Zig mZig = null;
    ZigEngageSingleUser mZigEngageSingleUser = null;
    ZigCallbackBehaviour mZigCallbackBehaviour = null;
    ZigInput mZigInput = null;
    
    public void initialize(ZigManager aZig)
    {
        
        var mZigObject = ManagerManager.Manager.gameObject;
        //mZigObject.AddComponent<kinectSpecific>();
        mZig = mZigObject.GetComponent<Zig>();
        
        /*
        mZig = mZigObject.AddComponent<Zig>();
        mZig.inputType = ZigInputType.Auto;
        mZig.settings.UpdateDepth = true;
        mZig.settings.UpdateImage = true;
        mZig.settings.AlignDepthToRGB = false;
        mZig.settings.OpenNISpecific.Mirror = true;
        mZigObject.AddComponent<ZigEngageSingleUser>();
        */
        
        
        //ZigEngageSingleUser scans for all users but only reports results from one of them (the first I guess)
        //normally this is set in editor initializers but we don't do that here
        mZigEngageSingleUser = mZigObject.GetComponent<ZigEngageSingleUser>();
        mZigEngageSingleUser.EngagedUsers = new System.Collections.Generic.List<UnityEngine.GameObject>();
        mZigEngageSingleUser.EngagedUsers.Add(mZigObject);
        
        //this is the only way to get callbacks from ZigEngageSingleUser
        mZigCallbackBehaviour = mZigObject.AddComponent<ZigCallbackBehaviour>();
        mZigCallbackBehaviour.mUpdateUserDelegate += aZig.Zig_UpdateUser;
    }
    
    public bool has_user()
    {
        return mZigEngageSingleUser.engagedTrackedUser != null;
    }

    public void update()
    {
    }
    
    public ZigInput ZigInput
    {
        get
        {
            //I'm not sure why I can't just do this in the statr routine
            if (mZigInput == null)
            {
                GameObject container = GameObject.Find("ZigInputContainer");
                if (container != null)
                    mZigInput = container.GetComponent<ZigInput>();
                
                //this is important!, this is the only way to get output from ZigInput
                mZigInput.AddListener(ManagerManager.Manager.gameObject);
            }
            return mZigInput;
        }
    }
}