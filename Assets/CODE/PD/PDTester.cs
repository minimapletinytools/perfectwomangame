using UnityEngine;
using System.Collections;


//put me on a camera game object with gui 
public class PDTester : MonoBehaviour {

    PDManager mP = new PDManager(null);
    int mCurrentState = 0;
	void Update () {
	    
	}

    void OnGUI()
    {
        //poo poo
        GUI.TextArea(new Rect(50, 50, Screen.width-100, Screen.height-100), "nonsense");

        for (int i = 0; i < 4; i++)
            if (GUI.Button(new Rect(100 + i * 100, 400, 75, 75), i.ToString()))
                advance(i);
    }

    void advance(int choice)
    {
    }
}
