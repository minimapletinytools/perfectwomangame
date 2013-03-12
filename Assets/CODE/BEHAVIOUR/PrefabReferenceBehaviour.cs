using UnityEngine;
using System.Collections;

public class PrefabReferenceBehaviour : MonoBehaviour {

    public GameObject mDebugLimb;

	public GameObject mDemoChar;
	public Shader mDefaultCharacterShader;
	public Shader mTransparentCharacaterShader;
	
    public Texture2D mDefaultParticleTexture;
    //public GameObject[] mCharacters; //29, 0 is fetus, row major (or was it collumn?)
    public GameObject mGrave;
    public GameObject mImageEffectsPrefabs;
    public GameObject mPlanePrefab;
    public TextAsset[] mPossiblePoses;
    public TextAsset[] mDefaultTargetPoses;
        
}
