using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public interface ZgInterface
{
    void initialize(ZgManager aZig);
    ZgInput ZgInput{get;}
    bool has_user();
    void update();
       
}

public class EmptyZg : ZgInterface
{
    //if MonoBehaviours are needed, they can be added to the aZig.gameObject
    public void initialize(ZgManager aZig)
    {
    }
    
    public bool has_user()
    {
        return false;
    }

    public void update()
    {

    }

    public ZgInput ZgInput
    {
        get
        {
            return null;
        }
    }
}

