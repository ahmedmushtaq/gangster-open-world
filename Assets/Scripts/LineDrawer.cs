using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(LineRenderer))]
public class LineDrawer : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private LineRenderer linerend;
    public GameObject targetobject,bus;
    public Vector3 target;
    private NavMeshPath path;
    public GameObject[] Levels;
    
    // Start is called before the first frame update
    void Start()
    {
        //nav = GetComponent<NavMeshAgent>();
        linerend = GetComponent<LineRenderer>();
        linerend.startWidth = 4f;
        linerend.endWidth = 4f;
        linerend.positionCount = 0;
        path = new NavMeshPath();
        
        //target = targetobject.transform.position;
        //Invoke("setdestination", 5f);
       // nav.isStopped = true;

    }

    // Update is called once per frame
    void Update()
    {
        //if (LevelSelectionDemo.instance.currentLevel == 1)

        //{
        //    target = Levels[0].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    if (!Levels[0].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[0].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 2)

        //{
        //    target = Levels[1].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[1].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[1].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 3)
        //{
        //    target = Levels[2].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[2].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[2].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 4)
        //{
        //    target = Levels[3].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[3].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[3].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[3].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[3].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 5)
        //{
        //    target = Levels[4].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[4].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[4].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[4].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[4].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 6)
        //{
        //    target = Levels[5].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[5].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[5].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[5].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[5].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 7)
        //{
        //    target = Levels[6].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[6].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[6].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[6].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[6].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[6].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[6].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }

        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 8)
        //{
        //    target = Levels[7].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[7].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[7].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[7].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[7].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[7].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[7].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }

        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 9)
        //{
        //    target = Levels[8].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[8].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[8].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[8].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[8].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[8].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[8].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }
        //    if (Levels[8].gameObject.transform.GetChild(0).GetChild(4).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[8].gameObject.transform.GetChild(0).GetChild(4).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 10)
        //{
        //    target = Levels[9].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[9].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[9].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[9].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[9].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[9].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[9].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }
        //    if (Levels[9].gameObject.transform.GetChild(0).GetChild(4).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[9].gameObject.transform.GetChild(0).GetChild(4).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 11)
        //{
        //    target = Levels[10].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[10].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[10].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[10].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[10].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[10].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[10].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }
        //    if (Levels[10].gameObject.transform.GetChild(0).GetChild(4).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[10].gameObject.transform.GetChild(0).GetChild(4).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 12)
        //{
        //    target = Levels[11].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[11].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[11].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[11].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[11].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[11].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[11].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }
        //    if (Levels[11].gameObject.transform.GetChild(0).GetChild(4).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[11].gameObject.transform.GetChild(0).GetChild(4).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 13)
        //{
        //    target = Levels[12].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[12].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[12].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[12].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[12].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[12].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[12].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }
        //    if (Levels[12].gameObject.transform.GetChild(0).GetChild(4).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[12].gameObject.transform.GetChild(0).GetChild(4).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 14)
        //{
        //    target = Levels[13].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[13].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[13].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[13].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[13].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[13].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[13].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }
        //    if (Levels[13].gameObject.transform.GetChild(0).GetChild(4).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[13].gameObject.transform.GetChild(0).GetChild(4).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 15)
        //{
        //    target = Levels[14].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[14].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[14].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[14].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[14].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[14].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[14].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }
        //    if (Levels[14].gameObject.transform.GetChild(0).GetChild(4).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[14].gameObject.transform.GetChild(0).GetChild(4).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 16)
        //{
        //    target = Levels[15].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[15].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[15].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[15].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[15].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[15].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[15].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }
        //    if (Levels[15].gameObject.transform.GetChild(0).GetChild(4).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[15].gameObject.transform.GetChild(0).GetChild(4).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 17)
        //{
        //    target = Levels[16].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[16].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[16].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[16].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[16].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[16].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[16].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }
        //    if (Levels[16].gameObject.transform.GetChild(0).GetChild(4).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[16].gameObject.transform.GetChild(0).GetChild(4).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 18)
        //{
        //    target = Levels[17].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[17].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[17].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[17].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[17].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[17].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[17].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }
        //    if (Levels[17].gameObject.transform.GetChild(0).GetChild(4).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[17].gameObject.transform.GetChild(0).GetChild(4).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 19)
        //{
        //    target = Levels[18].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[18].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[18].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[18].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[18].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[18].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[18].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }
        //    if (Levels[18].gameObject.transform.GetChild(0).GetChild(4).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[18].gameObject.transform.GetChild(0).GetChild(4).gameObject.transform.position;
        //    }
        //}
        //else if (LevelSelectionDemo.instance.currentLevel == 20)
        //{
        //    target = Levels[19].gameObject.transform.GetChild(0).GetChild(0).gameObject.transform.position;
        //    if (!Levels[19].gameObject.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[19].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.position;
        //    }
        //    if (Levels[19].gameObject.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[19].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.position;
        //    }
        //    if (Levels[19].gameObject.transform.GetChild(0).GetChild(3).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[19].gameObject.transform.GetChild(0).GetChild(3).gameObject.transform.position;
        //    }
        //    if (Levels[19].gameObject.transform.GetChild(0).GetChild(4).gameObject.activeInHierarchy)
        //    {
        //        target = Levels[19].gameObject.transform.GetChild(0).GetChild(4).gameObject.transform.position;
        //    }
        //}






        //nav.Move(bus.transform.position);
        //target = targetobject.transform.position;
        //setdestination();
        //if(Input.GetMouseButtonDown(0))
        //{
        //    clicktomove();
        //}
        //if(Vector3.Distance(nav.destination,transform.position) <= nav.stoppingDistance)
        //{
        //    prefab.SetActive(false);
        //}
        /*else*/
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
    //private void clicktomove()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;
    //    bool hashit = Physics.Raycast(ray, out hit);
    //    if(hashit)
    //    {
    //        setdestination(hit.point);
    //    }
    //}
    private void setdestination()
    {
        //prefab.SetActive(true);
        //prefab.transform.position = target;
        //nav.CalculatePath(target, path);
        NavMesh.CalculatePath(bus.transform.position, target, NavMesh.AllAreas, path);
        //NavMesh.CalculatePath(bus.transform.position, target, NavMesh.AllAreas, path);
        //for (int i = 0; i < path.corners.Length - 1; i++)
        //{
        //    Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);

        //    Debug.Log("Running");


        //}
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
