using UnityEngine;
using System.Collections;

public class FlatRenderTextureGraphElement : FlatElementImage {
	RenderTexture mRenderTexture = null;
    public FlatRenderTextureGraphElement(int width, int height, int aDepth):base(null,aDepth)
    {
		mRenderTexture = new RenderTexture(width,height,0);
        initialize(mRenderTexture, null, aDepth);
    }
	
	public FlatRenderTextureGraphElement(Texture2D aBgTex, int aDepth):base(null,aDepth)
    {
        mRenderTexture = new RenderTexture(aBgTex.width,aBgTex.height,0);
		
        initialize(mRenderTexture, null, aDepth);
		draw_background(aBgTex);
    }
    
    //x y are in [0,1] from lower left
    public void draw_point(Vector2 aCenter, GameObject aDot, Color aColor)
    {
		aDot.GetComponentInChildren<Renderer>().renderer.material.color = aColor;
		Vector2 newCenter = 2*(aCenter - new Vector2(0.5f,0.5f));
		draw_gameObject(aDot,newCenter);
    }
	
	//position is relative to camera
	public void draw_gameObject(GameObject go, Vector2 position, bool clear = false)
	{
		
		Camera cam = (new GameObject("genDrawCamera")).AddComponent<Camera>();
		cam.aspect = mRenderTexture.width/(float)mRenderTexture.height;
		cam.isOrthoGraphic = true;
		cam.orthographicSize = mRenderTexture.height/2f;
		cam.targetTexture = mRenderTexture;
		
		if(clear)
		{
			cam.clearFlags = CameraClearFlags.SolidColor;
			cam.backgroundColor = new Color(0,0,0,0);
			cam.DoClear();
		}
		else
		{
			cam.clearFlags = CameraClearFlags.Nothing;
		}

		
		Vector3 center = new Vector3(-10000,-10000,0);
		cam.transform.position = center + new Vector3(0,0,-10);
		cam.transform.LookAt(center);
		
		go.transform.position = center + new Vector3(position.x*mRenderTexture.width/2f,position.y*mRenderTexture.height/2f,0);
		
		//return Center + new Vector3(aX * Width / 2.0f, aY * Height / 2.0f, 0);
		cam.Render();
		GameObject.Destroy(cam.gameObject);
	}
	
	//ugg
	public void draw_background(Texture2D aBg)
	{
		GameObject bg = ImageGameObjectUtility.create(aBg);
		bg.GetComponentInChildren<Renderer>().material.shader = ManagerManager.Manager.mReferences.mGraphShader;
		draw_gameObject(bg,Vector2.zero,true);
		GameObject.Destroy(bg);
	}
}
