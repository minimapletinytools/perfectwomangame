using UnityEngine;
using System.Collections.Generic;

public static class GameStrings {


    public static string GetString(string id, string token1 = "", string token2 = ""){
		Dictionary<string,string> lang = null;
		if (GameConstants.language == 0)
			lang = english;
		else if (GameConstants.language == 1)
			lang = french;
        string r = lang [id];
        r = r.Replace("<token1>", token1);
        r = r.Replace("<token2>", token2);
        return r;
    }


    static Dictionary<string,string> english = new Dictionary<string,string>()
    {
        {"GCcredits1","A game by:"},
        {"GCcredits2","With assistance from:"},
        {"GCcredits3","Sound + Music:"},
        {"GCcredits4","Special thanks:"},

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

        {"MNPfetus1","Try and make your first movements."},
        {"MNPfetus2","Match the pose behind you."},
        {"MNPfetus3","You lived to be very old. Enjoy what's left of your life."},
        {"MNPdeath1","BAD PERFORMANCE!"},
        {"MNPdeath2","HORRIBLE PERFORMANCE!"},
        {"MNPdeath3","You die an early death."},
        {"MNPdeath4","But that's okay, you are already old."},
        {"MNPdeath5","Next time you perform that bad you will die."},
        {"MNPastro","You turn 110"},
        {"MNPtrans1","Make your first life decision."},
        {"MNPtrans2","You turn <token1>."},
        {"MNPdiff1","That's an easy choice. You should be able to manage that!"},
        {"MNPdiff2","You made a normal choice. Show how good you are!"},
        {"MNPdiff3","That's a hard one. Show your skills!"},
        {"MNPdiff4","You made an extreme choice? Let's see if you survive!"},

        {"NIM1","You graduate from Space Camp???"},
        {"NIM2","But everyone must pass."},
        {"NIM3","You die at the age of <token1>."},

		{"NIMIMG1","TEXTBOX-FETUS"},
		{"NIMIMG2","TEXTBOX-110"},

        {"SM1","You rest here beneath the earth..."},
        {"SM2","Unfortunately you died "},
        {"SM3","You died "},
        {"SM4","as an "},
        {"SM5","as a "},

        {"UR1","Your career can start early!"},
        {"UR2","Education can start early!"},
        {"UR3","Maybe you should play some video games..."},
        {"UR4","A simple job. A simple life."},
        {"UR5","Thar she blows."},
        {"UR6","Perhaps you should try your sex appeal."},
        {"UR7","Oooga Booga! Oooga Booga!"},


    };

	static Dictionary<string,string> french = new Dictionary<string,string>()
	{
		{"GCcredits1","Un jeu de"},
		{"GCcredits2","Avec l'aide de"},
		{"GCcredits3","Son + Musique"},
		{"GCcredits4","Remerciements particuliers"},

		{"TCMkinect1","Placez-vous dans\nle champs sans personne\nd'autre autour"},
		{"TCMkinect2","Kinect non trouvée"},
		{"TCMkinect3","Placez-vous au centre de l'écran"},

		{"CM1","Choisissez votre vie idéale à l'âge de <token1> !"},
		{"CM2","C'est un "},
		{"CM3"," choix."},
		{"CMdiffphrase1","facile"},
		{"CMdiffphrase2","normal"},
		{"CMdiffphrase3","difficile"},
		{"CMdiffphrase4","extrême"},

		{"MNPfetus1","Expérimentez vos premiers mouvements"},
		{"MNPfetus2","Imitez la pose derrière vous."},
		{"MNPfetus3","Vous avez vécu très agée. Profitez de ce qui reste de votre vie."},

		{"MNPdeath1","MAUVAISE PERFORMANCE!"},
		{"MNPdeath2","HORRIBLE PERFORMANCE!"},
		{"MNPdeath3","Vous mourez précocèment."},
	
		{"MNPdeath4","Mais ça va, vous êtes déjà vieille."},
		{"MNPdeath5","À la prochaine mauvaise performance, vous mourrez."},
		{"MNPastro","Vous avez 110 ans"},
		{"MNPtrans1","Faites votre premier choix de vie."},
		{"MNPtrans2","Vous avez <token1> ans."},
		{"MNPdiff1","C'est un choix facile. Vous devriez pouvoir gérer cela!"},
		{"MNPdiff2","C'est un choix normal. Montrez comme vous gérer !"},
		{"MNPdiff3","En voilà un difficile. Faites preuve de talent !"},
		{"MNPdiff4","Vous avez fait un choix extrême? Voyons si vous y survivez !"},

		{"NIM1","Vous êtes diplomée du Space Camp???"},
		{"NIM2","Mais tout le monde doit mourir"},
		{"NIM3","Vous mourez à l'âge de <token1> ans."},

		{"NIMIMG1","TEXTBOX-FETUS_FR"},
		{"NIMIMG2","TEXTBOX-110_FR"},

		{"SM1","Vous reposez ici sous terre..."},
		{"SM2","Malheureusement vous mourez"},
		{"SM3","Vous êtes morte "},
		{"SM4","en tant que"},
		{"SM5","en tant que"},

		{"UR1","Votre carrière peut démarrer tôt"},
		{"UR2","Les étudens peuvent démarrer tôt!"},
		{"UR3","Peut-être devriez-vous jouer aux jeux vidéo..."},
		{"UR4","Un travail simple. Une vie simple."},
		{"UR5","Baleine en vue."},
		{"UR6","Peut-être devriez-vous utiliser votre sex-appeal."},
		{"UR7","Oooga Booga! Oooga Booga!"},
	};
}
