using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(LineRenderer))]
public class LineDrawer : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private LineRenderer linerend;
    public float lineRendererWidth = 2;
    public GameObject targetobject,bus;
    public Vector3 target;
    private NavMeshPath path;
    public GameObject[] Levels;
    
    // Start is called before the first frame update
    void Start()
    {
        linerend = GetComponent<LineRenderer>();
        linerend.startWidth = lineRendererWidth;
        linerend.endWidth = lineRendererWidth;
        linerend.positionCount = 0;
        path = new NavMeshPath();

    }

    // Update is called once per frame
    void Update()
    {
        //setdestination();
        //if (path.status == NavMeshPathStatus.PathComplete)
        //{

        //    //drawpath();
        //    DrawStraightLine();
        //}

        // Calculate the path (you can call this only when target changes, but every frame is ok)
        SetDestination();

        // If a valid path exists, draw it at fixed height
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            DrawPath();
        }
    }
    private void setdestination()
    {
        NavMesh.CalculatePath(bus.transform.position, target, NavMesh.AllAreas, path);
    }

    void drawpath()
    {
        linerend.positionCount = path.corners.Length;
        linerend.SetPosition(0, transform.position);
        if (path.corners.Length < 2)
        {
            return;
        }
        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 pointposition = new Vector3(path.corners[i].x, path.corners[i].y + 3f, path.corners[i].z);// + new Vector3(0,0,0) ;
            linerend.SetPosition(i, pointposition);
        }
    }

    void DrawStraightLine()
    {
        if (bus == null) return;

        // Get the Y level of this GameObject (the LineDrawer)
        float fixedY = transform.position.y;

        // Start point: bus position, but with fixed Y
        Vector3 start = new Vector3(bus.transform.position.x, fixedY, bus.transform.position.z);

        // End point: target position, but with fixed Y
        Vector3 end = new Vector3(target.x, fixedY, target.z);

        // Assign the two points to the LineRenderer
        linerend.SetPosition(0, start);
        linerend.SetPosition(1, end);
    }

    private void SetDestination()
    {
        // Calculate path from bus to target using all areas
        NavMesh.CalculatePath(bus.transform.position, target, NavMesh.AllAreas, path);
    }

    void DrawPath()
    {
        if (path.corners.Length < 2)
        {
            // Not enough points to draw a line
            linerend.positionCount = 0;
            return;
        }

        // Set number of line points to match path corners
        linerend.positionCount = path.corners.Length;

        // Fixed Y height (use the LineDrawer's Y, or any constant like 0)
        float fixedY = transform.position.y;

        // Set the first point (bus position) with fixed Y
        Vector3 startPoint = new Vector3(bus.transform.position.x, fixedY, bus.transform.position.z);
        linerend.SetPosition(0, startPoint);

        // Set remaining corners (skip index 0 because it's nearly the same as bus position)
        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 corner = path.corners[i];
            Vector3 point = new Vector3(corner.x, fixedY, corner.z);
            linerend.SetPosition(i, point);
        }
    }
}
