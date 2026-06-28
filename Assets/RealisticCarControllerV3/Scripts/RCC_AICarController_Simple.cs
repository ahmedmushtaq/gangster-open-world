//----------------------------------------------
//            Realistic Car Controller
//            Simple AI Waypoint Follower
//  Based on RCC_AICarController but uses SWS PathManager
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using SWS;  // Required for PathManager

[AddComponentMenu("BoneCracker Games/Realistic Car Controller/AI/RCC AI Car Controller Simple")]
public class RCC_AICarController_Simple : MonoBehaviour
{
    // Reference to the RCC vehicle controller
    internal RCC_CarControllerV3 carController;

    // PathManager from SWS – assign in Inspector
    public PathManager pathManager;

    // Current waypoint index
    public int currentWaypoint = 0;

    // Radius to consider waypoint reached
    public float nextWaypointPassRadius = 40f;

    // Should the path loop?
    public bool loopPath = true;

    // Option to start from the closest waypoint instead of index 0
    public bool useClosestWaypointOnStart = true;

    // Raycast settings (obstacle avoidance)
    public int wideRayLength = 20;
    public int tightRayLength = 20;
    public int sideRayLength = 3;
    public LayerMask obstacleLayers = -1;

    private float rayInput = 0f;
    private bool raycasting = false;
    private float resetTime = 0f;

    // Steering, gas and brake inputs
    private float steerInput = 0f;
    private float gasInput = 0f;
    private float brakeInput = 0f;

    // Speed limiting
    public bool limitSpeed = false;
    public float maximumSpeed = 100f;

    // Smoothed steering
    public bool smoothedSteer = true;

    // Brake zone
    private float maximumSpeedInBrakeZone = 0f;
    private bool inBrakeZone = false;

    // Lap counting
    public int lap = 0;
    public int totalWaypointPassed = 0;

    // NEW: Master toggle to enable/disable AI movement
    public bool isActive = true;   // Set this to false to stop the car

    // Events
    public delegate void onRCCAISpawned(RCC_AICarController_Simple RCCAI);
    public static event onRCCAISpawned OnRCCAISpawned;

    public delegate void onRCCAIDestroyed(RCC_AICarController_Simple RCCAI);
    public static event onRCCAIDestroyed OnRCCAIDestroyed;

    void Start()
    {
        // Get the RCC controller
        carController = GetComponent<RCC_CarControllerV3>();
        carController.externalController = true;

        // If no pathManager assigned, try to find one in the scene
        if (!pathManager)
            pathManager = FindObjectOfType<PathManager>();

        if (!pathManager)
        {
            Debug.LogError("No PathManager found in the scene! Please assign one.");
            enabled = false;
            return;
        }

        // If we have waypoints, optionally start from the closest one
        if (useClosestWaypointOnStart && pathManager.waypoints.Length > 0)
        {
            currentWaypoint = GetClosestWaypointIndex();
        }
    }

    void OnEnable()
    {
        if (OnRCCAISpawned != null)
            OnRCCAISpawned(this);
    }

    void Update()
    {
        if (!carController.canControl || !isActive)   // <-- added !isActive
            return;

        // (Optional) Add any per‑frame logic here
    }

    void FixedUpdate()
    {
        // If not active, zero all inputs and stop
        if (!isActive || !carController.canControl)
        {
            // Ensure car stops
            carController.gasInput = 0f;
            carController.brakeInput = 1f;
            carController.steerInput = 0f;
            return;
        }

        Navigation();
        FixedRaycasts();
        FeedRCC();
        Resetting();
    }

    // ---- NAVIGATION ----
    void Navigation()
    {
        if (pathManager == null || pathManager.waypoints.Length == 0)
        {
            Stop();
            return;
        }

        // Ensure waypoint index is valid
        if (currentWaypoint >= pathManager.waypoints.Length)
            currentWaypoint = 0;

        Transform targetWaypoint = pathManager.waypoints[currentWaypoint];
        if (targetWaypoint == null)
            return;

        // Direction to waypoint in local space
        Vector3 localTarget = transform.InverseTransformPoint(targetWaypoint.position);
        float distance = localTarget.magnitude;

        // Steering: angle between forward and target direction
        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float targetSteer = Mathf.Clamp(targetAngle / 45f, -1f, 1f); // 45° = full lock

        // Combine with raycast avoidance
        steerInput = Mathf.Clamp(targetSteer + rayInput, -1f, 1f) * carController.direction;

        // Throttle and brake
        if (!inBrakeZone)
        {
            // Basic throttle: full when slow, reduce when cornering or at high speed
            float speedFactor = carController.speed / maximumSpeed;
            if (!carController.changingGear)
            {
                gasInput = Mathf.Lerp(0.75f, 1f, 1f - Mathf.Abs(steerInput) * 0.5f);
                if (limitSpeed)
                    gasInput *= Mathf.Clamp01(1f - speedFactor);
            }
            else
                gasInput = 0f;

            // Slight braking while turning at high speed
            if (carController.speed > 25f)
                brakeInput = Mathf.Lerp(0f, 0.25f, Mathf.Abs(steerInput));
            else
                brakeInput = 0f;
        }
        else
        {
            // In brake zone: reduce throttle and apply brake as needed
            float speedDiff = carController.speed - maximumSpeedInBrakeZone;
            if (speedDiff > 0)
            {
                brakeInput = Mathf.Lerp(0f, 1f, speedDiff / maximumSpeedInBrakeZone);
                gasInput = 0f;
            }
            else
            {
                gasInput = Mathf.Lerp(1f, 0f, -speedDiff / maximumSpeedInBrakeZone);
                brakeInput = 0f;
            }
        }

        // Check if we reached the current waypoint
        if (distance < nextWaypointPassRadius)
        {
            totalWaypointPassed++;
            currentWaypoint++;

            if (currentWaypoint >= pathManager.waypoints.Length)
            {
                if (loopPath)
                {
                    currentWaypoint = 0;
                    lap++;
                }
                else
                {
                    // Stop at the end of the path
                    currentWaypoint = pathManager.waypoints.Length - 1;
                    Stop();
                }
            }
        }
    }

    // ---- OBSTACLE AVOIDANCE (raycasts) ----
    void FixedRaycasts()
    {
        Vector3 pivotPos = transform.position;
        pivotPos += transform.forward * carController.FrontLeftWheelCollider.transform.localPosition.z;

        RaycastHit hit;
        bool tightTurn = false, wideTurn = false, sideTurn = false;
        bool tightTurn1 = false, wideTurn1 = false, sideTurn1 = false;
        float newinputSteer1 = 0f, newinputSteer2 = 0f, newinputSteer3 = 0f;
        float newinputSteer4 = 0f, newinputSteer5 = 0f, newinputSteer6 = 0f;

        // Draw and cast rays
        Debug.DrawRay(pivotPos, Quaternion.AngleAxis(25, transform.up) * transform.forward * wideRayLength, Color.white);
        Debug.DrawRay(pivotPos, Quaternion.AngleAxis(-25, transform.up) * transform.forward * wideRayLength, Color.white);
        Debug.DrawRay(pivotPos, Quaternion.AngleAxis(7, transform.up) * transform.forward * tightRayLength, Color.white);
        Debug.DrawRay(pivotPos, Quaternion.AngleAxis(-7, transform.up) * transform.forward * tightRayLength, Color.white);
        Debug.DrawRay(pivotPos, Quaternion.AngleAxis(90, transform.up) * transform.forward * sideRayLength, Color.white);
        Debug.DrawRay(pivotPos, Quaternion.AngleAxis(-90, transform.up) * transform.forward * sideRayLength, Color.white);

        // Wide right
        if (Physics.Raycast(pivotPos, Quaternion.AngleAxis(25, transform.up) * transform.forward, out hit, wideRayLength, obstacleLayers) && !hit.collider.isTrigger && hit.transform.root != transform)
        {
            Debug.DrawRay(pivotPos, Quaternion.AngleAxis(25, transform.up) * transform.forward * wideRayLength, Color.red);
            newinputSteer1 = Mathf.Lerp(-0.5f, 0f, hit.distance / wideRayLength);
            wideTurn = true;
        }

        // Wide left
        if (Physics.Raycast(pivotPos, Quaternion.AngleAxis(-25, transform.up) * transform.forward, out hit, wideRayLength, obstacleLayers) && !hit.collider.isTrigger && hit.transform.root != transform)
        {
            Debug.DrawRay(pivotPos, Quaternion.AngleAxis(-25, transform.up) * transform.forward * wideRayLength, Color.red);
            newinputSteer4 = Mathf.Lerp(0.5f, 0f, hit.distance / wideRayLength);
            wideTurn1 = true;
        }

        // Tight right
        if (Physics.Raycast(pivotPos, Quaternion.AngleAxis(7, transform.up) * transform.forward, out hit, tightRayLength, obstacleLayers) && !hit.collider.isTrigger && hit.transform.root != transform)
        {
            Debug.DrawRay(pivotPos, Quaternion.AngleAxis(7, transform.up) * transform.forward * tightRayLength, Color.red);
            newinputSteer3 = Mathf.Lerp(-1f, 0f, hit.distance / tightRayLength);
            tightTurn = true;
        }

        // Tight left
        if (Physics.Raycast(pivotPos, Quaternion.AngleAxis(-7, transform.up) * transform.forward, out hit, tightRayLength, obstacleLayers) && !hit.collider.isTrigger && hit.transform.root != transform)
        {
            Debug.DrawRay(pivotPos, Quaternion.AngleAxis(-7, transform.up) * transform.forward * tightRayLength, Color.red);
            newinputSteer2 = Mathf.Lerp(1f, 0f, hit.distance / tightRayLength);
            tightTurn1 = true;
        }

        // Side right
        if (Physics.Raycast(pivotPos, Quaternion.AngleAxis(90, transform.up) * transform.forward, out hit, sideRayLength, obstacleLayers) && !hit.collider.isTrigger && hit.transform.root != transform)
        {
            Debug.DrawRay(pivotPos, Quaternion.AngleAxis(90, transform.up) * transform.forward * sideRayLength, Color.red);
            newinputSteer5 = Mathf.Lerp(-1f, 0f, hit.distance / sideRayLength);
            sideTurn = true;
        }

        // Side left
        if (Physics.Raycast(pivotPos, Quaternion.AngleAxis(-90, transform.up) * transform.forward, out hit, sideRayLength, obstacleLayers) && !hit.collider.isTrigger && hit.transform.root != transform)
        {
            Debug.DrawRay(pivotPos, Quaternion.AngleAxis(-90, transform.up) * transform.forward * sideRayLength, Color.red);
            newinputSteer6 = Mathf.Lerp(1f, 0f, hit.distance / sideRayLength);
            sideTurn1 = true;
        }

        raycasting = wideTurn || wideTurn1 || tightTurn || tightTurn1 || sideTurn || sideTurn1;

        if (raycasting)
            rayInput = newinputSteer1 + newinputSteer2 + newinputSteer3 + newinputSteer4 + newinputSteer5 + newinputSteer6;
        else
            rayInput = 0f;
    }

    // ---- FEED INPUTS TO RCC ----
    void FeedRCC()
    {
        // Gas
        if (carController.direction == 1)
        {
            if (!limitSpeed)
                carController.gasInput = gasInput;
            else
                carController.gasInput = gasInput * Mathf.Clamp01(Mathf.Lerp(10f, 0f, carController.speed / maximumSpeed));
        }
        else
            carController.gasInput = 0f;

        // Steer (smoothed)
        if (smoothedSteer)
            carController.steerInput = Mathf.Lerp(carController.steerInput, steerInput, Time.deltaTime * 20f);
        else
            carController.steerInput = steerInput;

        // Brake
        if (carController.direction == 1)
            carController.brakeInput = brakeInput;
        else
            carController.brakeInput = gasInput;
    }

    // ---- RESET IF STUCK ----
    void Resetting()
    {
        if (carController.speed <= 5 && transform.InverseTransformDirection(carController.rigid.linearVelocity).z < 1f)
            resetTime += Time.deltaTime;

        if (resetTime >= 2f)
            carController.direction = -1;

        if (resetTime >= 4f || carController.speed >= 25f)
        {
            carController.direction = 1;
            resetTime = 0f;
        }
    }

    // ---- STOP ----
    void Stop()
    {
        gasInput = 0f;
        steerInput = 0f;
        brakeInput = 1f;
    }

    // ---- BRAKE ZONE TRIGGERS ----
    void OnTriggerEnter(Collider col)
    {
        RCC_AIBrakeZone brakeZone = col.GetComponent<RCC_AIBrakeZone>();
        if (brakeZone)
        {
            inBrakeZone = true;
            maximumSpeedInBrakeZone = brakeZone.targetSpeed;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.GetComponent<RCC_AIBrakeZone>())
        {
            inBrakeZone = false;
            maximumSpeedInBrakeZone = 0f;
        }
    }

    // ---- HELPER: find closest waypoint ----
    int GetClosestWaypointIndex()
    {
        int closest = 0;
        float closestDist = Mathf.Infinity;
        Vector3 pos = transform.position;

        for (int i = 0; i < pathManager.waypoints.Length; i++)
        {
            if (pathManager.waypoints[i] == null) continue;
            float dist = Vector3.SqrMagnitude(pathManager.waypoints[i].position - pos);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = i;
            }
        }
        return closest;
    }

    void OnDestroy()
    {
        if (OnRCCAIDestroyed != null)
            OnRCCAIDestroyed(this);
    }
}