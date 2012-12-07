using UnityEngine;
using System.Collections.Generic;

public class MiniManObject : FlatElementBase
{
    BodyManager mManager;
    public MiniManObject()
    {
        CharacterTextureBehaviour character = ((GameObject)GameObject.Instantiate(ManagerManager.Manager.mMenuReferences.miniMan)).GetComponent<CharacterTextureBehaviour>();
        mManager.create_body(character);
        PrimaryGameObject = new GameObject("genMiniManObjectParent");
        mManager.mParts[ZigJointId.Waist].transform.parent = PrimaryGameObject.transform;
        mManager.mParts[ZigJointId.Torso].transform.parent = PrimaryGameObject.transform;
        mManager.set_target_pose(ProGrading.read_pose(ManagerManager.Manager.mMenuReferences.cheapPose));
    }
}
