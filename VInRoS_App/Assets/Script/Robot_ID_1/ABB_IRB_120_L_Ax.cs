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
        Structures, enumerations, etc.
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
    private readonly float[,] Q_limit = new float[7,2] {{0.0f, 800.0f}, {-165.0f, 165.0f}, {-110.0f, 110.0f}, {-110.0f, 70.0f},
                                                        {-160.0f, 160.0f}, {-120.0f, 120.0f}, {-400.0f, 400.0f}};

    /*
    Description:
        Start called before the first frame update.
    */
    void Start()
    {
        // Set the actual position and the target position to the home position.
        var i = 0;
        foreach(float Q_home_i in Q_home){
            G_ABB_IRB_120_L_Ax_Str.Q_target[i] = Q_home_i;
            G_ABB_IRB_120_L_Ax_Str.Q_actual[i] = Q_home_i;
            i++;
        }
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {

    }

    /*
    Description:
        Help functions for the control.
    */
    void OnApplicationQuit()
    {
        try
        {
            Destroy(this);
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }
}
