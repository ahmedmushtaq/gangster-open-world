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

    [Header("UI Car References")]
    [SerializeField] private Sprite[] carImages;          // Sprites for each car
    [SerializeField] private Image[] carRenders;          // UI images to display car sprites
    [SerializeField] private Button[] carButtons;         // Buttons for direct car selection
    [SerializeField] private GameObject[] carSelected;    // Highlight indicators
    [SerializeField] private int[] carPrize;              // Price of each car
    [SerializeField] private GameObject carsScrollView;
    [SerializeField] private GameObject colorsScrollView;

    [Header("UI References")]
    [SerializeField] private Button previousBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button selectBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button colorsBtn;
    [SerializeField] private Button carsBtn;



    private int currentCarIndex = 0;

    public RCC_CarControllerV3[] Cars => cars;
    public RCC_CarControllerV3 ActiveCar => activeCar;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"Multiple {nameof(CarSelection)} instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        _instance = this;

        ValidateReferences();
        RegisterButtonListeners();

        Application.targetFrameRate = 60;

    }

    private void Start()
    {
        // Load saved selected car index
        currentCarIndex = PlayerPrefs.GetInt("SelectedCarIndex", 0);
        // Clamp in case array size changed
        currentCarIndex = Mathf.Clamp(currentCarIndex, 0, cars.Length - 1);

        // Activate the correct 3D car
        for (int i = 0; i < cars.Length; i++)
            cars[i].gameObject.SetActive(i == currentCarIndex);
        activeCar = cars[currentCarIndex];

        
        // Assign car sprites to UI renders
        AssignCarSprites();

        // Setup direct selection buttons
        SetupCarButtons();

        // Ensure first car (index 0) is always unlocked
        if (!PlayerPrefs.HasKey("CarOwned_0"))
        {
            PlayerPrefs.SetInt("CarOwned_0", 1);
            PlayerPrefs.Save();
        }

        // Update UI (selected indicator, buy/select buttons)
        UpdateCarUI();

        // Delayed reference for active player vehicle
        Invoke(nameof(GetActiveCar), 1f);
    }

    private void ValidateReferences()
    {
        if (previousBtn == null) Debug.LogWarning($"{nameof(previousBtn)} is not assigned.");
        if (nextBtn == null) Debug.LogWarning($"{nameof(nextBtn)} is not assigned.");
        if (selectBtn == null) Debug.LogWarning($"{nameof(selectBtn)} is not assigned.");
        if (buyBtn == null) Debug.LogWarning($"{nameof(buyBtn)} is not assigned.");
        if (carsBtn == null) Debug.LogWarning($"{nameof(carsBtn)} is not assigned.");
        if (colorsBtn == null) Debug.LogWarning($"{nameof(colorsBtn)} is not assigned.");

        // Ensure arrays have same length
        if (carImages.Length != carRenders.Length)
            Debug.LogError("carImages and carRenders must have the same length.");
        if (carButtons.Length != carRenders.Length)
            Debug.LogError("carButtons and carRenders must have the same length.");
        if (carSelected.Length != carRenders.Length)
            Debug.LogError("carSelected and carRenders must have the same length.");
        if (carPrize.Length != carRenders.Length)
            Debug.LogError("carPrize and carRenders must have the same length.");
    }

    private void RegisterButtonListeners()
    {
        if (previousBtn != null)
            previousBtn.onClick.AddListener(SwitchToPreviousCar);
        if (nextBtn != null)
            nextBtn.onClick.AddListener(SwitchToNextCar);
        if (selectBtn != null)
            selectBtn.onClick.AddListener(SelectCar);
        if (buyBtn != null)
            buyBtn.onClick.AddListener(BuyCar);
        if (carsBtn != null)
            carsBtn.onClick.AddListener(() =>
            {
                if (carsScrollView != null) carsScrollView.SetActive(true);
                if (colorsScrollView != null) colorsScrollView.SetActive(false);
            });
        if (colorsBtn != null)
            colorsBtn.onClick.AddListener(() =>
            {
                if (carsScrollView != null) carsScrollView.SetActive(false);
                if (colorsScrollView != null) colorsScrollView.SetActive(true);
            });
    }

    private void AssignCarSprites()
    {
        int count = Mathf.Min(carImages.Length, carRenders.Length);
        for (int i = 0; i < count; i++)
        {
            if (carRenders[i] != null && carImages[i] != null)
                carRenders[i].sprite = carImages[i];
        }
    }

    private void SetupCarButtons()
    {
        for (int i = 0; i < carButtons.Length; i++)
        {
            int index = i; // local copy for lambda
            if (carButtons[index] != null)
                carButtons[index].onClick.AddListener(() => OnCarButtonClicked(index));
        }
    }

    private void OnCarButtonClicked(int index)
    {
        if (index < 0 || index >= cars.Length) return;

        // Deactivate current car
        if (activeCar != null)
            activeCar.gameObject.SetActive(false);

        // Switch to new car
        currentCarIndex = index;
        activeCar = cars[currentCarIndex];
        if (activeCar != null)
            activeCar.gameObject.SetActive(true);

        // Update UI
        UpdateCarUI();
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
            Debug.LogWarning("No cars assigned.");
            return;
        }

        if (activeCar != null)
            activeCar.gameObject.SetActive(false);

        currentCarIndex = (currentCarIndex - 1 + cars.Length) % cars.Length;
        activeCar = cars[currentCarIndex];

        if (activeCar != null)
            activeCar.gameObject.SetActive(true);

        UpdateCarUI();
    }

    private void SwitchToNextCar()
    {
        if (cars == null || cars.Length == 0)
        {
            Debug.LogWarning("No cars assigned.");
            return;
        }

        if (activeCar != null)
            activeCar.gameObject.SetActive(false);

        currentCarIndex = (currentCarIndex + 1) % cars.Length;
        activeCar = cars[currentCarIndex];

        if (activeCar != null)
            activeCar.gameObject.SetActive(true);

        UpdateCarUI();
    }

    public void SelectCar()
    {
        // Only allow selection if the car is owned
        if (!IsCarOwned(currentCarIndex))
        {
            Debug.Log("Car is not owned, cannot select.");
            return;
        }

        PlayerPrefs.SetInt("SelectedCarIndex", currentCarIndex);
        //SceneManager.LoadScene(1);
        MainMenu.Instance.OnCarSelectionNextClicked();
    }

    private void BuyCar()
    {
        if (currentCarIndex < 0 || currentCarIndex >= carPrize.Length)
            return;

        if (IsCarOwned(currentCarIndex))
        {
            Debug.Log("Car already owned.");
            return;
        }

        int price = carPrize[currentCarIndex];
        int coins = PlayerPrefs.GetInt("Coins", 0);

        if (coins < price)
        {
            Debug.Log($"Not enough coins! Need {price}, have {coins}.");
            // Optionally show a UI message
            return;
        }

        // Deduct coins and mark as owned
        coins -= price;
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt($"CarOwned_{currentCarIndex}", 1);
        PlayerPrefs.Save();

        Debug.Log($"Car {currentCarIndex} bought! Remaining coins: {coins}");

        // Update UI to show Select button instead of Buy
        UpdateCarUI();
    }

    private bool IsCarOwned(int index)
    {
        return PlayerPrefs.GetInt($"CarOwned_{index}", 0) == 1;
    }

    private void UpdateCarUI()
    {
        // Update selected indicator
        for (int i = 0; i < carSelected.Length; i++)
        {
            if (carSelected[i] != null)
                carSelected[i].SetActive(i == currentCarIndex);
        }

        // Show/hide Select and Buy buttons based on ownership
        bool owned = IsCarOwned(currentCarIndex);
        if (selectBtn != null)
            selectBtn.gameObject.SetActive(owned);
        if (buyBtn != null)
            buyBtn.gameObject.SetActive(!owned);

        // Optionally update button texts or interactability
    }

    public void ChangeCarBehaviour(int index)
    {
        if (RCC_Settings.Instance == null)
        {
            Debug.LogError("RCC_Settings.Instance is null.");
            return;
        }
        RCC_Settings.Instance.behaviorSelectedIndex = index;
    }
}