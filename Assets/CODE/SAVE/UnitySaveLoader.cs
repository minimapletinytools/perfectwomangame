using UnityEngine;
using System.Collections;


public class UnitySaveLoader
{
	
	
	//do we want to use a serialized struct instead???
	public void read_text_saves()
	{
		
	}
	
	public void write_text_save(string aText)
	{
		
	}
	
	public void read_images(string[] aFiles)
	{
		//TODO
	}
	
	public void write_images(string aFilename, Texture2D aImage)
	{
		//TODO more...
        byte[] bytes = aImage.EncodeToPNG();
		System.IO.File.WriteAllBytes(aFilename,bytes);
	}
}
