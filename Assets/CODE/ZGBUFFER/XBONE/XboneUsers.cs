using UnityEngine;
using System.Collections;
#if UNITY_XBOXONE 
using Users;
#endif


public class XboneUsers {
#if UNITY_XBOXONE 


	public void Start () {
		//moved to XboneAll.cs UsersManager.Create();
		

	}


#else
    public void Start () {
    }
#endif
}
