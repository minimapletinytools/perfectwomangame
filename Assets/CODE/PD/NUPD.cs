using UnityEngine;
using System;
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
		public CharIndexContainerInt Changes {get; set;}

        public ChangeSubSet()
        {
            Description = "";
            Changes = new CharIndexContainerInt(){ Contents = new int[][]{
				new int[]{0},
				new int[]{0,0,0,0,0,0,0},
				new int[]{0,0,0,0,0,0,0},
				new int[]{0,0,0,0,0,0,0},
				new int[]{0,0,0,0,0,0,0},
				new int[]{0,0,0,0,0,0,0},
				new int[]{0,0,0,0,0,0,0},
				new int[]{0,0,0,0,0,0,0},
				new int[]{0,0,0,0,0,0,0},
				new int[]{0},
				new int[]{0}
			}};
		}

        public bool is_positive()
        {
            return Changes.to_array().Where(e => e < 0).Count() == 0;
        }
	}
	
	public class ChangeSet
	{
		public float UpperThreshold {get; set;}
		public float LowerThreshold {get; set;}
		public string PerformanceDescription {get; set;}
		public string Audio {get; set;} //TODO delete
		public List<ChangeSubSet> Changes {get; set;}
		public int Index {get; set;}

        public ChangeSet()
        {
			Index = -1;
            UpperThreshold = 1;
            LowerThreshold = 0;
			PerformanceDescription = "";
			Audio = "";
            Changes = new List<ChangeSubSet>();
			
        }
		
		public CharIndexContainerInt accumulative_changes()
		{
			CharIndexContainerInt r = new CharIndexContainerInt();
			if(Changes.Count > 0)
				r = Changes[0].Changes;
			for(int i = 1; i < Changes.Count; i++)
			{
				r = r.sum(Changes[i].Changes);
			}
			return r;
		}
	}
	
	public class CharacterInformation
	{
		public string ShortName {get; set;}
		public string LongName {get; set;} //TODO delete this
		public string Description {get; set;}
		public bool IsDescriptionAdjective {get;set;}
		public CharacterIndex Index {get; set;}
		public List<ChangeSet> ChangeSet {get; set;}
		public CharIndexContainerString HardConnections{get; set;}
		public CharIndexContainerString EasyConnections{get; set;}
		public Color CharacterOutlineColor {get; set;}
		public float BPM {get; set;}
		public float BPMOFFSET {get; set;}
		
		public CharacterInformation()
		{
			ShortName = "";
			Description = "";
			IsDescriptionAdjective = false;
			Index = new CharacterIndex(-1,0);
			ChangeSet = new List<ChangeSet>();
			HardConnections = new CharIndexContainerString();
			EasyConnections = new CharIndexContainerString();
			CharacterOutlineColor = GameConstants.TransparentBodyDefaultColor;
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
			string[] keywords = new string[]{"NAME", "NDESC", "INDEX", "CHANGE", "CDESC", "CONNECTION", "AUDIO","COLOR"};
			CharacterInformation ci = new CharacterInformation();
			string[] process = aChar.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
			string lastState = "";
			
			
			List<ChangeSet> operatingChangeSetList = new List<ChangeSet>();
			//ChangeSet operatingChangeSet = null; TODO DELETE
			ChangeSubSet operatingChangeSubSet = null;
			int changeSubsetLevelIndexCounter = 0;
			
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
						//TODO DELETE
						//operatingChangeSet.LowerThreshold = (float)System.Convert.ToDouble(sp[0]);
						//operatingChangeSet.UpperThreshold = (float)System.Convert.ToDouble(sp[1]);
						
						operatingChangeSetList.Last().LowerThreshold = (float)System.Convert.ToDouble(sp[0]);
						operatingChangeSetList.Last().UpperThreshold = (float)System.Convert.ToDouble(sp[1]);
						
					} else if(lastState == "CDESC") {
						//Debug.Log (sp.Aggregate((s1,s2)=>s1+"|"+s2+"|"));
						int changeSubsetChoiceIndexCounter = 0;
						foreach(string f in sp){
							operatingChangeSubSet.Changes[changeSubsetLevelIndexCounter,changeSubsetChoiceIndexCounter] = (System.Convert.ToInt32(f));
							changeSubsetChoiceIndexCounter++;
						}
						changeSubsetLevelIndexCounter++;
					}
				} else if((lastState == "CDESC" && first != "CDESC")) //we are finished with our chain of cdescs, start a new group
				{
					foreach(ChangeSet f in operatingChangeSetList)
						ci.ChangeSet.Add(f);
					operatingChangeSetList.Clear();
				}
				
				if(first == "NAME"){
					if(sp.Length > 1)
						ci.ShortName = sp.Skip(1).Aggregate((s1,s2)=>s1+" "+s2);
					//Debug.Log(ci.ShortName);
				} else if(first == "NDESC"){
					if(sp.Length > 1)
					{
						ci.Description = sp.Skip(1).Aggregate((s1,s2)=>s1+" "+s2);
						if(ci.Description.Contains("<A>"))
							ci.IsDescriptionAdjective = true;
						//we will actually do this processing in CharacterIndex instead
						//ci.Description = ci.Description.Replace("<A> ", ""); 
						//ci.Description = ci.Description.Replace("<A>", ""); 
					}
				} else if(first == "INDEX"){
					//TODO index should be two numbers now
					ci.Index = new CharacterIndex(System.Convert.ToInt32(sp[1]),System.Convert.ToInt32(sp[2]));//CharacterIndex.INDEX_TO_CHARACTER[System.Convert.ToInt32(sp[1])];
				} else if(first == "CHANGE"){
					//TODO DELETE
					//operatingChangeSet = new ChangeSet();
					//operatingChangeSet.Index = ci.ChangeSet.Count;
					operatingChangeSetList.Add(new ChangeSet());
					operatingChangeSetList.Last().Index = ci.ChangeSet.Count + operatingChangeSetList.Count;
					if(sp.Length > 1)
						//operatingChangeSet.PerformanceDescription = sp.Skip(1).Aggregate((s1,s2)=>s1+" "+s2);
						operatingChangeSetList.Last().PerformanceDescription = sp.Skip(1).Aggregate((s1,s2)=>s1+" "+s2).Replace("poor","mediocre").Replace("poorly","ok");
					//ci.ChangeSet.Add(operatingChangeSet);
				} else if(first == "CDESC")
				{
					changeSubsetLevelIndexCounter = 0;
					operatingChangeSubSet = new ChangeSubSet();
					if(sp.Length > 1)
						operatingChangeSubSet.Description = sp.Skip(1).Aggregate((s1,s2)=>s1+" "+s2);
					//NOTE by doing things this way, all change subsets will be references to each other!!
					foreach(ChangeSet f in operatingChangeSetList)
						f.Changes.Add(operatingChangeSubSet);
					//TODO DELETE
					//operatingChangeSet.Changes.Add(operatingChangeSubSet);
				} else if(first == "CONNECTION"){
					CharacterIndex conind = new CharacterIndex(System.Convert.ToInt32(sp[1]),System.Convert.ToInt32(sp[2]));
					bool easy = sp[3] == "+" ? true : false;
					if(easy)
						ci.EasyConnections[conind] = sp.Skip(4).Aggregate((s1,s2)=>s1+" "+s2);
					else
						ci.HardConnections[conind] = sp.Skip(4).Aggregate((s1,s2)=>s1+" "+s2);
				} else if (first == "COLOR"){
					ci.CharacterOutlineColor = 
						new Color32((byte)System.Convert.ToInt32(sp[1]),(byte)System.Convert.ToInt32(sp[2]),(byte)System.Convert.ToInt32(sp[3]),(byte)System.Convert.ToInt32(sp[4]));
				} else if (first == "BPM"){
					ci.BPM = (float)System.Convert.ToDouble(sp[1]);
					if(sp.Length > 2)
						ci.BPMOFFSET = (float)System.Convert.ToDouble(sp[2]);
				}
				
				if(keywords.Contains(first))
					lastState = first;
			}
		
			//in case we have some changes that did not get added
			foreach(ChangeSet f in operatingChangeSetList)
				ci.ChangeSet.Add(f);
			operatingChangeSetList.Clear();

			//TODO DELETE
			//this is a hack to set .4-.6 changes to be positive
			if(ci.ChangeSet.Count > 2)
			{
				Debug.Log(ci.ChangeSet[2].UpperThreshold + " " + ci.ChangeSet[4].UpperThreshold);
				ci.ChangeSet[2].Changes = ci.ChangeSet[4].Changes;
			}
			
			return ci;
		}
	}
	
	
}
