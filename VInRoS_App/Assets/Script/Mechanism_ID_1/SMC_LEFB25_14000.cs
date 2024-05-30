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

    /*
    Description:
        Private variables.
    */
    private readonly float[] Q_home = new float[2] {700.0f, 700.0f};

    /*
    Description:
        Start called before the first frame update.
    */
    void Start()
    {
        G_SMC_LEFB25_14000_Str.Q_target = G_SMC_LEFB25_14000_Str.Q_actual = Q_home;
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        
    }
}
