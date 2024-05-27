using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float targetAngle; // The target angle you want to reach
    public float smoothTime = 0.3f; // The time over which the damping occurs
    public float maxSpeed = Mathf.Infinity; // The maximum speed of the angle change (optional)
    public float speed = 5.0f; // The speed of the angle change
    
    private float currentAngle; // The current angle
    private float currentVelocity; // The current velocity of the angle change

    void Start()
    {
        currentAngle = transform.eulerAngles.y; // Initialize the current angle to the object's y-axis rotation
    }

    void Update()
    {
        // Update the current angle smoothly towards the target angle
        currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref currentVelocity, smoothTime, maxSpeed, Time.deltaTime * speed);

        // float distanceToTarget = Vector3.Distance(joint.position, targetPosition);
        // Apply the smoothly damped angle to the transform's rotation
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentAngle, transform.eulerAngles.z);
    }
}
