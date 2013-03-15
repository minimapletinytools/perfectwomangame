using UnityEngine;
using System.Collections;

public class PopupTextObject : FlatElementMultiBase {
    public FlatElementImage mBackground;
	public FlatElementText mText;
	
	public string Text
	{
		get{
			return mText.Text;
		}
		set{
			mText.Text = value;
		}
	}
    public PopupTextObject(Texture2D aBgTex, int aDepth)
    {
		//TODO finish and position
		
		mBackground = new FlatElementImage(aBgTex, aDepth);
		mText = new FlatElementText(ManagerManager.Manager.mNewRef.genericFont,500,"",aDepth+1);
        
		mElements.Add(new FlatElementMultiBase.ElementOffset(mBackground, new Vector3(0, 0, 0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mText, new Vector3(0, 0, 0)));
        PrimaryGameObject = create_primary_from_elements();
		Depth = aDepth;
    }
}
