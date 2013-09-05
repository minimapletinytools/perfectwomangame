using UnityEngine;
using System.Collections;

public class MeterImageObject : FlatElementBase
{

    float mCurrentPercentage;
    float mPercentage;
    public float Percentage
    {
        get
        {
            return mPercentage;
        }
        set
        {
            mPercentage = value;
        }
    }

    public enum FillStyle
    {
        LR,RL,UD,DU
    }
    public FillStyle Style{get; set;}
    public ImageGameObjectUtility mImage;
    public MeterImageObject(Texture2D aTex, FillStyle aStyle, int aDepth)
    {
		SoftColor = new Color(0.5f,0.5f,0.5f,1);
        Style = aStyle;
        mImage = new ImageGameObjectUtility(aTex);
        PrimaryGameObject = mImage.ParentObject;
        Depth = aDepth;
        mCurrentPercentage = mPercentage = 0;
    }

    public override void destroy()
    {
        mImage.destroy();
    }

    //TODO/NOTE the way I handle this, it wont work withscaling because I'm reading base scale from BaseDimension...
    public override void update_parameters(float aDeltaTime)
    {
        mCurrentPercentage = mCurrentPercentage * (1 - SoftInterpolation) + mPercentage * (SoftInterpolation);
		//TODO the right way to do this is to use SoftColor to cache the desired color
		//and actually set it over here blending against the meter color.
        //SoftColor = (new Color(0.5f, 0, 0, 0.2f))*mCurrentPercentage + (new Color(0,0,0.5f,0.2f))*(1-mCurrentPercentage); //hack
		//base.SoftColor =  new Color32(0/2, 81/2, 229/2,(int)(.8*255/2f));
        if (Style == FillStyle.DU)
        {
            Material m = PrimaryGameObject.GetComponentInChildren<Renderer>().material;
            m.mainTextureScale = new Vector2(1f, mCurrentPercentage);
            //m.mainTextureOffset = new Vector2(0, mCurrentPercentage == 0 ? 0 : 1/mCurrentPercentage);
            m.mainTextureOffset = new Vector2(0, 0);
            //float scaleX = Mathf.Cos(Time.time) * 0.5F + 1;
            //float scaleY = Mathf.Sin(Time.time) * 0.5F + 1;
            //PrimaryGameObject.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(scaleX, scaleY);
            Vector2 dim = mImage.PixelDimension;
            dim.y = mImage.BaseDimension.y * mCurrentPercentage;
            mImage.PixelDimension = dim;
        }
        else
            throw new UnityException("Peter was too lazy to implement this fill style");
        this.SoftInterpolation = 1;
        base.update_parameters(aDeltaTime);
        
    }
    public override void set_position(Vector3 aPos)
    {
        if (Style == FillStyle.DU)
            PrimaryGameObject.transform.position = aPos + new Vector3(0,-(1-mPercentage)*mImage.BaseDimension.y/2f,0);
    }
	
	public override Color SoftColor
	{
		set{
			base.SoftColor = new Color32(0/2, 81/2, 229/2,(int)(.8*255/2f))*value*2;
		}
	}
}
