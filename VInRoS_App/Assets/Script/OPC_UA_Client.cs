// System
using System;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Linq;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;
// OPC UA
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

using static User_Interface;
using static SMC_LEFB25_14000;

/*
Description:
    A simple demonstration of node identification within B&R Automation.
        node_id = "ns=6;s=::Task:Var"

        Note:
            <ns>   = All variables use the number 6.
            <Task> = The name of the program (task).
                        Note:
                            If the variable is global, the task name is AsGlobalPV.
            <Var>  = The name of the variable to be processed.
*/

public class OPC_UA_Client : MonoBehaviour
{
    /*
    Description:
        Structures, enumerations, etc.
    */
    public enum OPC_UA_Client_STATE_Enum
    {
        DISCONNECTED = 0,
        CONNECTED = 10
    }

    public static class G_OPC_UA_Client_Str
    {
        public static bool Is_Connected;

        /*
        Description:
            Client settings.
        */
        public static string Ip_Address;
        // OPC UA port number for communication.
        public const ushort port_number = 4840;
        // Communication speed in milliseconds.
        public static int time_step;
    }

    public static class G_OPC_UA_Client_General_Data_Str
    {
        /*
        Description:
            Variables used to read data from the client.
        */
        public static NodeId Simulation_Enabled_Node = "ns=6;s=::AsGlobalPV:SIMULATION_ENABLE";
        public static bool Simulation_Enabled;
    }


    public static class G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str
    {
        /*
        Description:
            Variables used to read data from the client.

            Note:
                Use node as the NodeId data type.

            The process used to read the data requires a variable that corresponds 
            to the data type used in the OPC UA server.
        */
        public static NodeId[] Start_Node = new NodeId[2]{"ns=6;s=::AsGlobalPV:Global_VInRoS_Str.Mech_Id_1.Command.Start",
                                                          "ns=6;s=::AsGlobalPV:Global_VInRoS_Str.Mech_Id_2.Command.Start"};
        public static bool[] Start = new bool[2];
        public static NodeId[] Stop_Node = new NodeId[2]{"ns=6;s=::AsGlobalPV:Global_VInRoS_Str.Mech_Id_1.Command.Stop",
                                                         "ns=6;s=::AsGlobalPV:Global_VInRoS_Str.Mech_Id_2.Command.Stop"};
        public static bool[] Stop = new bool[2];
        public static NodeId[] Home_Node = new NodeId[2]{"ns=6;s=::AsGlobalPV:Global_VInRoS_Str.Mech_Id_1.Command.Home",
                                                         "ns=6;s=::AsGlobalPV:Global_VInRoS_Str.Mech_Id_2.Command.Home"};
        public static bool[] Home = new bool[2];
        public static NodeId[] Trajectory_Node = new NodeId[2]{"ns=6;s=::T_MECH_1:Trajectory_Str.Targets.Position",
                                                               "ns=6;s=::T_MECH_2:Trajectory_Str.Targets.Position"};
        public static float[,] Trajectory = new float[100,2];
        public static NodeId[] Trajectory_Length_Node = new NodeId[2]{"ns=6;s=::T_MECH_1:Trajectory_Str.Length",
                                                                      "ns=6;s=::T_MECH_2:Trajectory_Str.Length"};
        public static byte[] Trajectory_Length = new byte[2];

        /*
        Description:
            Variables used to write data to the client.

            Note:
                Use node as a string data type.
        */
        public static string[] Active_Node = new string[2]{"ns=6;s=::AsGlobalPV:Global_VInRoS_Str.Mech_Id_1.Info.Active",
                                                           "ns=6;s=::AsGlobalPV:Global_VInRoS_Str.Mech_Id_2.Info.Active"};
        public static bool[] Active = new bool[2];
        public static string[] Move_Active_Node = new string[2]{"ns=6;s=::AsGlobalPV:Global_VInRoS_Str.Mech_Id_1.Info.Move_Active",
                                                                "ns=6;s=::AsGlobalPV:Global_VInRoS_Str.Mech_Id_2.Info.Move_Active"};
        public static bool[] Move_Active = new bool[2];
        public static string[] Q_actual_Node = new string[2]{"ns=6;s=::AsGlobalPV:Global_VInRoS_Str.Mech_Id_1.Position",
                                                             "ns=6;s=::AsGlobalPV:Global_VInRoS_Str.Mech_Id_2.Position"};
    }

    /*
    Description:
        Global variables.
    */
    private OPC_UA_Client_STATE_Enum state_id;

    // Initialization of classes that will read and write data via OPC UA communication.
    private OPC_UA_Client_Read_Data_Cls OPC_UA_Client_R_Cls = new OPC_UA_Client_Read_Data_Cls();
    private OPC_UA_Client_Write_Data_Cls OPC_UA_Client_W_Cls = new OPC_UA_Client_Write_Data_Cls();

    // Information about whether the thread is alive or not.
    public static bool opc_ua_client_r_is_alive = false; 
    public static bool opc_ua_client_w_is_alive = false;

    /*
    Description:
        Start called before the first frame update.
    */
    void Start()
    {
        // Set the initial parameters of the OPC UA client.
        G_OPC_UA_Client_Str.Ip_Address = "127.0.0.1";
        G_OPC_UA_Client_Str.time_step = 10;

        /*
        Description:
            Initialization of OPC UA nodes that will be used to read data.
        */

        /*
        Description:
            Initialization of OPC UA nodes that will be used to write data.
        */
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        switch(state_id){
            case OPC_UA_Client_STATE_Enum.DISCONNECTED:
            {
                G_OPC_UA_Client_Str.Ip_Address = User_Interface.G_UI_Str.Ip_Address;

                G_OPC_UA_Client_Str.Is_Connected = false;
                if(User_Interface.G_UI_Str.Connect == true){
                    OPC_UA_Client_R_Cls.Start(); OPC_UA_Client_W_Cls.Start();
                    state_id = OPC_UA_Client_STATE_Enum.CONNECTED;
                }
            }
            break;

            case OPC_UA_Client_STATE_Enum.CONNECTED:
            {
                G_OPC_UA_Client_Str.Is_Connected = true;
                if(User_Interface.G_UI_Str.Disconnect == true){
                    if(opc_ua_client_r_is_alive == true){
                        OPC_UA_Client_R_Cls.Stop();
                    }
                    if(opc_ua_client_w_is_alive == true){
                        OPC_UA_Client_W_Cls.Stop();
                    }

                    if(opc_ua_client_r_is_alive == false && opc_ua_client_w_is_alive == false){
                        state_id = OPC_UA_Client_STATE_Enum.DISCONNECTED;
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
            // Destroy the threads used to read and write data via OPC UA communication.
            OPC_UA_Client_R_Cls.Destroy(); OPC_UA_Client_W_Cls.Destroy();

            Destroy(this);
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }

    public static Session OPC_UA_Client_Create_Session(ApplicationConfiguration client_configuration, EndpointDescription client_end_point)
    {
        return Session.Create(client_configuration, new ConfiguredEndpoint(null, client_end_point, EndpointConfiguration.Create(client_configuration)), false, "", 10000, null, null).GetAwaiter().GetResult();
    }

    public static ApplicationConfiguration OPC_UA_Client_Configuration()
    {
        // Configuration OPCUA Client {W/R -> Data}
        var config = new ApplicationConfiguration()
        {
            // Initialization (Name, Uri, etc.)
            ApplicationName = "OPCUA_AS", // OPCUA AS (Automation Studio B&R)
            ApplicationUri = Utils.Format(@"urn:{0}:OPCUA_AS", System.Net.Dns.GetHostName()),
            // Type -> Client
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                // Security Configuration - Certificate
                ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = Utils.Format(@"CN={0}, DC={1}", "OPCUA_AS", System.Net.Dns.GetHostName()) },
                TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" },
                AutoAcceptUntrustedCertificates = true,
                AddAppCertToTrustedStore = true
            },
            TransportConfigurations = new TransportConfigurationCollection(),
            TransportQuotas = new TransportQuotas { OperationTimeout = 10000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 50000 },
            TraceConfiguration = new TraceConfiguration()
        };
        config.Validate(ApplicationType.Client).GetAwaiter().GetResult();
        if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
        {
            config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted); };
        }

        return config;
    }

    class OPC_UA_Client_Read_Data_Cls
    {
        // Initialization of the class variables.
        private Thread ctrl_thread = null;
        private bool exit_thread = false;
        // OPC UA client app. configuration class.
        ApplicationConfiguration app_configuration = new ApplicationConfiguration();

        public void Cls_Core_Thread()
        {
            try
            {
                // OPC UA Client configuration.
                app_configuration = OPC_UA_Client_Configuration();
                // Establishing communication.
                EndpointDescription end_point = CoreClientUtils.SelectEndpoint("opc.tcp://" + G_OPC_UA_Client_Str.Ip_Address + ":" + G_OPC_UA_Client_Str.port_number, useSecurity: false);
                // Create session.
                Session client_session = OPC_UA_Client_Create_Session(app_configuration, end_point);

                // Initialization timer.
                var t = new Stopwatch();

                while (exit_thread == false)
                {
                    // t_{0}: Timer start.
                    t.Start();

                    /*
                    Description:
                        Block used to read data from the server via the OPC UA communication.
                    */

                    // General data obtained from the server.
                    G_OPC_UA_Client_General_Data_Str.Simulation_Enabled = bool.Parse(client_session.ReadValue(G_OPC_UA_Client_General_Data_Str.Simulation_Enabled_Node).ToString());

                    // Data to control the SMC LEFB25UNZS 14000C mechanism for both axes ID 1 and ID 2 obtained from the server.
                    G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Start[0] = bool.Parse(client_session.ReadValue(G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Start_Node[0]).ToString());
                    G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Start[1] = bool.Parse(client_session.ReadValue(G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Start_Node[1]).ToString());
                    G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Stop[0]  = bool.Parse(client_session.ReadValue(G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Stop_Node[0]).ToString());
                    G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Stop[1]  = bool.Parse(client_session.ReadValue(G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Stop_Node[1]).ToString());
                    G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Home[0]  = bool.Parse(client_session.ReadValue(G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Home_Node[0]).ToString());
                    G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Home[1]  = bool.Parse(client_session.ReadValue(G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Home_Node[1]).ToString());
                    G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Trajectory_Length[0] = byte.Parse(client_session.ReadValue(G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Trajectory_Length_Node[0]).ToString());
                    G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Trajectory_Length[1] = byte.Parse(client_session.ReadValue(G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Trajectory_Length_Node[1]).ToString());
                    float[] t_mech_id_1 = Array.ConvertAll(client_session.ReadValue(G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Trajectory_Node[0]).ToString().Split(new[] { '{', '}', '|', }, 
                                                                                    StringSplitOptions.RemoveEmptyEntries), float.Parse);
                    float[] t_mech_id_2 = Array.ConvertAll(client_session.ReadValue(G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Trajectory_Node[1]).ToString().Split(new[] { '{', '}', '|', }, 
                                                                                    StringSplitOptions.RemoveEmptyEntries), float.Parse);
                    Enumerable.Range(0, t_mech_id_1.Length).ToList().ForEach(i => { G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Trajectory[i, 0] = t_mech_id_1[i]; 
                                                                                    G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Trajectory[i, 1] = t_mech_id_2[i]; });

                    // t_{1}: Timer stop.
                    t.Stop();

                    // Recalculate the time: t = t_{1} - t_{0} -> Elapsed Time in milliseconds.
                    if (t.ElapsedMilliseconds < G_OPC_UA_Client_Str.time_step)
                    {
                        Thread.Sleep(G_OPC_UA_Client_Str.time_step - (int)t.ElapsedMilliseconds);
                    }

                    // Reset (Restart) timer.
                    t.Restart();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Communication Problem: {0}", e);
            }
        }

        public void Start()
        {
            exit_thread = false;
            // Start a thread to read data from the OPC UA server.
            ctrl_thread = new Thread(new ThreadStart(Cls_Core_Thread));
            ctrl_thread.IsBackground = true;
            ctrl_thread.Start();

            // The thread is active.
            opc_ua_client_r_is_alive = true;
        }

        public void Stop()
        {
            exit_thread = true;
            ctrl_thread.Abort();
            // Stop a thread.
            Thread.Sleep(100);

            // The thread is inactive.
            opc_ua_client_r_is_alive = false;
        }

        public void Destroy()
        {
            // Stop a thread (OPC UA communication).
            Stop();
            Thread.Sleep(100);
        }
    }

    class OPC_UA_Client_Write_Data_Cls
    {
        // Initialization of the class variables.
        private Thread ctrl_thread = null;
        private bool exit_thread = false;
        // OPC UA client app. configuration class.
        ApplicationConfiguration app_configuration = new ApplicationConfiguration();

        public void Cls_Core_Thread()
        {
            try
            {
                // OPC UA Client configuration.
                app_configuration = OPC_UA_Client_Configuration();
                // Establishing communication
                EndpointDescription end_point = CoreClientUtils.SelectEndpoint("opc.tcp://" + G_OPC_UA_Client_Str.Ip_Address + ":" + G_OPC_UA_Client_Str.port_number, useSecurity: false);
                // Create session.
                Session client_session = OPC_UA_Client_Create_Session(app_configuration, end_point);

                // Initialization timer
                var t = new Stopwatch();

                while (exit_thread == false)
                {
                    // t_{0}: Timer start.
                    t.Start();

                    /*
                    Description:
                        Block used to write data to the server via the OPC UA communication.
                    */
                    if(G_OPC_UA_Client_General_Data_Str.Simulation_Enabled == true){
                        // Information data about the SMC LEFB25UNZS 14000C mechanism for both axes ID 1 and ID 2 transmitted to the server.
                        OPC_UA_Client_Write_Value(client_session, G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Active_Node[0],
                                                  G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Active[0].ToString());
                        OPC_UA_Client_Write_Value(client_session, G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Active_Node[1],
                                                  G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Active[1].ToString());
                        OPC_UA_Client_Write_Value(client_session, G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Move_Active_Node[0],
                                                  G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Move_Active[0].ToString());
                        OPC_UA_Client_Write_Value(client_session, G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Move_Active_Node[1],
                                                  G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Move_Active[1].ToString());
                        // The actual position of the SMC LEFB25UNZS 14000C mechanism for both axes ID 1 and ID 2 transmitted to the server.
                        OPC_UA_Client_Write_Value(client_session, G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Q_actual_Node[0], 
                                                  SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.Q_actual[0].ToString());
                        OPC_UA_Client_Write_Value(client_session, G_OPC_UA_Client_SMC_LEFB25_14000_Data_Str.Q_actual_Node[1], 
                                                  SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.Q_actual[1].ToString());
                    }

                    // t_{1}: Timer stop.
                    t.Stop();

                    // Recalculate the time: t = t_{1} - t_{0} -> Elapsed Time in milliseconds
                    if (t.ElapsedMilliseconds < G_OPC_UA_Client_Str.time_step)
                    {
                        Thread.Sleep(G_OPC_UA_Client_Str.time_step - (int)t.ElapsedMilliseconds);
                    }

                    // Reset (Restart) timer.
                    t.Restart();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Communication Problem: {0}", e);
            }
        }

        bool OPC_UA_Client_Write_Value(Session client_session, string node_id, string value_write)
        {
            NodeId init_node = NodeId.Parse(node_id);
            try
            {
                Node node = client_session.NodeCache.Find(init_node) as Node;
                DataValue init_data_value = client_session.ReadValue(node.NodeId);

                // Preparation of the data to be written.
                WriteValue value = new WriteValue()
                {
                    NodeId = init_node,
                    AttributeId = Attributes.Value,
                    Value = new DataValue(new Variant(Convert.ChangeType(value_write, init_data_value.Value.GetType()))),
                };

                WriteValueCollection init_write = new WriteValueCollection();
                init_write.Add(value);

                StatusCodeCollection results = null; 
                DiagnosticInfoCollection diagnostic_info = null;

                // Write data.
                client_session.Write(null, init_write, out results, out diagnostic_info);

                // Check the status of the results.
                return (results[0] == StatusCodes.Good) ? true : false;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }

        public void Start()
        {
            exit_thread = false;
            // Start a thread to write data to the OPC UA server.
            ctrl_thread = new Thread(new ThreadStart(Cls_Core_Thread));
            ctrl_thread.IsBackground = true;
            ctrl_thread.Start();

            // The thread is active.
            opc_ua_client_w_is_alive = true;
        }

        public void Stop()
        {
            exit_thread = true;
            ctrl_thread.Abort();
            // Stop a thread.
            Thread.Sleep(100);

            // The thread is inactive.
            opc_ua_client_w_is_alive = false;
        }

        public void Destroy()
        {
            // Stop a thread (OPC UA communication).
            Stop();
            Thread.Sleep(100);
        }
    }
}
