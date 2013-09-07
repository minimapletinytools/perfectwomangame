using UnityEngine;
using System.Collections;

public class NewMenuReferenceBehaviour : MonoBehaviour {
	
	//generic
	public Texture2D genericFontTex;
	public int genericFontTexWidth;
	public Font genericFont;
	
	//configurator
	public Texture2D perfectWomanLogo;
	public Texture2D gameLabLogo;
	public Texture2D filmAkademieLogo;
	public Texture2D depthBorder;
	
	
	//ui
	public Texture2D uiPerfectStar;
	public GameObject uiParticlePrefab;
	
	//blue bar
	public Texture2D bbBackground;
	public Texture2D bbGraphBackground;
	public Texture2D bbGraphFrame;
	public Texture2D bbGraphGraveFrame;
	public Texture2D bbGraphDot;
	public Texture2D bbScoreBackground;
	public Texture2D[] bbScoreMultiplier; //TODO rename with bbDifficultyImage
	
	//choice
	public Texture2D bbChoiceBox;
	public Texture2D bbChoiceFrame;
	public Texture2D[] bbChoicePerfectIcons; //TODO DELETE remove this, no longer needed
	
	//pink bar
	public Texture2D pbBackground;
	public Texture2D pbCharacterIconBackground;
	
	//grave
	public Texture2D gravePerfectnessEngraving;
	public Texture2D filmAkademieLogoGrave;
	
	//text
	public Texture2D[] textSmallBubble;
	
	//audio
	public AudioClip transitionIn;
	public AudioClip transitionOut;
	public AudioClip transitionCutscene; //played when transitioning from PLAY to CUTSCENE
	public AudioClip choiceBlip; //played when your selection changes
	public AudioClip choiceMade; //played when selection is made
	public AudioClip choiceMusic;
	public AudioClip graveAngel;
	//TODO more
	
}
