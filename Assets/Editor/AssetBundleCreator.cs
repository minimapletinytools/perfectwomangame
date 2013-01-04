
using UnityEngine;
using UnityEditor;

public class AssetBundleCreator
{
    [MenuItem("Custom/LEAS ASSET BUNDLING SCRIPT")]
    static void ExportResource()
    {
        /*
        Object[] objects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            //Selection.objects;
        foreach (Object e in objects)
        {
            Debug.Log(e.name);
            if (System.IO.Directory.Exists(AssetDatabase.GetAssetPath(e)))
            {
                Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
                //BuildPipeline.BuildAssetBundle(e, e, "Assets/Resources/"+Selection.activeObject.name + ".unity3d",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,EditorUserBuildSettings.activeBuildTarget);//,BuildOptions.UncompressedAssetBundle);//, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
                //Selection.objects = selection;
            }
             
        }*/
        // Build the resource file from the active selection.
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, "Assets/Resources/" + Selection.activeObject.name + ".unity3d", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, EditorUserBuildSettings.activeBuildTarget);//,BuildOptions.UncompressedAssetBundle);//, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
        Selection.objects = selection;        
    }
    [MenuItem("Custom/poopootest")]
    static void ConvertTest()
    {
        Object[] objects = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        Debug.Log(AssetDatabase.GetAssetPath(Selection.activeObject));
        foreach (Object e in objects)
            Debug.Log(e.GetType() + " " + AssetDatabase.GetAssetPath(e));


        //TODO find selected directory and parse for needed files (<limb>_A/B.png, BG.png, POSITIONS.png, AUDIO.mp3)
        //TODO change import settings??
        //foreach <limb>_B, parse for information and add to serializable data struct
        //parse POSITIONS.png for information and add to data struct
        //TODO parse pose information
        //TODO change import settings on images and store their original file sizes into data struct
        //save <limb>_A.png, BG.png, AUDIO.mp3, BG/FG_<num>.png, and serialized text assets, to AssetBundle
    }


    public static bool is_asset_in_directory(Object aObj, string aDir)
    {
        return aDir.StartsWith(aDir);
    }
    public static bool is_asset(Object aObj)
    {
        return AssetDatabase.Contains(aObj);
    }
    public static bool is_folder(Object aObj)
    {
        if (!is_asset(aObj))
            return false;
        return System.IO.Directory.Exists(AssetDatabase.GetAssetPath(aObj));
    }

}