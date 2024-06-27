// System
using System;
using System.Text;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

using static SMC_LEJSH63NZA_800;

public class Mech_ID_3_Prismatic_Joint_Control : MonoBehaviour
{
    /*
    Description:
        Public variables.
    */
    public const float conversion_value = 0.00001f;

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
        Q_0 = transform.localPosition; SMC_LEJSH63NZA_800.G_SMC_LEJSH63NZA_800_Str.In_Position = false;
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        // Smooth movement of the current mechanism position to the target position.
        if(Mathf.Abs(SMC_LEJSH63NZA_800.G_SMC_LEJSH63NZA_800_Str.Q_target - SMC_LEJSH63NZA_800.G_SMC_LEJSH63NZA_800_Str.Q_actual) <= tolerance){
            v = 0.0f;
            SMC_LEJSH63NZA_800.G_SMC_LEJSH63NZA_800_Str.In_Position = true;
        }else{
            SMC_LEJSH63NZA_800.G_SMC_LEJSH63NZA_800_Str.Q_actual = Mathf.SmoothDamp(SMC_LEJSH63NZA_800.G_SMC_LEJSH63NZA_800_Str.Q_actual, 
                                                                                    SMC_LEJSH63NZA_800.G_SMC_LEJSH63NZA_800_Str.Q_target, 
                                                                                    ref v, t_smooth, Mathf.Infinity, Time.deltaTime);
            SMC_LEJSH63NZA_800.G_SMC_LEJSH63NZA_800_Str.In_Position = false;
        }

        transform.localPosition = new Vector3(Q_0[0] + SMC_LEJSH63NZA_800.G_SMC_LEJSH63NZA_800_Str.Q_actual*conversion_value, Q_0[1], Q_0[2]);
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
