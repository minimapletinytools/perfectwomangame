using UnityEngine;
using System.Collections;

public class PopupTextObject : FlatElementMultiBase {
    public FlatElementImage mBackground;
	public FlatElementSpriteText mText;
	public FlatElementSpriteText mTextLine1; //MULTILINE VERSIONS
	public FlatElementSpriteText mTextLine2;

    public PopupTextObject(Texture2D aBgTex, int aDepth)
    {
		//TODO finish and position
		
		mBackground = new FlatElementImage(aBgTex, aDepth);
		//mText = new FlatElementSpriteText(,,"",aDepth+1);
		//mTextLine1 = new FlatElementSpriteText(,,"",aDepth+1);
		//mTextLine2 =  = new FlatElementSpriteText(,,"",aDepth+1);
        
		mElements.Add(new FlatElementMultiBase.ElementOffset(mBackground, new Vector3(0, 0, 0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mText, new Vector3(0, 0, 0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mTextLine1, new Vector3(0, 0, 0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mTextLine2, new Vector3(0, 0, 0)));
        PrimaryGameObject = create_primary_from_elements();
		Depth = aDepth;
    }
	
	public void set_text_one_line(string aText)
	{
		mTextLine1.clear();
		mTextLine2.clear();
		mText.new_string(aText);
	}
	
	public void set_text_two_line(string l1, string l2)
	{
		mText.clear();
		mTextLine1.new_string(l1);
		mTextLine2.new_string(l2);
	}
}
