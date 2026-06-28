using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject carSelectionPanel;
    [SerializeField] private GameObject modeSelectionPanel;
    [SerializeField] private GameObject dailyRewardPanel;

    [Header("UI Elements")]
    [SerializeField] private Text mainmenuCoinsText;
    [SerializeField] private Text carSelectionCoinsText;
    [SerializeField] private Text modeSelectionCoinsText;

    [Header("MainMenu Buttons")]
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button dailyRewardBtn;
    [SerializeField] private Button garageBtn;
    [SerializeField] private Button playBtn;

    [Header("Mode Selection Buttons")]
    [SerializeField] private Button mode1Btn;

    private void Awake()
    {
        Instance = this;
        PlayerPrefs.SetInt("Coins", 5000000);
    }

    private void Start()
    {
        quitBtn.onClick.RemoveAllListeners();
        quitBtn.onClick.AddListener(OnQuitClicked);

        dailyRewardBtn.onClick.RemoveAllListeners();
        dailyRewardBtn.onClick.AddListener(OnDailyRewardClicked);

        garageBtn.onClick.RemoveAllListeners();
        garageBtn.onClick.AddListener(OnGarageClicked);

        playBtn.onClick.RemoveAllListeners();
        playBtn.onClick.AddListener(OnPlayClicked);

        mode1Btn.onClick.RemoveAllListeners();
        mode1Btn.onClick.AddListener(OnMode1Clicked);

    }

    private void Update()
    {
        int coins = PlayerPrefs.GetInt("Coins", 0);
        mainmenuCoinsText.text = coins.ToString();
        carSelectionCoinsText.text = coins.ToString();
        modeSelectionCoinsText.text = coins.ToString();
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }

    public void OnDailyRewardClicked()
    {
        dailyRewardPanel.SetActive(true);
    }

    public void OnGarageClicked()
    {
        mainMenuPanel.SetActive(false);
        carSelectionPanel.SetActive(true);
    }

    public void OnPlayClicked()
    {
        mainMenuPanel.SetActive(false);
        modeSelectionPanel.SetActive(true);
    }

    public void OnMode1Clicked()
    {
        // Load the scene for mode 1
        SceneManager.LoadScene(2);
    }

    public void OnCarSelectionNextClicked()
    {
        carSelectionPanel.SetActive(false);
        modeSelectionPanel.SetActive(true);
    }

}
