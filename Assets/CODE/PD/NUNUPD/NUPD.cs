using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//this is what character info should have
//each character has a set of thresholds
//each threshold has a set of changes duh.

namespace NUPD
{
	
	public class ChangeSubSet
	{
		
		public string Description {get; set;} 
		public int[] Changes {get; set;}

        public ChangeSubSet()
        {
            Description = "";
            Changes = new int[CharacterIndex.NUMBER_CHARACTERS];

        }
	}
	
	public class ChangeSet
	{
		public float UpperThreshold {get; set;}
		public float LowerThreshold {get; set;}
		public List<ChangeSubSet> Changes {get; set;}

        public ChangeSet()
        {
            UpperThreshold = 1;
            LowerThreshold = 0;
            Changes = new List<ChangeSubSet>();
        }
	}
	
	public class CharacterInformation
	{
		public string ShortName {get; set;}
		public string LongName {get; set;}
		public string Description {get; set;}
		public CharacterIndex Index {get; set;}
		public List<ChangeSet> ChangeSet {get; set;}
		
		public static CharacterInformation default_character_info(CharacterIndex aIndex)
		{
			CharacterInformation r = new CharacterInformation(){
				ShortName = aIndex.ShortName,
				Description = "",
				Index = aIndex,
				ChangeSet = new List<ChangeSet>()
			};
			return r;
		}
		
		public static CharacterInformation[] sAllCharacters = new CharacterInformation[]
		{
			new CharacterInformation()
			{
				ShortName = "Fetus",
				LongName = "Fetus",
				Description = "In your mother's womb",
				Index = new CharacterIndex(0),
				ChangeSet = new List<ChangeSet>()
				{
					new ChangeSet()
					{
						UpperThreshold = 1,
						LowerThreshold = 0,
						Changes = new List<ChangeSubSet>()
						{
							new ChangeSubSet()
							{
								Description = "Prepare to be born!",
								Changes = new int[]
								{
									0, //fetus
									0,0,0,0, //5
									0,0,0,0, //16
									0,0,0,0, //27
									0,0,0,0, //34
									0,0,0,0, //45
									0,0,0,0, //60
									0,0,0,0, //85
									0, //100
									0, //999
								}
							}
						}
					}
				}
			},
			
			new CharacterInformation()
			{
				ShortName = "Princess",
				LongName = "In your mother's womb",
				Description = "",
				Index = new CharacterIndex(0),
				ChangeSet = new List<ChangeSet>()
				{
					new ChangeSet()
					{
						UpperThreshold = 1f,
						LowerThreshold = 0.5f,
						Changes = new List<ChangeSubSet>()
						{
							new ChangeSubSet()
							{
								Description = "You're childhood memories are filled with magic",
								Changes = new int[]
								{
									0, //fetus
									0,0,0,0, //5
									0,0,0,0, //16
									0,0,0,0, //27
									0,0,0,0, //34
									0,0,0,0, //45
									0,0,0,0, //60
									0,0,0,0, //85
									0, //100
									0, //999
								}
							},
							new ChangeSubSet()
							{
								Description = "But there was no Prince Charming",
								Changes = new int[]
								{
									0, //fetus
									0,0,0,0, //5
									0,0,0,0, //16
									0,0,0,0, //27
									0,0,0,0, //34
									0,0,0,0, //45
									0,0,0,0, //60
									0,0,0,0, //85
									0, //100
									0, //999
								}
							}
						}
					},
					new ChangeSet()
					{
						UpperThreshold = 0.5f,
						LowerThreshold = 0,
						Changes = new List<ChangeSubSet>()
						{
							new ChangeSubSet()
							{
								Description = "You're castle was seiged by ogres!!!",
								Changes = new int[]
								{
									0, //fetus
									0,0,0,0, //5
									0,0,0,0, //16
									0,0,0,0, //27
									0,0,0,0, //34
									0,0,0,0, //45
									0,0,0,0, //60
									0,0,0,0, //85
									0, //100
									0, //999
								}
							},
							new ChangeSubSet()
							{
								Description = "This is no fantasy, go to your room!",
								Changes = new int[]
								{
									0, //fetus
									0,0,0,0, //5
									0,0,0,0, //16
									0,0,0,0, //27
									0,0,0,0, //34
									0,0,0,0, //45
									0,0,0,0, //60
									0,0,0,0, //85
									0, //100
									0, //999
								}
							}
						}
					}
				}
			}
		};
	}
	
	
	public class CharacterInformationProcessor
	{
		public static CharacterInformation process_character(string aChar)
		{
			string[] keywords = new string[]{"NAME", "NDESC", "INDEX", "CHANGE", "CDESC"};
			CharacterInformation ci = new CharacterInformation();
			string[] process = aChar.Split('\n');
			string lastState = "";
			
			ChangeSet operatingChangeSet = null;
			ChangeSubSet operatingChangeSubSet = null;
			int changeSubsetIndexCounter = 0;
			foreach(string e in process)
			{
				string[] sp = e.Split(' ',',');
				if(sp.Length == 0)
					continue;
				string first = sp[0];
				
				if(!keywords.Contains(first))
				{
					if(lastState == "CHANGE") {
						operatingChangeSet.LowerThreshold = (float)System.Convert.ToDouble(sp[0]);
						operatingChangeSet.LowerThreshold = (float)System.Convert.ToDouble(sp[1]);
						
					} else if(lastState == "CDESC") {
						foreach(string f in sp){
							operatingChangeSubSet.Changes[changeSubsetIndexCounter] = (System.Convert.ToInt32(f));
							changeSubsetIndexCounter++;
						}
					}
				}
				
				if(first == "NAME"){
					ci.ShortName = sp[1];
				} else if(first == "NDESC"){
					ci.Description = sp[1];
				} else if(first == "INDEX"){
					ci.Index = new CharacterIndex(System.Convert.ToInt32(sp[1]));
				} else if(first == "CHANGE"){
					operatingChangeSet = new ChangeSet();
					operatingChangeSet.Changes = new List<ChangeSubSet>();
					ci.ChangeSet.Add(operatingChangeSet);
				} else if(first == "CDESC")
				{
					changeSubsetIndexCounter = 0;
					operatingChangeSubSet = new ChangeSubSet();
					operatingChangeSubSet.Description = sp[1];
					operatingChangeSet.Changes.Add(operatingChangeSubSet);
				}
				
				lastState = first;
			}
			return ci;
		}
	}
	
	
}
