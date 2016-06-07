using UnityEngine;
using System.Collections.Generic;

public static class GameStrings {


    public static string GetString(string id, string token1 = "", string token2 = ""){
        string r = english [id];
        r.Replace("<token1>", token1);
        r.Replace("<token2>", token2);
        return r;
    }


    static Dictionary<string,string> english = new Dictionary<string,string>()
    {
        {"GCcredits1","A game by"},
        {"GCcredits2","With assistance from"},
        {"GCcredits3","Sound + Music"},
        {"GCcredits4","Special thanks"},

        {"TCMkinect1","Make sure you are\nin frame and no body\nparts are covered"},
        {"TCMkinect2","Kinect not found"},
        {"TCMkinect3","Center yourself in the screen"},

        {"CM1","Choose your perfect life at age <token1> !"},
        {"CM2","That is a "},
        {"CM3"," choice."},
        {"CMdiffphrase1","easy"},
        {"CMdiffphrase2","normal"},
        {"CMdiffphrase3","hard"},
        {"CMdiffphrase4","extreme"},


    };
}
