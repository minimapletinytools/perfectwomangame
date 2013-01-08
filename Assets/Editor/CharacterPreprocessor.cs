using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public static class CharacterPreprocessor {

    [System.Serializable]
    public class CharacterData
    {
        //ordered as in CharacterPreprocessor.sLimbs
        public List<List<Vector3>> mMountingPositions = new List<List<Vector3>>();
        public List<Vector3> mBackgroundPositions = new List<Vector3>();
        public List<Vector3> mForegroundPositions = new List<Vector3>();

        public List<Vector2> mLimbSizes = new List<Vector2>();
        public List<Vector2> mBackgroundSizes = new List<Vector2>();
        public List<Vector2> mForegroundSizes = new List<Vector2>();

        public Vector2 mBackSize = new Vector2();

        public string mName = "";
    }

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
            CharacterData cd = new CharacterData();
            cd.mName = e.Key.name;
            //first process <limb>_B images
            IEnumerable<Object> bImages = e.Value.Where(f => (sLimbs.Contains(strip_to_root(f)) && is_B_image(f))).OrderBy(f=>strip_to_root(f));
            foreach (Object f in bImages)
            {
                List<Vector3> positions = new List<Vector3>();
                Texture2D limbProcessing = f as Texture2D;
                set_texture_for_reading(limbProcessing);
                cd.mMountingPositions.Add(get_limb_positions_in_order(limbProcessing));
            }

            //parse POSITIONS.png
            Object positionImage = e.Value.First((Object f) => strip_to_root(f) == "POSITIONS");
            Texture2D bgProcessing = positionImage as Texture2D;
            set_texture_for_reading(bgProcessing);
            cd.mBackgroundPositions = get_background_positions_in_order(bgProcessing);
            cd.mForegroundPositions = get_foreground_positions_in_order(bgProcessing);

            //TODO parse POSE info

            //get images that are saved
            IEnumerable<Object> aImages = e.Value.Where(f => (sLimbs.Contains(strip_to_root(f)) && !is_B_image(f))).OrderBy(f=>f.name);
            IEnumerable<Object> bgImages = e.Value.Where(f => strip_to_root(f).StartsWith("BG")).OrderBy(f => f.name);
            IEnumerable<Object> fgImages = e.Value.Where(f => strip_to_root(f).StartsWith("FG")).OrderBy(f => f.name);
            IEnumerable<Object> bgImage = e.Value.Where(f => strip_to_root(f) == "BACKGROUND");

            //record image sizes
            foreach (Object f in aImages)
            {
                set_texture_for_reading((Texture2D)f);
                cd.mLimbSizes.Add(new Vector2(((Texture2D)f).width,((Texture2D)f).height));
            }
            foreach (Object f in bgImages)
            {
                set_texture_for_reading((Texture2D)f);
                cd.mBackgroundSizes.Add(new Vector2(((Texture2D)f).width, ((Texture2D)f).height));
            }
            foreach (Object f in fgImages)
            {
                set_texture_for_reading((Texture2D)f);
                cd.mForegroundSizes.Add(new Vector2(((Texture2D)f).width, ((Texture2D)f).height));
            }
            foreach (Object f in bgImage)
            {
                set_texture_for_reading((Texture2D)f);
                cd.mBackSize = (new Vector2(((Texture2D)f).width, ((Texture2D)f).height));
            }

            //update import settings on <limb>_A images, BG/FG/background images, 
            foreach (Object f in aImages.Concat(bgImages).Concat(fgImages).Concat(bgImage))
            {
                set_texture_for_render((Texture2D)f);
            }



            

            
            //package
            TextAsset cdtxt = new TextAsset();
            AssetDatabase.CreateAsset(cdtxt, "CD.txt");
            System.IO.Stream stream = System.IO.File.Open(AssetDatabase.GetAssetPath(cdtxt), System.IO.FileMode.Append);
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(CharacterData));
            xs.Serialize(stream, cd);
            stream.Close();
            IEnumerable<Object> package = e.Value.Where(f => !is_B_image(f)).Where(f => strip_to_root(f) != "POSITIONS").Where(f => strip_to_root(f) != "AUDIO");
            List<Object> assets = new List<Object>();
            foreach (Object f in package) 
                assets.Add(f);
            assets.Add(cdtxt);
            BuildPipeline.BuildAssetBundle(e.Key, assets.ToArray(), "Assets/Resources/" + e.Key.name + ".unity3d", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, EditorUserBuildSettings.activeBuildTarget);//,BuildOptions.UncompressedAssetBundle);//, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
        }




       

        //TODO find selected directory and parse for needed files (<limb>_A/B.png, BG.png, POSITIONS.png, AUDIO.mp3)
        //TODO change import settings??
        //foreach <limb>_B, parse for information and add to serializable data struct
        //parse POSITIONS.png for information and add to data struct
        //TODO parse pose information
        //TODO change import settings on images and store their original file sizes into data struct
        //change image compression as appropirate
        //save <limb>_A.png, BG.png, AUDIO.mp3, BG/FG_<num>.png, and serialized text assets, to AssetBundle


    }


    public static List<Vector3> get_background_positions_in_order(Texture2D aTex)
    {
        List<Vector3> r = new List<Vector3>();
        for (int i = 0; i < 99; i++)
        {
            try
            {
                r.Add(FlatBodyObject.find_first_color(new Color(255,0,5*i,255), aTex));
            }
            catch
            {
                break;
            }
        }
        return r;
    }
    public static List<Vector3> get_foreground_positions_in_order(Texture2D aTex)
    {
        List<Vector3> r = new List<Vector3>();
        for (int i = 0; i < 99; i++)
        {
            try
            {
                r.Add(FlatBodyObject.find_first_color(new Color(0, 255, 5 * i, 255), aTex));
            }
            catch
            {
                break;
            }
        }
        return r;
    }
    public static List<Vector3> get_limb_positions_in_order(Texture2D aTex)
    {
        List<Vector3> r = new List<Vector3>();
        for (int i = 0; i < 4; i++)
        {
            try
            {
                r.Add(FlatBodyObject.get_attachment_point(i, aTex));
            }
            catch
            {
                break;
            }
        }
        return r;
    }

    public static void set_texture_for_render(Texture2D aTex)
    {
        string path = AssetDatabase.GetAssetPath(aTex);
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        textureImporter.textureType = TextureImporterType.Image;
        textureImporter.mipmapEnabled = false;
        textureImporter.filterMode = FilterMode.Point;
        textureImporter.textureFormat = TextureImporterFormat.RGBA32;
        textureImporter.normalmap = false;
        textureImporter.maxTextureSize = 2048;
        TextureImporterSettings st = new TextureImporterSettings();
        textureImporter.ReadTextureSettings(st);
        st.wrapMode = TextureWrapMode.Clamp;
        textureImporter.SetTextureSettings(st);
        AssetDatabase.ImportAsset(path);
    }

    public static void set_texture_for_reading(Texture2D aTex)
    {
        string path = AssetDatabase.GetAssetPath(aTex);
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

    public static string strip_to_root(Object aObj)
    {
        string r =  System.IO.Path.GetFileNameWithoutExtension(aObj.name);
        if(r.Length >= 2 || r[r.Length - 2] == '_')
            return r.Substring(0,r.Length-2);
        return r;
    }

    public static bool is_B_image(Object aObj)
    {
        string r = System.IO.Path.GetFileNameWithoutExtension(aObj.name);
        if (r[r.Length - 1] == 'B')
            return true;
        return false;
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
