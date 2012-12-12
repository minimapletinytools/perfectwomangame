
using UnityEngine;
using UnityEditor;

public class AssetBundleCreator
{
    [MenuItem("Custom/LEAS ASSET BUNDLING SCRIPT")]
    static void ExportResource()
    {
        Debug.Log("Bundling " + Selection.activeObject.name + " poop");
        // Build the resource file from the active selection.
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        
        BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, Selection.activeObject.name + ".AssetBundle",BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,EditorUserBuildSettings.activeBuildTarget);//,BuildOptions.UncompressedAssetBundle);//, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
        Selection.objects = selection;        
    }
}