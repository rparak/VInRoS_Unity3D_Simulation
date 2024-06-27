// System
using System;
using System.Text;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

using static SMC_LEFB25_14000;

public class Mech_ID_1_Prismatic_Joint_Control : MonoBehaviour
{
    /*
    Description:
        Public variables.
    */
    public const float conversion_value = 0.00001f;
    // Index of the global variable used to control a specific joint.
    public int index;

    /*
    Description:
        Private variables.
    */
    // The initial speed to move the current value towards the target value.
    private float v = 0.0f;
    // Approximate time for the current value to reach the target value.
    private const float t_smooth = 1.0f;
    // The tolerance of the Euclidean distance between the target value.
    // and the current position
    private const float tolerance = 1.0f;
    // Initial joint position in the form of a 3D vector.
    private Vector3 Q_0 = new Vector3(0.0f, 0.0f, 0.0f);

    /*
    Description:
        Start called before the first frame update.
    */
    void Start()
    {
        Q_0 = transform.localPosition; SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.In_Position[index] = false;
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        // Smooth movement of the current mechanism position to the target position.
        if(Mathf.Abs(SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.Q_target[index] - SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.Q_actual[index]) <= tolerance){
            v = 0.0f;
            SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.In_Position[index] = true;
        }else{
            SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.Q_actual[index] = Mathf.SmoothDamp(SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.Q_actual[index], 
                                                                                       SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.Q_target[index], 
                                                                                       ref v, t_smooth, Mathf.Infinity, Time.deltaTime);
            SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.In_Position[index] = false;
        }

        transform.localPosition = new Vector3(Q_0[0], Q_0[1] + SMC_LEFB25_14000.G_SMC_LEFB25_14000_Str.Q_actual[index]*conversion_value, Q_0[2]);
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
