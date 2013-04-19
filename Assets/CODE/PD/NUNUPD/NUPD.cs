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

        public bool is_positive()
        {
            return Changes.Where(e => e < 0).Count() == 0;
        }
	}
	
	public class ChangeSet
	{
		public float UpperThreshold {get; set;}
		public float LowerThreshold {get; set;}
		public string PerformanceDescription {get; set;}
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
		public string LongName {get; set;} //TODO delete this
		public string Description {get; set;}
		public CharacterIndex Index {get; set;}
		public List<ChangeSet> ChangeSet {get; set;}
		
		public CharacterInformation()
		{
			ShortName = "";
			Description = "";
			Index = new CharacterIndex(-1);
			ChangeSet = new List<ChangeSet>();
		}
		
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
	}
	
	
	public class CharacterInformationProcessor
	{
		public static CharacterInformation process_character(string aChar)
		{
			string[] keywords = new string[]{"NAME", "NDESC", "INDEX", "CHANGE", "CDESC"};
			CharacterInformation ci = new CharacterInformation();
			string[] process = aChar.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
			string lastState = "";
			
			ChangeSet operatingChangeSet = null;
			ChangeSubSet operatingChangeSubSet = null;
			int changeSubsetIndexCounter = 0;
			foreach(string e in process)
			{
				string[] sp = System.Text.RegularExpressions.Regex.Split(e, @"\s*,\s*|\s\s*").Where(f=>f!="" && f != " ").ToArray();
				
				if(sp.Length == 0)
					continue;
				string first = sp[0];
				
				//Debug.Log (sp.Aggregate((s1,s2)=>s1+"|"+s2+"|"));
				
				if(!keywords.Contains(first))
				{
					if(lastState == "CHANGE") {
						operatingChangeSet.LowerThreshold = (float)System.Convert.ToDouble(sp[0]);
						operatingChangeSet.UpperThreshold = (float)System.Convert.ToDouble(sp[1]);
						
					} else if(lastState == "CDESC") {
						//Debug.Log (sp.Aggregate((s1,s2)=>s1+"|"+s2+"|"));
						foreach(string f in sp){
							operatingChangeSubSet.Changes[changeSubsetIndexCounter] = (System.Convert.ToInt32(f));
							changeSubsetIndexCounter++;
						}
					}
				}
				
				if(first == "NAME"){
					if(sp.Length > 1)
						ci.ShortName = sp.Skip(1).Aggregate((s1,s2)=>s1+" "+s2);
				} else if(first == "NDESC"){
					if(sp.Length > 1)
						ci.Description = sp.Skip(1).Aggregate((s1,s2)=>s1+" "+s2);
				} else if(first == "INDEX"){
					try{ //TODO delete trycatch
						ci.Index = new CharacterIndex(System.Convert.ToInt32(sp[1] ));
					}catch{
						ci.Index = new CharacterIndex(-1);
					}
				} else if(first == "CHANGE"){
					operatingChangeSet = new ChangeSet();
					operatingChangeSet.Changes = new List<ChangeSubSet>();
					if(sp.Length > 1)
						operatingChangeSet.PerformanceDescription = sp.Skip(1).Aggregate((s1,s2)=>s1+" "+s2);
					ci.ChangeSet.Add(operatingChangeSet);
				} else if(first == "CDESC")
				{
					changeSubsetIndexCounter = 0;
					operatingChangeSubSet = new ChangeSubSet();
					if(sp.Length > 1)
						operatingChangeSubSet.Description = sp.Skip(1).Aggregate((s1,s2)=>s1+" "+s2);
					operatingChangeSet.Changes.Add(operatingChangeSubSet);
				}
				
				if(keywords.Contains(first))
					lastState = first;
			}
			return ci;
		}
	}
	
	
}
