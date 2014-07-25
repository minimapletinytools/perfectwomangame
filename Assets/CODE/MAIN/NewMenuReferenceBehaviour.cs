using UnityEngine;
using System.Collections;

public class NewMenuReferenceBehaviour : MonoBehaviour {
	
	//generic
	//public Texture2D genericFontTex;
	//public int genericFontTexWidth;
	public Font genericFont;
	public Font skinnyFont;
	public Font fatFont;
	public Font serifFont;
	
	//configurator
	//TODO DELETE
	//public Texture2D perfectWomanLogo;
	//public Texture2D gameLabLogo;
	//public Texture2D filmAkademieLogo;
	public Texture2D depthBorder;
	public Texture2D redWarning;


	
	//partices
	public Texture2D partGold;
	public Texture2D partSilver;
	public Texture2D partSilver2;

	//these assets are no longer being used
	//ui
	//public Texture2D uiPerfectStar;
	//public GameObject uiParticlePrefab;
	//blue bar
	//public Texture2D bbGraphBackground;
	//public Texture2D bbGraphFrame;
	//public Texture2D bbGraphDot;
	//pink bar
	//public Texture2D pbCharacterIconBackground;
	
	
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
