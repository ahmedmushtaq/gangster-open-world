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

    [Header("Color Customization")]
    [SerializeField] private ColorOption[] colorOptions;      // Define your colors in Inspector
    [SerializeField] private Button[] colorButtons;           // UI buttons for each color
    [SerializeField] private Image[] colorSwatches;           // Images to show the color
    [SerializeField] private GameObject[] colorLockIcons;     // Optional lock icons
    [SerializeField] private GameObject[] colorHighlighters;  // Selection highlights
    [SerializeField] private TextMeshProUGUI colorPriceText;  // If using unlock system

    [System.Serializable]
    public class ColorOption
    {
        public string name;
        public Color color;
        public int price;          // 0 = free
        public bool isLockedByDefault;
    }



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

        // Initialize color buttons
        SetupColorButtons();
        // Load saved color for the current car and apply
        ApplySavedColor();

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

        // Apply saved color for the new car
        ApplySavedColor();
        // Reset color highlights (they'll be set by ApplySavedColor)
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

    private void SetupColorButtons()
    {
        int count = Mathf.Min(colorOptions.Length, colorButtons.Length);
        for (int i = 0; i < count; i++)
        {
            int index = i; // local copy for lambda
            ColorOption opt = colorOptions[index];

            // Set swatch color
            if (colorSwatches != null && index < colorSwatches.Length)
                colorSwatches[index].color = opt.color;

            // Set button listener
            if (colorButtons[index] != null)
            {
                colorButtons[index].onClick.RemoveAllListeners();
                colorButtons[index].onClick.AddListener(() => OnColorButtonClicked(index));
            }

            // Update lock/highlighter state (initially, all unlocked if price=0)
            bool unlocked = (opt.price == 0) || IsColorUnlocked(index);
            if (colorLockIcons != null && index < colorLockIcons.Length)
                colorLockIcons[index].SetActive(!unlocked);
            if (colorHighlighters != null && index < colorHighlighters.Length)
                colorHighlighters[index].SetActive(false);
        }
    }

    private bool IsColorUnlocked(int index)
    {
        // Check if this color has been purchased
        return PlayerPrefs.GetInt($"ColorUnlocked_{activeCar.gameObject.name}_{index}", 0) == 1;
    }

    private void OnColorButtonClicked(int index)
    {
        if (index < 0 || index >= colorOptions.Length) return;

        ColorOption opt = colorOptions[index];
        bool unlocked = (opt.price == 0) || IsColorUnlocked(index);

        if (!unlocked)
        {
            // Optionally show a purchase dialog (see step 3)
            Debug.Log($"Color {opt.name} is locked. Price: {opt.price}");
            // You can call a buy method here
            BuyColor(index);
            return;
        }

        // Apply color to active car
        ApplyColorToCar(opt.color);

        // Update selection highlighter
        for (int i = 0; i < colorHighlighters.Length; i++)
        {
            if (colorHighlighters[i] != null)
                colorHighlighters[i].SetActive(i == index);
        }

        // Save selected color index for this car
        PlayerPrefs.SetInt($"SelectedColor_{activeCar.gameObject.name}", index);
        PlayerPrefs.Save();
    }

    private void ApplyColorToCar(Color color)
    {
        if (activeCar == null) return;

        // Try to get PaintManager on the car
        RCCV3_PaintManager paintManager = activeCar.GetComponent<RCCV3_PaintManager>();
        if (paintManager != null)
        {
            paintManager.Paint(color);
        }
        else
        {
            // Fallback: change all materials that have a color property
            Renderer[] renderers = activeCar.GetComponentsInChildren<Renderer>();
            foreach (var rend in renderers)
            {
                Material[] mats = rend.materials;
                foreach (var mat in mats)
                {
                    if (mat.HasProperty("_Color"))
                        mat.SetColor("_Color", color);
                    else if (mat.HasProperty("_BaseColor"))
                        mat.SetColor("_BaseColor", color);
                }
            }
        }
    }

    private void ApplySavedColor()
    {
        if (activeCar == null) return;
        int savedIndex = PlayerPrefs.GetInt($"SelectedColor_{activeCar.gameObject.name}", 0);
        if (savedIndex >= 0 && savedIndex < colorOptions.Length)
        {
            // Check if unlocked; if not, fallback to first color
            if (colorOptions[savedIndex].price == 0 || IsColorUnlocked(savedIndex))
            {
                ApplyColorToCar(colorOptions[savedIndex].color);
                // Highlight the saved color
                for (int i = 0; i < colorHighlighters.Length; i++)
                {
                    if (colorHighlighters[i] != null)
                        colorHighlighters[i].SetActive(i == savedIndex);
                }
            }
        }
    }

    public void BuyColor(int index)
    {
        if (index < 0 || index >= colorOptions.Length) return;
        ColorOption opt = colorOptions[index];
        if (opt.price <= 0) return; // Already free

        int coins = PlayerPrefs.GetInt("Coins", 0);
        if (coins >= opt.price)
        {
            coins -= opt.price;
            PlayerPrefs.SetInt("Coins", coins);
            PlayerPrefs.SetInt($"ColorUnlocked_{activeCar.gameObject.name}_{index}", 1);
            PlayerPrefs.Save();

            // Refresh UI
            SetupColorButtons();
            // Automatically select the just-bought color
            OnColorButtonClicked(index);
        }
        else
        {
            // Show "not enough coins" message
            Debug.Log("Not enough coins!");
        }
    }
}