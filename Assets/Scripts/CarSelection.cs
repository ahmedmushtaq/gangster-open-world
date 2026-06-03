using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarSelection : MonoBehaviour
{
    private static CarSelection _instance;
    public static CarSelection Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<CarSelection>();
            return _instance;
        }
        private set => _instance = value;
    }

    [Header("Vehicle References")]
    [SerializeField] private RCC_CarControllerV3[] cars;
    [SerializeField] private RCC_CarControllerV3 activeCar;

    [Header("UI References")]
    [SerializeField] private Button previousBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button selectBtn;

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
        Invoke(nameof(GetActiveCar), 1f); // Delay to ensure RCC_SceneManager has initialized
    }

    private void ValidateReferences()
    {
        if (previousBtn == null) Debug.LogWarning($"{nameof(previousBtn)} is not assigned in {nameof(GameManager)}.");
        if (nextBtn == null) Debug.LogWarning($"{nameof(nextBtn)} is not assigned in {nameof(GameManager)}.");
        if (selectBtn == null) Debug.LogWarning($"{nameof(selectBtn)} is not assigned in {nameof(GameManager)}.");
    }

    private void RegisterButtonListeners()
    {
        if (previousBtn != null)
            previousBtn.onClick.AddListener(() => SwitchToPreviousCar());

        if (nextBtn != null)
            nextBtn.onClick.AddListener(() => SwitchToNextCar());

        if (selectBtn != null)
            selectBtn.onClick.AddListener(() => SelectCar());
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

    private void SwitchToPreviousCar()
    {
        if (cars == null || cars.Length == 0)
        {
            Debug.LogWarning("No cars assigned to switch between.");
            return;
        }

        if (activeCar != null)
            activeCar.gameObject.SetActive(false);

        currentCarIndex = (currentCarIndex - 1 + cars.Length) % cars.Length;
        activeCar = cars[currentCarIndex];

        if (activeCar != null)
            activeCar.gameObject.SetActive(true);
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
    }

    public void SelectCar()
    {
        PlayerPrefs.SetInt("SelectedCarIndex", currentCarIndex);
        SceneManager.LoadScene(1);
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
}
