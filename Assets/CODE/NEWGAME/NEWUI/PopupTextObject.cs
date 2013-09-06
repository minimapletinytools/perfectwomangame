using UnityEngine;
using System.Collections;

public class PopupTextObject : FlatElementMultiBase {
    public FlatElementImage mBackground;
	public FlatElementText mText;
	public FlatElementMultiBase.ElementOffset mTextOffset;
	public int SplitNumber
	{ get; set; }
	
	bool mDestroyed = false;
	public bool IsDestroyed
	{ 	get{
			//kinda hacky but it's fine
			return mDestroyed | (SoftColor == new Color(0.5f,0.5f,0.5f,0));
		}
		private set{
			mDestroyed = value;
		} 
	}
	
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
			string aText = value;
			SplitNumber = 20;
			float textOffset = 0;
			if(aText.Length > 20  && aText.Length < 40)
			{
				textOffset = 15;
				aText = FlatElementText.convert_to_multiline(2,aText);
			} else if (aText.Length >= 40 && aText.Length < 55)
			{
				textOffset = 30;
				aText = FlatElementText.convert_to_multiline(3,aText);
			} else if (aText.Length >= 55)
			{
				textOffset = 45;
				aText = FlatElementText.convert_to_multiline(4,aText);
			}
			mText.Text = aText.ToUpper();
			mTextOffset.Position = new Vector3(0,textOffset,0);
			
			if(aText.Length > 80)
				set_font_size(80);
			if(aText.Length > 100)
				set_font_size(70);
			if(aText.Length > 120)
				set_font_size(60);
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
		
		
		mBackground = new FlatElementImage(random_bubble(0), aDepth);
		
		
		
		mText = new FlatElementText(ManagerManager.Manager.mNewRef.genericFont,100,"",aDepth+1);
        mText.HardColor = new Color(0, 0, 0);
		mElements.Add(new FlatElementMultiBase.ElementOffset(mBackground, new Vector3(0, 0, 0)));
		mTextOffset = new FlatElementMultiBase.ElementOffset(mText, new Vector3(0, 0, 0));
		mElements.Add(mTextOffset);
		Text = aText;
        PrimaryGameObject = create_primary_from_elements();
		
		Depth = aDepth;
    }
	
	Texture2D random_bubble(int size)
	{
		//TODO size
		return ManagerManager.Manager.mNewRef.textSmallBubble[Random.Range(0,ManagerManager.Manager.mNewRef.textSmallBubble.Length)];
	}
	public void set_font_size(int aSize)
	{
		mText.Size = aSize;
	}
    public void set_text_color(Color aColor,bool hard = false)
    {
		if(!hard)
			mText.SoftColor = aColor;	
		else mText.HardColor = aColor;	
    }
	//hack
	public void fade_out()
	{
		Color c = mText.SoftColor;
		c.a = 0;
		SoftColor = new Color(0.5f,0.5f,0.5f,0);
		mText.SoftColor = c;
	}
	public void set_background_color(Color aColor, bool hard = false)
	{
		if(!hard)
			mBackground.SoftColor = aColor;	
		else mBackground.HardColor = aColor;	
	}
	
	public override Color SoftColor
    {
        get { return base.SoftColor; }
        set { 
            base.SoftColor = value;
			//TODO make fading work right
        }
    }
	
	
	public static System.Func<float,bool> skip(float displayDur, PopupTextObject po)
	{
		return delegate(float aTime){
			if(aTime > displayDur || po == null || po.IsDestroyed)
			{
				return true;
			}
			return false;
		};
	}
}
