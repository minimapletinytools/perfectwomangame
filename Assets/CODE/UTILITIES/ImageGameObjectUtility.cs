using UnityEngine;
using System.Collections;

public class ImageGameObjectUtility  {

    public static GameObject create(Texture2D aTex)
    {
        return (new ImageGameObjectUtility(aTex)).ParentObject;
    }
    public GameObject ParentObject { get; private set; }
    public GameObject PlaneObject { get; private set; }
    public Material PlaneMaterial { get; private set; }
    public ImageGameObjectUtility(Texture2D aTex)
    {
        ParentObject = new GameObject("genImageObjectParent");
        PlaneObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        PlaneMaterial = new Material(ManagerManager.Manager.mReferences.mDefaultCharacterShader);
        PlaneObject.renderer.material = PlaneMaterial;
        if (aTex != null)
            PlaneObject.transform.localScale = new Vector3(BodyManager.convert_units(aTex.width) / 10.0f, 1,BodyManager.convert_units(aTex.height) / 10.0f);
        PlaneObject.transform.rotation = Quaternion.AngleAxis(90, Vector3.right) * PlaneObject.transform.rotation;
        PlaneObject.renderer.material.mainTexture = aTex;
        PlaneObject.transform.parent = ParentObject.transform;
    }

    public void destroy()
    {
        GameObject.Destroy(ParentObject);
        GameObject.Destroy(PlaneObject);
    }

    public void resize()
    {
        //TODO
    }

}
