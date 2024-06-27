// System
using System;
using System.Text;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

using static ABB_IRB_14000_R;

public class Rob_ID_2_R_Revolute_Joint_Control : MonoBehaviour
{
    /*
    Description:
        Public variables.
    */
    public const float conversion_value = -1.0f;
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
        Q_0 = transform.localEulerAngles; ABB_IRB_14000_R.G_ABB_IRB_14000_R_Str.In_Position[index] = false;
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        // Smooth movement of the current robot position to the target position.
        if(Mathf.Abs(ABB_IRB_14000_R.G_ABB_IRB_14000_R_Str.Q_target[index] - ABB_IRB_14000_R.G_ABB_IRB_14000_R_Str.Q_actual[index]) <= tolerance){
            v = 0.0f;
            ABB_IRB_14000_R.G_ABB_IRB_14000_R_Str.In_Position[index] = true;
        }else{
            ABB_IRB_14000_R.G_ABB_IRB_14000_R_Str.Q_actual[index] = Mathf.SmoothDamp(ABB_IRB_14000_R.G_ABB_IRB_14000_R_Str.Q_actual[index], 
                                                                                     ABB_IRB_14000_R.G_ABB_IRB_14000_R_Str.Q_target[index], 
                                                                                     ref v, t_smooth, Mathf.Infinity, Time.deltaTime);
            ABB_IRB_14000_R.G_ABB_IRB_14000_R_Str.In_Position[index] = false;
        }

        transform.localEulerAngles = new Vector3(Q_0[0], Q_0[1], Q_0[2] + ABB_IRB_14000_R.G_ABB_IRB_14000_R_Str.Q_actual[index]*conversion_value);
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
