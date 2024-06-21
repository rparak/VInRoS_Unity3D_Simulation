// System
using System;
using System.Text;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

using static OPC_UA_Client;

/*
Description:
    Mechanism Type - SMC Linear Axis LEJSH63NZA 800
        Absolute Joint Position:
            Joint L: [0.0, 0.8] [m]
*/

public class SMC_LEJSH63NZA_800 : MonoBehaviour
{
    /*
    Description:
        Structures, enumerations, etc.
    */
    public static class G_SMC_LEJSH63NZA_800_Str
    {
        public static float Q_target;
        public static float Q_actual;
        public static bool In_Position;
    }

    public enum SMC_LEJSH63NZA_800_STATE_Enum
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
    private SMC_LEJSH63NZA_800_STATE_Enum state_id;
    private int t_index;

    /*
    Description:
        Private variables.
    */
    private readonly float Q_home = 400.0f;
    private readonly float[] Q_limit = new float[2] {0.0f, 800.0f};

    /*
    Description:
        Start called before the first frame update.
    */
    void Start()
    {
        // Set the actual position and the target position to the home position.
        G_SMC_LEJSH63NZA_800_Str.Q_target = Q_home;
        G_SMC_LEJSH63NZA_800_Str.Q_actual = Q_home;

        state_id = SMC_LEJSH63NZA_800_STATE_Enum.INIT;
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        // The state machine used to control the mechanism, specifically the 7th linear axis of the ABB IRB 120 industrial robot.
        switch(state_id){
            case SMC_LEJSH63NZA_800_STATE_Enum.INIT:
            {
                OPC_UA_Client.G_OPC_UA_Client_SMC_LEJSH63NZA_800_Data_Str.Active = true;

                if(OPC_UA_Client.G_OPC_UA_Client_Str.Is_Connected == true && G_OPC_UA_Client_General_Data_Str.Simulation_Enabled == true){
                    state_id = SMC_LEJSH63NZA_800_STATE_Enum.WAIT_COMMAND;
                }
            }
            break;

            case SMC_LEJSH63NZA_800_STATE_Enum.WAIT_COMMAND:
            {
                OPC_UA_Client.G_OPC_UA_Client_SMC_LEJSH63NZA_800_Data_Str.Move_Active = false;

                if(OPC_UA_Client.G_OPC_UA_Client_SMC_LEJSH63NZA_800_Data_Str.Home == true){
                    state_id = SMC_LEJSH63NZA_800_STATE_Enum.HOME_INIT;
                }

                if(OPC_UA_Client.G_OPC_UA_Client_SMC_LEJSH63NZA_800_Data_Str.Start == true){
                    state_id = SMC_LEJSH63NZA_800_STATE_Enum.MOVE_CHECK;
                }
            }
            break;

            case SMC_LEJSH63NZA_800_STATE_Enum.HOME_INIT:
            {
                // Set the target position of the mechanism to the home position.
                //  Note:
                //      Check that the desired absolute joint positions are not out of limit.
                G_SMC_LEJSH63NZA_800_Str.Q_target = Mathf.Clamp(Q_home, Q_limit[0], Q_limit[1]);

                state_id = SMC_LEJSH63NZA_800_STATE_Enum.HOME_PERFORM;
            }
            break;

            case SMC_LEJSH63NZA_800_STATE_Enum.HOME_PERFORM:
            {
                OPC_UA_Client.G_OPC_UA_Client_SMC_LEJSH63NZA_800_Data_Str.Move_Active = true;

                if(G_SMC_LEJSH63NZA_800_Str.In_Position == true){
                    state_id = SMC_LEJSH63NZA_800_STATE_Enum.WAIT_COMMAND;
                }
            }
            break;

            case SMC_LEJSH63NZA_800_STATE_Enum.MOVE_CHECK:
            {
                if(t_index == OPC_UA_Client.G_OPC_UA_Client_SMC_LEJSH63NZA_800_Data_Str.Trajectory_Length){
                    t_index = 0;
                }

                state_id = SMC_LEJSH63NZA_800_STATE_Enum.MOVE_INIT;
            }
            break;

            case SMC_LEJSH63NZA_800_STATE_Enum.MOVE_INIT:
            {
                // Set the target position of the mechanism to the trajectory position.
                //  Note:
                //      Check that the desired absolute joint positions are not out of limit.
                G_SMC_LEJSH63NZA_800_Str.Q_target = Mathf.Clamp(OPC_UA_Client.G_OPC_UA_Client_SMC_LEJSH63NZA_800_Data_Str.Trajectory[t_index], 
                                                                Q_limit[0], Q_limit[1]);


                state_id = SMC_LEJSH63NZA_800_STATE_Enum.MOVE_PERFORM;
            }
            break;

            case SMC_LEJSH63NZA_800_STATE_Enum.MOVE_PERFORM:
            {
                OPC_UA_Client.G_OPC_UA_Client_SMC_LEJSH63NZA_800_Data_Str.Move_Active = true;
                
                if(OPC_UA_Client.G_OPC_UA_Client_SMC_LEJSH63NZA_800_Data_Str.Stop == true){
                    t_index = 0;
                    state_id = SMC_LEJSH63NZA_800_STATE_Enum.WAIT_COMMAND;
                }else{
                    if(G_SMC_LEJSH63NZA_800_Str.In_Position == true){
                        t_index++;
                        state_id = SMC_LEJSH63NZA_800_STATE_Enum.MOVE_CHECK;
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
