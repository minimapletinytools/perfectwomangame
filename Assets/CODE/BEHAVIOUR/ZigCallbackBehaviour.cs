using UnityEngine;
using System.Collections;

//This class is used by ZigFuZig to get callbacks from ZigFu
public class ZigCallbackBehaviour : MonoBehaviour {

	public delegate void UpdateUserDelegate(ZigTrackedUser aUser);
    public UpdateUserDelegate mUpdateUserDelegate;
        
	void Zig_UpdateUser(ZigTrackedUser user)
    {
        if (mUpdateUserDelegate != null)
            mUpdateUserDelegate(user);
    }
}
