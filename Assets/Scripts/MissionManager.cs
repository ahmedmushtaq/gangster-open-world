using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Serializable]
    public class Mission
    {
        public string missionName;
        public string description;
        public int reward;
        public MissionType type;
        public GameObject missionPrefab;
    }

    public int currentMissionIndex = -1;
    public MissionType missionType = MissionType.Parking;

    

    [Header("Mission Popup")]
    public int timeToShowMission = 5;
    public GameObject missionPopUp;
    public Text missionTitleText;
    public Text missionDescriptionText;
    public Text missionRewardText;
    public Button acceptMission;
    public Button declineMission;

    [Header("Current Mission Data")]
    public LevelData levelData;
    public RCC_CarControllerV3 activeCar;
    public LineDrawer lineDrawer;
    public GameObject missionPrefab;

    [Header("Parking Mission")]
    public Mission[] parkingMissions;


    private void Awake()
    {
        Instance = this;

        missionPopUp.SetActive(false);
    }

    private void Start()
    {
        Invoke(nameof(ShowMission), timeToShowMission);
        acceptMission.onClick.AddListener(OnAcceptMission);
        declineMission.onClick.AddListener(OnDeclineMission);
    }

    public void ShowMission()
    {
        missionType = UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(MissionType)).Length) switch
        {
            0 => MissionType.Parking,
            _ => MissionType.Parking
        };

        if (missionType == MissionType.Parking)
        {
            Debug.LogError($"Current{MissionType.Parking}Mission");
            currentMissionIndex = PlayerPrefs.GetInt($"Current{MissionType.Parking}Mission", 0);
            missionTitleText.text = parkingMissions[currentMissionIndex].missionName;
            missionDescriptionText.text = parkingMissions[currentMissionIndex].description;
            missionRewardText.text = $"Reward: {parkingMissions[currentMissionIndex].reward} coins";
            missionPopUp.SetActive(true);
        }
        StartCoroutine(SetTimeScaleSmooth(0.3f, 0f, 0.5f));
    }

    public void OnAcceptMission()
    {
        PlayerPrefs.SetInt($"Current{MissionType.Parking}Mission", currentMissionIndex);
        missionPopUp.SetActive(false);
        StartMission();
        StartCoroutine(SetTimeScaleSmooth(1f, 0f, 0.5f));
    }

    public void OnDeclineMission()
    {
        missionPopUp.SetActive(false);
        Invoke(nameof(ShowMission), timeToShowMission);
        StartCoroutine(SetTimeScaleSmooth(1f, 0f ,0.5f));
    }

    public void StartMission()
    {
        Debug.LogError($"Start {missionType} Mission: {parkingMissions[currentMissionIndex].missionName}");
        activeCar = GameManager.Instance.ActiveCar;
        
        missionPrefab = parkingMissions[currentMissionIndex].missionPrefab;
        levelData = missionPrefab.GetComponent<LevelData>();
        if (levelData != null)
        {
            activeCar.gameObject.SetActive(false);
            activeCar.transform.position = levelData.playerStartingPoint.transform.position;
            activeCar.transform.rotation = levelData.playerStartingPoint.transform.rotation;
            lineDrawer = activeCar.GetComponent<CarData>().LineDrawer;
            lineDrawer.target = levelData.playerEndingPoint.transform.position;
            lineDrawer.gameObject.SetActive(true);
            missionPrefab.SetActive(true);
            activeCar.gameObject.SetActive(true);
        }
    }

    IEnumerator SetTimeScaleWithDelay(float timeScale, float delay)
    {
        yield return new WaitForSeconds(delay);
        float i = Time.timeScale;
       
    }

    IEnumerator SetTimeScaleSmooth(float targetScale, float delay, float transitionDuration)
    {
        // Optional delay before starting the transition
        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay); // real-time unaffected by timeScale

        float startScale = Time.timeScale;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / transitionDuration; // 0..1
            Time.timeScale = Mathf.Lerp(startScale, targetScale, t);
            yield return null; // wait one frame
        }

        // Ensure final exact value
        Time.timeScale = targetScale;
    }

    public void LevelComplete()
    {
        missionPrefab.SetActive(false);
        lineDrawer.gameObject.SetActive(false);
        levelData = null;
        missionPrefab = null;
        activeCar = null;
        lineDrawer = null;
    }
}

public enum MissionType
{
    Parking,
}
