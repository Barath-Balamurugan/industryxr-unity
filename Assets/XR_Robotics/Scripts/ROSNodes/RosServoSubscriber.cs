using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class RosServoSubscriber : MonoBehaviour
{
    ROSConnection ros;

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
    private string batteryTopic;

    [SerializeField]
    private string servoTempTopic;

    [SerializeField]
    private string servoVoltageTopic;

    [SerializeField]
    private RobotEventHandler robotEventHandler;
    
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        

        ros.Subscribe<Int32Msg>(baseTopic, RobotBaseCbk);
        ros.Subscribe<Int32Msg>(lowerArmTopic, RobotLowerArmCbk);
        ros.Subscribe<Int32Msg>(middleArmTopic, RobotMiddleArmCbk);
        ros.Subscribe<Int32Msg>(upperArmTopic, RobotUpperArmCbk);
        ros.Subscribe<Int32Msg>(gripperBaseTopic, RobotGripperBaseCbk);
        ros.Subscribe<Int32Msg>(gripperMainTopic, RobotGripperMainCbk);
        ros.Subscribe<Int32Msg>(batteryTopic, RobotBatteryCbk);
        ros.Subscribe<StringMsg>(servoTempTopic, RobotTemperatureCbk);
        ros.Subscribe<StringMsg>(servoVoltageTopic, RobotVoltageCbk);
    }

    public void RobotBaseCbk(Int32Msg msg){
        float data = msg.data;
        robotEventHandler.setRotBaseServo(data);
    } 

    public void RobotLowerArmCbk(Int32Msg msg){
        float data = msg.data;
        robotEventHandler.setRotLowerArm(data);
    } 

    public void RobotMiddleArmCbk(Int32Msg msg){
        float data = msg.data;
        robotEventHandler.setRotMiddleArm(data);
    } 

    public void RobotUpperArmCbk(Int32Msg msg){
        float data = msg.data;
        robotEventHandler.setRotUpperArm(data);
    } 

    public void RobotGripperBaseCbk(Int32Msg msg){
        float data = msg.data;
        robotEventHandler.setRotGripperBase(data);
    } 

    public void RobotGripperMainCbk(Int32Msg msg){
        float data = msg.data;
        robotEventHandler.setRotGripperLink(data);
    }

    public void RobotBatteryCbk(Int32Msg msg)
    {
        float data = msg.data;
        robotEventHandler.setBatteryText(data);
    }

    public void RobotTemperatureCbk(StringMsg msg)
    {
        string data = msg.data;
        robotEventHandler.setTemperatureText(data);
    }

     public void RobotVoltageCbk(StringMsg msg)
    {
        string data = msg.data;
        robotEventHandler.setVoltageText(data);
    }

}
