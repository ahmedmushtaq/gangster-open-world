using UnityEngine;

public class ParkingFinish : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag of the player's car root object")]
    public string carTag = "Player";

    [Header("Debug")]
    public bool isCarFullyInside = false;

    // Private references
    public Collider triggerCollider;
    public GameObject playerCar;

    // For detecting when the state changes
    public bool wasFullyInside = false;

    void Start()
    {
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null || !triggerCollider.isTrigger)
        {
            Debug.LogError("ParkingFinish: This script requires a Trigger Collider on the same GameObject!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.LogError($"ParkingFinish: OnTriggerEnter with '{other.transform.root.gameObject.name}' (tag: {other.transform.root.gameObject.tag})");
        // Get the root object of whatever entered
        Transform root = other.transform.root;
        if (root.CompareTag(carTag))
        {
            if (playerCar == null)
            {
                playerCar = root.gameObject;
                Debug.Log($"ParkingFinish: Detected player car '{playerCar.name}'");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.LogError($"ParkingFinish: OnTriggerExit with '{other.gameObject.name}' (tag: {other.gameObject.tag})");
        Transform root = other.transform.root;
        if (root.CompareTag(carTag))
        {
            // Car has completely left the trigger area
            playerCar = null;
            isCarFullyInside = false;
            Debug.Log("ParkingFinish: Car left the parking area");
        }
    }

    void Update()
    {
        // Update the fully-inside flag
        if (playerCar != null)
        {
            isCarFullyInside = IsCarCompletelyInside();
        }
        else
        {
            isCarFullyInside = false;
        }

        // Detect rising edge (just became fully inside)
        if (isCarFullyInside && !wasFullyInside)
        {
            OnCarParked();
        }
        wasFullyInside = isCarFullyInside;
    }

    /// <summary>
    /// Checks if the entire car's combined collider bounds are fully inside this trigger.
    /// Works with rotated trigger and rotated car (uses local space comparison).
    /// </summary>
    private bool IsCarCompletelyInside()
    {
        if (playerCar == null || triggerCollider == null)
            return false;

        // Get the combined bounding box of all car colliders in world space
        Bounds carBounds = GetCombinedBounds(playerCar);

        // Convert the 8 corners of the car's bounds into the trigger's local space
        Vector3[] carCorners = GetBoundsCorners(carBounds);
        foreach (Vector3 worldCorner in carCorners)
        {
            Vector3 localCorner = transform.InverseTransformPoint(worldCorner);
            // Check if this point is inside the trigger's collider volume
            if (!IsPointInsideTriggerCollider(localCorner))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Gets the combined world-space bounds of all colliders on a GameObject and its children.
    /// </summary>
    private Bounds GetCombinedBounds(GameObject obj)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        if (colliders.Length == 0)
            return new Bounds(obj.transform.position, Vector3.zero);

        Bounds combined = colliders[0].bounds;
        for (int i = 1; i < colliders.Length; i++)
        {
            combined.Encapsulate(colliders[i].bounds);
        }
        return combined;
    }

    /// <summary>
    /// Returns the 8 corner points of a bounding box in world space.
    /// </summary>
    private Vector3[] GetBoundsCorners(Bounds bounds)
    {
        Vector3[] corners = new Vector3[8];
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        corners[0] = new Vector3(min.x, min.y, min.z);
        corners[1] = new Vector3(min.x, min.y, max.z);
        corners[2] = new Vector3(max.x, min.y, min.z);
        corners[3] = new Vector3(max.x, min.y, max.z);
        corners[4] = new Vector3(min.x, max.y, min.z);
        corners[5] = new Vector3(min.x, max.y, max.z);
        corners[6] = new Vector3(max.x, max.y, min.z);
        corners[7] = new Vector3(max.x, max.y, max.z);
        return corners;
    }

    /// <summary>
    /// Checks whether a point (in the trigger's local space) is inside the trigger's collider volume.
    /// Supports BoxCollider, SphereCollider, CapsuleCollider, and MeshCollider (convex).
    /// </summary>
    private bool IsPointInsideTriggerCollider(Vector3 localPoint)
    {
        // Use the built-in ClosestPoint method - if the closest point is the same as the input point, it's inside
        Vector3 closestPoint = triggerCollider.ClosestPoint(transform.TransformPoint(localPoint));
        return Vector3.Distance(triggerCollider.ClosestPoint(transform.TransformPoint(localPoint)), transform.TransformPoint(localPoint)) < 0.001f;
    }

    /// <summary>
    /// Called when the car becomes fully inside the parking area.
    /// Override this or add your own logic (e.g., play sound, finish level, etc.)
    /// </summary>
    private void OnCarParked()
    {
        Debug.LogError("CAR FULLY PARKED!");

        Invoke(nameof(LevelComplete), 1f); // Delay to allow for any final adjustments or effects

        // Example: Disable car movement, show UI, load next level, etc.
        // You can also expose a UnityEvent in the inspector for flexibility.
    }

    public void LevelComplete()
    {
        MissionManager.Instance.LevelComplete();
    }

    // ========== GIZMOS FOR VISUAL DEBUGGING ==========

    private void OnDrawGizmos()
    {
        // Draw the trigger collider (always)
        if (triggerCollider == null)
            triggerCollider = GetComponent<Collider>();

        if (triggerCollider != null)
        {
            Gizmos.color = isCarFullyInside ? Color.green : Color.yellow;
            DrawColliderGizmo(triggerCollider);
        }

        // Draw the car's combined bounds if a car is inside
        if (playerCar != null && Application.isPlaying)
        {
            Bounds carBounds = GetCombinedBounds(playerCar);
            Gizmos.color = isCarFullyInside ? Color.green : Color.red;
            Gizmos.DrawWireCube(carBounds.center, carBounds.size);

            // Optionally draw the 8 corner points for debugging
            if (isCarFullyInside == false && carBounds.size.magnitude > 0.1f)
            {
                Vector3[] corners = GetBoundsCorners(carBounds);
                Gizmos.color = Color.magenta;
                foreach (Vector3 corner in corners)
                {
                    Gizmos.DrawSphere(corner, 0.1f);
                }
            }
        }
    }

    /// <summary>
    /// Helper to draw gizmo for different collider types.
    /// </summary>
    private void DrawColliderGizmo(Collider col)
    {
        if (col is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.center, box.size);
            Gizmos.matrix = Matrix4x4.identity;
        }
        else if (col is SphereCollider sphere)
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(sphere.center), sphere.radius);
        }
        else if (col is CapsuleCollider capsule)
        {
            // Simple approximation: draw the capsule as a cylinder + spheres
            Vector3 worldCenter = transform.TransformPoint(capsule.center);
            float radius = capsule.radius;
            float height = capsule.height;
            int direction = capsule.direction;
            // Not implementing full capsule gizmo for brevity, but you can extend.
            Gizmos.DrawWireSphere(worldCenter, radius);
        }
        else
        {
            // Fallback: draw bounds
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}