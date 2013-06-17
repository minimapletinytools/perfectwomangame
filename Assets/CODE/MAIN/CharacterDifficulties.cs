using UnityEngine;
using System.Collections;

//TODO DELETE

public class CharacterDifficulties
{
    public class Difficulty
    {
        public float[] values = new float[29];

    }

    public static Difficulty[] difficulties = new Difficulty[29];

    static CharacterDifficulties()
    {
        for (int i = 0; i < 29; i++)
            difficulties[i] = new Difficulty();

        difficulties[0].values = new float[] { 0, 
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0 };

        difficulties[1].values = new float[] { 0,  //5-1
            0, 0, 0, 0, 
            100, 0, -1, 1, 
            0, 0, -1, 0, 
            -1, 1, 0, 1, 
            -1, 0, 0, 0, 
            -1, 1, -1, 1, 
            0, 1, 0, 1 };
        difficulties[2].values = new float[] { 0, 
            0, 0, 0, 0, 
            1, 0, 1, 100, 
            100, -1, 0, -1, 
            0, -1, 0, 0, 
            0, -1, 0, 0, 
            1, -1, 0, 0, 
            -1, -1, 0, 0 };
        difficulties[3].values = new float[] { 0, 
            0, 0, 0, 0, 
            -1, 1, 1, -1, 
            100, 100, 1, -1, 
            100, 100, -1, 0, 
            100, 1, 0, 0, 
            100, 1, 0, 0, 
            0, 0, 1, -1 };
        difficulties[4].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 1, 0, 0, 
            1, 0, 0, -1, 
            1, 1, 0, -1, 
            1, 0, 1, -1, 
            0, 100, 0, 0 };
        difficulties[5].values = new float[] { 0, //16-1
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 100, 1, 0, 
            1, 100, 0, 1, 
            1, 100, 0, 0, 
            1, 1, 1, 1, 
            0, 1, 0, -1 };
        difficulties[6].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            1, -1, 1, 0, 
            1, -1, 0, 0, 
            0, -1, 0, 0, 
            100, 0, 100, 0, 
            0, -1, 0, 1 };
        difficulties[7].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            1, -1, -1, 0, 
            1, 1, -1, 0, 
            -1, 100, -1, 0, 
            1, 0, -1, 0, 
            0, 1, -1, 0 };
        difficulties[8].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 100, 0, -1, 
            100, 0, 0, -1, 
            1, 100, 0, -1, 
            1, 100, 1, 0, 
            0, 1, 0, 1 };
        difficulties[9].values = new float[] { 0, //27-1
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            -1, 1, 0, 100, 
            -1, 1, 1, 0, 
            0, 0, 0, 0, 
            -1, 1, -1, -1 };
        difficulties[10].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            1, -1, 0, 1, 
            100, -1, 0, 1, 
            0, -1, 0, 1, 
            1, -1, 1, 1 };
        difficulties[11].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            1, 100, 0, 0, 
            0, 1, 1, 0, 
            0, 1, 0, 1, 
            0, 1, 0, 1 };
        difficulties[12].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            1, 100, 0, -1, 
            1, 100, 1, -1, 
            1, 100, 1, -1, 
            0, 1, -1, 0 };
        difficulties[13].values = new float[] { 0, //34-1
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            -1, 0, 1, 1, 
            -1, 100, 0, 1,
            -1, 1, -1, 0 };
        difficulties[14].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            1, -1, 1, 1, 
            0, -1, 1, -1, 
            1, -1, -1, 100 };
        difficulties[15].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            100, -1, 0, 0, 
            1, 0, -1, 0, 
            -1, 1, 0, -1 };
        difficulties[16].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            1, 1, 0, 0, 
            1, 1, 100, 0, 
            0, 0, -1, 0 };
        difficulties[17].values = new float[] { 0, //45-1
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            -1, 1, 0, 0, 
            -1, 100, -1, 1 };
        difficulties[18].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, -1, 1, 1, 
            1, -1, 100, 1 };
        difficulties[19].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            1, 1, -1, 0, 
            0, -1, 1, 1 };
        difficulties[20].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            1, 100, 0, 1, 
            0, 1, 0, 0 };
        difficulties[21].values = new float[] { 0, //60-1
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            -1, 100, 0, 0 };
        difficulties[22].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            100, 0, -1, 1 };
        difficulties[23].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            -1, 0, 0, 100 };
        difficulties[24].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            100, 0, 0, 0 };
        difficulties[25].values = new float[] { 0, //80-1
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0 };
        difficulties[26].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0 };
        difficulties[27].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0 };
        difficulties[28].values = new float[] { 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0, 

            0, 0, 0, 0, 
            0, 0, 0, 0, 
            0, 0, 0, 0 };

    }
}