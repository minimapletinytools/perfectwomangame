using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
	
	public TextAlignment Alignment
	{
		get { return mMesh.alignment; }
		set { mMesh.alignment = value; }
	}
	
	public TextAnchor Anchor
	{
		get { return mMesh.anchor; }
		set { mMesh.anchor = value; }
	}

    
    
   	public override Rect BoundingBox {
		get {
			/*GUIStyle style = new GUIStyle();
			style.font = mMesh.font;
			style.fontSize = Size;
			Vector2 size = style.CalcSize(new GUIContent(aText.Aggregate((e,f) => e + f))).x;*/

			//TODO this is probably wrong w/e
			var r = mMesh.renderer.bounds;
			return new Rect(r.min.x,r.max.y,r.size.x,r.size.y);
		}
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
		mMesh.alignment = TextAlignment.Center;
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
		mMesh.alignment = TextAlignment.Center;
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
		int[] ideal = Enumerable.Repeat((aString.Length/numberSplits),numberSplits).ToArray();
		return convert_to_multiline(ideal,aString);
	}
	
	public static float grade_words(int ideal, string[] words)
	{
		return Mathf.Pow((ideal - (words.Sum(e => e.Length) + words.Length - 1)),2);
	}
	
	public static int[] best_fit(int[] ideal, string[] words, out float aScore)
	{
		if(ideal.Length == 1)
		{
			aScore = grade_words(ideal[0],words);
			return new int[]{words.Length};
		}
		float minScore = Mathf.Infinity;
		int[] r = null;
		for(int i = 0; i < words.Length; i++)
		{
			
			float cScore = grade_words(ideal[0],words.Take(i).ToArray());
			float oScore;
			int[] answer = (best_fit(ideal.Skip(1).ToArray(), words.Skip(i).ToArray(),out oScore)).ToArray();
			if(cScore + oScore < minScore)
			{
				minScore = cScore + oScore;
				r = new int[]{i};
				r = r.Concat(answer).ToArray();
			}
		}
		aScore = minScore;
		return r;
	}
	public static string convert_to_multiline(int[] ideal, string aString)
	{
		string[] process = aString.Split(new string[] { " " }, System.StringSplitOptions.None);
		float dummy;
		int[] split = best_fit (ideal,process,out dummy);
		
		string r = "";
		for(int i = 0; i < split.Length; i++)
		{
			if(split[i] > 0)
			{
				r += process.Take(split[i]).Aggregate((e,f) => e + " " + f);
				if(i != split.Length -1)
					r += "\n";
				process = process.Skip(split[i]).ToArray();
			}
		}
		return r;
	}
	
}
