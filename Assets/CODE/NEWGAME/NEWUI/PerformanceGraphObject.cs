using UnityEngine;
using System.Collections;

public class PerformanceGraphObject  : FlatElementMultiBase {
    public FlatRenderTextureGraphElement mBackground;
	public FlatElementImage mForeground;
	GameObject mStupidDot;
	public PerformanceGraphObject(int aDepth)
	{
		//mBackground = new FlatGraphElement(ManagerManager.Manager.mNewRef.bbGraphBackground,aDepth);	
		mBackground = new FlatRenderTextureGraphElement(ManagerManager.Manager.mNewRef.bbGraphBackground,aDepth);	
		mForeground = new FlatElementImage(ManagerManager.Manager.mNewRef.bbGraphFrame,aDepth+1);
		mElements.Add(new FlatElementMultiBase.ElementOffset(mBackground, new Vector3(0,0,0)));
		mElements.Add(new FlatElementMultiBase.ElementOffset(mForeground, new Vector3(0,0,0)));
        PrimaryGameObject = create_primary_from_elements();
		Depth = aDepth;
		
		
		mStupidDot = ImageGameObjectUtility.create(ManagerManager.Manager.mNewRef.bbGraphDot);
		mStupidDot.GetComponentInChildren<Renderer>().material.color = new Color(0,0,0,1);
		mStupidDot.GetComponentInChildren<Renderer>().material.shader = ManagerManager.Manager.mReferences.mGraphShader;
	}
	
	public void update_graph(float aTime, float aScore)
	{
		aScore = 0.06f + aScore * (0.71f-0.06f);
		aTime = 0.016f + aTime * (0.986f-0.016f);
		//TODO needs to compensate for the fact that the score box is not exactly in the center
		//TODO colorInterp should go trhough more colors...
		//Color colorInterp = (new Color(1,0,0)) * (1-aScore) + (new Color(0,1,0.5f)) * (aScore);
		Color colorInterp = new Color(1,0,0,1);
		
		//mBackground.draw_point(new Vector2(aTime,aScore),5,colorInterp);
		
		mBackground.draw_point(new Vector2(aTime,aScore),mStupidDot,colorInterp);
		
		mStupidDot.GetComponentInChildren<Renderer>().material.color = new Color(0,0,0,1);
	}
}
