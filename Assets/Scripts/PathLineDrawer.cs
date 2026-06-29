using UnityEngine;
using SWS;

[RequireComponent(typeof(LineRenderer))]
public class PathLineDrawer : MonoBehaviour
{
    [Header("Path Reference")]
    public PathManager pathManager;      // Drag your PathManager here

    [Header("Line Settings")]
    public float heightOffset = 0.5f;    // Raise line above ground
    public float lineWidth = 2f;
    public bool updateEveryFrame = false; // Set true if waypoints move dynamically

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        RefreshLine();
    }

    void Update()
    {
        if (updateEveryFrame)
            RefreshLine();
    }

    /// <summary>
    /// Updates the LineRenderer to follow all waypoints of the PathManager.
    /// Call this manually if waypoints change.
    /// </summary>
    public void RefreshLine()
    {
        if (pathManager == null)
        {
            Debug.Log("PathLineDrawer: No PathManager assigned!");
            lineRenderer.positionCount = 0;
            return;
        }

        Transform[] waypoints = pathManager.waypoints;
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogWarning("PathLineDrawer: Not enough waypoints to draw a line.");
            lineRenderer.positionCount = 0;
            return;
        }

        // Get world positions of all waypoints
        Vector3[] positions = new Vector3[waypoints.Length];
        float fixedY = transform.position.y + heightOffset; // or use a constant

        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 wp = waypoints[i].position;
            positions[i] = new Vector3(wp.x, fixedY, wp.z);
        }

        // Apply to LineRenderer
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    /// <summary>
    /// Optionally draws only a segment of the path.
    /// </summary>
    public void DrawSegment(int startIndex, int endIndex)
    {
        if (pathManager == null) return;
        Transform[] waypoints = pathManager.waypoints;
        if (waypoints == null || waypoints.Length == 0) return;

        startIndex = Mathf.Clamp(startIndex, 0, waypoints.Length - 1);
        endIndex = Mathf.Clamp(endIndex, 0, waypoints.Length - 1);
        if (startIndex >= endIndex) return;

        int count = endIndex - startIndex + 1;
        Vector3[] positions = new Vector3[count];
        float fixedY = transform.position.y + heightOffset;

        for (int i = 0; i < count; i++)
        {
            Vector3 wp = waypoints[startIndex + i].position;
            positions[i] = new Vector3(wp.x, fixedY, wp.z);
        }

        lineRenderer.positionCount = count;
        lineRenderer.SetPositions(positions);
    }
}