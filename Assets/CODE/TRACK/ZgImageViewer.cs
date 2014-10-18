using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class ZgImageViewer
{
    
    public ZgResolution imageResolution = ZgResolution.VGA_640x480;
    Texture2D imageTexture = null;
    ZgResolutionData imageSizeData;
    Color32[] imageOutputPixels;
    
    public ZgResolution userResolution = ZgResolution.QQVGA_160x120;
    //Texture2D userTexture;
    ZgResolutionData userTextureSize;
    Color32 defaultColor = new Color(255,255,255,255);
    Color32 bgColor = new Color32(0,0,0,0);
    Color32[] labelToColor = new Color32[1] {new Color(255,255,255,255)};
    Color32[] userOutputPixels;
    
	public ZgImageViewer()
    {
        //TODO nedes to handle Kinect2.0 resolutions

        imageSizeData = ZgResolutionData.FromZgResolution(imageResolution);
        imageTexture = new Texture2D(imageSizeData.Width, imageSizeData.Height);
        imageTexture.wrapMode = TextureWrapMode.Clamp;
        imageOutputPixels = new Color32[imageSizeData.Width * imageSizeData.Height];
        
        userTextureSize = ZgResolutionData.FromZgResolution(userResolution);
        //userTexture = new Texture2D(userTextureSize.Width, userTextureSize.Height);
        //userTexture.wrapMode = TextureWrapMode.Clamp;             
        userOutputPixels = new Color32[userTextureSize.Width * userTextureSize.Height];
    }

    public Texture2D UpdateTexture(Texture2D image, Texture2D labelmap)
    {
        imageTexture = image;
        var imageData = image.GetPixels();
        var labelData = labelmap.GetPixels();
        int labelMapIndex = 0;
        int labelMapFactorX = labelmap.width/ image.width;
        int labelMapFactorY = ((labelmap.height / image.height) - 1) * labelmap.width;
        // invert Y axis while doing the update
        for (int y = image.height - 1; y >= 0; --y, labelMapIndex += labelMapFactorY)
        {
            int outputIndex = y * image.width;
            for (int x = 0; x < image.width; ++x, labelMapIndex += labelMapFactorX, ++outputIndex)
            {
                Color label = labelData [labelMapIndex];
                //TODO do proper conversion here...
                if(label.grayscale == 0)
                    imageData [outputIndex] = bgColor;
            }
        }
        imageTexture.SetPixels(imageData);
        imageTexture.Apply();
        return imageTexture;
    }

    //call this manually to update imageTexture
    public Texture2D UpdateTexture(ZgImage image, ZgLabelMap labelmap)
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
                short label = rawLabelMap [labelMapIndex];
                userOutputPixels [outputIndex] = (label > 0) ? ((label <= labelToColor.Length) ? labelToColor [label - 1] : defaultColor) : bgColor;                
            }
        }
        //userTexture.SetPixels32(userOutputPixels);
        //userTexture.Apply();
    

        //use userTexture to mask out imageTexture
        Color32[] rawImageMap = image.data;
        int srcIndex = 0;
        int factorX = image.xres / imageSizeData.Width;
        int factorY = ((image.yres / imageSizeData.Height) - 1) * image.xres;
        // invert Y axis while doing the update
        for (int y = imageSizeData.Height - 1; y >= 0; --y, srcIndex += factorY)
        {
            int outputIndex = y * imageSizeData.Width;
            for (int x = 0; x < imageSizeData.Width; ++x, srcIndex += factorX, ++outputIndex)
            {
                int userY = (int)((y / (float)imageSizeData.Height) * userTextureSize.Height);
                int userX = (int)((x / (float)imageSizeData.Width) * userTextureSize.Width);
                int userIndex = userY * userTextureSize.Width + userX;
                if (userOutputPixels [userIndex].a != 0)
                {
                    imageOutputPixels [outputIndex] = rawImageMap [srcIndex];
                    imageOutputPixels [outputIndex].a = 255;
                } else
                    imageOutputPixels [outputIndex] = new Color32(0, 0, 0, 0);
            }
        }
        imageTexture.SetPixels32(imageOutputPixels);
        imageTexture.Apply();

        return imageTexture;
    }

    public void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 300, 300), ManagerManager.Manager.mZigManager.ZgInterface.take_color_image());
    }
}
