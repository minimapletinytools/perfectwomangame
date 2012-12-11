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
            
    public FlatElementText(Font aFont, int aSize, string aText, int aDepth)
    {
        
        PrimaryGameObject = new GameObject("genTextElementParent");
        GameObject textElement = new GameObject("genTextElement");
        textElement.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        mRenderer = textElement.AddComponent<MeshRenderer>();
        mRenderer.material = aFont.material;
        mMesh = textElement.AddComponent<TextMesh>();
        mMesh.font = aFont;
        mMesh.anchor = TextAnchor.MiddleCenter;
        Size = aSize;
        Text = aText;
        textElement.transform.parent = PrimaryGameObject.transform;
        Depth = aDepth;
    }
}
