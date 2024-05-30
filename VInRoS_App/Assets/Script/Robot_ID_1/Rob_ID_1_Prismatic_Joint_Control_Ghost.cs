// System
using System;
using System.Text;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

using static ABB_IRB_120_L_Ax;

public class Rob_ID_1_Prismatic_Joint_Control_Ghost : MonoBehaviour
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
    // Initial joint position in the form of a 3D vector.
    private Vector3 Q_0 = new Vector3(0.0f, 0.0f, 0.0f);

    /*
    Description:
        Start called before the first frame update.
    */
    void Start()
    {
        Q_0 = transform.localPosition;
    }

    /*
    Description:
        Update called once per frame.
    */
    void Update()
    {
        transform.localPosition = new Vector3(Q_0[0] + ABB_IRB_120_L_Ax.G_ABB_IRB_120_L_Ax_Str.Q_target[index]*conversion_value, Q_0[1], Q_0[2]);
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
