using UnityEngine;
using System.Collections;

public class ZigUpdateUserBehaviour : MonoBehaviour {

	public delegate void UpdateUserDelegate(ZigTrackedUser aUser);
    public UpdateUserDelegate mUpdateUserDelegate;
        
	void Zig_UpdateUser(ZigTrackedUser user)
    {
        if (mUpdateUserDelegate != null)
            mUpdateUserDelegate(user);
    }
}
