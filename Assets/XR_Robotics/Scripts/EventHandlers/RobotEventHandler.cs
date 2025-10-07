using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class RobotEventHandler : MonoBehaviour
{
    [SerializeField]
    private ArticulationBody base_articulation;

    [SerializeField]
    private ArticulationBody lower_arm_articulation;

    [SerializeField]
    private ArticulationBody middle_arm_articulation;

    [SerializeField]
    private ArticulationBody upper_arm_articulation;

    [SerializeField]
    private ArticulationBody gripper_base_articulation;

    [SerializeField]
    private ArticulationBody gripper_link_r_articulation;

    [SerializeField]
    private ArticulationBody gripper_link_l_articulation;

    [SerializeField]
    private ArticulationBody gripper_link_out_l_articulation;

    [SerializeField]
    private ArticulationBody gripper_link_out_r_articulation;

    [SerializeField]
    public TextMeshProUGUI batteryText;

    [SerializeField]
    public TextMeshProUGUI[] servoTemperatureTexts;

    [SerializeField]
    public TextMeshProUGUI[] servoVoltageTexts;

    private void setRotProperty(GameObject obj, string axis, float rotValue)
    {
        Vector3 rotationAxis; 
        switch (axis.ToLower())
        {
            case "x":
                rotationAxis = Vector3.right;
                break;
            case "y":
                rotationAxis = Vector3.up;
                break;
            case "z":
                rotationAxis = Vector3.forward;
                break;
            default:
                Debug.LogError("Invalid axis. Please use 'x', 'y', or 'z'.");
                return;
        }

        Quaternion currentRotation = obj.transform.rotation;
        Quaternion deltaRotation = Quaternion.AngleAxis(rotValue, rotationAxis);
        obj.transform.rotation = deltaRotation;
    }

    void RotateBaseTo(float primaryAxisRotation)
    {
        var drive = base_articulation.xDrive;
        drive.target = primaryAxisRotation;
        base_articulation.xDrive = drive;
    }

    void RotateLowerArmTo(float primaryAxisRotation)
    {
        var drive = lower_arm_articulation.xDrive;
        drive.target = primaryAxisRotation;
        lower_arm_articulation.xDrive = drive;
    }
    void RotateMiddleArmTo(float primaryAxisRotation)
    {
        var drive = middle_arm_articulation.xDrive;
        drive.target = primaryAxisRotation;
        middle_arm_articulation.xDrive = drive;
    }
    void RotateUpperArmTo(float primaryAxisRotation)
    {
        var drive = upper_arm_articulation.xDrive;
        drive.target = primaryAxisRotation;
        upper_arm_articulation.xDrive = drive;
    }
    void RotateGripperTo(float primaryAxisRotation)
    {
        var drive = gripper_base_articulation.xDrive;
        drive.target = primaryAxisRotation;
        gripper_base_articulation.xDrive = drive;
    }
    void RotateGripperLinkRightTo(float primaryAxisRotation)
    {
        var drive = gripper_link_r_articulation.xDrive;
        drive.target = primaryAxisRotation;
        gripper_link_r_articulation.xDrive = drive;
    }
    void RotateGripperLinkLeftTo(float primaryAxisRotation)
    {
        var drive = gripper_link_l_articulation.xDrive;
        drive.target = primaryAxisRotation;
        gripper_link_l_articulation.xDrive = drive;
    }

    public void updateBaseArticulate(float rotValue)
    {
        float rotationGoal = rotValue - 90;
        RotateBaseTo(rotationGoal);
    }
    public void updateLowerArmArticulate(float rotValue)
    {
        float rotationGoal = rotValue - 90;
        RotateLowerArmTo(rotationGoal);
    }
    public void updateMiddleArmArticulate(float rotValue)
    {
        float rotationGoal = rotValue - 90;
        RotateMiddleArmTo(rotationGoal);
    }
    public void updateUpperArmArticulate(float rotValue)
    {
        float rotationGoal = rotValue - 90;
        RotateUpperArmTo(rotationGoal);
    }
    public void updateGripperBaseArticulate(float rotValue)
    {
        float rotationGoal = rotValue - 90;
        RotateGripperTo(rotationGoal);
    }
    public void updateGripperLinks(float rotValue)
    {
        float gripGoal = rotValue - 90;
        Debug.Log("Grip Goal : " + gripGoal);
        RotateGripperLinkRightTo(gripGoal);
        RotateGripperLinkLeftTo(-gripGoal);
    }
    public void updateBatteryText(float batteryValue)
    {
        batteryText.text = "Robot Battery : " + batteryValue + " mV";
    }

    public void updateTemperatureText(string temperatures)
    {
        string[] temperature_values = temperatures.Split(',');
        // Debug.Log(temperature_values);
        for (int i = 1; i <= servoTemperatureTexts.Length; i++)
        {
            float temperature_value = float.Parse(temperature_values[i]);
            servoTemperatureTexts[i-1].text = "Temperature: " + temperature_value + " ºC";
        }
    }

    public void updateVoltageText(string voltages)
    {
        string[] voltage_values = voltages.Split(',');
        // Debug.Log(voltage_values);
        for (int i = 1; i <= servoVoltageTexts.Length; i++)
        {
            float voltage_value = float.Parse(voltage_values[i]);
            servoVoltageTexts[i-1].text = "Voltage: " + voltage_value + " mV";
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void setRotBaseServo(float rotValue)
    {
        updateBaseArticulate(rotValue);
    }
    public void setRotLowerArm(float rotValue)
    {
        updateLowerArmArticulate(rotValue);
    }
    public void setRotMiddleArm(float rotValue)
    {
        updateMiddleArmArticulate(rotValue);
    }
    public void setRotUpperArm(float rotValue)
    {
        updateUpperArmArticulate(rotValue);
    }
    public void setRotGripperBase(float rotValue)
    {
        updateGripperBaseArticulate(rotValue);
    }
    public void setRotGripperLink(float rotValue)
    {
        updateGripperLinks(rotValue);
    }
    public void setBatteryText(float batteryValue)
    {
        updateBatteryText(batteryValue);
    }
    public void setTemperatureText(string temperatureValue)
    {
        updateTemperatureText(temperatureValue);
    }

    public void setVoltageText(string voltageValue){
        updateVoltageText(voltageValue);
    }
}
