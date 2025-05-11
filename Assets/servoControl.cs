using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class servoControl : MonoBehaviour
{
    // Start is called before the first frame update
    public HingeJoint HingeJoint;
    public float angle;
    public float nowInputAngle;
    float lastInputAngle;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        JointMotor motor = HingeJoint.motor;
        nowInputAngle += HingeJoint.angle - lastInputAngle;
        if (lastInputAngle > 90 && HingeJoint.angle < -90)
        {
            nowInputAngle += 360;
        }
        if (lastInputAngle < -90 && HingeJoint.angle > 90)
        {
            nowInputAngle -= 360;
        }
        lastInputAngle = HingeJoint.angle;
        float speed = 100;
        float maxspeed = 100;

        motor.targetVelocity = Mathf.Clamp((angle - nowInputAngle) * speed, -maxspeed, maxspeed);
        motor.force = 1000f;
        HingeJoint.motor = motor;
    }
}
