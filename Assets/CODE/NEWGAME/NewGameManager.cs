using UnityEngine;
using System.Collections;


public class NewGameManager : FakeMonoBehaviour
{
    public NewGameManager(ManagerManager aManager)
        : base(aManager) 
    {
    }
	
	//TODO implement these
	public int CurrentLevel
    { get; private set; }
	
	
	AssetBundleCallbacks mAssetCallbacks;
	
	//actual game data
	//PerformanceStats[] mPerformanceStats = new PerformanceStats[10];
	
	public override void Start()
	{
		mAssetCallbacks = new AssetBundleCallbacks(this);
		mManager.mAssetLoader.new_load_character("0-1",mAssetCallbacks);
		mManager.mAssetLoader.new_load_poses("POSES",mAssetCallbacks);
		
		//TODO initialize game state
			//start in on loading screen
			//interfaceManager -> loading screen ...
		
		//initialize game data
		
	
	}

    
    public override void Update()
    {
        //User = (mManager.mZigManager.has_user());
		
		//begin mode (nothing)
		//play mode (timer, score, and tracking running)
		//cutscene mode (disable characters, bg manager needs to transition into cutscene mode)
		//selection mode (nothing) -> prompts fade out
		//change character behind fade -> fade in
        
	}
	
	public void cleanup()
	{
		//TODO
	}
	
	
	public void hack_choice(int choice, float time = -1)
	{
		//TODO
	}
    
}
