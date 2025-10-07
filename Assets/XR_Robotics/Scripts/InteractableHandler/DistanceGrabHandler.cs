using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceGrabHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject distanceGrabInteractable;
    [SerializeField]
    private Transform controlPanel_transform;
    [SerializeField]
    private Transform user_transform;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(controlPanel_transform.position, user_transform.position);

        if(distance > 1.5)
        {
            distanceGrabInteractable.SetActive(true);
        }
        else
        {
            distanceGrabInteractable.SetActive(false);
        }

        // Debug.Log("Distance between A and B: " + distance);
    }
}
