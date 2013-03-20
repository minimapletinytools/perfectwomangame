using UnityEngine;
using System.Collections;

public class PopupTextObject : FlatElementMultiBase {
    public FlatElementImage mBackground;
	public FlatElementText mText;
	int mSize = 0;
	
	public string Text
	{
		get{
			return mText.Text;
		}
		set{
			mText.Text = value;
		}
	}
	
	bool need_split(string text)
	{
		return split_text(text).Contains("\n");
	}
	string split_text(string text)
	{
		int splitCount = mSize == 0 ? 20 : 30;
		if(text.Length >= splitCount)
		{
			int splitIndex = (text.Length+1)/2;
			string r = text.Substring(0,splitIndex);
			if( text[splitIndex] != ' ')
				r += "-";
			r += "\n" + text.Substring(splitIndex);
			return r;
		}
		return text;
	}
	
    public PopupTextObject(string aText, int aDepth)
    {
		//TODO finish and position
		
		mBackground = new FlatElementImage(random_bubble(0), aDepth);
		
		
		float textOffset = 0;
		if(need_split(aText))
		{
			textOffset = 25;
			aText = split_text(aText);
		}
		
		mText = new FlatElementText(ManagerManager.Manager.mNewRef.genericFont,1200,aText,aDepth+1);
		mElements.Add(new FlatElementMultiBase.ElementOffset(mBackground, new Vector3(0, 0, 0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mText, new Vector3(0, textOffset, 0)));
        PrimaryGameObject = create_primary_from_elements();
		Depth = aDepth;
    }
	
	Texture2D random_bubble(int size)
	{
		//TODO size
		return ManagerManager.Manager.mNewRef.textSmallBubble[Random.Range(0,ManagerManager.Manager.mNewRef.textSmallBubble.Length)];
	}
}
