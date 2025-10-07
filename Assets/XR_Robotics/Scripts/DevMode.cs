using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevMode : MonoBehaviour
{
    [SerializeField]
    private bool devMode = false;

    [SerializeField]
    private bool controlDigitalTwin = false;

    public GameObject robot;

    void Update()
    {
        if (devMode && controlDigitalTwin)
        {
            RobotController robotController = robot.GetComponent<RobotController>();
            for (int i = 0; i < robotController.joints.Length; i++)
            {
                Debug.Log("Here");
                float inputVal = Input.GetAxis(robotController.joints[i].inputAxis);
                if (Mathf.Abs(inputVal) > 0)
                {
                    RotationDirection direction = GetRotationDirection(inputVal);
                    robotController.RotateJoint(i, direction);
                    return;
                }
            }
            robotController.StopAllJointRotations();
        }
    }

    static RotationDirection GetRotationDirection(float inputVal)
    {
        if (inputVal > 0)
        {
            return RotationDirection.Positive;
        }
        else if (inputVal < 0)
        {
            return RotationDirection.Negative;
        }
        else
        {
            return RotationDirection.None;
        }
    }
}
