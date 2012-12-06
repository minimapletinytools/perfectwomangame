using UnityEngine;
using System.Collections;

public class MeterObject : FlatElementBase {

    float mLength = 1;
    //TODO
    GameObject mFillObject;
    public MeterObject(Texture2D aFront, Texture2D aBack, Color aFill, float aDepth)
    {
        Depth = aDepth;
        SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); //= aFill;
        PrimaryGameObject = new GameObject("genFlatElementImageParent");

        mFillObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        GameObject c1 = GameObject.CreatePrimitive(PrimitiveType.Plane);
        GameObject c2 = GameObject.CreatePrimitive(PrimitiveType.Plane);



        GameObject[] planes = new GameObject[] { c2, mFillObject, c1};
        Texture2D[] textures = new Texture2D[] { aBack, aBack, aFront };
        int i = 0;
        foreach(GameObject e in planes)
        {
            e.transform.localScale = new Vector3(
            BodyManager.convert_units(textures[i].width) / 10.0f, 1,
            BodyManager.convert_units(textures[i].height) / 10.0f);
            e.transform.rotation = Quaternion.AngleAxis(90, Vector3.right) * e.transform.rotation;
            e.transform.position = e.transform.position + new Vector3(0, 0, i*0.000001f);
            e.renderer.material.mainTexture = textures[i];
            e.transform.position += new Vector3(0, 0, 0.00001f) * i;
            e.transform.parent = PrimaryGameObject.transform;
            i++;
        }
        mFillObject.renderer.material.mainTexture = null;
        mFillObject.renderer.material.color = aFill;
        //UCK hardcoding the location of meter...
        //TODO
    }

    //TODO overload color setting nonsense
}
