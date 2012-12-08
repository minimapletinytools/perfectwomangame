using UnityEngine;
using System.Collections.Generic;

public class GameEvents {

    public class FadeEvent
    {
        bool mIn;
        public FadeEvent(bool aIn)
        {
            mIn = aIn;
        }

        bool call(float aTime)
        {
            return true;
        }

        GameManager.GameEventDelegate get_event()
        {
            return call;
        }
    }
    
}
