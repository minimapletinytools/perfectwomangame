using UnityEngine;
using System.Collections;

public class FlatElementImage : FlatElementBase
{

    public FlatElementImage(Texture2D aTex, float aDepth)
    {
        PrimaryGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        PrimaryGameObject.renderer.material = new Material(ManagerManager.Manager.mReferences.mDefaultCharacterShader);
        PrimaryGameObject.transform.rotation = Quaternion.AngleAxis(90, Vector3.right) * PrimaryGameObject.transform.rotation;
        PrimaryGameObject.transform.position = PrimaryGameObject.transform.position + new Vector3(0, 0, aDepth);
        PrimaryGameObject.transform.localScale = new Vector3(
            BodyManager.convert_units(aTex.width) / 10.0f, 1,
            BodyManager.convert_units(aTex.height) / 10.0f);
        PrimaryGameObject.renderer.material.mainTexture = aTex;
    }

}
