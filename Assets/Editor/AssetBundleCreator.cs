
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
  
}