using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CharacterLoader {
    public bool Done { get; private set; }
    public CharacterData.CharacterDataImages Images { get; private set; } //this contains background images, cutscene images, and anything else
    public CharacterData.CharacterDataSizes Sizes { get; private set; } //and their sizes
	public CharacterIndex Character {get{return new CharacterIndex(Name);}}
    public string Name { get; private set; }
    public CharacterLoader()
    {
        Done = false;
        Images = new CharacterData.CharacterDataImages();
        Sizes = new CharacterData.CharacterDataSizes();
        Name = "unset";
    }
	
	public bool has_cutscene(int aIndex)
	{
		return Sizes.find_static_element("CUTSCENE"+aIndex+"_0") != null;
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
		
        if (aName != "999") //TODO DELETE do not need a check here since everything below will load null anyhow.
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
        Images.backgroundMusic = aBundle.Load("AUDIO", typeof(AudioClip)) as AudioClip; //optional
		
		for(int i = 0; i < 10; i++)
		{
			AudioClip clip = aBundle.Load("CSAUDIO_" + i, typeof(AudioClip)) as AudioClip;
			if(clip != null)
				Images.cutsceneMusic.Add(clip);
			else 
			{
				//Debug.Log("loaded " + i + " cutscene songs");
				break;
			}
		}
		
		Images.deathMusic = aBundle.Load("CSAUDIO_4", typeof(AudioClip)) as AudioClip;
		//if(Images.deathMusic == null)
		//	Debug.Log("no death music for " + Character.StringIdentifier);

        TextAsset cd = aBundle.Load("CD", typeof(TextAsset)) as TextAsset;
        System.IO.MemoryStream stream = new System.IO.MemoryStream(cd.bytes);
        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(CharacterData.CharacterDataSizes));
        Sizes = xs.Deserialize(stream) as CharacterData.CharacterDataSizes;
        output += "offset " + Sizes.mOffset;
		
		//new static
		foreach(CharacterData.ImageSizeOffsetAnimationData e in Sizes.mStaticElements)
		{
			Images.staticElements.Add(e.Name,aBundle.Load(e.Name) as Texture2D);
		}
		
		
		
        //Debug.Log(output);
        Done = true;
        yield break;
    }
}
