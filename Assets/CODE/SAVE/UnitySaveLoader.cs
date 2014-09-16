using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class UnitySaveLoader
{
	
	
	//do we want to use a serialized struct instead???
	public string read_text_saves()
	{
		
		string words = "";
		if(File.Exists("save.txt"))
		{
			using(StreamReader reader = new StreamReader("save.txt"))
			{
				words = reader.ReadToEnd();
			}
		}
		return words;
	}
	
	public void write_text_save(string aText)
	{
		using(StreamWriter writer = new StreamWriter("save.txt"))
		{
			writer.Write(aText);
		}
	}
	
	public void read_images(string[] aFiles)
	{
		//TODO test I somehow doubt this actually works...
		foreach(var e in Directory.GetFiles(Directory.GetCurrentDirectory() + "/Assets/Resources/"))
		{
			if(Path.GetExtension(e) != ".png")
				continue;
			Debug.Log(e);
			//var tex = Resources.Load(e) as Texture2D;
		}
	}
	
	public void write_images(string aFilename, Texture2D aImage)
	{
		//TODO more...
        byte[] bytes = aImage.EncodeToPNG();
		File.WriteAllBytes("Assets/Resources/"+aFilename,bytes);
	}
	
	public void clean_image(string aFilename)
	{
		File.Delete("Assets/Resources/"+aFilename);
	}
		
}
