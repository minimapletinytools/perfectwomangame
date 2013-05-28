using UnityEngine;
using System.Collections;
using System.Linq;

public static class GameConstants
{
	
	public static Color[] IconDifficultyColors { get { return new Color[]{new Color(0,0.8f,0,1), new Color(0.8f,0.8f,0,1), new Color(0.9f,0.4f,0,1), new Color(0.8f,0,0,1)};}}
	public static Color[] IconDifficultyColorsOverTwo { get{ return IconDifficultyColors.Select(e=> e/2f).ToArray();}}
	
	
	public static Color PopupTextColor { get {return new Color(0,0,0); } } //TODO
	public static Color ChoosingTextColor { get {return new Color(0,0,0); } } //TODO
	
	
	
	//grave stuff
	//public static string[] credits = {"Perfect Woman","created by", "Lea Sch\u00F6enfelder", "Peter Lu"};
    public static string[] credits = { "Thanks: Benedikts","Haas, David Elliott,","Sabrina Winter, Eddo","Stern, Ingo Von","Staden, Andreas","Hykade" };
	
	
	public static float fadingTime = 2.3f;
	public static float transitionToChoiceDelayTime = 2; //2;
}