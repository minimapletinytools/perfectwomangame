using UnityEngine;
using System.Collections.Generic;

public class CharacterHelper 
{
	
	public CharIndexContainerCharacterStats Characters
	{ get; private set; }
	
	public CharacterHelper()
	{
		
		Characters = new CharIndexContainerCharacterStats();
		foreach(CharacterIndex e in CharacterIndex.sAllCharacters)
		{
			Characters[e] = new CharacterStats(){Character = e};
			Characters[e].Difficulty = 1;
			//Characters[e].Perfect = mPerfectness[e.Choice]; //TODO delete lul no perfect anymore..
		}
	}
	
	//TODO delete
	//this really does not belong here
	public static List<List<string>> sCharacterSentencs = new List<List<string>>()
	{
		new List<string>(),
		
		new List<string>(){"life was magical","but there was no prince charming"},
		new List<string>(){"you rocked it like a star", "a young Elvis", "but life was very stressful"},
		new List<string>(){"your youth was tough and real", "you learned much about life", "you were the boss of your turf"},
		new List<string>(),
		new List<string>(){"you were the hottest girl in school", "you lost your virginity to your true love...", "but he turned out ot be a jerk"},
		new List<string>(){"you rocked it out in the woods", "but you could not get a contract"},
		new List<string>(){"you were hungry and cold", "life was hard for you and your yonuger brother", "but you survived"},
		new List<string>(),
		new List<string>(){"children are the best", "too bad you didn't have any time to yourself"},
		new List<string>(){"you drank your youth away with the beer", "but you were drunk and happy"},
		new List<string>(){"you would die for your ideals"},
		new List<string>(),
		new List<string>(){"you gave a very popular TED talk", "you gained much respect", "but life was stressful"},
		new List<string>(){"you were very much in love with your wife", "you fostered a baby boy"},
		new List<string>(){"you lost your boy to leukemia", "you struggle to continue"},
		new List<string>(),
		new List<string>(){"your husband was very rich","you bought out the entire ikea catalog","but were you happy?"},
		new List<string>(){"you are very content with your job", "but you feel you could do more with your life"},
		new List<string>(){"you were very popular on twitter", "your cat was an internet meme", "but you did not go out much"},
		new List<string>(),
		new List<string>(){"you won a nobel peace prize", "AND you earned 10 million frequest flyer points", "you grew apart from your famil"},
		new List<string>(){"you raised a lot of money for charity"},
		new List<string>(){"you're daughter had a sex change", "how could she!", "your blood pressure went up"},
		new List<string>(),
		new List<string>(){"you were very happy"},
		new List<string>(){"you were very happy"},
		new List<string>(){"you saw the light of god"},
		new List<string>(),
		new List<string>(),
		new List<string>()
	};
}
