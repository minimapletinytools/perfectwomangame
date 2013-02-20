using UnityEngine;
using System.Collections;

public class FlatGraphElement : FlatElementImage {
    Texture2D mGraphTexture;

    public FlatGraphElement(int width, int height, int aDepth):base(null,aDepth)
    {
        mGraphTexture = new Texture2D(width,height);
        mImage.set_new_texture(mGraphTexture);
    }

    public void draw_point(int x, int y, Color aColor)
    {
        //TODO
    }

    
    public ImageGameObjectUtility mImage;
}
