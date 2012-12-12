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
        SoftColor = (new Color(0.5f, 0, 0, 0.5f))*mCurrentPercentage + (new Color(0,0,0.5f,0.5f))*(1-mCurrentPercentage); //hack
        if (Style == FillStyle.DU)
        {
            PrimaryGameObject.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(1f, mCurrentPercentage);
            //float scaleX = Mathf.Cos(Time.time) * 0.5F + 1;
            //float scaleY = Mathf.Sin(Time.time) * 0.5F + 1;
            //PrimaryGameObject.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(scaleX, scaleY);
            Vector2 dim = mImage.PixelDimension;
            dim.y = mImage.BaseDimension.y * mCurrentPercentage;
            mImage.PixelDimension = dim;
        }
        else
            throw new UnityException("Peter was too lazy to implement this fill style");
        base.update_parameters(aDeltaTime);
    }
    public override void set_position(Vector3 aPos)
    {
        if (Style == FillStyle.DU)
            PrimaryGameObject.transform.position = aPos + new Vector3(0,-mPercentage*mImage.BaseDimension.y/2,0);
    }
}
