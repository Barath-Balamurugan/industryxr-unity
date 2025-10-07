using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressTweezers : MonoBehaviour, IHandGrabUseDelegate
{
    [Header("Input")]
    [SerializeField]
    private Transform _trigger;
    [SerializeField]
    private AnimationCurve _triggerRotationCurve;
    [SerializeField]
    private SnapAxis _axis = SnapAxis.X;
    [SerializeField]
    [Range(0f, 1f)]
    private float _releaseThresold = 0.3f;
    [SerializeField]
    [Range(0f, 1f)]
    private float _fireThresold = 0.9f;
    [SerializeField]
    private float _triggerSpeed = 3f;
    [SerializeField]
    private AnimationCurve _strengthCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField]
    private float sensitivity_threshold = 10f;

    [SerializeField]
    public OVRHand rightHand;

    private bool _wasFired = false;
    private float _dampedUseStrength = 0;
    private float _lastUseTime;

    #region input

    /*private void Update()
    {
        if (rightHand != null)
        {
            float rightIndexPinch = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
            Debug.Log($"Right Hand Index Pinch Strength: {rightIndexPinch}");

            if (rightIndexPinch > 0.2f) // Example threshold
            {
                Debug.Log("Right Index Pinch Detected!");
            }
        }
    }*/

    private void UpdateTweezerRotation(float progress)
    {
        float value = _triggerRotationCurve.Evaluate(progress) * sensitivity_threshold;
        Vector3 angles = _trigger.localEulerAngles;
        if ((_axis & SnapAxis.X) != 0)
        {
            angles.x = value;
        }
        if ((_axis & SnapAxis.Y) != 0)
        {
            angles.y = value;
        }
        if ((_axis & SnapAxis.Z) != 0)
        {
            angles.z = -87 - value;
            angles.z = -(Mathf.Clamp(-angles.z, 87, 91));
        }
        _trigger.localEulerAngles = angles;
    }

    #endregion

    #region output

    public void BeginUse()
    {
        _dampedUseStrength = 0f;
        _lastUseTime = Time.realtimeSinceStartup;
    }

    public void EndUse()
    {

    }

    public float ComputeUseStrength(float strength)
    {
        float delta = Time.realtimeSinceStartup - _lastUseTime;
        _lastUseTime = Time.realtimeSinceStartup;
        if (strength > _dampedUseStrength)
        {
            _dampedUseStrength = Mathf.Lerp(_dampedUseStrength, strength, _triggerSpeed * delta);
        }
        else
        {
            _dampedUseStrength = strength;
        }
        float progress = _strengthCurve.Evaluate(_dampedUseStrength);
        // Debug.Log("Progess " + progress);

        UpdateTweezerProgress(progress);
        return progress;
    }

    private void UpdateTweezerProgress(float progress)
    {
        UpdateTweezerRotation(progress);

        if (progress >= _fireThresold && !_wasFired)
        {
            _wasFired = true;
        }
        else if (progress <= _releaseThresold)
        {
            _wasFired = false;
        }
    }

    #endregion

    static class NonAlloc
    {

    }
}
