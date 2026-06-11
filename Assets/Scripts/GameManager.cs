using Gley.TrafficSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<GameManager>();
            return _instance;
        }
        private set => _instance = value;
    }

    [Header("Vehicle References")]
    [SerializeField] private RCC_CarControllerV3[] cars;
    [SerializeField] private RCC_CarControllerV3 activeCar;


    [Header("TrafficController")]
    [SerializeField] private TrafficComponent trafficComponent;

    [Header("UI References")]
    [SerializeField] private Button behaviour1Btn;
    [SerializeField] private Button behaviour2Btn;
    [SerializeField] private Button behaviour3Btn;
    [SerializeField] private Button changeCarBtn;
    [SerializeField] private Button carSelectionBtn;

    private int currentCarIndex = 0;

    public RCC_CarControllerV3[] Cars => cars;
    public RCC_CarControllerV3 ActiveCar => activeCar;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"Multiple {nameof(GameManager)} instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        _instance = this;

        ValidateReferences();
        RegisterButtonListeners();

        Application.targetFrameRate = 60; // Set target frame rate for smoother gameplay
    }

    private void Start()
    {
        currentCarIndex = PlayerPrefs.GetInt("SelectedCarIndex", 0);
        cars[currentCarIndex].gameObject.SetActive(true);
        activeCar = cars[currentCarIndex];
        trafficComponent.player = activeCar.transform; // Set the traffic system's player reference to the active car
        trafficComponent.gameObject.SetActive(true);
        Invoke(nameof(GetActiveCar), 1f); // Delay to ensure RCC_SceneManager has initialized
    }

    private void ValidateReferences()
    {
        if (behaviour1Btn == null) Debug.LogWarning($"{nameof(behaviour1Btn)} is not assigned in {nameof(GameManager)}.");
        if (behaviour2Btn == null) Debug.LogWarning($"{nameof(behaviour2Btn)} is not assigned in {nameof(GameManager)}.");
        if (behaviour3Btn == null) Debug.LogWarning($"{nameof(behaviour3Btn)} is not assigned in {nameof(GameManager)}.");
        if (changeCarBtn == null) Debug.LogWarning($"{nameof(changeCarBtn)} is not assigned in {nameof(GameManager)}.");
        if (carSelectionBtn == null) Debug.LogWarning($"{nameof(carSelectionBtn)} is not assigned in {nameof(GameManager)}.");
    }

    private void RegisterButtonListeners()
    {
        if (behaviour1Btn != null)
            behaviour1Btn.onClick.AddListener(() => ChangeCarBehaviour(3));

        if (behaviour2Btn != null)
            behaviour2Btn.onClick.AddListener(() => ChangeCarBehaviour(4));

        if (behaviour3Btn != null)
            behaviour3Btn.onClick.AddListener(() => ChangeCarBehaviour(5));

        if (changeCarBtn != null)
            changeCarBtn.onClick.AddListener(SwitchToNextCar);

        if (carSelectionBtn != null)
            carSelectionBtn.onClick.AddListener(BacktoCarSelection);
    }

    public void GetActiveCar()
    {
        if (RCC_SceneManager.Instance == null || RCC_SceneManager.Instance.activePlayerVehicle == null)
            return;

        if (RCC_SceneManager.Instance.activePlayerVehicle.TryGetComponent(out RCC_CarControllerV3 carController))
        {
            activeCar = carController;
        }
    }

    private void SwitchToNextCar()
    {
        if (cars == null || cars.Length == 0)
        {
            Debug.LogWarning("No cars assigned to switch between.");
            return;
        }

        if (activeCar != null)
            activeCar.gameObject.SetActive(false);

        currentCarIndex = (currentCarIndex + 1) % cars.Length;
        activeCar = cars[currentCarIndex];

        if (activeCar != null)
            activeCar.gameObject.SetActive(true);

        PlayerPrefs.SetInt("SelectedCarIndex", currentCarIndex);
        PlayerPrefs.Save();
    }

    public void ChangeCarBehaviour(int index)
    {
        if (RCC_Settings.Instance == null)
        {
            Debug.LogError("RCC_Settings.Instance is null. Cannot change behavior.");
            return;
        }

        RCC_Settings.Instance.behaviorSelectedIndex = index;
    }

    public void BacktoCarSelection()
    {
        //PlayerPrefs.SetInt("SelectedCarIndex", currentCarIndex);
        SceneManager.LoadScene(0);
    }
}
