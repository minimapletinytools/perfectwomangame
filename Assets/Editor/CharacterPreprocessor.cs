using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public static class CharacterPreprocessor {

    static List<string> sLimbs = new List<string>()
    {
        "HEAD",
        "LLA",
        "LLL",
        "LUA",
        "LUL",
        "RLA",
        "RLL",
        "RUA",
        "RUL",
        "TORSO",
        "WAIST"
    };
    static List<string> sExpected = new List<string>()
    {
        "POSITIONS.png",
        "BG.png",
        "AUDIO.mp3"
    };

    static CharacterPreprocessor()
    {
        sExpected.AddRange(sLimbs.ConvertAll<string>(s => s + "_A.png"));
        sExpected.AddRange(sLimbs.ConvertAll<string>(s => s + "_B.png"));
    }

    [MenuItem("Custom/poopootest")]
    static void ConvertTest()
    {
        Object[] objects = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        Debug.Log(AssetDatabase.GetAssetPath(Selection.activeObject));
        foreach (Object e in objects)
            Debug.Log(e.GetType() + " " + AssetDatabase.GetAssetPath(e));


        IEnumerable<Object> folders = Selection.GetFiltered(typeof(Object), SelectionMode.TopLevel).Where(e => is_folder(e));
        Dictionary<Object, IEnumerable<Object>> fileMap = new Dictionary<Object, IEnumerable<Object>>();
        foreach (Object e in folders)
            fileMap[e] = Selection.objects.Where<Object>(delegate(Object f) { return is_asset_in_directory(f,e); });
        fileMap = (Dictionary<Object, IEnumerable<Object>>)fileMap.Where(delegate(KeyValuePair<Object, IEnumerable<Object>> e) { return is_character(e.Value); });

        foreach (KeyValuePair<Object, IEnumerable<Object>> e in fileMap)
        {
            //TODO start doing actual work here
        }




       

        //TODO find selected directory and parse for needed files (<limb>_A/B.png, BG.png, POSITIONS.png, AUDIO.mp3)
        //TODO change import settings??
        //foreach <limb>_B, parse for information and add to serializable data struct
        //parse POSITIONS.png for information and add to data struct
        //TODO parse pose information
        //TODO change import settings on images and store their original file sizes into data struct
        //save <limb>_A.png, BG.png, AUDIO.mp3, BG/FG_<num>.png, and serialized text assets, to AssetBundle


    }

    public static bool is_character(IEnumerable<Object> aObjects)
    {
        
        foreach (string e in sExpected)
        {
            bool pass = false;
            foreach (Object f in aObjects)
            {
                if (System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(f)).Equals(e))
                {
                    pass = true;
                    break;
                }
            }
            if (!pass) return false;
        }
        return true;
    }
    public static bool is_asset_in_directory(Object aObj, Object aDir)
    {
        return is_asset_in_directory(aObj,AssetDatabase.GetAssetPath(aDir));
    }
    public static bool is_asset_in_directory(Object aObj, string aDir)
    {
        return AssetDatabase.GetAssetPath(aObj).StartsWith(aDir);
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
