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

    public enum SMC_STATE_Enum
    {
        INIT = 0,
        WAIT = 10,
        HOME_1 = 20,
        HOME_2 = 21,
        MOVE_1 = 30,
        MOVE_2 = 31,
        MOVE_3 = 32,
    }

    public float[] Q_target_tmp = new float[2];
    public bool start; public bool stop; public bool home;
    public SMC_STATE_Enum state_id;
    public int traj_index;
    public float[,] Q_traj = new float[5,2] {{0.0f,   0.0f}, 
                                             {700.0f, 700.0f},
                                             {200.0f, 200.0f},
                                             {1000.0f, 1000.0f},
                                             {1400.0f, 1400.0f}};

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
            // = Q_home_i
            G_SMC_LEFB25_14000_Str.Q_target[i] = 0.0f;
            G_SMC_LEFB25_14000_Str.Q_actual[i] = 0.0f;
            i++;
        }

        state_id = SMC_STATE_Enum.WAIT;
        traj_index = 0;
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        switch(state_id){
            case SMC_STATE_Enum.INIT:
            {
                
            }
            break;

            case SMC_STATE_Enum.WAIT:
            {
                if(home == true){
                    state_id = SMC_STATE_Enum.HOME_1;
                }else if(start == true){
                    stop = false; traj_index = 0;
                    state_id = SMC_STATE_Enum.MOVE_1;
                }
            }
            break;

            case SMC_STATE_Enum.HOME_1:
            {
                home = false;

                // Check that the desired absolute joint positions are not out of limit.
                for(int i = 0; i < G_SMC_LEFB25_14000_Str.Q_target.Length; i++) 
                {
                    G_SMC_LEFB25_14000_Str.Q_target[i] = Mathf.Clamp(Q_home[i], Q_limit[i, 0], Q_limit[i, 1]);
                }
                state_id = SMC_STATE_Enum.HOME_2;
            }
            break;

            case SMC_STATE_Enum.HOME_2:
            {
                if(SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.In_Position[0] == true && 
                   SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.In_Position[1] == true){
                    state_id = SMC_STATE_Enum.WAIT;
                }
            }
            break;

            case SMC_STATE_Enum.MOVE_1:
            {
                start = false;

                if(traj_index == Q_traj.GetLength(0)){
                    traj_index = 0;
                    state_id = SMC_STATE_Enum.MOVE_1;
                }else{
                    state_id = SMC_STATE_Enum.MOVE_2;
                }
            }
            break;

            case SMC_STATE_Enum.MOVE_2:
            {
                for(int i = 0; i < G_SMC_LEFB25_14000_Str.Q_target.Length; i++) 
                {
                    G_SMC_LEFB25_14000_Str.Q_target[i] = Mathf.Clamp(Q_traj[traj_index, i], Q_limit[i, 0], Q_limit[i, 1]);
                }
                state_id = SMC_STATE_Enum.MOVE_3;
            }
            break;

            case SMC_STATE_Enum.MOVE_3:
            {
                if(stop == true){
                    stop = false; traj_index = 0;
                    state_id = SMC_STATE_Enum.WAIT;
                }else{
                    if(SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.In_Position[0] == true && 
                       SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.In_Position[1] == true){
                        traj_index++;
                        state_id = SMC_STATE_Enum.MOVE_1;
                    }
                }
            }
            break;
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
