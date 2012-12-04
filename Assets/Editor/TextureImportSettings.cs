using UnityEngine;
using UnityEditor;

// /////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Batch Texture import settings modifier.
//
// Modifies all selected textures in the project window and applies the requested modification on the 
// textures. Idea was to have the same choices for multiple files as you would have if you open the 
// import settings of a single texture. Put this into Assets/Editor and once compiled by Unity you find
// the new functionality in Custom -> Texture. Enjoy! :-)
// 
// Based on the great work of benblo in this thread: 
// http://forum.unity3d.com/viewtopic.php?t=16079&start=0&postdays=0&postorder=asc&highlight=textureimporter
// 
// Developed by Martin Schultz, Decane in August 2009
// e-mail: ms@decane.net
//
// Updated for Unity 3.0 by col000r in August 2010
// http://col000r.blogspot.com
//
// Improved to change Texture type (Image, Bump, GUI, Reflection, Cookie, Lightmap, Advanced, GUI_FULL) in January 2011
//   GUI_FULL change: wrapMode = clamp, Scale=None
//   Save a lot of time working with GUI/HUD textures (Converting all textures to GUI in one time)     
//   Justo Salcedo
//
// /////////////////////////////////////////////////////////////////////////////////////////////////////////
public class ChangeTextureImportSettingsUnity3 : ScriptableObject
{

    [MenuItem("Custom/Texture/Change Texture Type/LEA")]
    static void ChangeTextureType_GuiFull()
    {
        Object[] textures = GetSelectedTextures();
        Selection.objects = new Object[0];
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            //Debug.Log("path: " + path);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureType = TextureImporterType.Advanced;
            textureImporter.npotScale = TextureImporterNPOTScale.None;
            textureImporter.isReadable = true;
            textureImporter.mipmapEnabled = false;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.textureFormat = TextureImporterFormat.RGBA32;
            textureImporter.normalmap = false;
            textureImporter.maxTextureSize = 4096;
            TextureImporterSettings st = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(st);
            st.wrapMode = TextureWrapMode.Clamp;
            textureImporter.SetTextureSettings(st);
            AssetDatabase.ImportAsset(path);
        }
    }



    static void SelectedChangeIsReadable(bool enabled)
    {

        Object[] textures = GetSelectedTextures();
        Selection.objects = new Object[0];
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.isReadable = enabled;
            AssetDatabase.ImportAsset(path);
        }
    }


    static void SelectedChangeNonPowerOf2(TextureImporterNPOTScale npot)
    {

        Object[] textures = GetSelectedTextures();
        Selection.objects = new Object[0];
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.npotScale = npot;
            AssetDatabase.ImportAsset(path);
        }
    }

    static void SelectedChangeMimMap(bool enabled)
    {

        Object[] textures = GetSelectedTextures();
        Selection.objects = new Object[0];
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.mipmapEnabled = enabled;
            AssetDatabase.ImportAsset(path);
        }
    }

    static void SelectedChangeMaxTextureSize(int size)
    {

        Object[] textures = GetSelectedTextures();
        Selection.objects = new Object[0];
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.maxTextureSize = size;
            AssetDatabase.ImportAsset(path);
        }
    }

    static void SelectedChangeTextureFormatSettings(TextureImporterFormat newFormat)
    {

        Object[] textures = GetSelectedTextures();
        Selection.objects = new Object[0];
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            //Debug.Log("path: " + path);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureFormat = newFormat;
            AssetDatabase.ImportAsset(path);
        }
    }

    static void SelectedChangeTextureTypeSettings(TextureImporterType newType)
    {

        Object[] textures = GetSelectedTextures();
        Selection.objects = new Object[0];
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            //Debug.Log("path: " + path);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureType = newType;
            AssetDatabase.ImportAsset(path);
        }
    }

    static Object[] GetSelectedTextures()
    {
        return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
    }
}