using UnityEngine;
using System.Collections.Generic;


//this class also handles initialization camera nonsense
public class TransitionCameraManager : FakeMonoBehaviour
{
	
	//render to this guy someday eventually ha ha...
	public RenderTexture AllRenderTexture { get; private set; }
	
	public TimedEventDistributor TED { get; private set; }
	
    public TransitionCameraManager(ManagerManager aManager)
        : base(aManager) 
    {
		AllRenderTexture = new RenderTexture(Screen.width,Screen.height,16); 
		TED = new TimedEventDistributor();
    }
	
	public override void Start()
	{
		
	}

    
    public override void Update()
    {
        TED.update(Time.deltaTime);

        
	}
	
	public void fade(System.Action aFadeCompleteCb)
	{
		TimedEventDistributor.TimedEventChain chain = TED.add_event(
			delegate(float time)
            {
                //TODO if fade is complete
					return true;
            },
        0).then_one_shot(
			delegate()
			{
				aFadeCompleteCb();
			}
		).then(
			delegate(float time)
			{
				//TODO if fade out is complete
					return true;
			},
		0);
	}
	
    
}
