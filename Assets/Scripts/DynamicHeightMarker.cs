using UnityEngine;
using SickscoreGames.HUDNavigationSystem;
using UnityEngine.UI;

public class DynamicHeightMarker : MonoBehaviour
{
    [Header("Marker Settings")]
    public Transform markerTransform;        // The child object that will move up/down
    public float maxHeight = 100f;           // Height when distance >= 500m
    public float farDistance = 500f;         // Distance at which marker is at max height
    public float nearDistance = 50f;         // Distance at which marker is at ground (0)

    [Header("Distance Text (optional)")]
    public Text distanceText;                // Reference to the distance text (e.g., from indicator prefab)
    public bool hideTextBeyondFar = true;

    private HUDNavigationElement navElement;
    private float currentHeight;

    void Start()
    {
        navElement = GetComponent<HUDNavigationElement>();
        if (navElement == null)
        {
            Debug.LogError("HUDNavigationElement not found on this GameObject.");
            enabled = false;
            return;
        }

        // Subscribe to the update event
        navElement.OnElementUpdate.AddListener(OnElementUpdated);

        // If markerTransform not set, try to find a child named "Marker"
        if (markerTransform == null)
            markerTransform = transform.Find("Marker");
    }

    void OnElementUpdated(Vector3 worldPos, Vector3 screenPos, float distance)
    {
        // 1. Update marker height
        float t = Mathf.InverseLerp(nearDistance, farDistance, distance);
        t = Mathf.Clamp01(t);
        currentHeight = Mathf.Lerp(0f, maxHeight, t);

        // Apply to the marker's local position (assuming marker is child of this transform)
        if (markerTransform != null)
        {
            Vector3 localPos = markerTransform.localPosition;
            localPos.y = currentHeight;
            markerTransform.localPosition = localPos;
        }

        // 2. Handle distance text visibility (if provided)
        if (distanceText != null && hideTextBeyondFar)
        {
            bool show = distance < farDistance;
            if (distanceText.gameObject.activeSelf != show)
                distanceText.gameObject.SetActive(show);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (navElement != null)
            navElement.OnElementUpdate.RemoveListener(OnElementUpdated);
    }
}