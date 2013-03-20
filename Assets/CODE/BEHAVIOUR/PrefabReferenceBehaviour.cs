using UnityEngine;
using System.Collections;

public class PrefabReferenceBehaviour : MonoBehaviour {

	public GameObject mMiniChar;
	public TextAsset mCheapPose;
	
	//shaders/materials
	public Shader mDefaultCharacterShader;
	public Shader mTransparentCharacaterShader;
	public Shader mMiniCharacterShader;
	
	//important prefabs
	public GameObject mImageEffectsPrefabs;
    public GameObject mPlanePrefab;
	
	public TextAsset[] mPossiblePoses;
    //public GameObject[] mCharacters; //29, 0 is fetus, row major (or was it collumn?)
    //public TextAsset[] mDefaultTargetPoses;
        
}
