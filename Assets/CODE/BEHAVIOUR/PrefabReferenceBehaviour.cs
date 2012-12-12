using UnityEngine;
using System.Collections;

public class PrefabReferenceBehaviour : MonoBehaviour {
    
	public GameObject mDemoChar;
	public Shader mDefaultCharacterShader;
    public Texture2D mDefaultParticleTexture;
    public GameObject[] mCharacters; //29, 0 is fetus, row major (or was it collumn?)
    public GameObject mImageEffectsPrefabs;

    public TextAsset[] mPossiblePoses;

    //ScreenEffectShader nonsense here
}
