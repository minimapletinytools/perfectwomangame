using UnityEngine;
using System.Collections.Generic;

public class CharacterLoader {
    public bool Done { get; private set; }
    public CharacterData.CharacterDataImages Images { get; private set; }
    public CharacterData.CharacterDataSizes Sizes { get; private set; }
    public string Name { get; private set; }
    public CharacterLoader()
    {
        Done = false;
        Images = new CharacterData.CharacterDataImages();
        Sizes = new CharacterData.CharacterDataSizes();
        Name = "unset";
    }
    public void complete_load_character(AssetBundle aBundle, string aName)
    {
        while (load_character(aBundle,aName).GetEnumerator().MoveNext())
            ;
    }
    public IEnumerable<int> load_character(AssetBundle aBundle, string aName)
    {
        Name = aName;
        string output = "Loading character " + aName + "\n";
        //TODO this wont work....
        if (aName != "999")
        {
            Images.head = aBundle.Load("HEAD_A", typeof(Texture2D)) as Texture2D;
            Images.leftLowerArm = aBundle.Load("LLA_A", typeof(Texture2D)) as Texture2D;
            Images.leftLowerLeg = aBundle.Load("LLL_A", typeof(Texture2D)) as Texture2D;
            Images.leftUpperArm = aBundle.Load("LUA_A", typeof(Texture2D)) as Texture2D;
            Images.leftUpperLeg = aBundle.Load("LUL_A", typeof(Texture2D)) as Texture2D;
            Images.rightLowerArm = aBundle.Load("RLA_A", typeof(Texture2D)) as Texture2D;
            Images.rightLowerLeg = aBundle.Load("RLL_A", typeof(Texture2D)) as Texture2D;
            Images.rightUpperArm = aBundle.Load("RUA_A", typeof(Texture2D)) as Texture2D;
            Images.rightUpperLeg = aBundle.Load("RUL_A", typeof(Texture2D)) as Texture2D;
            Images.torso = aBundle.Load("TORSO_A", typeof(Texture2D)) as Texture2D;
            Images.waist = aBundle.Load("WAIST_A", typeof(Texture2D)) as Texture2D;
        }

        Images.background1 = aBundle.Load("BACKGROUND", typeof(Texture2D)) as Texture2D;
        for (int i = 0; aBundle.Contains("BG_" + (i+1)); i++)
            Images.backgroundElements.Add(aBundle.Load("BG_" + (i + 1), typeof(Texture2D)) as Texture2D);
        for (int i = 0; aBundle.Contains("FG_" + (i + 1)); i++)
            Images.foregroundElements.Add(aBundle.Load("FG_" + (i + 1), typeof(Texture2D)) as Texture2D);
        output += "found bg fg: " + Images.backgroundElements.Count + " " + Images.foregroundElements.Count + "\n";

        Images.backgroundMusic = aBundle.Load("AUDIO", typeof(AudioClip)) as AudioClip; //optional

        TextAsset cd = aBundle.Load("CD", typeof(TextAsset)) as TextAsset;
        System.IO.MemoryStream stream = new System.IO.MemoryStream(cd.bytes);
        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(CharacterData.CharacterDataSizes));
        Sizes = xs.Deserialize(stream) as CharacterData.CharacterDataSizes;
        output += "offset " + Sizes.mOffset;
        Debug.Log(output);
        Done = true;
        yield break;
    }
}
