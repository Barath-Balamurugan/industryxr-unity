using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RobotDataHandler : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI depended_metrics;

    [SerializeField]
    private TextMeshProUGUI panel_to_robot;

    [SerializeField]
    private TextMeshProUGUI robot_to_model;

    [SerializeField]
    private TextMeshProUGUI panel_to_model;

    [SerializeField]
    private TextMeshProUGUI base_angle;

    [SerializeField]
    private TextMeshProUGUI lower_arm_angle;

    [SerializeField]
    private TextMeshProUGUI middle_arm_angle;

    [SerializeField]
    private TextMeshProUGUI upper_arm_angle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void set_panel_to_robot_text(float panel_to_robot_delay)
    {
        panel_to_robot.text = panel_to_robot.text + panel_to_robot_delay.ToString();
    }

    public void set_robot_to_model_text(float robot_to_model_delay)
    {
        robot_to_model.text = panel_to_robot.text + robot_to_model_delay.ToString();
    }

    public void set_panel_to_model_text(float panel_to_model_delay)
    {
        panel_to_model.text = panel_to_robot.text + panel_to_model.ToString();
    }

    public void set_base_angle_text(float panel_to_model_delay)
    {
        base_angle.text = panel_to_robot.text + panel_to_model.ToString();
    }

    public void set_lower_arm_angle_text(float panel_to_model_delay)
    {
        lower_arm_angle.text = panel_to_robot.text + panel_to_model.ToString();
    }

    public void set_middle_arm_angle_text(float panel_to_model_delay)
    {
        middle_arm_angle.text = panel_to_robot.text + panel_to_model.ToString();
    }

    public void set_upper_arm_angle_text(float panel_to_model_delay)
    {
        upper_arm_angle.text = panel_to_robot.text + panel_to_model.ToString();
    }
}
