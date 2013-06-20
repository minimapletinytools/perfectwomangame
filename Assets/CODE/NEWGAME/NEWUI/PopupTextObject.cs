using UnityEngine;
using System.Collections;

public class PopupTextObject : FlatElementMultiBase {
    public FlatElementImage mBackground;
	public FlatElementText mText;
	public int SplitNumber
	{ get; set; }
	
	public bool IsDestroyed
	{ get; private set; }
	
	public override void destroy()
	{
		IsDestroyed = true;
		base.destroy();
	}
	
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
		
		int splitCount = 20;
		if(text.Length >= splitCount)
		{
			
			int splitIndex = (text.Length+1)/2;
			string r = text.Substring(0,splitIndex);
			if( text[splitIndex] != ' ' && text[splitIndex-1] != ' ')
				r += "-";
			r += "\n" + text.Substring(splitIndex);
			return r;
		}
		return text;
	}
	
    public PopupTextObject(string aText, int aDepth)
    {
		IsDestroyed = false;
		
		SplitNumber = 20;
		mBackground = new FlatElementImage(random_bubble(0), aDepth);
		
		
		float textOffset = 0;
		if(aText.Length > 25  && aText.Length < 50)
		{
			textOffset = 25;
			aText = FlatElementText.convert_to_multiline(2,aText);
		} else if (aText.Length >= 50)
		{
			textOffset = 50;
			aText = FlatElementText.convert_to_multiline(3,aText);
		}
		
		mText = new FlatElementText(ManagerManager.Manager.mNewRef.genericFont,120,aText,aDepth+1);
		mText = new FlatElementText(ManagerManager.Manager.mNewRef.genericFont,100,aText,aDepth+1);
        mText.HardColor = new Color(0, 0, 0);
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
    public void set_text_color(Color aColor)
    {
        mText.SoftColor = aColor;
    }
	public void set_background_color(Color aColor)
	{
		mBackground.SoftColor = aColor;	
	}
}
