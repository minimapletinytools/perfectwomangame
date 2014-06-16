using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public interface ZigInterface
{
    void initialize(ZigManager aZig);
    ZigInput ZigInput{get;}
    bool has_user();
}

public class EmptyZig : ZigInterface
{
    
    public void initialize(ZigManager aZig)
    {
    }
    
    public bool has_user()
    {
        return false;
    }
    
    public ZigInput ZigInput
    {
        get
        {
            return null;
        }
    }
}
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

//TODO
public class MicrosoftZig : ZigInterface
{
    ZigInput mZigInput = null;
    public void initialize(ZigManager aZig)
    {
        mZigInput = aZig.mManager.gameObject.AddComponent<ZigInput>();
        mZigInput.UpdateCallback += update_cb;
        //do we need to add aZig to mZigInput listeners??
        //or am I suppose to use that silly callback behaviour class ImadeL??
    }

    public void update_cb(List<GameObject> aObj)
    {
        //TODO finish this
        ZigTrackedUser user = new ZigTrackedUser(0);
        foreach (var e in aObj)
        {
            e.SendMessage("Zig_UpdateUser",user);
        }
    }

    public bool has_user()
    {
        return false;
    }
    
    public ZigInput ZigInput
    {
        get
        {

            return null;
        }
    }
}

