using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using System;

public class RosServoPublisher : MonoBehaviour
{
    ROSConnection ros;

    [SerializeField]
    private string resetTopic;

    [SerializeField]
    private string baseTopic;

    [SerializeField]
    private string lowerArmTopic;
    
    [SerializeField]
    private string middleArmTopic;

    [SerializeField]
    private string upperArmTopic;

    [SerializeField]
    private string gripperBaseTopic;

    [SerializeField]
    private string gripperMainTopic;

    [SerializeField]
    private PanelEventHandler panelEventHandler;

    [SerializeField]
    private float publishInterval_ms = 20.0f;
    
    private float timeSinceLastPublish = 0f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        // instantiate publisher
        ros.RegisterPublisher<Float32Msg>(resetTopic);
        ros.RegisterPublisher<Float32Msg>(baseTopic);
        ros.RegisterPublisher<Float32Msg>(lowerArmTopic);
        ros.RegisterPublisher<Float32Msg>(middleArmTopic);
        ros.RegisterPublisher<Float32Msg>(upperArmTopic);
        ros.RegisterPublisher<Float32Msg>(gripperBaseTopic);
        ros.RegisterPublisher<Float32Msg>(gripperMainTopic);
        
        Debug.Log("Sending pose reset command for the robot bus servo");
        Float32Msg msg = new Float32Msg(1);
        ros.Publish(resetTopic, msg);

    }

    public void resetServo(){
        Debug.Log("Servo Pose reset");
        Float32Msg msg = new Float32Msg(0);
        Debug.Log("Servo Pose reset" + msg);
        ros.Publish(resetTopic, msg);
    }

    private void updateBasePos(){
        // float basePos = panelEventHandler.get_Knob_1_speed_multiplier();
        float baseDirection = panelEventHandler.get_Knob1_direction();
        // Debug.Log("Message: "+ basePos);

        Float32Msg msg = new Float32Msg(baseDirection);
        // Debug.Log("Message: "+ msg);
        ros.Publish(baseTopic, msg);

    }

// test
    private void updateLowerArmPos(){
        // float lowerArmPos = panelEventHandler.get_lever_1_speed_multiplier();
        float lowerArmDirection = panelEventHandler.get_Lever1_direction();
        // Debug.Log("Message: "+ basePos);

        Float32Msg msg = new Float32Msg(lowerArmDirection);
        // Debug.Log("Message: "+ msg);
        ros.Publish(lowerArmTopic, msg);

    }

    private void updateMiddleArmPos(){
        // float middlePos = panelEventHandler.get_lever_2_speed_multiplier();
        float middleArmDirection = panelEventHandler.get_Lever2_direction();
        // Debug.Log("Message: "+ basePos);
        
        Float32Msg msg = new Float32Msg(middleArmDirection);
        // Debug.Log("Message: "+ msg);
        ros.Publish(middleArmTopic, msg);

    }

    private void updateUpperArmPos(){
        // float upperPos = panelEventHandler.get_lever_3_speed_multiplier();
        float upperArmDirection = panelEventHandler.get_Lever3_direction();
        // Debug.Log("Message: "+ basePos);
        
        Float32Msg msg = new Float32Msg(upperArmDirection);
        // Debug.Log("Message: "+ msg);
        ros.Publish(upperArmTopic, msg);

    }


    public void updateGripperBasePos(){
        // float sliderPos= 32.0f;
        float gripperDirection = panelEventHandler.get_Knob2_direction();
        Float32Msg msg = new Float32Msg(gripperDirection);
        ros.Publish(gripperBaseTopic, msg);
      
    }

    public void updateGripperMainPos(){
        // float sliderPos= panelEventHandler.get_slider_pos();
        float sliderDirection = panelEventHandler.get_Slider_Direction();
        Float32Msg msg = new Float32Msg(sliderDirection);
        ros.Publish(gripperMainTopic, msg);
      
    }

    void Update()
    {
       
        timeSinceLastPublish += Time.deltaTime * 1000;
        if (timeSinceLastPublish >= publishInterval_ms)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Debug.Log("Space key is held down");
                updateBasePos();
            }
         
            if(panelEventHandler.get_Knob_1_state()){
                updateBasePos();
            }

            if(panelEventHandler.get_lever1_state()){
                updateLowerArmPos();
            }

            if(panelEventHandler.get_lever2_state()){
                updateMiddleArmPos();
            }

            if(panelEventHandler.get_lever3_state()){
                updateUpperArmPos();
            }

            if(panelEventHandler.get_slider_state()){
                updateGripperMainPos();
            }

            if(panelEventHandler.get_Knob_2_state()){
                updateGripperBasePos();
            }

            timeSinceLastPublish = 0f;
        }
    }
}