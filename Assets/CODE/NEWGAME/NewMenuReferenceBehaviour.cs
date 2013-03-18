using UnityEngine;
using System.Collections;

public class NewMenuReferenceBehaviour : MonoBehaviour {
	
	//generic
	public Texture2D genericFontTex;
	public int genericFontTexWidth;
	public Font genericFont;
	public GameObject genericFontPrefab;
	
	//configurator logos
	public Texture2D perfectWomanLogo;
	public Texture2D gameLabLogo;
	
	//ui
	public Texture2D uiPerfectStar;
	
	//blue bar
	public Texture2D bbBackground;
	public Texture2D bbGraphBackground;
	public Texture2D bbGraphFrame;
	public Texture2D bbScoreBackground;
	
	//choice
	public Texture2D bbChoiceBox;
	public Texture2D bbChoiceFrame;
	
	//pink bar
	public Texture2D pbBackground;
	public Texture2D pbCharacterIconBackground;
	
	
	
	
	//audio
	public AudioClip transitionIn;
	public AudioClip transitionOut;
	public AudioClip choiceBlip; //played when your selection changes
	public AudioClip choiceMade; //played when selection is made
	//TODO mroe
	
}
