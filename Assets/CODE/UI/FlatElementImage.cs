using UnityEngine;
using System.Collections;

public class FlatElementImage : FlatElementBase
{

    public FlatElementImage(Texture2D aTex, float aDepth)
    {
        Depth = aDepth;
        GameObject child = GameObject.CreatePrimitive(PrimitiveType.Plane);

        child.renderer.material = new Material(ManagerManager.Manager.mReferences.mDefaultCharacterShader);
        child.transform.localScale = new Vector3(
            BodyManager.convert_units(aTex.width) / 10.0f, 1,
            BodyManager.convert_units(aTex.height) / 10.0f);
        child.transform.rotation = Quaternion.AngleAxis(90, Vector3.right) * child.transform.rotation;
        child.transform.position = child.transform.position;
        child.renderer.material.mainTexture = aTex;
        SoftColor = new Color(0.5f,0.5f,0.5f,0.5f);

        PrimaryGameObject = new GameObject("genFlatElementImageParent");
        child.transform.parent = PrimaryGameObject.transform;
    }

}
