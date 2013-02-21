using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class FlatBodyObject : FlatElementBase
{

    public static ZigJointId[] mRenderedJoints = { ZigJointId.Neck, ZigJointId.LeftElbow, ZigJointId.LeftKnee, ZigJointId.LeftShoulder, ZigJointId.LeftHip, ZigJointId.RightElbow, ZigJointId.RightKnee, ZigJointId.RightShoulder, ZigJointId.RightHip, ZigJointId.Torso, ZigJointId.Waist };
    public int get_joint_alpha_index(ZigJointId aId)
    {
        int r = -1;
        for (int i = 0; i < mRenderedJoints.Length; i++)
            if (mRenderedJoints[i] == aId)
                r = i;
        return r;
    }
    public ProGrading.Pose mTargetPose = null;
    public Dictionary<ZigJointId, GameObject> mParts = new Dictionary<ZigJointId, GameObject>();
    Vector3 mOffset = Vector3.zero;
    public bool UseDepth { get; set; }


    public FlatBodyObject(CharacterTextureBehaviour aChar, int aDepth = -1)
    {
        create_body(aChar);
        PrimaryGameObject = new GameObject("genBodyParent");
        mParts[ZigJointId.Waist].transform.parent = PrimaryGameObject.transform;
        mParts[ZigJointId.Torso].transform.parent = PrimaryGameObject.transform;
        SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        SoftInterpolation = 0.08f;

        UseDepth = (aDepth != -1);
        if (UseDepth)
            Depth = aDepth;
    }

    public FlatBodyObject(CharacterLoader aChar, int aDepth = -1)
    {
        //TODO fake thread this eventually please..
        foreach (FlatBodyObject e in load_sequential(aChar.Images, aChar.Sizes))
            ;

        mOffset = (new Vector3(aChar.Sizes.mOffset.x, aChar.Sizes.mOffset.y, 0));

        PrimaryGameObject = new GameObject("genBodyParent");
        mParts[ZigJointId.Waist].transform.parent = PrimaryGameObject.transform;
        mParts[ZigJointId.Torso].transform.parent = PrimaryGameObject.transform;
        SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        SoftInterpolation = 0.08f;

        UseDepth = (aDepth != -1);
        if (UseDepth)
            Depth = aDepth;
    }


    public void match_body_location_to_projection(ZigManager aManager)
    {
        Vector3 position = Vector3.zero;
        if (!aManager.using_nite())
            position = aManager.Joints[ZigJointId.Waist].Position;
        else position = aManager.Joints[ZigJointId.Torso].Position;
        position.z = 0;
        position.y = 0;
        position.x *= -1;
        if (Mathf.Abs(position.x) < 10) position.x = 0; //fake snapping, TODO this should probbaly be in grading manager if anywhere...
        SoftPosition = position / 1.5f + mOffset;
    }
    public void match_body_to_projection(ProjectionManager aManager)
    {
        foreach (KeyValuePair<ZigJointId, ProjectionManager.Stupid> e in aManager.mImportant)
        {
            mParts[e.Key].transform.rotation = Quaternion.AngleAxis(e.Value.smoothing.current, Vector3.forward);
        }
        mParts[ZigJointId.Waist].transform.rotation = Quaternion.AngleAxis(aManager.mWaist.current, Vector3.forward);
    }

    public void set_target_pose(ProGrading.Pose aPose)
    {
        mTargetPose = aPose;
    }

    public override void update_parameters(float aDeltaTime)
    {
        if (mTargetPose != null)
        {
            foreach (ProGrading.PoseElement e in mTargetPose.mElements)
            {
                mParts[e.joint].transform.rotation = Quaternion.Slerp(mParts[e.joint].transform.rotation, Quaternion.AngleAxis(e.angle, Vector3.forward), 0.1f);
            }
        }
        base.update_parameters(aDeltaTime);
    }

    public override void destroy()
    {
        mTargetPose = null;
        //mMode = -1;
        foreach (GameObject e in mParts.Values)
            GameObject.Destroy(e);
        mParts.Clear();
    }


    public IEnumerable<FlatBodyObject> load_sequential(CharacterData.CharacterDataImages aImages, CharacterData.CharacterDataSizes aSizes)
    {
        //TODOgit 
        GameObject head = create_object(ZigJointId.Neck, aImages.head, aSizes.mLimbSizes[0], aSizes.mMountingPositions[0]);
        yield return null;
        GameObject leftLowerArm = create_object(ZigJointId.LeftElbow, aImages.leftLowerArm, aSizes.mLimbSizes[1], aSizes.mMountingPositions[1]);
        yield return null;
        GameObject leftLowerLeg = create_object(ZigJointId.LeftKnee, aImages.leftLowerLeg, aSizes.mLimbSizes[2], aSizes.mMountingPositions[2]);
        yield return null;
        GameObject leftUpperArm = create_object(ZigJointId.LeftShoulder, aImages.leftUpperArm, aSizes.mLimbSizes[3], aSizes.mMountingPositions[3]);
        yield return null;
        GameObject leftUpperLeg = create_object(ZigJointId.LeftHip, aImages.leftUpperLeg, aSizes.mLimbSizes[4], aSizes.mMountingPositions[4]);
        yield return null;
        GameObject rightLowerArm = create_object(ZigJointId.RightElbow, aImages.rightLowerArm, aSizes.mLimbSizes[5], aSizes.mMountingPositions[5]);
        yield return null;
        GameObject rightLowerLeg = create_object(ZigJointId.RightKnee, aImages.rightLowerLeg, aSizes.mLimbSizes[6], aSizes.mMountingPositions[6]);
        yield return null;
        GameObject rightUpperArm = create_object(ZigJointId.RightShoulder, aImages.rightUpperArm, aSizes.mLimbSizes[7], aSizes.mMountingPositions[7]);
        yield return null;
        GameObject rightUpperLeg = create_object(ZigJointId.RightHip, aImages.rightUpperLeg, aSizes.mLimbSizes[8], aSizes.mMountingPositions[8]);
        yield return null;
        GameObject torso = create_object(ZigJointId.Torso, aImages.torso, aSizes.mLimbSizes[9], aSizes.mMountingPositions[9]);
        yield return null;
        GameObject waist = create_object(ZigJointId.Waist, aImages.waist, aSizes.mLimbSizes[10], aSizes.mMountingPositions[10]);
        yield return null;

        //order things
        Dictionary<ZigJointId, GameObject> jointObject = new Dictionary<ZigJointId, GameObject>();
        Dictionary<ZigJointId, Texture2D> jointTexture = new Dictionary<ZigJointId, Texture2D>();
        jointObject[ZigJointId.Torso] = torso;
        jointObject[ZigJointId.Waist] = waist;
        jointObject[ZigJointId.Neck] = head;
        jointObject[ZigJointId.LeftShoulder] = leftUpperArm;
        jointObject[ZigJointId.RightShoulder] = rightUpperArm;
        jointObject[ZigJointId.LeftElbow] = leftLowerArm;
        jointObject[ZigJointId.RightElbow] = rightLowerArm;
        jointObject[ZigJointId.LeftHip] = leftUpperLeg;
        jointObject[ZigJointId.RightHip] = rightUpperLeg;
        jointObject[ZigJointId.LeftKnee] = leftLowerLeg;
        jointObject[ZigJointId.RightKnee] = rightLowerLeg;

        //these two are special
        torso.transform.position = waist.transform.position;
        torso.transform.position += get_Z_offset(ZigJointId.Torso);
        waist.transform.position += get_Z_offset(ZigJointId.Waist);

        List<KeyValuePair<ZigJointId, ZigJointId>> relations = new List<KeyValuePair<ZigJointId, ZigJointId>>();
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftShoulder, ZigJointId.Torso));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightShoulder, ZigJointId.Torso));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftElbow, ZigJointId.LeftShoulder));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightElbow, ZigJointId.RightShoulder));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftHip, ZigJointId.Waist));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightHip, ZigJointId.Waist));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftKnee, ZigJointId.LeftHip));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightKnee, ZigJointId.RightHip));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.Neck, ZigJointId.Torso));

        foreach (KeyValuePair<ZigJointId, ZigJointId> e in relations)
        {
            jointObject[e.Key].transform.parent = jointObject[e.Value].transform;
            jointObject[e.Key].transform.position =
                jointObject[e.Value].transform.position
                + get_offset_of_plane(jointObject[e.Value].transform)
                + get_connection_point_list(e.Key, e.Value, aSizes.mMountingPositions[get_joint_alpha_index(e.Value)])
                + get_Z_offset(e.Key)
                - get_Z_offset(e.Value);

        }

        yield return null;

        List<KeyValuePair<GameObject, float>> rotateMe = new List<KeyValuePair<GameObject, float>>();
        rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperArm, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperArm, -90));

        rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(torso, 90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(waist, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(head, 90));

        rotateMe.Add(new KeyValuePair<GameObject, float>(leftLowerLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightLowerLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(leftLowerArm, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightLowerArm, -90));

        foreach (KeyValuePair<GameObject, float> e in rotateMe)
        {
            GameObject tempParent = new GameObject("genTempParent");
            tempParent.transform.position = e.Key.transform.position;
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < e.Key.transform.GetChildCount(); i++)
                if (e.Key.transform.GetChild(i).parent == e.Key.transform)
                    children.Add(e.Key.transform.GetChild(i));
            foreach (Transform f in children)
                f.parent = tempParent.transform;
            tempParent.transform.rotation = Quaternion.AngleAxis(e.Value, Vector3.forward) * tempParent.transform.rotation;
            foreach (Transform f in children)
                f.parent = e.Key.transform;
            GameObject.Destroy(tempParent);
        }

        yield return this;
    }



    //useful
    void create_body(CharacterTextureBehaviour aChar)
    {
        GameObject torso = create_object(ZigJointId.Torso, aChar.torso, aChar.atTorso);
        GameObject waist = create_object(ZigJointId.Waist, aChar.waist, aChar.atWaist);
        GameObject head = create_object(ZigJointId.Neck, aChar.head, aChar.atHead);
        GameObject leftUpperArm = create_object(ZigJointId.LeftShoulder, aChar.leftUpperArm, aChar.atLeftUpperArm);
        GameObject rightUpperArm = create_object(ZigJointId.RightShoulder, aChar.rightUpperArm, aChar.atRightUpperArm);
        GameObject leftLowerArm = create_object(ZigJointId.LeftElbow, aChar.leftLowerArm, aChar.atLeftLowerArm);
        GameObject rightLowerArm = create_object(ZigJointId.RightElbow, aChar.rightLowerArm, aChar.atRightLowerArm);
        GameObject leftUpperLeg = create_object(ZigJointId.LeftHip, aChar.leftUpperLeg, aChar.atLeftUpperLeg);
        GameObject rightUpperLeg = create_object(ZigJointId.RightHip, aChar.rightUpperLeg, aChar.atRightUpperLeg);
        GameObject leftLowerLeg = create_object(ZigJointId.LeftKnee, aChar.leftLowerLeg, aChar.atLeftLowerLeg);
        GameObject rightLowerLeg = create_object(ZigJointId.RightKnee, aChar.rightLowerLeg, aChar.atRightLowerLeg);

        //order things
        Dictionary<ZigJointId, GameObject> jointObject = new Dictionary<ZigJointId, GameObject>();
        Dictionary<ZigJointId, Texture2D> jointTexture = new Dictionary<ZigJointId, Texture2D>();
        jointObject[ZigJointId.Torso] = torso;
        jointObject[ZigJointId.Waist] = waist;
        jointObject[ZigJointId.Neck] = head;
        jointObject[ZigJointId.LeftShoulder] = leftUpperArm;
        jointObject[ZigJointId.RightShoulder] = rightUpperArm;
        jointObject[ZigJointId.LeftElbow] = leftLowerArm;
        jointObject[ZigJointId.RightElbow] = rightLowerArm;
        jointObject[ZigJointId.LeftHip] = leftUpperLeg;
        jointObject[ZigJointId.RightHip] = rightUpperLeg;
        jointObject[ZigJointId.LeftKnee] = leftLowerLeg;
        jointObject[ZigJointId.RightKnee] = rightLowerLeg;


        jointTexture[ZigJointId.Torso] = aChar.atTorso;
        jointTexture[ZigJointId.Waist] = aChar.atWaist;
        jointTexture[ZigJointId.Neck] = aChar.atHead;
        jointTexture[ZigJointId.LeftShoulder] = aChar.atLeftUpperArm;
        jointTexture[ZigJointId.RightShoulder] = aChar.atRightUpperArm;
        jointTexture[ZigJointId.LeftElbow] = aChar.atLeftLowerArm;
        jointTexture[ZigJointId.RightElbow] = aChar.atRightLowerArm;
        jointTexture[ZigJointId.LeftHip] = aChar.atLeftUpperLeg;
        jointTexture[ZigJointId.RightHip] = aChar.atRightUpperLeg;
        jointTexture[ZigJointId.LeftKnee] = aChar.atLeftLowerLeg;
        jointTexture[ZigJointId.RightKnee] = aChar.atRightLowerLeg;


        //these two are special
        torso.transform.position = waist.transform.position;
        torso.transform.position += get_Z_offset(ZigJointId.Torso);
        waist.transform.position += get_Z_offset(ZigJointId.Waist);

        List<KeyValuePair<ZigJointId, ZigJointId>> relations = new List<KeyValuePair<ZigJointId, ZigJointId>>();
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftShoulder, ZigJointId.Torso));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightShoulder, ZigJointId.Torso));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftElbow, ZigJointId.LeftShoulder));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightElbow, ZigJointId.RightShoulder));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftHip, ZigJointId.Waist));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightHip, ZigJointId.Waist));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftKnee, ZigJointId.LeftHip));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightKnee, ZigJointId.RightHip));
        relations.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.Neck, ZigJointId.Torso));

        foreach (KeyValuePair<ZigJointId, ZigJointId> e in relations)
        {
            jointObject[e.Key].transform.parent = jointObject[e.Value].transform;
            jointObject[e.Key].transform.position =
                jointObject[e.Value].transform.position
                + get_offset_of_plane(jointObject[e.Value].transform)
                + get_connection_point_image(e.Key, e.Value, jointTexture[e.Value])
                + get_Z_offset(e.Key)
                - get_Z_offset(e.Value);

        }

        List<KeyValuePair<GameObject, float>> rotateMe = new List<KeyValuePair<GameObject, float>>();
        rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperArm, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperArm, -90));

        rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(torso, 90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(waist, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(head, 90));

        rotateMe.Add(new KeyValuePair<GameObject, float>(leftLowerLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightLowerLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(leftLowerArm, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightLowerArm, -90));

        foreach (KeyValuePair<GameObject, float> e in rotateMe)
        {
            GameObject tempParent = new GameObject("genTempParent");
            tempParent.transform.position = e.Key.transform.position;
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < e.Key.transform.GetChildCount(); i++)
                if (e.Key.transform.GetChild(i).parent == e.Key.transform)
                    children.Add(e.Key.transform.GetChild(i));
            foreach (Transform f in children)
                f.parent = tempParent.transform;
            tempParent.transform.rotation = Quaternion.AngleAxis(e.Value, Vector3.forward) * tempParent.transform.rotation;
            foreach (Transform f in children)
                f.parent = e.Key.transform;
            GameObject.Destroy(tempParent);
        }
    }

    GameObject create_object(ZigJointId aId, Texture2D aTex, Texture2D aAttachTex)  
    {
        List<Vector3> attach = new List<Vector3>(){get_attachment_point(0, aAttachTex)};
        return create_object(aId, aTex, new Vector2(aTex.width,aTex.height), attach);
    }

    GameObject create_object(ZigJointId aId, Texture2D aTex, Vector2 aDim, List<Vector3> aAttach)
    {
        GameObject parent = new GameObject("genParent" + aId.ToString());
        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //sphere.transform.localScale = Vector3.one * 0.2f;
        //sphere.transform.parent = parent.transform;
        GameObject kid = GameObject.CreatePrimitive(PrimitiveType.Plane);
        kid.renderer.material = new Material(ManagerManager.Manager.mReferences.mDefaultCharacterShader);
        kid.renderer.material.mainTexture = aTex;
        kid.transform.rotation = Quaternion.AngleAxis(90, Vector3.right) * kid.transform.rotation;

        kid.transform.localScale = new Vector3(BodyManager.convert_units(aDim.x) / 10.0f, 1, BodyManager.convert_units(aDim.y) / 10.0f);
        kid.transform.position = -aAttach[0];
        kid.transform.parent = parent.transform;

        mParts[aId] = parent;
        return parent;
    }



    



    //utilities for positioning
    Vector3 get_offset_of_plane(Transform aGo)
    {
        Transform plane = aGo.FindChild("Plane");
        if (plane != null)
            return plane.position - aGo.transform.position;
        throw new UnityException("no plane child exsits");
    }
    static bool is_same_color(Color32 c1, Color32 c2)
    {
        return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b;
    }
    static Vector3 index_to_position(int i, Texture2D aTex)
    {

        int x = i % aTex.width - aTex.width / 2;
        int y = i / aTex.width - aTex.height / 2;

        return new Vector3(-BodyManager.convert_units(x), BodyManager.convert_units(y));
    }
    public static Vector3 find_first_color(Color32 c, Texture2D aTex)
    {

        Color32[] colors = aTex.GetPixels32();
        for (int i = 0; i < colors.Length; i++)
        {
            if (is_same_color(colors[i], c))
            {

                return index_to_position(i, aTex);
            }
        }
        //return Vector3.zero;
        throw new UnityException("color " + c.ToString() + " not found in texture " + aTex.name);
    }

    public static Vector3 get_attachment_point(int aId, Texture2D aTex)
    {
        Color32 c;
        switch (aId)
        {
            case 0:
                c = new Color32(255, 0, 0, 255);
                break;
            case 1:
                c = new Color32(0, 255, 0, 255);
                break;
            case 2:
                c = new Color32(0, 0, 255, 255);
                break;
            case 3:
                c = new Color32(255, 255, 0, 255);
                break;
            default:
                return Vector3.zero;
        }
        return find_first_color(c, aTex);
    }


    Vector3 get_connection_point_image(ZigJointId A, ZigJointId B, Texture2D aBTex)
    {
        if (A == B)
        {
            return get_attachment_point(0, aBTex);
        }
        if (B == ZigJointId.Waist)
        {
            if (A == ZigJointId.Torso)
                return get_attachment_point(0, aBTex);
            else if (A == ZigJointId.LeftHip)
                return get_attachment_point(1, aBTex);
            else if (A == ZigJointId.RightHip)
                return get_attachment_point(2, aBTex);
        }
        else if (B == ZigJointId.Torso)
        {
            if (A == ZigJointId.Neck)
                return get_attachment_point(3, aBTex);
            else if (A == ZigJointId.LeftShoulder)
                return get_attachment_point(1, aBTex);
            else if (A == ZigJointId.RightShoulder)
                return get_attachment_point(2, aBTex);
        }
        else
        {
            return get_attachment_point(1, aBTex);
        }
        throw new UnityException("uh oh, can't find attachment point zigjointid map");
    }

    Vector3 get_connection_point_list(ZigJointId A, ZigJointId B, List<Vector3> aConnect)
    {
        int index = -1;
        if (A == B)
        {
            index = 0;
        }
        if (B == ZigJointId.Waist)
        {
            if (A == ZigJointId.Torso)
                index = 0;
            else if (A == ZigJointId.LeftHip)
                index = 1;
            else if (A == ZigJointId.RightHip)
                index = 2;
        }
        else if (B == ZigJointId.Torso)
        {
            if (A == ZigJointId.Neck)
                index = 3;
            else if (A == ZigJointId.LeftShoulder)
                index = 1;
            else if (A == ZigJointId.RightShoulder)
                index = 2;
        }
        else
        {
            index = 1;
        }
        if(index == -1)
            throw new UnityException("uh oh, can't find attachment point zigjointid map");
        return aConnect[index];
    }

    public Vector3 get_Z_offset(ZigJointId id)
    {
        switch (id)
        {
            case ZigJointId.RightElbow:
                return new Vector3(0, 0, -0.0f);
            case ZigJointId.LeftElbow:
                return new Vector3(0, 0, -0.1f);
            case ZigJointId.RightShoulder:
                return new Vector3(0, 0, -0.2f);
            case ZigJointId.LeftShoulder:
                return new Vector3(0, 0, -0.3f);
            case ZigJointId.Torso:
                return new Vector3(0, 0, -0.4f);
            case ZigJointId.Waist:
                return new Vector3(0, 0, -0.5f);
            case ZigJointId.LeftHip:
                return new Vector3(0, 0, -0.6f);
            case ZigJointId.RightHip:
                return new Vector3(0, 0, -0.7f);
            case ZigJointId.RightKnee:
                return new Vector3(0, 0, -0.8f);
            case ZigJointId.LeftKnee:
                return new Vector3(0, 0, -0.9f);
        }
        return Vector3.zero;
    }
}


