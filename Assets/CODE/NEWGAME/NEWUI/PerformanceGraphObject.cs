using UnityEngine;
using System.Collections;

public class PerformanceGraphObject  : FlatElementMultiBase {
    public FlatGraphElement mBackground;
	public FlatElementImage mForeground;
	public PerformanceGraphObject(int aDepth)
	{
		mBackground = new FlatGraphElement(ManagerManager.Manager.mNewRef.bbGraphBackground,aDepth);	
		mForeground = new FlatElementImage(ManagerManager.Manager.mNewRef.bbGraphFrame,aDepth+1);
		mElements.Add(new FlatElementMultiBase.ElementOffset(mBackground, new Vector3(0,0,0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mForeground, new Vector3(0,0,0)));
        PrimaryGameObject = create_primary_from_elements();
		Depth = aDepth;
	}
	
	public void update_graph(float aTime, float aScore)
	{
		//TODO needs to compensate for the fact that the score box is not exactly in the center
		//TODO colorInterp should go trhough more colors...
		//Color colorInterp = (new Color(1,0,0)) * (1-aScore) + (new Color(0,1,0.5f)) * (aScore);
		Color colorInterp = new Color(0,0,0);
		mBackground.draw_point(new Vector2(aTime,aScore),1,colorInterp);
	}
}
