using UnityEngine;
using System.Collections.Generic;

public class HackPD 
{
	
	//public static 
	//PDCharacters.characters
	
	//need to take two characters are randomly return a trait that they share in comon or not in common so we can generate this as the decisive sentence
	//better yet, takes one character and a list of characters and randomly decides hom to construct the sentences
	public static List<KeyValuePair<PDStats.Stats,List<CharacterIndex>>> choose_traits(CharacterIndex A,  List<KeyValuePair<CharacterIndex,bool>> B)
	{
		//TODO
		return null;
	}
	
	//needs to take character, performance, and list of N characters to decide how to adjust their difficulties (so the distribution does not get too skewed)
	//this fuction will diretly modify aCharacters and return a list of characters that had their stats (true for increase in difficulty)
	public static List<KeyValuePair<CharacterIndex,bool>> get_difficulty_adjust(PerformanceStats aCharacter, List<CharacterStats> aCharacters)
	{
		//TODO
		return null;
	}
	
	
}
