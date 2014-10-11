using UnityEngine;
using System.Collections;

public class ZgDepthViewer
{
    public ZgResolution TextureSize = ZgResolution.QQVGA_160x120;
    Color32 BackgroundColor = Color.white;
    Color32 BaseColor = Color.gray;
    public bool UseHistogram = true;
    public Texture2D DepthTexture {get; private set;}
    ZgResolutionData textureSize;
    
    float[] depthHistogramMap;
    Color32[] depthToColor;
    Color32[] outputPixels;
    public int MaxDepth = 10000; //DO NOT MODIFY IN RUNTIME!!
    // Use this for initialization
	public ZgDepthViewer () {

        textureSize = ZgResolutionData.FromZgResolution(TextureSize);
        DepthTexture = new Texture2D(textureSize.Width, textureSize.Height);
        DepthTexture.wrapMode = TextureWrapMode.Clamp;
        depthHistogramMap = new float[MaxDepth];
        depthToColor = new Color32[MaxDepth];
        outputPixels = new Color32[textureSize.Width * textureSize.Height];
    }
    
    void UpdateHistogram(ZgDepth depth)
    {
        int i, numOfPoints = 0;
        
        System.Array.Clear(depthHistogramMap, 0, depthHistogramMap.Length);
        short[] rawDepthMap = depth.data;
        
        int depthIndex = 0;
        // assume only downscaling
        // calculate the amount of source pixels to move per column and row in
        // output pixels
        int factorX = depth.xres/textureSize.Width;
        int factorY = ((depth.yres / textureSize.Height) - 1) * depth.xres;
        for (int y = 0; y < textureSize.Height; ++y, depthIndex += factorY) {
            for (int x = 0; x < textureSize.Width; ++x, depthIndex += factorX) {
                short pixel = rawDepthMap[depthIndex];
                if (pixel != 0) {
                    depthHistogramMap[pixel]++;
                    numOfPoints++;
                }
            }
        }
        depthHistogramMap[0] = 0;
        if (numOfPoints > 0) {
            for (i = 1; i < depthHistogramMap.Length; i++) {
                depthHistogramMap[i] += depthHistogramMap[i - 1];
            }
            depthToColor[0] = BackgroundColor;
            for (i = 1; i < depthHistogramMap.Length; i++) {
                float intensity = (1.0f - (depthHistogramMap[i] / numOfPoints));
                //depthHistogramMap[i] = intensity * 255;
                depthToColor[i].r = (byte)(BackgroundColor.r * (1 - intensity) + (BaseColor.r * intensity));
                depthToColor[i].g = (byte)(BackgroundColor.g * (1 - intensity) + (BaseColor.g * intensity));
                depthToColor[i].b = (byte)(BackgroundColor.b * (1 - intensity) + (BaseColor.b * intensity));
                depthToColor[i].a = 255;//(byte)(BaseColor.a * intensity);
            }
        }
        
        
    }
    
    void UpdateTexture(ZgDepth depth)
    {
        short[] rawDepthMap = depth.data;
        int depthIndex = 0;
        int factorX = depth.xres / textureSize.Width;
        int factorY = ((depth.yres / textureSize.Height) - 1) * depth.xres;
        // invert Y axis while doing the update
        for (int y = textureSize.Height - 1; y >= 0 ; --y, depthIndex += factorY) {
            int outputIndex = y * textureSize.Width;
            for (int x = 0; x < textureSize.Width; ++x, depthIndex += factorX, ++outputIndex) {
                outputPixels[outputIndex] = depthToColor[rawDepthMap[depthIndex]];
            }
        }
        DepthTexture.SetPixels32(outputPixels);
        DepthTexture.Apply();
    }
    
    public void Zig_Update(ZgInput input)
    {
        if (UseHistogram) {
            UpdateHistogram(ZgInput.Depth);
        }
        else {
            //TODO: don't repeat this every frame
            depthToColor[0] = BackgroundColor;
            for (int i = 1; i < MaxDepth; i++) {
                float intensity = 1.0f - (i/(float)MaxDepth);
                //depthHistogramMap[i] = intensity * 255;
                depthToColor[i].r = (byte)(BackgroundColor.r * (1 - intensity) + (BaseColor.r * intensity));
                depthToColor[i].g = (byte)(BackgroundColor.g * (1 - intensity) + (BaseColor.g * intensity));
                depthToColor[i].b = (byte)(BackgroundColor.b * (1 - intensity) + (BaseColor.b * intensity));
                depthToColor[i].a = 255;//(byte)(BaseColor.a * intensity);
            }
        }
        UpdateTexture(ZgInput.Depth);
    }
    
    
    Rect targetRect = new Rect(20, Screen.height - 120 - 20, 160, 120);
    Rect currentRect = new Rect(20, Screen.height - 120 - 20, 160, 120);
    
    public void show_indicator(bool show)
    {
        Vector2 give = FlatCameraManager.get_fit_difference();
        targetRect = new Rect(-400, Screen.height - 120 - 20 - give.y/2, 160, 120);
        currentRect.y = Screen.height - 120 - 20 - give.y/2;
        if(!show)
        {
            targetRect.x = -400;
        }
        else targetRect.x = 20 + give.x/2;
        
        
    }
    public void Update()
    {
        float lambda = 0.1f;
        currentRect.x = (1 - lambda) * currentRect.x + lambda * targetRect.x;
        currentRect.y = (1 - lambda) * currentRect.y + lambda * targetRect.y;
        currentRect.width = (1 - lambda) * currentRect.width + lambda * targetRect.width;
        currentRect.height = (1 - lambda) * currentRect.height + lambda * targetRect.height;
    }
    
    public string Message{get;set;}
    
    public void OnGUI() {
        GUI.depth = int.MinValue;
		//GUI.DrawTexture(new Rect(Screen.width - texture.width - 10, Screen.height - texture.height - 10, texture.width, texture.height), texture);

		GUI.DrawTexture(currentRect, DepthTexture);
		GUI.DrawTexture(currentRect.expand(4),ManagerManager.Manager.mNewRef.depthBorder);
            
        
        var style = new GUIStyle();
        style.font = ManagerManager.Manager.mNewRef.genericFont;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 100;
        GUI.Box(new Rect(0,0,Screen.width,Screen.height),Message,style);
    }
}
