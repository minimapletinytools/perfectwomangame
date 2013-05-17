using UnityEngine;
using System.Collections;

public class FlatElementText : FlatElementBase
{
    TextMesh mMesh;
    Renderer mRenderer;

    public int Size
    {
        get
        {
            return mMesh.fontSize;
        }
        set
        {
            mMesh.fontSize = value;
        }
    }

    public string Text
    {
        get { return mMesh.text; }
        set { mMesh.text = value; }
    }
            
	public FlatElementText(GameObject fontPrefab, int aSize, string aText, int aDepth)
	{
		PrimaryGameObject = new GameObject("genTextElementParent");
		GameObject textElement = GameObject.Instantiate(fontPrefab) as GameObject;
		textElement.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
		textElement.transform.localScale = new Vector3(10,10,10);
		mRenderer = PrimaryGameObject.renderer;
		mMesh = PrimaryGameObject.GetComponent<TextMesh>();
		mMesh.anchor = TextAnchor.MiddleCenter;
        Size = aSize;
        Text = aText;
		
		textElement.transform.parent = PrimaryGameObject.transform.parent;
		Depth = aDepth;
	}
	
    public FlatElementText(Font aFont, int aSize, string aText, int aDepth)
    {
        PrimaryGameObject = new GameObject("genTextElementParent");
        GameObject textElement = new GameObject("genTextElement");
        textElement.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
		textElement.transform.localScale = new Vector3(10,10,10);
		
        mRenderer = textElement.AddComponent<MeshRenderer>();
        mMesh = textElement.AddComponent<TextMesh>();
        mMesh.font = aFont;
        mMesh.anchor = TextAnchor.MiddleCenter;
        Size = aSize;
        Text = aText;
		mRenderer.material = mMesh.font.material;
		
        textElement.transform.parent = PrimaryGameObject.transform;
        Depth = aDepth;
    }
	
	public override void destroy ()
	{
		GameObject.Destroy(PrimaryGameObject);
	}
	//this is stupid and its here to make the font shadre compatible with the usual shader..
	public override void set_color(Color aColor)
    {
		float r = Mathf.Clamp01(aColor.r*2);
		float g = Mathf.Clamp01(aColor.g*2);
		float b = Mathf.Clamp01(aColor.b*2);
		float a = Mathf.Clamp01(aColor.a*2);
		Color setMe = new Color(r,g,b,a);
		//mRenderer.material.SetColor("_TintColor", setMe); 
		base.set_color(setMe);
    }
	
	
	public static string convert_to_multiline(int numberSplits, string aString)
	{
		//TODO
		return aString;
	}
	
	
	public static string convert_to_multiline(int[] max_chars, string aString)
	{
		//TOOD
		return aString;
	}
	
}
