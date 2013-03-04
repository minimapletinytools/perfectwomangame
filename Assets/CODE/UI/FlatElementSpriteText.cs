using UnityEngine;
using System.Collections;


public class FlatElementSpriteChar : FlatElementImage {
	static string sWordOrder = "0123456789abcdefghijklmnopqrstuvwxyz!?[]{}()\\/+-*\" ,.:;=%<>|'^#_";
    public FlatElementSpriteChar(Texture2D aTex, int pixelWidth, char aChar, int aDepth):base(aTex,aDepth)
    {
		int index = sWordOrder.IndexOf(aChar);
		mImage.PixelDimension = new Vector2(pixelWidth,aTex.height);
		//mImage.ParentObject.GetComponentInChildren<Renderer>().material.SetTextureScale("_MainTex", new Vector2(pixelWidth / (float) aTex.width,1));
        mImage.TextureScale = new Vector2(pixelWidth / (float) aTex.width,1);
		mImage.TextureOffset = new Vector2((index*pixelWidth)/(float)aTex.width,0);
		
    }

}
public class FlatElementSpriteText : FlatElementMultiBase {
	bool mHasWord = false;
	Texture2D mTex = null;
	int mCharacterWidth;
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
	Vector3 get_offset()
	{
		return new Vector3(-mCharacterWidth * (4 + mElements.Count),0,0); //TODO wrapping
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
		
		mElements.Add(new FlatElementMultiBase.ElementOffset(new FlatElementSpriteChar(mTex,mCharacterWidth,aWord,Depth),get_offset()));
	}
	void add_string(string aStr)
	{	
		foreach(char s in aStr)
			add_word(s);
	}
	void clear()
	{
		destroy();
		mHasWord = false;
		create_filler();
	}
		
}
