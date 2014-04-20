using UnityEngine;
using System.Collections;

public class NewMenuReferenceBehaviour : MonoBehaviour {
	
	//generic
	public Texture2D genericFontTex;
	public int genericFontTexWidth;
	public Font genericFont;
	public Font skinnyFont;
	public Font fatFont;
	public Font serifFont;
	
	//configurator
	public Texture2D perfectWomanLogo;
	public Texture2D gameLabLogo;
	public Texture2D filmAkademieLogo;
	public Texture2D depthBorder;
	public Texture2D redWarning;

	//ui
	public Texture2D uiPerfectStar;
	public GameObject uiParticlePrefab;
	
	//partices
	public Texture2D partGold;
	public Texture2D partSilver;
	public Texture2D partSilver2;
	public Texture2D partGlow;
	public Texture2D partRed;
	
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
	public AudioClip cutGood;
	public AudioClip cutBad;
	public AudioClip cutDie;
	public AudioClip headPopupGood;
	public AudioClip headPopupBad;
	public AudioClip sunRises;
	public AudioClip graveShine;
	public AudioClip featureUnlocked;
	public AudioClip counting;
	public AudioClip pose0;
	public AudioClip pose1;
	public AudioClip pose2;
	public AudioClip pose3;
	public AudioClip pose4;
	public AudioClip pose5;

	//music
	public AudioClip startMusic;
	public AudioClip creditsMusic;

	//farseer
	public Material farseerMaterial;
}
