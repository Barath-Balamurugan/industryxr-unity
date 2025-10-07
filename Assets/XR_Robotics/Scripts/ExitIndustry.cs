using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class ExitIndustry : MonoBehaviour
{
    public OVRHand spawn_hand;

    public float long_press_duration = 2.0f;
    private float press_time = 0f;
    private bool is_long_press_active = false;

    public string sceneName;

    void Update()
    {
        // Debug.Log("OVRInput " + OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger));

        if (spawn_hand.IsTracked)
        {
            if (spawn_hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
            {
                press_time += Time.deltaTime;
                Debug.Log(press_time);
                if (press_time >= long_press_duration && !is_long_press_active)
                {
                    is_long_press_active = true;  // Activate long press
                    // SpawnRadialPart();
                    SceneManager.LoadScene(sceneName);
                }
                else
                {

                }
            }
            else if (!spawn_hand.GetFingerIsPinching(OVRHand.HandFinger.Index) && press_time >= long_press_duration)
            {
                // HideAndTriggerSelected();
                press_time = 0f;
                is_long_press_active = false;
            }
            else
            {
                press_time = 0f;
                is_long_press_active = false;
            }
        }

        if (is_long_press_active)
        {
            Debug.Log("Long pressed");
        }
    }
}
