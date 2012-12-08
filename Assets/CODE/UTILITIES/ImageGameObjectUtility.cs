using UnityEngine;
using System.Collections;

public class ImageGameObjectUtility  
{

    public static GameObject create(Texture2D aTex)
    {
        return (new ImageGameObjectUtility(aTex)).ParentObject;
    }

    public GameObject ParentObject { get; private set; }
    public GameObject PlaneObject { get; private set; }
    public Material PlaneMaterial { get; private set; }

    public Vector2 BaseDimension { get; private set; }
    Vector2 mPixelDimension;
    public Vector2 PixelDimension
    {
        get { return mPixelDimension; }
        set
        {
            mPixelDimension = value;
            PlaneObject.transform.localScale = new Vector3(BodyManager.convert_units(mPixelDimension.x) / 10.0f, 1, BodyManager.convert_units(mPixelDimension.y) / 10.0f);
        }
    }

    public ImageGameObjectUtility(Texture2D aTex)
    {
        ParentObject = new GameObject("genImageObjectParent");
        PlaneObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        PlaneMaterial = new Material(ManagerManager.Manager.mReferences.mDefaultCharacterShader);
        PlaneObject.renderer.material = PlaneMaterial;
        set_new_texture(aTex);
        PixelDimension = BaseDimension;

        PlaneObject.transform.rotation = Quaternion.AngleAxis(90, Vector3.right) * PlaneObject.transform.rotation;
        PlaneObject.transform.parent = ParentObject.transform;
    }

    public void set_new_texture(Texture2D aTex)
    {

        if (aTex != null)
        {
            BaseDimension = new Vector2(aTex.width, aTex.height);
        }
        else
        {
            BaseDimension = new Vector2(1, 1);
        }
        PlaneObject.renderer.material.mainTexture = aTex;
    }

    public void destroy()
    {
        GameObject.Destroy(ParentObject);
        GameObject.Destroy(PlaneObject);
    }

}
