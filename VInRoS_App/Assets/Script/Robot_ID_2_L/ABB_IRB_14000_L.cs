// System
using System;
using System.Text;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

/*
Description:
    Robot Type - ABB IRB 14000 (Left)
        Absolute Joint Position:
            Joint 1: [+/- 168.5] [°]
            Joint 2: [-143.5, +43.5] [°]
            Joint 7: [+/- 168.5] [°]
            Joint 3: [-123.5, +80.0] [°]
            Joint 4: [+/- 290.0] [°]
            Joint 5: [-88.0, +138.0] [°]
            Joint 6: [+/- 229.0] [°]
*/

public class ABB_IRB_14000_L : MonoBehaviour
{
    /*
    Description:
        Global variables.
    */
    public static class G_ABB_IRB_14000_L_Str
    {
        public static float[] Q_target = new float[7];
        public static float[] Q_actual = new float[7];
        public static bool[] In_Position = new bool[7];
    }

    /*
    Description:
        Private variables.
    */
    private readonly float[] Q_home = new float[7] {0.0f, -130.0f, 135.0f, 30.0f, 0.0f, 40.0f, 0.0f};

    /*
    Description:
        Start called before the first frame update.
    */
    void Start()
    {
        G_ABB_IRB_14000_L_Str.Q_target = G_ABB_IRB_14000_L_Str.Q_actual = Q_home;
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        
    }
}
