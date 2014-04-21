using UnityEngine;
using System.Collections;

public class ImageGameObjectUtility  
{

    public static GameObject create(Texture aTex)
    {
        return (new ImageGameObjectUtility(aTex)).ParentObject;
    }

    public GameObject ParentObject { get; private set; }
    public GameObject PlaneObject { get; private set; }
    public Material PlaneMaterial { get; private set; }
	
	public Vector2 TextureOffset {
		get{
			return PlaneObject.renderer.material.mainTextureOffset;
		}
		set{
			PlaneObject.renderer.material.mainTextureOffset = value;
		}
	}
	public Vector2 TextureScale {
		get{
			return PlaneObject.renderer.material.mainTextureScale;
		}
		set{
			PlaneObject.renderer.material.mainTextureScale = value;
		}
	}
    public Vector2 BaseDimension { get; private set; }
    public Vector2 PixelDimension
    {
        get 
        { 
            Vector3 planeScale = PlaneObject.transform.localScale;
            return new Vector2(planeScale.x*10, planeScale.z*10); //techincally, you need to do something like convert units
        }
        set
        {
            PlaneObject.transform.localScale = new Vector3(BodyManager.convert_units(value.x) / 10.0f, 1, BodyManager.convert_units(value.y) / 10.0f);
        }
    }

    public ImageGameObjectUtility(Texture aTex, System.Nullable<Vector2> aSize = null)
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
		ParentObject.transform.position = new Vector3(12321,124345,1234); //this will solve some one frame appearance glitches 
    }

    public void set_new_texture(Texture aTex, System.Nullable<Vector2> aSize = null)
    {
		aSize *= GameConstants.SCALE;
        if (aSize == null)
        {
            if (aTex != null)
                BaseDimension = new Vector2(aTex.width, aTex.height);
            else
                BaseDimension = new Vector2(1, 1);
        }
        else
            BaseDimension = aSize.Value;
        PlaneObject.renderer.material.mainTexture = aTex;	
        PixelDimension = BaseDimension;
    }

	
	//0,0, is upper left, width,height is bottom right
	//always relative to BaseDimension ofc
	public void pixel_crop(Rect pixelRect)
	{
		relative_crop(new Rect(pixelRect.x / BaseDimension.x, 
		                       pixelRect.y / BaseDimension.y, 
		                       pixelRect.width / BaseDimension.x, 
		                       pixelRect.height / BaseDimension.y));
	}
	
	//0,0 is upper left, 1,1, is bottom right
	public void relative_crop(Rect relRect)
	{
		//TODO check if this is right???
		TextureOffset = -new Vector2(relRect.x,relRect.y);
		TextureScale = new Vector2(relRect.width,relRect.height);
		PixelDimension = new Vector2(relRect.width * BaseDimension.x,relRect.height * BaseDimension.y);
	}


    public void destroy()
    {
        GameObject.Destroy(ParentObject);
        GameObject.Destroy(PlaneObject);
    }

}
