using UnityEngine;

public class LevelData : MonoBehaviour
{
    public MissionType missionType;
    public GameObject playerStartingPoint;
    public GameObject playerEndingPoint;
    //public GameObject playerStartCamera;

    public GameObject[] aiCars;
    public GameObject[] aiPositions;

    [Header("Time Limit")]
    public float timeLimit = 60f;   // seconds for this level

    [Header("Win Effects")]
    public GameObject winVFX;           // Particle system or VFX to play on successful parking
    //public GameObject successCamera;    // Camera to activate when parking is complete
}
