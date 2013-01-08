using UnityEngine;
using System.Collections.Generic;

public class CharacterData {

    [System.Serializable]
    public class CharacterDataSizes
    {
        //ordered as in CharacterPreprocessor.sLimbs
        public List<List<Vector3>> mMountingPositions = new List<List<Vector3>>();
        public List<Vector3> mBackgroundPositions = new List<Vector3>();
        public List<Vector3> mForegroundPositions = new List<Vector3>();

        public List<Vector2> mLimbSizes = new List<Vector2>();
        public List<Vector2> mBackgroundSizes = new List<Vector2>();
        public List<Vector2> mForegroundSizes = new List<Vector2>();

        public Vector2 mBackSize = new Vector2();

        public string mName = "";
    }

    public class ChaarcterDataImages
    {
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
        public Texture2D[] backgroundElements;
        public Texture2D[] foregroundElements;
    }

    public class CharacterDataInstantiated
    {
        public CharacterDataSizes Data {get; private set;}
        public CharacterDataInstantiated(CharacterDataSizes aData)
        {
            Data = aData;
        }
    }
}
