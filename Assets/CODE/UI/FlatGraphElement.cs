using UnityEngine;
using System.Collections;

public class FlatGraphElement : FlatElementImage {
    Texture2D mGraphTexture;

    public FlatGraphElement(int width, int height, int aDepth):base(null,aDepth)
    {
        mGraphTexture = new Texture2D(width,height);
        clear_to_white();

        //kind of a hack
        initialize(mGraphTexture, null, aDepth);
    }
	
	public FlatGraphElement(Texture2D aBgTex, int aDepth):base(aBgTex,aDepth)
    {
        mGraphTexture = aBgTex;
    }

    public void clear_to_white()
    {
        Color[] modifyMe = mGraphTexture.GetPixels();
        for (int i = 0; i < modifyMe.Length; i++)
            modifyMe[i] = new Color(1, 1, 1);
        mGraphTexture.SetPixels(modifyMe);
        mGraphTexture.Apply();
    }
    //TODO test
    //x y are in [0,1] from lower left
    public void draw_point(Vector2 aCenter, float radius, Color aColor)
    {
        Color[] modifyMe = mGraphTexture.GetPixels();
        int x = (int)(aCenter.x * mGraphTexture.width);
        int y = (int)(aCenter.y * mGraphTexture.height);

        for (int i = -(int)(radius + 1); i < radius + 1; i++)
        {
            int mx = x + i;
            if (mx < 0 || mx >= mGraphTexture.width - 1)
                continue;
            for (int j = -(int)(radius + 1); j < radius + 1; j++)
            {
                int my = y + j;
                if (my < 0 || my >= mGraphTexture.height - 1)
                    continue;
                float distance = (Mathf.Abs(i) + Mathf.Abs(j)) / (2*radius);
                Color orig = modifyMe[my * mGraphTexture.width + mx];
                modifyMe[my * mGraphTexture.width + mx] = orig * (distance) + aColor * (1-distance);
            }
        }
        mGraphTexture.SetPixels(modifyMe);
        mGraphTexture.Apply();
    }
}
