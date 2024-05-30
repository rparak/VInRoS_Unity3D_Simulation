// System
using System;
using System.Text;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

/*
Description:
    Robot Type - ABB IRB 14000 (Right)
        Absolute Joint Position:
            Joint 1: [+/- 168.5] [°]
            Joint 2: [-143.5, +43.5] [°]
            Joint 7: [+/- 168.5] [°]
            Joint 3: [-123.5, +80.0] [°]
            Joint 4: [+/- 290.0] [°]
            Joint 5: [-88.0, +138.0] [°]
            Joint 6: [+/- 229.0] [°]
*/

public class ABB_IRB_14000_R : MonoBehaviour
{
    /*
    Description:
        Global variables.
    */
    public static class G_ABB_IRB_14000_R_Str
    {
        public static float[] Q_target = new float[7];
        public static float[] Q_actual = new float[7];
        public static bool[] In_Position = new bool[7];
    }

    /*
    Description:
        Private variables.
    */
    private readonly float[] Q_home = new float[7] {0.0f, -130.0f, -135.0f, 30.0f, 0.0f, 40.0f, 0.0f};
    private readonly float[,] Q_limit = new float[7,2] {{-168.5f, 168.5f}, {-143.5f, 43.5f}, {-168.5f, 168.5f}, {-123.5f, 80.0f},
                                                        {-290.0f, 290.0f}, {-88.0f, 138.0f}, {-229.0f, 229.0f}};
                            
    /*
    Description:
        Start called before the first frame update.
    */
    void Start()
    {
        // Set the actual position and the target position to the home position.
        var i = 0;
        foreach(float Q_home_i in Q_home){
            G_ABB_IRB_14000_R_Str.Q_target[i] = Q_home_i;
            G_ABB_IRB_14000_R_Str.Q_actual[i] = Q_home_i;
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
