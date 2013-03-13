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
		PrimaryGameObject = GameObject.Instantiate(fontPrefab) as GameObject;
		mRenderer = PrimaryGameObject.renderer;
		mMesh = PrimaryGameObject.GetComponent<TextMesh>();
		mMesh.anchor = TextAnchor.MiddleCenter;
        Size = aSize;
        Text = aText;
		Depth = aDepth;
	}
	
	//this wont work...
    public FlatElementText(Font aFont, int aSize, string aText, int aDepth)
    {
        
        PrimaryGameObject = new GameObject("genTextElementParent");
        GameObject textElement = new GameObject("genTextElement");
        textElement.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
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
}
