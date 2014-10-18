using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public interface ZgInterface
{
    void initialize(ZgManager aZig);
	ZgDepth DepthImage{get;}
	ZgImage ColorImage{get;}
	ZgLabelMap LabelMap{get;}
	bool ReaderInitialized { get; }
	bool IsMicrosoftKinectSDK { get; }

    bool has_user();
    void update();

	bool can_start();

    //TODO
    //void write_data(byte[] aData, string aName);
	//TODO this needs to be async...
    //byte[] read_data(string aName);


}

public class EmptyZig : ZgInterface
{
    //if MonoBehaviours are needed, they can be added to the aZig.gameObject
    public void initialize(ZgManager aZig)
    {
    }
    
    public bool has_user()
    {
        return false;
    }

    public void update()
    {

    }

	public bool can_start()
	{
		return true;
	}


	public ZgDepth DepthImage{get{ return null; }}
	public ZgImage ColorImage{get{ return null; }}
	public ZgLabelMap LabelMap{get{ return null; }}
	public bool ReaderInitialized { get{ return false; } }
	public bool IsMicrosoftKinectSDK { get{ return false; } }

}

