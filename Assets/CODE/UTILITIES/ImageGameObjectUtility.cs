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
	
	public Vector2 TextureOffset {
		get{
			return PlaneMaterial.mainTextureOffset;
		}
		set{
			PlaneMaterial.mainTextureOffset = value;
		}
	}
	public Vector2 TextureScale {
		get{
			return PlaneMaterial.mainTextureScale;
		}
		set{
			PlaneMaterial.mainTextureScale = value;
		}
	}
    public Vector2 BaseDimension { get; private set; }
    public Vector2 PixelDimension
    {
        get 
        { 
            Vector3 planeScale = PlaneObject.transform.localScale;
            return new Vector2(planeScale.x*10, planeScale.y*10); //techincally, you need to do something like convert units
        }
        set
        {
            PlaneObject.transform.localScale = new Vector3(BodyManager.convert_units(value.x) / 10.0f, 1, BodyManager.convert_units(value.y) / 10.0f);
        }
    }

    public ImageGameObjectUtility(Texture2D aTex, System.Nullable<Vector2> aSize = null)
    {
        ParentObject = new GameObject("genImageObjectParent");
        //PlaneObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        PlaneObject = (GameObject)GameObject.Instantiate(ManagerManager.Manager.mReferences.mPlanePrefab);
        //GameObject.DestroyImmediate(PlaneObject.GetComponent<BoxCollider>());
        PlaneMaterial = new Material(ManagerManager.Manager.mReferences.mDefaultCharacterShader);
        PlaneObject.renderer.material = PlaneMaterial;
        set_new_texture(aTex, aSize);

        PlaneObject.transform.rotation = Quaternion.AngleAxis(90, Vector3.right) * PlaneObject.transform.rotation;
        PlaneObject.transform.parent = ParentObject.transform;
    }

    public void set_new_texture(Texture2D aTex, System.Nullable<Vector2> aSize = null)
    {

        if (aSize == null)
        {
            if (aTex != null)
                BaseDimension = new Vector2(aTex.width, aTex.height);
            else
                BaseDimension = new Vector2(1, 1);
        }
        else
            BaseDimension = aSize.Value;
        //PlaneObject.renderer.material.mainTexture = aTex;
		PlaneMaterial.mainTexture = aTex;
        PixelDimension = BaseDimension;
    }

    public void destroy()
    {
        GameObject.Destroy(ParentObject);
        GameObject.Destroy(PlaneObject);
    }

}
