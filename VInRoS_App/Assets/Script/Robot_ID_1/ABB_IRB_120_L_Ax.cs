// System
using System;
using System.Text;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

/*
Description:
    Robot Type - ABB IRB 120 with SMC Linear Axis (LEJSH63NZA 800)
        Absolute Joint Position:
            Joint L: [0.0, 0.8] [m]
            Joint 1: [+/- 165.0] [°]
            Joint 2: [+/- 110.0] [°]
            Joint 3: [-110.0, +70.0] [°]
            Joint 4: [+/- 160.0] [°]
            Joint 5: [+/- 120.0] [°]
            Joint 6: [+/- 400.0] [°]
*/

public class ABB_IRB_120_L_Ax : MonoBehaviour
{
    /*
    Description:
        Global variables.
    */
    public static class G_ABB_IRB_120_L_Ax_Str
    {
        public static float[] Q_target = new float[7];
        public static float[] Q_actual = new float[7];
        public static bool[] In_Position = new bool[7];
    }

    /*
    Description:
        Private variables.
    */
    private readonly float[] Q_home = new float[7] {400.0f, 90.0f, 0.0f, 0.0f, 0.0f, 90.0f, 0.0f};

    /*
    Description:
        Start called before the first frame update.
    */
    void Start()
    {
        G_ABB_IRB_120_L_Ax_Str.Q_target = G_ABB_IRB_120_L_Ax_Str.Q_actual = Q_home;
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        
    }
}
