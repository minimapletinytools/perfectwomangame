using UnityEngine;
using System.Collections;

public class MiniCharacterHelper 
{
	public MiniCharacterHelper()
	{
	}
	
	
	FlatBodyObject mBody;
	public void load_character(AssetBundle aBundle, string aName)
	{
		CharacterLoader loader = new CharacterLoader();
		loader.complete_load_character(aBundle,aName);
		//foreach(FlatBodyObject e in mBody.load_sequential(loader.Images,loader.Sizes));
		
		//TODO position mBody;
	}
	
	public void position_character(FlatCameraManager aCam, Vector3 aPosition)
	{
		mBody.SoftPosition = aCam.get_point(Vector3.zero) + aPosition;
	}
}
