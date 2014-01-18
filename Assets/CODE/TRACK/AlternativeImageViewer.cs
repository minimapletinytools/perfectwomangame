using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

//need not be MB?
public class AlternativeImageViewer : MonoBehaviour
{
	[Serializable()]
	public class ScorePair
	{
		float score;
		DateTime date; 
		byte[] image;
	}
	
	public Texture2D take_color_image()
	{
		if(ManagerManager.Manager.mZigManager.is_reader_connected() == 2)
		{
			UpdateTexture (ZigInput.Image,ZigInput.LabelMap);
			//Debug.Log ("updated image");
		}
		return imageTexture;
	}

	public ZigResolution imageResolution = ZigResolution.VGA_640x480;
    Texture2D imageTexture = null;
    ResolutionData imageSizeData;
    Color32[] imageOutputPixels;

	public ZigResolution userResolution = ZigResolution.QQVGA_160x120;
	//Texture2D userTexture;
	ResolutionData userTextureSize;
	Color32 defaultColor = new Color(255,255,255,255);
	Color32 bgColor = new Color32(0,0,0,0);
	Color32[] labelToColor = new Color32[1] {new Color(255,255,255,255)};
	Color32[] userOutputPixels;
	
    void Start()
    {
        imageSizeData = ResolutionData.FromZigResolution(imageResolution);
        imageTexture = new Texture2D(imageSizeData.Width, imageSizeData.Height);
        imageTexture.wrapMode = TextureWrapMode.Clamp;
        imageOutputPixels = new Color32[imageSizeData.Width * imageSizeData.Height];

		userTextureSize = ResolutionData.FromZigResolution(userResolution);
		//userTexture = new Texture2D(userTextureSize.Width, userTextureSize.Height);
		//userTexture.wrapMode = TextureWrapMode.Clamp;             
		userOutputPixels = new Color32[userTextureSize.Width * userTextureSize.Height];

		//already added by depth..
        //ZigInput.Instance.AddListener(gameObject);
    }

	void UpdateTexture(ZigImage image, ZigLabelMap labelmap)
    {

		short[] rawLabelMap = labelmap.data;
		int labelMapIndex = 0;
		int labelMapFactorX = labelmap.xres / userTextureSize.Width;
		int labelMapFactorY = ((labelmap.yres / userTextureSize.Height) - 1) * labelmap.xres;
		// invert Y axis while doing the update
		for (int y = userTextureSize.Height - 1; y >= 0; --y, labelMapIndex += labelMapFactorY)
		{
			int outputIndex = y * userTextureSize.Width;
			for (int x = 0; x < userTextureSize.Width; ++x, labelMapIndex += labelMapFactorX, ++outputIndex)
			{
				short label = rawLabelMap[labelMapIndex];
				userOutputPixels[outputIndex] = (label>0) ? ((label <= labelToColor.Length) ? labelToColor[label-1] : defaultColor) : bgColor;                
			}
		}
		//userTexture.SetPixels32(userOutputPixels);
		//userTexture.Apply();

	
        Color32[] rawImageMap = image.data;
        int srcIndex = 0;
        int factorX = image.xres / imageSizeData.Width;
        int factorY = ((image.yres / imageSizeData.Height) - 1) * image.xres;
        // invert Y axis while doing the update
        for (int y = imageSizeData.Height - 1; y >= 0; --y, srcIndex += factorY) {
            int outputIndex = y * imageSizeData.Width;
            for (int x = 0; x < imageSizeData.Width; ++x, srcIndex += factorX, ++outputIndex) 
			{
				int userY = (int)((y/(float)imageSizeData.Height)*userTextureSize.Height);
				int userX = (int)((x/(float)imageSizeData.Width)*userTextureSize.Width);
				int userIndex = userY*userTextureSize.Width + userX;
				if(userOutputPixels[userIndex].a != 0)
				{
                	imageOutputPixels[outputIndex] = rawImageMap[srcIndex];
					imageOutputPixels[outputIndex].a = 255;
				}
				else
					imageOutputPixels[outputIndex] = new Color32(0,0,0,0);
            }
        }
        imageTexture.SetPixels32(imageOutputPixels);
        imageTexture.Apply();

    }

	/*
	void Zig_Update(ZigInput input)
	{
		UpdateTexture(ZigInput.Image,ZigInput.LabelMap);
	}



	void OnGUI() {
		GUI.depth = int.MaxValue;
		GUI.DrawTexture(new Rect(0,0,200,200), imageTexture);
	}*/
}
