// System
using System;
using System.Text;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

using static OPC_UA_Client;

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
        Structures, enumerations, etc.
    */
    public static class G_SMC_LEFB25_14000_Str
    {
        public static float[] Q_target = new float[2];
        public static float[] Q_actual = new float[2];
        public static bool[] In_Position = new bool[2];
    }

    public enum SMC_LEFB25_14000_STATE_Enum
    {
        INIT         = 0,
        WAIT_COMMAND = 10,
        HOME_INIT    = 20,
        HOME_PERFORM = 21,
        MOVE_CHECK   = 30,
        MOVE_INIT    = 31,
        MOVE_PERFORM = 32,
    }

    /*
    Description:
        Global variables.
    */
    private SMC_LEFB25_14000_STATE_Enum state_id_mech_1;
    private SMC_LEFB25_14000_STATE_Enum state_id_mech_2; 
    private int mech_1_trajectory_index = 0;
    private int mech_2_trajectory_index = 0;

    /*
    public float[] Q_target_tmp = new float[2];
    public bool start; public bool stop; public bool home;
    public SMC_STATE_Enum state_id;
    public int traj_index;
    public float[,] Q_traj = new float[5,2] {{0.0f,   0.0f}, 
                                             {700.0f, 700.0f},
                                             {200.0f, 200.0f},
                                             {1000.0f, 1000.0f},
                                             {1400.0f, 1400.0f}};
    */

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

        state_id_mech_1 = SMC_LEFB25_14000_STATE_Enum.INIT;
        state_id_mech_2 = SMC_LEFB25_14000_STATE_Enum.INIT;
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        /*
        Description:
            A state machine used to control a mechanism with identification number 1 (blue shuttle).
        */
        switch(state_id_mech_1){
            case SMC_LEFB25_14000_STATE_Enum.INIT:
            {
                if(OPC_UA_Client.G_OPC_UA_Client_Str.Is_Connected == true){
                    state_id_mech_1 = SMC_LEFB25_14000_STATE_Enum.WAIT_COMMAND;
                }
            }
            break;

            case SMC_LEFB25_14000_STATE_Enum.WAIT_COMMAND:
            {
                if(OPC_UA_Client.G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Home[0] == true){
                    state_id_mech_1 = SMC_LEFB25_14000_STATE_Enum.HOME_INIT;
                }

                if(OPC_UA_Client.G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Start[0] == true){
                    state_id_mech_1 = SMC_LEFB25_14000_STATE_Enum.HOME_INIT;
                }
            }
            break;

            case SMC_LEFB25_14000_STATE_Enum.HOME_INIT:
            {
                OPC_UA_Client.G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Home[0] = false;

                // Set the target position of the mechanism to the home position.
                //  Note:
                //      Check that the desired absolute joint positions are not out of limit.
                G_SMC_LEFB25_14000_Str.Q_target[0] = Mathf.Clamp(Q_home[0], Q_limit[0, 0], Q_limit[0, 1]);

                state_id_mech_1 = SMC_LEFB25_14000_STATE_Enum.HOME_PERFORM;
            }
            break;

            case SMC_LEFB25_14000_STATE_Enum.HOME_PERFORM:
            {
                if(G_SMC_LEFB25_14000_Str.In_Position[0] == true){
                    state_id_mech_1 = SMC_LEFB25_14000_STATE_Enum.WAIT_COMMAND;
                }
            }
            break;

            case SMC_LEFB25_14000_STATE_Enum.MOVE_CHECK:
            {

            }
            break;

            case SMC_LEFB25_14000_STATE_Enum.MOVE_INIT:
            {

            }
            break;

            case SMC_LEFB25_14000_STATE_Enum.MOVE_PERFORM:
            {

            }
            break;
        }



        /*
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
        */
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
