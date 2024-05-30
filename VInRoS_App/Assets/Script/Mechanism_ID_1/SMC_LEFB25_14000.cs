// System
using System;
using System.Text;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

/*
Description:
    Mechanism Type - SMC LEFB25UNZS 14000C
        Absolute Joint Position: 
            Joint L: [0.0, 1.4] [m]
*/

public class SMC_LEFB25_14000 : MonoBehaviour
{
    /*
    Description:
        Global variables.
    */
    public static class G_SMC_LEFB25_14000_Str
    {
        public static float[] Q_target = new float[2];
        public static float[] Q_actual = new float[2];
        public static bool[] In_Position = new bool[2];
    }

    public float[] Q_target_tmp = new float[2];

    /*
    Description:
        Private variables.
    */
    private readonly float[] Q_home = new float[2] {700.0f, 700.0f};
    private readonly float[,] Q_limit = new float[2,2] {{0.0f, 1400.0f}, {0.0f, 1400.0f}};

    /*
    Description:
        Start called before the first frame update.
    */
    void Start()
    {
        // Set the actual position and the target position to the home position.
        var i = 0;
        foreach(float Q_home_i in Q_home){
            G_SMC_LEFB25_14000_Str.Q_target[i] = Q_home_i;
            G_SMC_LEFB25_14000_Str.Q_actual[i] = Q_home_i;
            i++;
        }
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        // Check that the desired absolute joint positions are not out of limit.
        for(int i = 0; i < G_SMC_LEFB25_14000_Str.Q_target.Length; i++) 
        {
            G_SMC_LEFB25_14000_Str.Q_target[i] = Mathf.Clamp(Q_target_tmp[i], Q_limit[i, 0], Q_limit[i, 1]);
        }
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
