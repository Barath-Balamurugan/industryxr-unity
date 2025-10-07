using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleArm_CollisionDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("other.gameObject.tag");
        if (other.gameObject.tag == "Obstacle")
        {
            Debug.Log(gameObject.name + " Link is about to Collide with " + other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            Debug.Log("Its fine");
        }
    }
}
