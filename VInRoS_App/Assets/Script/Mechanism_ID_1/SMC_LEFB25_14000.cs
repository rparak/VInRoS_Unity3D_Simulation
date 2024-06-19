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
    private SMC_LEFB25_14000_STATE_Enum[] state_id = new SMC_LEFB25_14000_STATE_Enum[2];
    private int[] t_index = new int[2];

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

        state_id[0] = SMC_LEFB25_14000_STATE_Enum.INIT;
        state_id[1] = SMC_LEFB25_14000_STATE_Enum.INIT;
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        // The state machine used to control the mechanism with identification 
        // number 1 (blue shuttle), which is defined by index 0.
        Control_Mechanism(0);

        // The state machine used to control the mechanism with identification 
        // number 2 (red shuttle), which is defined by index 1.
        Control_Mechanism(1);
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

    public void Control_Mechanism(int mech_id){
        /*
        Description:
            A state machine used to control a mechanism with identification number <mech_id>.
        */
        switch(state_id[mech_id]){
            case SMC_LEFB25_14000_STATE_Enum.INIT:
            {
                G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Active[mech_id] = true;

                if(OPC_UA_Client.G_OPC_UA_Client_Str.Is_Connected == true && G_OPC_UA_Client_General_Data_Str.Simulation_Enabled == true){
                    state_id[mech_id] = SMC_LEFB25_14000_STATE_Enum.WAIT_COMMAND;
                }
            }
            break;

            case SMC_LEFB25_14000_STATE_Enum.WAIT_COMMAND:
            {
                G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Move_Active[mech_id] = false;

                if(OPC_UA_Client.G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Home[mech_id] == true){
                    state_id[mech_id] = SMC_LEFB25_14000_STATE_Enum.HOME_INIT;
                }

                if(OPC_UA_Client.G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Start[mech_id] == true){
                    state_id[mech_id] = SMC_LEFB25_14000_STATE_Enum.MOVE_CHECK;
                }
            }
            break;

            case SMC_LEFB25_14000_STATE_Enum.HOME_INIT:
            {
                G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Move_Active[mech_id] = true;

                // Set the target position of the mechanism to the home position.
                //  Note:
                //      Check that the desired absolute joint positions are not out of limit.
                G_SMC_LEFB25_14000_Str.Q_target[mech_id] = Mathf.Clamp(Q_home[mech_id], Q_limit[mech_id, 0], Q_limit[mech_id, 1]);

                state_id[mech_id] = SMC_LEFB25_14000_STATE_Enum.HOME_PERFORM;
            }
            break;

            case SMC_LEFB25_14000_STATE_Enum.HOME_PERFORM:
            {
                if(G_SMC_LEFB25_14000_Str.In_Position[mech_id] == true){
                    state_id[mech_id] = SMC_LEFB25_14000_STATE_Enum.WAIT_COMMAND;
                }
            }
            break;

            case SMC_LEFB25_14000_STATE_Enum.MOVE_CHECK:
            {
                G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Move_Active[mech_id] = true;

                if(t_index[mech_id] == OPC_UA_Client.G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Trajectory_Length[mech_id]){
                    t_index[mech_id] = 0;
                }

                state_id[mech_id] = SMC_LEFB25_14000_STATE_Enum.MOVE_INIT;
            }
            break;

            case SMC_LEFB25_14000_STATE_Enum.MOVE_INIT:
            {
                // Set the target position of the mechanism to the trajectory position in index t_index{mech_id}.
                //  Note:
                //      Check that the desired absolute joint positions are not out of limit.
                G_SMC_LEFB25_14000_Str.Q_target[mech_id] = Mathf.Clamp(OPC_UA_Client.G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Trajectory[t_index[mech_id], mech_id], 
                                                                       Q_limit[mech_id, 0], Q_limit[mech_id, 1]);


                state_id[mech_id] = SMC_LEFB25_14000_STATE_Enum.MOVE_PERFORM;
            }
            break;

            case SMC_LEFB25_14000_STATE_Enum.MOVE_PERFORM:
            {
                if(OPC_UA_Client.G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Stop[mech_id] == true){
                    t_index[mech_id] = 0;
                    state_id[mech_id] = SMC_LEFB25_14000_STATE_Enum.WAIT_COMMAND;
                }else{
                    if(G_SMC_LEFB25_14000_Str.In_Position[mech_id] == true){
                        t_index[mech_id]++;
                        state_id[mech_id] = SMC_LEFB25_14000_STATE_Enum.MOVE_CHECK;
                    }
                }
            }
            break;
        }
    }
}
