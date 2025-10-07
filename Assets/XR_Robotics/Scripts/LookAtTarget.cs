using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    // Public variable for setting the target in the Inspector
    [SerializeField]
    public Transform target;

    void Update()
    {
        // Check if target is assigned to avoid errors
        if (target != null)
        {
            // Make the observer look at the target
            transform.LookAt(target);
        }
    }
}
