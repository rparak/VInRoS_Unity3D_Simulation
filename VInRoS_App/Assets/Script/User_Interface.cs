// System 
using System;
using System.Text;
// Unity
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
// TM 
using TMPro;

using static OPC_UA_Client;
using static SMC_LEFB25_14000;
using static SMC_LEJSH63NZA_800;
using static ABB_IRB_120;
using static ABB_IRB_14000_L;
using static ABB_IRB_14000_R;

public class User_Interface : MonoBehaviour
{
    /*
    Description:
        Structures, enumerations, etc.
    */
    public static class G_UI_Str
    {
        public static bool Connect, Disconnect;
        public static string Ip_Address;
    }

    /*
    Description:
        Public variables.
    */
    // TMP_InputField 
    public TMP_InputField Ip_Address_IF_Txt;
    // Image
    public Image Connection_Info_Img; public Image Graph_Panel_Img;
    // Toggle
    //  Id 0: Viewpoint, Id 1: Colliders, Id 2: Ghost, Id 3: Workspace
    public Toggle[] ABB_IRB_120_L_Ax_Toggle = new Toggle[4];
    public Toggle[] ABB_IRB_14000_Toggle = new Toggle[4];
    public Toggle[] SMC_LEFB25_14000_Toggle = new Toggle[3];
    // GameObject
    public GameObject Camera_Obj;
    public GameObject ABB_IRB_120_L_Ax_Obj; public GameObject ABB_IRB_14000_Obj;
    public GameObject SMC_LEFB25_14000_Obj;
    // TextMeshProUGUI
    public TextMeshProUGUI Connection_Info_Txt;
    public TextMeshProUGUI[] SMC_LEFB25_14000_Txt = new TextMeshProUGUI[2];
    public TextMeshProUGUI[] ABB_IRB_120_L_Ax_Txt = new TextMeshProUGUI[7];
    public TextMeshProUGUI[] ABB_IRB_14000_L_Txt = new TextMeshProUGUI[7];
    public TextMeshProUGUI[] ABB_IRB_14000_R_Txt = new TextMeshProUGUI[7];

    /*
    Description:
        Private variables.
    */
    // Bool
    private bool graph_panel_flag = false;
    // String
    private string[] q_names_rob_1 = new string[6] { "Joint_1", "Joint_2", "Joint_3", "Joint_4", "Joint_5", "Joint_6" };
    private string[] q_names_rob_2 = new string[7] { "Joint_1", "Joint_2", "Joint_3", "Joint_4", "Joint_5", "Joint_6", "Joint_7" };

    /*
    Description:
        Start called before the first frame update.
    */
    void Start()
    {
        // Connection information:
        //      Connected: Green Color <135, 255, 0, 50>
        //      Disconnected: Red Color <255, 0, 48, 50>
        Connection_Info_Img.GetComponent<Image>().color = new Color32(255, 0, 48, 50);
        // Connection information: Connected / Disconnected.
        Connection_Info_Txt.text = "Disconnected";
        // OPC UA Server IP Address.
        Ip_Address_IF_Txt.text = "127.0.0.1";

        // Graph panel initialization.
        Graph_Panel_Img.transform.localPosition = new Vector3(-(422.5f + 1000.0f), 0.0f, 0.0f);

        // Main camera initialization.
        Camera_Obj.transform.localPosition = new Vector3(-2.85f, 1.6f, -1.25f);
        Camera_Obj.transform.localEulerAngles = new Vector3(15f, 80f, 0f);

        // Turn off user interface Toggles.
        for(int i = 0; i < ABB_IRB_120_L_Ax_Toggle.Length; i++) 
        {
            ABB_IRB_120_L_Ax_Toggle[i].isOn = false; ABB_IRB_14000_Toggle[i].isOn = false;
            if(i != ABB_IRB_120_L_Ax_Toggle.Length - 1){
                SMC_LEFB25_14000_Toggle[i].isOn = false;
            }
        }

        // Data Information about the robot/mechanism's joint positions within a specific panel.
        SMC_LEFB25_14000_Txt[0].text = SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.Q_actual[0].ToString(); 
        SMC_LEFB25_14000_Txt[1].text = SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.Q_actual[1].ToString();
        ABB_IRB_120_L_Ax_Txt[0].text = SMC_LEJSH63NZA_800.G_SMC_LEJSH63NZA_800_Str.Q_actual.ToString(); 
        for(int i = 0; i < ABB_IRB_14000_L_Txt.Length; i++) 
        {
            if(i > 0){
                ABB_IRB_120_L_Ax_Txt[i].text = ABB_IRB_120.G_ABB_IRB_120_Str.Q_actual[i - 1].ToString(); 
            }
            ABB_IRB_14000_L_Txt[i].text = ABB_IRB_14000_L.G_ABB_IRB_14000_L_Str.Q_actual[i].ToString(); 
            ABB_IRB_14000_R_Txt[i].text = ABB_IRB_14000_L.G_ABB_IRB_14000_L_Str.Q_actual[i].ToString(); 
        }
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        G_UI_Str.Ip_Address = Ip_Address_IF_Txt.text;

        // Connection Information:
        //  If the connection to the OPC UA server is successfully established, change the connection information.
        if (OPC_UA_Client.G_OPC_UA_Client_Str.Is_Connected == true)
        {
            Connection_Info_Img.GetComponent<Image>().color = new Color32(135, 255, 0, 50);
            Connection_Info_Txt.text = "Connected";
        }
        else
        {
            Connection_Info_Img.GetComponent<Image>().color = new Color32(255, 0, 48, 50);
            Connection_Info_Txt.text = "Disconnected";
        }

        // Mechanism: SMC LEFB25UNZS 14000C.
        Get_Child_Game_Object(SMC_LEFB25_14000_Obj.transform, "Viewpoint_EE_" + SMC_LEFB25_14000_Obj.name + "_ID_001").SetActive(SMC_LEFB25_14000_Toggle[0].isOn);
        Get_Child_Game_Object(SMC_LEFB25_14000_Obj.transform, "Viewpoint_EE_" + SMC_LEFB25_14000_Obj.name + "_ID_002").SetActive(SMC_LEFB25_14000_Toggle[0].isOn);
        string[] mechanism_object_types = {"Collider", "Ghost"}; int[] mechanism_id = {1, 2};
        var i = 1;
        foreach(string mech_obj_type in mechanism_object_types){
            foreach(int mech_id in mechanism_id){
                if(mech_obj_type == "Collider"){
                    Get_Child_Game_Object(SMC_LEFB25_14000_Obj.transform, "Base_" + mech_obj_type + "_" + SMC_LEFB25_14000_Obj.name + "_ID_00" + mech_id).SetActive(SMC_LEFB25_14000_Toggle[i].isOn);
                }
                Get_Child_Game_Object(SMC_LEFB25_14000_Obj.transform, "Joint_L_" + mech_obj_type + "_" + SMC_LEFB25_14000_Obj.name + "_ID_00" + mech_id).SetActive(SMC_LEFB25_14000_Toggle[i].isOn);
                Get_Child_Game_Object(SMC_LEFB25_14000_Obj.transform, "Shuttle_" + mech_obj_type + "_" + SMC_LEFB25_14000_Obj.name + "_ID_00" + mech_id).SetActive(SMC_LEFB25_14000_Toggle[i].isOn);
            }
            i++;
        }
 
        // Industrial Robotic Arm: ABB IRB 120 with SMC Linear Axis (LEJSH63NZA 800).
        Get_Child_Game_Object(ABB_IRB_120_L_Ax_Obj.transform, "Viewpoint_EE_" + ABB_IRB_120_L_Ax_Obj.name + "_ID_001").SetActive(ABB_IRB_120_L_Ax_Toggle[0].isOn);
        Get_Child_Game_Object(ABB_IRB_120_L_Ax_Obj.transform, "Base_Collider_" + ABB_IRB_120_L_Ax_Obj.name + "_ID_001").SetActive(ABB_IRB_120_L_Ax_Toggle[1].isOn);
        Get_Child_Game_Object(ABB_IRB_120_L_Ax_Obj.transform, "Base_1_Collider_" + ABB_IRB_120_L_Ax_Obj.name + "_ID_001").SetActive(ABB_IRB_120_L_Ax_Toggle[1].isOn);
        Get_Child_Game_Object(ABB_IRB_120_L_Ax_Obj.transform, "Joint_L_Ghost_" + ABB_IRB_120_L_Ax_Obj.name + "_ID_001").SetActive(ABB_IRB_120_L_Ax_Toggle[2].isOn);
        Get_Child_Game_Object(ABB_IRB_120_L_Ax_Obj.transform, "ABB_IRB_120_Ghost_ID_002").SetActive(ABB_IRB_120_L_Ax_Toggle[2].isOn);
        foreach(string q_name in q_names_rob_1) 
        {
            Get_Child_Game_Object(ABB_IRB_120_L_Ax_Obj.transform, q_name + "_Collider_" + ABB_IRB_120_L_Ax_Obj.name + "_ID_001").SetActive(ABB_IRB_120_L_Ax_Toggle[1].isOn);
            Get_Child_Game_Object(ABB_IRB_120_L_Ax_Obj.transform, q_name + "_Ghost_" + ABB_IRB_120_L_Ax_Obj.name + "_ID_001").SetActive(ABB_IRB_120_L_Ax_Toggle[2].isOn);
        }
        Get_Child_Game_Object(ABB_IRB_120_L_Ax_Obj.transform, "Workspace_" + ABB_IRB_120_L_Ax_Obj.name + "_ID_001").SetActive(ABB_IRB_120_L_Ax_Toggle[3].isOn);

        // Collaborative Robotic Arm: ABB IRB 14000.
        Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, "Viewpoint_EE_" + ABB_IRB_14000_Obj.name + "_R_ID_001").SetActive(ABB_IRB_14000_Toggle[0].isOn);
        Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, "Viewpoint_EE_" + ABB_IRB_14000_Obj.name + "_L_ID_001").SetActive(ABB_IRB_14000_Toggle[0].isOn);
        Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, "Base_Collider_ID_1_" + ABB_IRB_14000_Obj.name + "_ID_001").SetActive(ABB_IRB_14000_Toggle[1].isOn);
        Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, "Base_Collider_ID_2_" + ABB_IRB_14000_Obj.name + "_ID_001").SetActive(ABB_IRB_14000_Toggle[1].isOn);
        Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, "Base_Collider_ID_3_" + ABB_IRB_14000_Obj.name + "_ID_001").SetActive(ABB_IRB_14000_Toggle[1].isOn);
        Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, "Base_Collider_ID_4_" + ABB_IRB_14000_Obj.name + "_ID_001").SetActive(ABB_IRB_14000_Toggle[1].isOn);
        Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, "Base_Collider_ID_5_" + ABB_IRB_14000_Obj.name + "_ID_001").SetActive(ABB_IRB_14000_Toggle[1].isOn);
        Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, "Base_Collider_" + ABB_IRB_14000_Obj.name + "_R_ID_001").SetActive(ABB_IRB_14000_Toggle[1].isOn);
        Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, "Base_Collider_" + ABB_IRB_14000_Obj.name + "_L_ID_001").SetActive(ABB_IRB_14000_Toggle[1].isOn);
        Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, ABB_IRB_14000_Obj.name + "_R_Ghost_ID_001").SetActive(ABB_IRB_14000_Toggle[2].isOn);
        Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, ABB_IRB_14000_Obj.name + "_L_Ghost_ID_001").SetActive(ABB_IRB_14000_Toggle[2].isOn);
        foreach(string q_name in q_names_rob_2){
            Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, q_name + "_Collider_" + ABB_IRB_14000_Obj.name + "_R_ID_001").SetActive(ABB_IRB_14000_Toggle[1].isOn);
            Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, q_name + "_Collider_" + ABB_IRB_14000_Obj.name + "_L_ID_001").SetActive(ABB_IRB_14000_Toggle[1].isOn); 
            Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, q_name + "_Ghost_" + ABB_IRB_14000_Obj.name + "_R_ID_001").SetActive(ABB_IRB_14000_Toggle[2].isOn);
            Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, q_name + "_Ghost_" + ABB_IRB_14000_Obj.name + "_L_ID_001").SetActive(ABB_IRB_14000_Toggle[2].isOn); 
        }
        Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, "Workspace_" + ABB_IRB_14000_Obj.name + "_R_ID_001").SetActive(ABB_IRB_14000_Toggle[3].isOn);
        Get_Child_Game_Object(ABB_IRB_14000_Obj.transform, "Workspace_" + ABB_IRB_14000_Obj.name + "_L_ID_001").SetActive(ABB_IRB_14000_Toggle[3].isOn);

        // Data Information about the robot/mechanism's joint positions.
        SMC_LEFB25_14000_Txt[0].text = Math.Round(SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.Q_actual[0], 2).ToString(); 
        SMC_LEFB25_14000_Txt[1].text = Math.Round(SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.Q_actual[1], 2).ToString();
        ABB_IRB_120_L_Ax_Txt[0].text = Math.Round(SMC_LEJSH63NZA_800.G_SMC_LEJSH63NZA_800_Str.Q_actual, 2).ToString(); 
        for(int j = 0; j < ABB_IRB_14000_L_Txt.Length; j++) 
        {
            if(j > 0){
                ABB_IRB_120_L_Ax_Txt[j].text = Math.Round(ABB_IRB_120.G_ABB_IRB_120_Str.Q_actual[j - 1], 2).ToString(); 
            }
            ABB_IRB_14000_L_Txt[j].text = Math.Round(ABB_IRB_14000_L.G_ABB_IRB_14000_L_Str.Q_actual[j], 2).ToString(); 
            ABB_IRB_14000_R_Txt[j].text = Math.Round(ABB_IRB_14000_L.G_ABB_IRB_14000_L_Str.Q_actual[j], 2).ToString(); 
        }
    }

    /*
    Description:
        Help functions for the user interface control.
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

    public GameObject Get_Child_Game_Object(Transform parent, string child_name)
    {
        foreach(Transform child in parent)
        {
            if(child.name == child_name)
            {
                return child.gameObject;
            }
            GameObject found = Get_Child_Game_Object(child, child_name);
            if(found != null)
            {
                return found;
            }
        }
        return null;
    }

    public void TaskOnClick_Camera_Left_Btn()
    {
        Camera_Obj.transform.localPosition = new Vector3(-3.05f, 1.6f, 0.8f);
        Camera_Obj.transform.localEulerAngles = new Vector3(15f, 110f, 0f);
    }

    public void TaskOnClick_Camera_Right_Btn()
    {
        Camera_Obj.transform.localPosition = new Vector3(-2.85f, 1.6f, -1.25f);
        Camera_Obj.transform.localEulerAngles = new Vector3(15f, 80f, 0f);
    }

    public void TaskOnClick_Camera_Home_Btn()
    {
        Camera_Obj.transform.localPosition = new Vector3(-3.15f, 1.6f, -0.6f);
        Camera_Obj.transform.localEulerAngles = new Vector3(15f, 90f, 0f);
    }

    public void TaskOnClick_Connect_Btn()
    {
        G_UI_Str.Disconnect = false; G_UI_Str.Connect = true;
    }

    public void TaskOnClick_Disconnect_Btn()
    {
        G_UI_Str.Connect = false; G_UI_Str.Disconnect = true;
    }

    public void TaskOnClick_Fly_Btn()
    {
        if(graph_panel_flag == false){
            Graph_Panel_Img.transform.localPosition = new Vector3(-422.5f, 0.0f, 0.0f);
            graph_panel_flag = true;
        }else{
            Graph_Panel_Img.transform.localPosition = new Vector3(-(422.5f + 1000.0f), 0.0f, 0.0f);
            graph_panel_flag = false;
        }
    }
}
