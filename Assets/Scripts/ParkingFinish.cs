using UnityEngine;
using UnityEngine.Events;

public class ParkingFinish : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag of the player's car root object")]
    public string carTag = "Player";

    [Header("Visual Feedback")]
    public MeshRenderer parkingSlotRenderer;   // Assign the renderer of the parking slot (or child object)
    public Material parkingSlotMaterial;       // Optional: assign material directly (overrides renderer.sharedMaterial)
    public Color idleColor = Color.blue;       // No car inside
    public Color enterColor = Color.red;       // Car entered but not fully inside
    public Color insideColor = Color.green;    // Car fully inside (timer counting)

    [Header("Timer")]
    public float requiredInsideTime = 3f;      // Seconds the car must stay fully inside to complete
    private float currentInsideTimer = 0f;
    private bool isTimerRunning = false;

    // ========== NEW: Win Effects Section ==========
    [Header("Win Effects")]
    public LevelData levelData;                // Reference to LevelData (contains winVFX & successCamera)
    // ============================================

    [Header("Debug")]
    public bool isCarFullyInside = false;

    // Private references
    private Collider triggerCollider;
    private GameObject playerCar;
    private bool wasFullyInside = false;
    public Material activeMaterial;            // The material instance we actually modify

    // ========== NEW: Events for MissionManager ==========
    [Header("Events for MissionManager")]
    public UnityEvent OnCarFullyInside;
    public UnityEvent OnCarExited;
    public UnityEvent OnParkingSuccess;

    // ========== NEW: Public method to manually trigger success (optional) ==========
    public void TriggerParkingSuccess()
    {
        OnParkingSuccess?.Invoke();
    }
    // ====================================================

    void Start()
    {
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null || !triggerCollider.isTrigger)
        {
            Debug.LogError("ParkingFinish: This script requires a Trigger Collider on the same GameObject!");
        }

        // Set up the material for color changes
        if (parkingSlotRenderer != null)
        {
            activeMaterial = parkingSlotRenderer.material; // Creates an instance
        }
        else if (parkingSlotMaterial != null)
        {
            activeMaterial = new Material(parkingSlotMaterial); // Create instance
            // If you assign a material directly, you'll need to apply it to a renderer manually
            Debug.LogWarning("ParkingFinish: parkingSlotRenderer not assigned; using fallback material but it won't be visible.");
        }
        else
        {
            Debug.LogError("ParkingFinish: No MeshRenderer or Material assigned for visual feedback!");
        }

        // Set initial color (idle/blue)
        SetColor(idleColor);

        // ========== NEW: Auto-find LevelData if not assigned ==========
        if (levelData == null)
        {
            // Try to find LevelData on the same GameObject or a parent
            levelData = GetComponentInParent<LevelData>();
            if (levelData == null)
                levelData = GetComponent<LevelData>();

            if (levelData == null)
                Debug.LogWarning("ParkingFinish: LevelData not assigned and could not be found. Win VFX/success camera will not activate.");
        }
        // ==============================================================

        // Automatically subscribe to MissionManager if available
        if (MissionManager.Instance != null)
        {
            OnCarFullyInside.AddListener(MissionManager.Instance.OnCarFullyInside);
            OnCarExited.AddListener(MissionManager.Instance.OnCarExited);
            OnParkingSuccess.AddListener(MissionManager.Instance.OnParkingSuccess);
            Debug.Log("ParkingFinish: Subscribed to MissionManager events.");
        }
        else
        {
            Debug.LogWarning("ParkingFinish: MissionManager.Instance not found – events will not trigger mission logic.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;
        if (root.CompareTag(carTag))
        {
            if (playerCar == null)
            {
                playerCar = root.gameObject;
                Debug.Log($"ParkingFinish: Car entered the area -> color RED");
                SetColor(enterColor);
            }
            // Reset timer when car enters (even if partially)
            ResetTimer();
        }
    }

    void OnTriggerExit(Collider other)
    {
        Transform root = other.transform.root;
        if (root.CompareTag(carTag))
        {
            playerCar = null;
            isCarFullyInside = false;
            ResetTimer();
            SetColor(idleColor);
            Debug.Log("ParkingFinish: Car left the parking area -> color BLUE");
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

        // Handle timer logic when fully inside
        if (isCarFullyInside)
        {
            if (!isTimerRunning)
            {
                // Just became fully inside – start timer and change color to green
                isTimerRunning = true;
                currentInsideTimer = 0f;
                SetColor(insideColor);
                OnCarFullyInside.Invoke();
                Debug.Log("ParkingFinish: Car fully inside! Starting timer...");
            }
            else
            {
                // Increment timer
                currentInsideTimer += Time.deltaTime;
                float progress = currentInsideTimer / requiredInsideTime;
                if (MissionManager.Instance.fillImage != null)
                    MissionManager.Instance.fillImage.fillAmount = 1f - Mathf.Clamp01(progress);
                if (MissionManager.Instance.fillText != null)
                    MissionManager.Instance.fillText.text = "Parking in " + (requiredInsideTime - currentInsideTimer).ToString("F1")/* + "s"*/;
                
                if (currentInsideTimer >= requiredInsideTime)
                {
                    // Timer complete – level success
                    Debug.LogError("CAR FULLY PARKED!");
                    OnParkingSuccess?.Invoke();
                    OnCarParked();
                    // Reset timer so we don't trigger again
                    ResetTimer();
                }
            }
        }
        else
        {
            // Car is not fully inside (either partially or left)
            if (isTimerRunning)
            {
                ResetTimer();
                // If car is still inside the trigger (but not fully), set color red
                if (playerCar != null)
                {
                    SetColor(enterColor);
                    OnCarExited?.Invoke();
                    Debug.Log("ParkingFinish: Car partially inside -> color RED, timer reset.");
                }
            }
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

        Bounds carBounds = GetCombinedBounds(playerCar);
        Vector3[] carCorners = GetBoundsCorners(carBounds);
        foreach (Vector3 worldCorner in carCorners)
        {
            Vector3 localCorner = transform.InverseTransformPoint(worldCorner);
            if (!IsPointInsideTriggerCollider(localCorner))
            {
                return false;
            }
        }
        return true;
    }

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

    private bool IsPointInsideTriggerCollider(Vector3 localPoint)
    {
        Vector3 worldPoint = transform.TransformPoint(localPoint);
        Vector3 closestPoint = triggerCollider.ClosestPoint(worldPoint);
        return Vector3.Distance(closestPoint, worldPoint) < 0.001f;
    }

    private void ResetTimer()
    {
        isTimerRunning = false;
        currentInsideTimer = 0f;
    }

    private void SetColor(Color color)
    {
        if (activeMaterial != null)
        {
            activeMaterial.SetColor("_TintColor", color);
            //activeMaterial.color = color;
        }
    }

    private void OnCarParked()
    {
        Debug.Log("ParkingFinish: CAR SUCCESSFULLY PARKED! Activating win effects.");

        // ========== NEW: Activate win VFX and success camera from LevelData ==========
        if (levelData != null)
        {
            if (levelData.winVFX != null)
            {
                levelData.winVFX.SetActive(true);
                ParticleSystem ps = levelData.winVFX.GetComponent<ParticleSystem>();
                if (ps != null) ps.Play();
                Debug.Log("ParkingFinish: Win VFX activated.");
            }
            //if (levelData.successCamera != null)
            //{
            //    //levelData.successCamera.SetActive(true);
            //    Debug.Log("ParkingFinish: Success camera activated.");
            //}
        }
        else
        {
            Debug.LogWarning("ParkingFinish: LevelData missing – win effects not played.");
        }
        // ============================================================================

        // Optional: disable car controls, play effects, etc.
        Invoke(nameof(LevelComplete), 1f); // Small delay for visual feedback
    }

    public void LevelComplete()
    {
        Debug.LogError("CAR FULLY PARKED!");
        
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.LevelComplete();
            GetComponent<BoxCollider>().enabled = false;
            enabled = false;
        }
        else
            Debug.LogError("MissionManager.Instance is null!");


    }

    // ========== GIZMOS FOR VISUAL DEBUGGING ==========
    private void OnDrawGizmos()
    {
        if (triggerCollider == null)
            triggerCollider = GetComponent<Collider>();

        if (triggerCollider != null)
        {
            Gizmos.color = isCarFullyInside ? Color.green : Color.yellow;
            DrawColliderGizmo(triggerCollider);
        }

        if (playerCar != null && Application.isPlaying)
        {
            Bounds carBounds = GetCombinedBounds(playerCar);
            Gizmos.color = isCarFullyInside ? Color.green : Color.red;
            Gizmos.DrawWireCube(carBounds.center, carBounds.size);
        }
    }

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
        else
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}