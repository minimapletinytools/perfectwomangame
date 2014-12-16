using UnityEngine;
using System.Collections;

public class PrefabReferenceBehaviour : MonoBehaviour {

	public GameObject mMiniChar;
	public TextAsset mCheapPose; //this is birth pose
	public TextAsset mDefaultPose;
	
	//shaders/materials
	public Shader mDefaultCharacterShader;
	public Shader mTransparentCharacaterShader;
	public Shader mMiniCharacterShader; //legacy
	public Shader mGraphShader;//legacy
	public Shader mRenderTextureShader; //legacy


    public Shader mXB1KinectImageMaskingShader;
    public Shader mXB1ClearShader;//TODO DELETE
    public Shader mXB1DepthImageShader;

	//important prefabs
	public GameObject mImageEffectsPrefabs;
    public GameObject mPlanePrefab;
	
	public TextAsset[] mPossiblePoses;
    //public GameObject[] mCharacters; //29, 0 is fetus, row major (or was it collumn?)
    //public TextAsset[] mDefaultTargetPoses;


	
	
	public Object[] mInclusions;        
}
