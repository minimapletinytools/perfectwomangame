using UnityEngine;
using UnityEditor;
using System.Collections;

public static class CharacterPreprocessor {

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
