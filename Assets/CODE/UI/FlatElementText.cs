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
        PrimaryGameObject = new GameObject("genTextElement");
        mRenderer = PrimaryGameObject.AddComponent<MeshRenderer>();
        //TODO set material
        mMesh = PrimaryGameObject.AddComponent<TextMesh>();
        mMesh.font = aFont;
        Size = aSize;
        Text = aText;
        Depth = aDepth;
    }
}
