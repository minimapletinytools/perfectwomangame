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
	
	
	public override void Start()
	{
		//TODO load necessary nonsense
			//mManager.mAssetLoader.load_poses("POSES");
		
		//TODO initialize game state
			//start in on loading screen
			//interfaceManager -> loading screen ...
	
	}

    
    public override void Update()
    {
        //User = (mManager.mZigManager.has_user());

        
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
