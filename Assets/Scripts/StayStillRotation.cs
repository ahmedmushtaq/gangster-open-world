using UnityEngine;

public class StayStillRotation : MonoBehaviour
{
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        // Keep the local rotation fixed relative to the parent
        transform.localRotation = initialRotation;
    }
}