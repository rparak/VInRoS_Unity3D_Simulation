// System
using System;
using System.Threading;
using System.Collections;
using System.Diagnostics;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;
// OPC UA
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

using static User_Interface;

public class OPC_UA_Client : MonoBehaviour
{
    /*
    Description:
        Global variables.
    */
    public static class G_OPC_UA_Client_Str
    {
        public static bool Is_Connected;
        // Client settings.
        //  Network IP Address.
        public static string Ip_Address;
        //  Network Port Number.
        public const ushort port_number = 4840;
        //  Communication speed in milliseconds.
        public static int time_step;
    }

    // Initialization of classes that will read and write data via OPC UA communication.
    private OPC_Ua_Client_Read_Data_Cls OPC_Ua_Client_R_Cls = new OPC_Ua_Client_Read_Data_Cls();
    private OPC_Ua_Client_Write_Data_Cls OPC_Ua_Client_W_Cls = new OPC_Ua_Client_Write_Data_Cls();

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
            // {Cls_Name}.Destroy();

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
        // Configuration OPCUa Client {W/R -> Data}
        var config = new ApplicationConfiguration()
        {
            // Initialization (Name, Uri, etc.)
            ApplicationName = "OPCUa_AS", // OPCUa AS (Automation Studio B&R)
            ApplicationUri = Utils.Format(@"urn:{0}:OPCUa_AS", System.Net.Dns.GetHostName()),
            // Type -> Client
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                // Security Configuration - Certificate
                ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = Utils.Format(@"CN={0}, DC={1}", "OPCUa_AS", System.Net.Dns.GetHostName()) },
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

    class OPC_Ua_Client_Read_Data_Cls
    {
        // Initialization of the class variables.
        private Thread opcua_thread = null;
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
            opcua_thread = new Thread(new ThreadStart(Cls_Core_Thread));
            opcua_thread.IsBackground = true;
            opcua_thread.Start();
        }
        public void Stop()
        {
            exit_thread = true;
            // Stop a thread.
            Thread.Sleep(100);
        }
        public void Destroy()
        {
            // Stop a thread (OPCUA communication).
            Stop();
            Thread.Sleep(100);
        }
    }

    class OPC_Ua_Client_Write_Data_Cls
    {
        // Initialization of the class variables.
        private Thread opcua_thread = null;
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
            opcua_thread = new Thread(new ThreadStart(Cls_Core_Thread));
            opcua_thread.IsBackground = true;
            opcua_thread.Start();
        }
        public void Stop()
        {
            exit_thread = true;
            // Stop a thread.
            Thread.Sleep(100);
        }
        public void Destroy()
        {
            // Stop a thread (OPC UA communication).
            Stop();
            Thread.Sleep(100);
        }
    }
}
