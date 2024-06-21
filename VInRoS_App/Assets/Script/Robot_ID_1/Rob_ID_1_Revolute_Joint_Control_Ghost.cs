// System
using System;
using System.Text;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

using static ABB_IRB_120;

public class Rob_ID_1_Revolute_Joint_Control_Ghost : MonoBehaviour
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
    // Initial joint position in the form of a 3D vector.
    private Vector3 Q_0 = new Vector3(0.0f, 0.0f, 0.0f);

    /*
    Description:
        Start called before the first frame update.
    */
    void Start()
    {
        Q_0 = transform.localEulerAngles;
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        transform.localEulerAngles = new Vector3(Q_0[0], Q_0[1], Q_0[2] + ABB_IRB_120.G_ABB_IRB_120_Str.Q_target[index]*conversion_value);
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
