using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Radial_Selection : MonoBehaviour
{
    public OVRHand spawn_hand;

    [Range(2,10)]
    public int number_of_radial_part;
    public GameObject radial_part_prefab;
    public Transform radial_part_canvas;
    public float angle_between_part = 10;
    public float translation_offset = 0.1f;
    public Quaternion rotation_offset = Quaternion.Euler(0f, 0f, 0f);

    private List<GameObject> spawned_parts = new List<GameObject>();

    public Transform hand_transfrom;
    private int current_selected_radial_part = -1;

    public float long_press_duration = 2.0f;
    private float press_time = 0f;
    private bool is_long_press_active = false;

    public UnityEvent<int> OnPartsSelected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("OVRInput " + OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger));

        if (spawn_hand.IsTracked)
        {
            if (spawn_hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
            {
                press_time += Time.deltaTime;
                Debug.Log(press_time);
                if (press_time >= long_press_duration && !is_long_press_active)
                {
                    is_long_press_active = true;  // Activate long press
                    SpawnRadialPart();
                }
                else
                {

                }
            }
            else if(!spawn_hand.GetFingerIsPinching(OVRHand.HandFinger.Index) && press_time >= long_press_duration)
            {
                HideAndTriggerSelected(); 
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
            GetSelectedRadialPart();
            Debug.Log("Long pressed");
        }
    }

    public void HideAndTriggerSelected()
    {
        OnPartsSelected.Invoke(current_selected_radial_part);
        radial_part_canvas.gameObject.SetActive(false);
    }

    public void GetSelectedRadialPart()
    {
        Vector3 center_to_hand = hand_transfrom.position - radial_part_canvas.position;
        Vector3 center_to_hand_projected = Vector3.ProjectOnPlane(center_to_hand, radial_part_canvas.forward);

        float angle = Vector3.SignedAngle(radial_part_canvas.up, center_to_hand_projected, -radial_part_canvas.forward);

        if (angle < 0)
            angle += 360;

        current_selected_radial_part = (int) angle * number_of_radial_part / 360;

        for (int i = 0; i < spawned_parts.Count; i++)
        {
            if(i == current_selected_radial_part)
            {
                spawned_parts[i].GetComponent<Image>().color = Color.blue;
                spawned_parts[i].transform.localScale = 1.1f * Vector3.one;
            }
            else
            {
                spawned_parts[i].GetComponent<Image>().color = Color.white;
                spawned_parts[i].transform.localScale = Vector3.one;
            }
        }
    }

    public void SpawnRadialPart()
    {
        radial_part_canvas.gameObject.SetActive(true);
        radial_part_canvas.position = hand_transfrom.position + hand_transfrom.right*translation_offset;
        radial_part_canvas.rotation = hand_transfrom.rotation * rotation_offset;

        foreach (var item in spawned_parts)
        {
            Destroy(item);
        }

        spawned_parts.Clear();

        for(int i = 0; i < number_of_radial_part; i++)
        {
            float angle = - i * 360 / number_of_radial_part - (angle_between_part / 2);
            Vector3 radial_part_eular_angle = new Vector3(0, 0, angle);

            GameObject spawned_radial_part = Instantiate(radial_part_prefab, radial_part_canvas);
            spawned_radial_part.transform.position = radial_part_canvas.position;
            spawned_radial_part.transform.localEulerAngles = radial_part_eular_angle;

            spawned_radial_part.GetComponent<Image>().fillAmount = (1 / (float)number_of_radial_part) - (angle_between_part/360);

            spawned_parts.Add(spawned_radial_part);
        }
    }
}
