using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class ColorTextObject : FlatElementMultiBase {
	public FlatElementText mText;
	GameObject mDummyObject = null;
	
    public ColorTextObject(int aDepth)
    {
		mDummyObject = new GameObject("genColorTextObjectDummy");
        PrimaryGameObject = mDummyObject;
		
		Depth = aDepth;
    }
	
	public void set_text(string[] aText, Color[] aColors)
	{
		if(mDummyObject != null)
			GameObject.Destroy(mDummyObject);
		mDummyObject = null;
		
		destroy();
		
		GUIStyle style = new GUIStyle();
		style.font = ManagerManager.Manager.mNewRef.genericFont;
		style.fontSize = 100;
		
		float totalWidth = style.CalcSize(new GUIContent(aText.Aggregate((e,f) => e + f))).x;
		
		float accum = 0;
		for(int i = 0; i < aText.Length; i++)
		{
			float wordWidth = style.CalcSize(new GUIContent(aText[i])).x;
			var addMe = new FlatElementText(ManagerManager.Manager.mNewRef.genericFont,100,aText[i],Depth);
			addMe.HardColor = aColors[i];
			mElements.Add(new FlatElementMultiBase.ElementOffset(addMe,new Vector3(totalWidth/2f - accum - wordWidth/2f,0,0)));
			accum += wordWidth;
		}
		
		PrimaryGameObject = create_primary_from_elements();
		
		//more hack nonsense
		HardPosition = SoftPosition;
		update(1);
	}
	
	
}
