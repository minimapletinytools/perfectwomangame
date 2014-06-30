using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public interface ZigInterface
{
    void initialize(ZigManager aZig);
    ZigInput ZigInput{get;}
    bool has_user();
    void update();
       
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

    public void update()
    {

    }

    public ZigInput ZigInput
    {
        get
        {
            return null;
        }
    }
}

