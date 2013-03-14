using UnityEngine;
using System.Collections.Generic;

public class CharacterData {
	
	[System.Serializable]
	public class ImageSizeOffsetAnimationData
	{
		public string Name {get; set;}
		public string AnimationEffect {get; set;}
		public Vector3 Offset {get; set;}
		public Vector2 Size {get; set;}
	}
    //must match with class defined in ggfiles
    [System.Serializable]
    public class CharacterDataSizes
    {
        //ordered as in CharacterPreprocessor.sLimbs
        public List<List<Vector3>> mMountingPositions = new List<List<Vector3>>();
		public List<Vector2> mLimbSizes = new List<Vector2>();
		
		//TODO delete
        public List<Vector3> mBackgroundPositions = new List<Vector3>();
        public List<Vector3> mForegroundPositions = new List<Vector3>();
		public List<List<Vector3>> mCutscenePositions = new List<List<Vector3>>();
        public List<Vector2> mBackgroundSizes = new List<Vector2>();
        public List<Vector2> mForegroundSizes = new List<Vector2>();
		public List<List<Vector2>> mCutsceneSizes = new List<List<Vector2>>();
		
		public List<ImageSizeOffsetAnimationData> mStaticElements = new List<ImageSizeOffsetAnimationData>();

        public Vector2 mBackSize = new Vector2();
        public Vector2 mOffset = new Vector2();
        public string mName = "";
    }

    public class CharacterDataImages
    {
        public AudioClip backgroundMusic;

        public Texture2D head;
        public Texture2D leftLowerArm;
        public Texture2D leftLowerLeg;
        public Texture2D leftUpperArm;
        public Texture2D leftUpperLeg;
        public Texture2D rightLowerArm;
        public Texture2D rightLowerLeg;
        public Texture2D rightUpperArm;
        public Texture2D rightUpperLeg;
        public Texture2D torso;
        public Texture2D waist;
        public Texture2D background1;
		
		//TODO delete
        public List<Texture2D> backgroundElements = new List<Texture2D>();
        public List<Texture2D> foregroundElements = new List<Texture2D>();
		public List<List<Texture2D>> cutsceneElements = new List<List<Texture2D>>();
		
		Dictionary<string, Texture2D> staticElements = new Dictionary<string, Texture2D>();
    }
}
