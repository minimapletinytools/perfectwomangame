using UnityEngine;
using System.Collections;
using System.Linq;


public class FlatElementSpriteChar : FlatElementImage {
	static string sWordOrder = "0123456789abcdefghijklmnopqrstuvwxyz!?[]{}()\\/+-*\" ,.:;=%<>|'^#_";
	
	Vector2 mDim;
	public Vector2 Dimensions 
	{
		get{
			return new Vector2(mDim.x*SoftScale.x,mDim.y*SoftScale.y);
		}
		private set
		{
			mDim = value;
		}
	}
    public FlatElementSpriteChar(Texture2D aTex, int pixelWidth, char aChar, int aDepth):base(aTex,aDepth)
    {
		int index = sWordOrder.IndexOf(aChar);
		mImage.PixelDimension = new Vector2(pixelWidth,aTex.height);
		//mImage.ParentObject.GetComponentInChildren<Renderer>().material.SetTextureScale("_MainTex", new Vector2(pixelWidth / (float) aTex.width,1));
        mImage.TextureScale = new Vector2(pixelWidth / (float) aTex.width,1);
		mImage.TextureOffset = new Vector2((index*pixelWidth)/(float)aTex.width,0);
		Dimensions = new Vector2(pixelWidth+1,aTex.height+1);
    }
}
public class FlatElementSpriteText : FlatElementMultiBase {
	bool mHasWord = false;
	Texture2D mTex = null;
	int mCharacterWidth;
	public float mfontScale = 1;
	//float mMaxRowWidth = 200;
	
	public override Vector3 SoftScale
    {
        get { return base.SoftScale; }
        set 
        {
            base.SoftScale = value;
			update_font_size();
        }
    }
	void update_font_size()
	{
		base.SoftScale = SoftScale;
		//lame version
		float netWidth = mElements.Sum(x => ((FlatElementSpriteChar)x.Element).Dimensions.x);
		float curWidth = 0;
		for(int i = 0; i < mElements.Count; i++)
		{
			var e = mElements[i];
			var f = e.Element as FlatElementSpriteChar;
			//f.SoftScale = new Vector3(FontScale,FontScale,FontScale);
			e.Position = new Vector3(netWidth/2 - curWidth,0,0);
			curWidth += f.Dimensions.x;
		}
		SoftPosition = SoftPosition; //this forces an update on position
	}
	public FlatElementSpriteText(Texture2D aTex, int pixelWidth, string aMsg, int aDepth)
	{
		mTex = aTex;
		mCharacterWidth = pixelWidth;
		create_filler();
		add_string(aMsg);
		Depth = aDepth;
        PrimaryGameObject = create_primary_from_elements();
        SoftInterpolation = SoftInterpolation;
	
	}
	void create_filler()
	{
		add_word(' ');
		PrimaryGameObject = create_primary_from_elements(); //for empty words, we need a dummy
		mHasWord = false;
	}
	void add_word(char aWord)
	{
		if(!mHasWord)
			destroy();
		mHasWord = true;
		mElements.Add(new FlatElementMultiBase.ElementOffset(new FlatElementSpriteChar(mTex,mCharacterWidth,aWord,Depth),Vector3.zero));
		update_font_size();
	}
	public void add_string(string aStr)
	{	
		foreach(char s in aStr)
			add_word(s);
	}
	public void clear()
	{
		destroy();
		mHasWord = false;
		create_filler();
	}
	public void new_string(string aStr)
	{
		clear();
		add_string(aStr);
	}
		
}
