using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class PanelEventHandler : MonoBehaviour
{

    [SerializeField]
    private GameObject slider;

    public float slider_min = -0.5f;
    public float slider_max = -2f;

    [SerializeField]
    private GameObject knob1;

    [SerializeField]
    private GameObject knob2;

    [SerializeField]
    private GameObject lever1;

    [SerializeField]
    private GameObject lever2;

    [SerializeField]
    private GameObject lever3;

    [SerializeField]
    private GameObject reset_button;

    [SerializeField]
    private GameObject debug_canvas;

    [SerializeField]
    private GameObject[] metric_canvases;

    private bool is_Knob1_Active= false;
    private bool is_Knob2_Active = false;
    private bool is_lever1_Active = false;
    private bool is_lever2_Active = false;
    private bool is_lever3_Active = false;
    private bool is_slider_Active = false;
    private bool is_debug_canvas_Active = false;

    private float previous_lever1_rotation;
    private float previous_lever2_rotation;
    private float previous_lever3_rotation;
    private float previous_knob1_rotation;
    private float previous_knob2_rotation;
    private float previous_slider_transform;

    private Vector3 initial_knob_1_rotation;

    public Grabbable grabbable;
    public HandGrabInteractable handGrabInteractable;
    public OneGrabRotateTransformer oneGrabRotateTransformer;

    private float getRotation(GameObject obj, string axis)
    {
        switch (axis.ToLower())
        {
            case "x":
                return obj.transform.eulerAngles.x;
            case "y":
                return obj.transform.eulerAngles.y;
            case "z":
                return obj.transform.eulerAngles.z;
            default:
                Debug.LogError("Invalid axis. Please use 'x', 'y', or 'z'.");
                return 0f; 
        }
    }

    private float getTransform(GameObject obj, string axis)
    {
        switch (axis.ToLower())
        {
            case "x":
                return obj.transform.localPosition.x;
            case "y":
                return obj.transform.localPosition.y;
            case "z":
                return obj.transform.localPosition.z;
            default:
                Debug.LogError("Invalid axis. Please use 'x', 'y', or 'z'.");
                return 0f;
        }
    }

    private float calculateSpeedMultiplier(float rotation, float oldMin, float oldMax, float newMin, float newMax)
    {
        float value = rotation;
        float remappedValue = Remap(value, oldMin, oldMax, newMin, newMax);
        return remappedValue;
    }

    public void show_debug_canvas()
    {
        if(getDebug_canvas_state())
        {
            setDebug_canvas_inactive();
        }
        else
        {
            setDebug_canvas_active();
        }
    }

    // Lever1 Direction
    private void set_Previous_Lever1_Rotation()
    {
        float rotation = getRotation(lever1, "x");
        previous_lever1_rotation = rotation > 180 ? rotation - 360 : rotation;
    }

    private float get_Previous_Lever1_Rotation()
    {
        return previous_lever1_rotation;
    }

    public int get_Lever1_direction()
    {
        float currentXRotation = get_lever_1_speed_multiplier();

        // Calculate the change in rotation
        float deltaRotation = Mathf.DeltaAngle(get_Previous_Lever1_Rotation(), currentXRotation);


        if (currentXRotation > 0)
        {
            // Debug.Log("Lever 1 Rotating Forward on X-axis");
            set_Previous_Lever1_Rotation();
            return 1;
        }
        else
        {
            // Debug.Log("Lever 1 Rotating Backward on X-axis");
            set_Previous_Lever1_Rotation();
            return -1;
        }
    }
    
    // Lever2 Direction
    private void set_Previous_Lever2_Rotation()
    {
        float rotation = getRotation(lever2, "x");
        previous_lever2_rotation = rotation > 180 ? rotation - 360 : rotation;
    }

    private float get_Previous_Lever2_Rotation()
    {
        return previous_lever2_rotation;
    }

    public int get_Lever2_direction()
    {
        float currentXRotation = get_lever_2_speed_multiplier();

        // Calculate the change in rotation
        float deltaRotation = Mathf.DeltaAngle(get_Previous_Lever2_Rotation(), currentXRotation);

        // Update previous rotation
        set_Previous_Lever2_Rotation();

        if (currentXRotation > 0)
        {
            // Debug.Log("Lever 2 Rotating Forward on X-axis");
            return 1;
        }
        else
        {
            // Debug.Log("Lever 2 Rotating Backward on X-axis");
            return -1;
        }
    }

    // Lever3 Direction
    private void set_Previous_Lever3_Rotation()
    {
        float rotation = getRotation(lever3, "x");
        previous_lever3_rotation = rotation > 180 ? rotation - 360 : rotation;
    }

    private float get_Previous_Lever3_Rotation()
    {
        return previous_lever3_rotation;
    }

    public int get_Lever3_direction()
    {
        float currentXRotation = get_lever_3_speed_multiplier();

        // Calculate the change in rotation
        float deltaRotation = Mathf.DeltaAngle(get_Previous_Lever3_Rotation(), currentXRotation);

        // Update previous rotation
        set_Previous_Lever3_Rotation();

        if (currentXRotation > 0)
        {
            // Debug.Log("Lever 3 Rotating Forward on X-axis");
            return 1;
        }
        else
        {
            // Debug.Log("Lever 3 Rotating Backward on X-axis");
            return -1;
        }
    }

    // Knob1 Direction
    private void set_Previous_Knob1_Rotation()
    {
        float rotation = getRotation(knob1, "z");
        previous_knob1_rotation = rotation > 180 ? rotation - 360 : rotation;
    }

    private float get_Previous_Knob1_Rotation()
    {
        return previous_knob1_rotation;
    }

    public int get_Knob1_direction()
    {
        float currentXRotation = get_Knob_1_speed_multiplier();

        // Calculate the change in rotation
        // float deltaRotation = Mathf.DeltaAngle(get_Previous_Knob1_Rotation(), currentXRotation);
        // Update previous rotation
        set_Previous_Knob1_Rotation();

        if (currentXRotation > 0)
        {
            // Debug.Log("Knob 1 Rotating Forward on X-axis");
            return 1;
        }
        else
        {
            // Debug.Log("Knob 1 Rotating Backward on X-axis");
            return -1;
        }
    }

    // Knob2 Direction
    private void set_Previous_Knob2_Rotation()
    {
        float rotation = getRotation(knob2, "z");
        previous_knob2_rotation = rotation > 180 ? rotation - 360 : rotation;
    }

    private float get_Previous_Knob2_Rotation()
    {
        return previous_knob2_rotation;
    }

    public int get_Knob2_direction()
    {
        float currentXRotation = get_Knob_2_speed_multiplier();

        // Calculate the change in rotation
        float deltaRotation = Mathf.DeltaAngle(get_Previous_Knob2_Rotation(), currentXRotation);

        // Update previous rotation
        set_Previous_Knob2_Rotation();

        if (currentXRotation > 0)
        {
            // Debug.Log("Knob 2 Rotating Forward on X-axis");
            return 1;
        }
        else
        {
            // Debug.Log("Knob 2 Rotating Backward on X-axis");
            return -1;
        }
    }

    // Slider Direction
    private void set_Previous_Slider_transform()
    {
        float slider_transform = getTransform(slider, "x");
        previous_slider_transform = slider_transform;
    }

    private float get_Previous_Slider_Transform()
    {
        return previous_slider_transform;
    }

    public float get_Slider_Direction()
    {
        float currentTransform = get_slider_pos();

        Debug.Log("Current Transform : " + currentTransform);

        // Update previous rotation
        set_Previous_Slider_transform();

        float currenTransformRemapped = Remap(currentTransform, slider_min, slider_max, 30, 150);

        return currenTransformRemapped;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    
    public float get_Knob_1_speed_multiplier()
    {
        float rotation = getRotation(knob1, "z");
        float speed_multiplier = calculateSpeedMultiplier(rotation-90, -90f, 90f, -10f, 10f);
        return speed_multiplier;
    }

    public float get_Knob_2_speed_multiplier()
    {
        float rotation = getRotation(knob2, "z");
        float speed_multiplier = calculateSpeedMultiplier(rotation - 90, -90f, 90f, -10f, 10f);
        return speed_multiplier;
    }

    public float get_lever_1_speed_multiplier()
    {
        float rotation = getRotation(lever1, "x");
        rotation = rotation > 180 ? rotation - 360 : rotation;
        float speed_multiplier = calculateSpeedMultiplier(rotation, -30f, 30f, -10f, 10f);
        return rotation;
    }

    public float get_lever_2_speed_multiplier()
    {
        float rotation = getRotation(lever2, "x");
        rotation = rotation > 180 ? rotation - 360 : rotation;
        float speed_multiplier = calculateSpeedMultiplier(rotation, -30f, 30f, -10f, 10f);
        return speed_multiplier;
    }

    public float get_lever_3_speed_multiplier()
    {
        float rotation = getRotation(lever3, "x");
        rotation = rotation > 180 ? rotation - 360 : rotation;
        float speed_multiplier = calculateSpeedMultiplier(rotation, -30f, 30f, -10f, 10f);
        return speed_multiplier;
    }

    public float get_slider_pos()
    {
        float position = getTransform(slider, "x");
        return position;
    }

    public void setKnob1_active()
    {
        is_Knob1_Active = true;
        // initial_knob_1_rotation = knob1.transform.eulerAngles;
        // Debug.Log("Initial Angle " + initial_knob_1_rotation);
    }

    public void setKnob1_inactive(){
        is_Knob1_Active = false;
    }

    public bool get_Knob_1_state(){
        return is_Knob1_Active;
    }

    public void setKnob2_active()
    {
        is_Knob2_Active = true;
    }

    public void setKnob2_inactive()
    {
        is_Knob2_Active = false;
    }

    public bool get_Knob_2_state()
    {
        return is_Knob2_Active;
    }

    public void setLever1_active()
    {
        // set_Previous_Lever1_Rotation();
        is_lever1_Active = true;
    }

    public void setLever1_inactive()
    {
        is_lever1_Active = false;
        set_Previous_Lever1_Rotation();
    }

    public bool get_lever1_state()
    {
        return is_lever1_Active;
    }

    public void setLever2_active()
    {
        is_lever2_Active = true;
    }

    public void setLever2_inactive()
    {
        is_lever2_Active = false;
    }

    public bool get_lever2_state()
    {
        return is_lever2_Active;
    }

    public void setLever3_active()
    {
        is_lever3_Active = true;
    }

    public void setLever3_inactive()
    {
        is_lever3_Active = false;
    }

    public bool get_lever3_state()
    {
        return is_lever3_Active;
    }

    public void setSlider_active()
    {
        is_slider_Active = true;
    }

    public void setSlider_inactive()
    {
        is_slider_Active = false;
    }

    public bool get_slider_state()
    {
        return is_slider_Active;
    }

    /*public float getSliderTransform(){
        float posY = getPosProperty(slider, "y");
        return posY;
    }*/

    public void setDebug_canvas_active()
    {
        is_debug_canvas_Active = true;
        foreach (GameObject canvas in metric_canvases)
        {
            canvas.SetActive(is_debug_canvas_Active);
        }
        debug_canvas.SetActive(is_debug_canvas_Active);
    }

    public void setDebug_canvas_inactive()
    {
        is_debug_canvas_Active = false;
        foreach (GameObject canvas in metric_canvases) 
        {
            canvas.SetActive(is_debug_canvas_Active);
        }
        debug_canvas.SetActive(is_debug_canvas_Active);
    }

    public bool getDebug_canvas_state()
    {
        return is_debug_canvas_Active;
    }

    float Remap(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (value - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
    }
}
