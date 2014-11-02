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
    Texture2D take_color_image();

    void write_data(byte[] aData, string aName);
    void read_data(string aName, System.Action<byte[]> aResponse);


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

    public Texture2D take_color_image(){return null;}

    public void write_data(byte[] aData, string aName){}
    public void read_data(string aName, System.Action<byte[]> aResponse){}


	public ZgDepth DepthImage{get{ return null; }}
	public ZgImage ColorImage{get{ return null; }}
	public ZgLabelMap LabelMap{get{ return null; }}
	public bool ReaderInitialized { get{ return false; } }
	public bool IsMicrosoftKinectSDK { get{ return false; } }

}

