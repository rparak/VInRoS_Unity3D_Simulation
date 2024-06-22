// System
using System;
using System.Text;
using System.Linq;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

using static OPC_UA_Client;

/*
Description:
    Robot Type - ABB IRB 120 with SMC Linear Axis (LEJSH63NZA 800)
        Absolute Joint Position:
            Joint L: [0.0, 0.8] [m] -> Implemented in the Mechanism_ID_3 folder.
            Joint 1: [+/- 165.0] [°]
            Joint 2: [+/- 110.0] [°]
            Joint 3: [-110.0, +70.0] [°]
            Joint 4: [+/- 160.0] [°]
            Joint 5: [+/- 120.0] [°]
            Joint 6: [+/- 400.0] [°]
            Joint 7: It doesn't exist. External axis connected in the IRC5 control unit.
*/

public class ABB_IRB_120 : MonoBehaviour
{
    /*
    Description:
        Structures, enumerations, etc.
    */
    public static class G_ABB_IRB_120_Str
    {
        public static float[] Q_target = new float[7];
        public static float[] Q_actual = new float[7];
        public static bool[] In_Position = new bool[6];
    }

    public enum ABB_IRB_120_STATE_Enum
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
        Private variables.
    */
    private readonly float[] Q_home = new float[7] {90.0f, 0.0f, 0.0f, 0.0f, 90.0f, 0.0f, 0.0f};
    private readonly float[,] Q_limit = new float[7,2] {{-165.0f, 165.0f}, {-110.0f, 110.0f}, {-110.0f, 70.0f},
                                                        {-160.0f, 160.0f}, {-120.0f, 120.0f}, {-400.0f, 400.0f}, {0.0f, 0.0f}};
    private ABB_IRB_120_STATE_Enum state_id;
    private int t_index;
    
    /*
    Description:
        Start called before the first frame update.
    */
    void Start()
    {
        // Set the actual position and the target position to the home position.
        var i = 0;
        foreach(float Q_home_i in Q_home){
            G_ABB_IRB_120_Str.Q_target[i] = Q_home_i;
            G_ABB_IRB_120_Str.Q_actual[i] = Q_home_i;
            i++;
        }
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        /*
        Description:
            The state machine used to control the industrial robot ABB IRB 120.
        */
        switch(state_id){
            case ABB_IRB_120_STATE_Enum.INIT:
            {
                OPC_UA_Client.G_OPC_UA_Client_IRB_120_Data_Str.Active = true;

                if(OPC_UA_Client.G_OPC_UA_Client_Str.Is_Connected == true && G_OPC_UA_Client_General_Data_Str.Simulation_Enabled == true){
                    state_id = ABB_IRB_120_STATE_Enum.WAIT_COMMAND;
                }
            }
            break;

            case ABB_IRB_120_STATE_Enum.WAIT_COMMAND:
            {
                OPC_UA_Client.G_OPC_UA_Client_IRB_120_Data_Str.Move_Active = false;

                if(OPC_UA_Client.G_OPC_UA_Client_IRB_120_Data_Str.Home == true){
                    state_id = ABB_IRB_120_STATE_Enum.HOME_INIT;
                }

                if(OPC_UA_Client.G_OPC_UA_Client_IRB_120_Data_Str.Start == true){
                    state_id = ABB_IRB_120_STATE_Enum.MOVE_CHECK;
                }
            }
            break;

            case ABB_IRB_120_STATE_Enum.HOME_INIT:
            {
                // Set the target position of the robot to the home position.
                //  Note:
                //      Check that the desired absolute joint positions are not out of limit.
                var j = 0;
                foreach(float Q_home_j in Q_home){
                    G_ABB_IRB_120_Str.Q_target[j] = Mathf.Clamp(Q_home_j, Q_limit[j, 0], Q_limit[j, 1]);
                    j++;
                }

                state_id = ABB_IRB_120_STATE_Enum.HOME_PERFORM;
            }
            break;

            case ABB_IRB_120_STATE_Enum.HOME_PERFORM:
            {
                OPC_UA_Client.G_OPC_UA_Client_IRB_120_Data_Str.Move_Active = true;

                bool in_position_all = true;
                foreach (bool in_position_i in G_ABB_IRB_120_Str.In_Position)
                {
                    if(in_position_i == false){
                        in_position_all = false;
                        break;
                    }
                }

                if(in_position_all == true){
                    state_id = ABB_IRB_120_STATE_Enum.WAIT_COMMAND;
                }
            }
            break;

            case ABB_IRB_120_STATE_Enum.MOVE_CHECK:
            {
                if(t_index == OPC_UA_Client.G_OPC_UA_Client_IRB_120_Data_Str.Trajectory_Length){
                    t_index = 0;
                }

                state_id = ABB_IRB_120_STATE_Enum.MOVE_INIT;
            }
            break;

            case ABB_IRB_120_STATE_Enum.MOVE_INIT:
            {
                // Set the target position of the robot to the trajectory position.
                //  Note:
                //      Check that the desired absolute joint positions are not out of limit.
                var j = 0;
                foreach(float Q_target_j in Enumerable.Range(0, OPC_UA_Client.G_OPC_UA_Client_IRB_120_Data_Str.Trajectory.GetLength(1))
                                                      .Select(i => OPC_UA_Client.G_OPC_UA_Client_IRB_120_Data_Str.Trajectory[t_index, i]).ToArray()){
                    G_ABB_IRB_120_Str.Q_target[j] = Mathf.Clamp(Q_target_j, Q_limit[j, 0], Q_limit[j, 1]);
                    j++;
                }

                state_id = ABB_IRB_120_STATE_Enum.MOVE_PERFORM;
            }
            break;

            case ABB_IRB_120_STATE_Enum.MOVE_PERFORM:
            {
                OPC_UA_Client.G_OPC_UA_Client_IRB_120_Data_Str.Move_Active = true;
                
                if(OPC_UA_Client.G_OPC_UA_Client_IRB_120_Data_Str.Stop == true){
                    t_index = 0;
                    state_id = ABB_IRB_120_STATE_Enum.WAIT_COMMAND;
                }else{
                    bool in_position_all = true;
                    foreach (bool in_position_i in G_ABB_IRB_120_Str.In_Position)
                    {
                        if(in_position_i == false){
                            in_position_all = false;
                            break;
                        }
                    }

                    if(in_position_all == true){
                        t_index++;
                        state_id = ABB_IRB_120_STATE_Enum.MOVE_CHECK;
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
