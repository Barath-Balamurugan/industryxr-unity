using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wafer : MonoBehaviour
{
    [SerializeField]
    private GameObject tweezers;

    [SerializeField]
    private string tweezer_end_one = "TweezerEndOne";

    [SerializeField]
    private string tweezer_end_two = "TweezerEndTwo";

    private bool isTweezerOneColliding = false;
    private bool isTweezerTwoColliding = false;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
        if (other.CompareTag(tweezer_end_one))
        {
            isTweezerOneColliding = true;
        }

        else if (other.CompareTag(tweezer_end_two))
        {
            isTweezerTwoColliding = true;
        }

        if (isTweezerOneColliding && isTweezerTwoColliding)
        {
            GrabObject();
        }
        else
        {
            ReleaseObject();
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
        if (other.CompareTag(tweezer_end_one))
        {
            isTweezerOneColliding = false;
        }

        if (other.CompareTag(tweezer_end_two))
        {
            isTweezerTwoColliding = false;
        }

        ReleaseObject();
    }

    void GrabObject()
    {
        transform.SetParent(tweezers.transform);
    }

    void ReleaseObject()
    {
        transform.parent = null;
    }
}
