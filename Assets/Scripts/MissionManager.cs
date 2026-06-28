using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    

    [Header("Cinematic Camera")]
    public Animator cinematicCamera;

    [Header("HUD Navigation")]
    public GameObject indicatorCanvas;
    public GameObject miniMapCanvas;


    [Header("Mission Popup")]
    public int timeToShowMission = 5;
    public float autoAcceptDelay = 8f;                 // NEW: seconds before auto‑accept
    public GameObject missionPopUp;
    public Text missionTitleText;
    public Text missionDescriptionText;
    public Text missionRewardText;
    public Text countdownTimerText;                    // NEW: UI Text for the timer
    public Button acceptMission;
    public Button declineMission;
    
    [Header("Level Pause Panel")]
    public GameObject levelPausePanel;
    public Button pauseBtn;
    public Button resumeBtn;
    public Button homeBtn;

    // --- NEW: Audio sliders in pause panel ---
    [Header("Pause Panel Audio")]
    public Slider musicSlider;
    public Slider soundSlider;

    [Header("Level Finish Panel")]
    public GameObject levelFinishPanel;
    public Text finishTitleText;
    public Text finishlevelText;
    public Text finishRewardText;
    public Text finishBonusRewardText;
    public Text finishTotalRewardText;
    public Button watchVideoX3;
    public Button garageBtn;
    public Button missOutBtn;
    public Button nextMissionBtn;
    public Button restartBtn;
    public Color winColor;
    public Color failColor;

    [Header("Loading Sequence")]
    public GameObject loadingScreenPanel;          // Panel that covers the screen
    public float loadingDuration = 2f;             // How long the loading screen stays visible
    //public Animator cameraAnimator;                // Animator on the camera that plays the "around vehicle" animation
    public GameObject goText;                      // "GO!" text object (enable/disable)
    public float goTextDisplayTime = 1f;           // How long "GO!" stays on screen
    public GameObject rccCanvas;                   // RCC UI Canvas (disables during loading)
    public GameObject rccCamera;                   // RCC Camera (disable during loading, re-enable after)

    [Header("Level Timer & Parking Countdown")]
    public Text levelTimerText;            // Shows total elapsed time since mission start
    public GameObject fillBar;                // Fill image (type = Filled) for parking countdown
    public Image fillImage;                // Fill image (type = Filled) for parking countdown
    public Text fillText;                // Fill text (type = Filled) for parking countdown
    public float requiredParkTime = 3f;    // How many seconds car must stay fully inside


    [Header("Current Mission Data")]
    public LevelData levelData;
    public RCC_CarControllerV3 activeCar;
    public LineDrawer lineDrawer;
    public GameObject missionPrefab;

    [Header("Parking Mission")]
    public Mission[] parkingMissions;

    [Header("Jump Trail Mission")]
    public Mission[] jumpTrailMissions;

    [Header("Mega Ramp Mission")]
    public Mission[] megaRampMissions;

    [Header("Race Mission")]
    public Mission[] raceMissions;

    private Coroutine autoAcceptCoroutine;             // NEW: reference to the countdown coroutine

    private float remainingTime = 0f;
    private bool levelCompleted = false;    // true when player has successfully completed the mission
    private bool isLevelActive = false;   // true while the mission is running (not completed/failed)
    private int currentBaseReward;
    private int currentBonusReward;
    private bool isFinishPanelActive = false;

    // --- NEW: Pause state ---
    private bool isPaused = false;
    private float engineVolumeBackup = 0f; // to restore engine audio volume

    private void Awake()
    {
        Instance = this;

        missionPopUp.SetActive(false);
        loadingScreenPanel.SetActive(false);
        goText.SetActive(false);

        // Ensure pause panel is hidden at start
        if (levelPausePanel != null) levelPausePanel.SetActive(false);
    }

    private void Start()
    {
        Invoke(nameof(ShowMission), timeToShowMission);
        if (acceptMission != null) acceptMission.onClick.AddListener(OnAcceptMission);
        if (declineMission != null) declineMission.onClick.AddListener(OnDeclineMission);
        if (pauseBtn != null) pauseBtn.onClick.AddListener(PauseGame);
        if (resumeBtn != null) resumeBtn.onClick.AddListener(ResumeGame);
        if (homeBtn != null) homeBtn.onClick.AddListener(GoHome);

        // --- Audio sliders ---
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        if (soundSlider != null)
            soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);

        // Load saved volumes and apply
        LoadAudioSettings();
    }

    public void ShowMission()
    {
        // Stop any pending auto‑accept if the popup is shown again early
        if (autoAcceptCoroutine != null)
        {
            StopCoroutine(autoAcceptCoroutine);
            autoAcceptCoroutine = null;
        }

        missionType = UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(MissionType)).Length) switch
        {
            0 => MissionType.Parking,
            1 => MissionType.JumpTrail,
            2 => MissionType.MegaRamp,
            3 => MissionType.Race,
            _ => MissionType.Parking
        };

        if (missionType == MissionType.Parking)
        {
            Debug.LogError($"Current{missionType}Mission " +
                PlayerPrefs.GetInt($"Current{missionType}Mission"));
            currentMissionIndex = PlayerPrefs.GetInt($"Current{missionType}Mission", 0);
            missionTitleText.text = parkingMissions[currentMissionIndex].missionName;
            missionDescriptionText.text = parkingMissions[currentMissionIndex].description;
            missionRewardText.text = $"Reward: {parkingMissions[currentMissionIndex].reward} coins";
            missionPopUp.SetActive(true);

            // NEW: start the auto‑accept countdown
            autoAcceptCoroutine = StartCoroutine(AutoAcceptCountdown());
        }
        else if (missionType == MissionType.JumpTrail)
        {
            Debug.LogError($"Current{missionType}Mission " +
                PlayerPrefs.GetInt($"Current{missionType}Mission"));
            currentMissionIndex = PlayerPrefs.GetInt($"Current{missionType}Mission", 0);
            missionTitleText.text = jumpTrailMissions[currentMissionIndex].missionName;
            missionDescriptionText.text = jumpTrailMissions[currentMissionIndex].description;
            missionRewardText.text = $"Reward: {jumpTrailMissions[currentMissionIndex].reward} coins";
            missionPopUp.SetActive(true);

            // NEW: start the auto‑accept countdown
            autoAcceptCoroutine = StartCoroutine(AutoAcceptCountdown());
        }
        else if (missionType == MissionType.MegaRamp)
        {
            Debug.LogError($"Current{missionType}Mission " +
                PlayerPrefs.GetInt($"Current{missionType}Mission"));
            currentMissionIndex = PlayerPrefs.GetInt($"Current{missionType}Mission", 0);
            missionTitleText.text = megaRampMissions[currentMissionIndex].missionName;
            missionDescriptionText.text = megaRampMissions[currentMissionIndex].description;
            missionRewardText.text = $"Reward: {megaRampMissions[currentMissionIndex].reward} coins";
            missionPopUp.SetActive(true);

            // NEW: start the auto‑accept countdown
            autoAcceptCoroutine = StartCoroutine(AutoAcceptCountdown());
        }
        else if (missionType == MissionType.Race)
        {
            Debug.LogError($"Current{missionType}Mission " +
                PlayerPrefs.GetInt($"Current{missionType}Mission"));
            currentMissionIndex = PlayerPrefs.GetInt($"Current{missionType}Mission", 0);
            missionTitleText.text = raceMissions[currentMissionIndex].missionName;
            missionDescriptionText.text = raceMissions[currentMissionIndex].description;
            missionRewardText.text = $"Reward: {raceMissions[currentMissionIndex].reward} coins";
            missionPopUp.SetActive(true);

            // NEW: start the auto‑accept countdown
            autoAcceptCoroutine = StartCoroutine(AutoAcceptCountdown());
        }
        StartCoroutine(SetTimeScaleSmooth(0.3f, 0f, 0.5f));
    }

    // NEW: countdown coroutine that uses real‑time (unscaled) seconds
    private IEnumerator AutoAcceptCountdown()
    {
        float remaining = autoAcceptDelay;
        while (remaining > 0f)
        {
            if (countdownTimerText != null)
                countdownTimerText.text = $"{remaining:F0}s";
            yield return new WaitForSecondsRealtime(1f);
            remaining -= 1f;
        }

        if (countdownTimerText != null)
            countdownTimerText.text = "0s";

        // Auto‑accept when timer reaches zero
        OnAcceptMission();
    }

    public void OnAcceptMission()
    {
        // Cancel the countdown
        if (autoAcceptCoroutine != null)
        {
            StopCoroutine(autoAcceptCoroutine);
            autoAcceptCoroutine = null;
        }

        //PlayerPrefs.SetInt($"Current{missionType}Mission", currentMissionIndex);
        missionPopUp.SetActive(false);
        //StartMission();
        StartCoroutine(LoadingSequence());
        StartCoroutine(SetTimeScaleSmooth(1f, 0f, 0.5f));
    }

    public void OnDeclineMission()
    {
        // Cancel the countdown
        if (autoAcceptCoroutine != null)
        {
            StopCoroutine(autoAcceptCoroutine);
            autoAcceptCoroutine = null;
        }

        missionPopUp.SetActive(false);
        Invoke(nameof(ShowMission), timeToShowMission);
        StartCoroutine(SetTimeScaleSmooth(1f, 0f ,0.5f));
    }

    // --- NEW: Pause methods ---
    public void PauseGame()
    {
        // Do not pause if finish panel is active (mission ended)
        if (isFinishPanelActive) return;
        if (isPaused) return; // already paused

        isPaused = true;
        Time.timeScale = 0f;

        if (levelPausePanel != null) levelPausePanel.SetActive(true);
        
        GameManager.Instance.ActiveCar.gameObject.SetActive(false);
        
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        if (levelPausePanel != null) levelPausePanel.SetActive(false);

        GameManager.Instance.ActiveCar.gameObject.SetActive(true);
    }

    public void GoHome()
    {
        // Clean up and load garage (scene index 0)
        Time.timeScale = 1f; // ensure time is normal
        isPaused = false;
        if (levelPausePanel != null) levelPausePanel.SetActive(false);
        loadingScreenPanel.SetActive(true);

        // Clean up current mission (similar to OnGarageClicked)
        CloseFinishPanelAndReset(); // we can reuse this, but it also resets finish panel; fine.
        SceneManager.LoadScene(1);
    }

    private void Update()
    {
        // --- NEW: Skip timer update if paused ---
        if (isPaused) return;

        // Countdown timer
        if (isLevelActive && !levelCompleted && remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                UpdateTimerDisplay();
                LevelFailed();   // handle timeout
            }
        }
    }

    // NEW: Prepares the mission (positions car, sets up line drawer) without enabling RCC UI/Camera
    private void PrepareMission()
    {
        activeCar = GameManager.Instance.ActiveCar;
        levelCompleted = false;
        isLevelActive = false;   // will be set true after loading sequence
        remainingTime = 0f;
        if(missionType == MissionType.Parking)
        {
            Debug.Log($"Prepare {missionType} Mission: {parkingMissions[currentMissionIndex].missionName}");
            missionPrefab = Instantiate(parkingMissions[currentMissionIndex].missionPrefab);
        }
        else if(missionType == MissionType.JumpTrail)
        {
            Debug.Log($"Prepare {missionType} Mission: {jumpTrailMissions[currentMissionIndex].missionName}");
            missionPrefab = Instantiate(jumpTrailMissions[currentMissionIndex].missionPrefab);
        }
        else if(missionType == MissionType.MegaRamp)
        {
            Debug.Log($"Prepare {missionType} Mission: {megaRampMissions[currentMissionIndex].missionName}");
            missionPrefab = Instantiate(megaRampMissions[currentMissionIndex].missionPrefab);
        }
        else if(missionType == MissionType.Race)
        {
            Debug.Log($"Prepare {missionType} Mission: {raceMissions[currentMissionIndex].missionName}");
            missionPrefab = Instantiate(raceMissions[currentMissionIndex].missionPrefab);
        }

        levelData = missionPrefab.GetComponent<LevelData>();
        if (levelData != null)
        {
            GameManager.Instance.DestroyandInstantiateCar(levelData.playerStartingPoint);
            activeCar = GameManager.Instance.ActiveCar;
            lineDrawer = activeCar.GetComponent<CarData>().LineDrawer;
            lineDrawer.target = levelData.playerEndingPoint.transform.position;
            lineDrawer.gameObject.SetActive(true);
            missionPrefab.SetActive(true);

            activeCar.gameObject.SetActive(true);
            activeCar.GetComponent<Rigidbody>().isKinematic = false;
            activeCar.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }

        if (missionType == MissionType.Race)
        {
            for(int i = 0; i < levelData.aiPositions.Length; i++)
            {
                levelData.aiCars[i].transform.position = levelData.aiPositions[i].transform.position;
                levelData.aiCars[i].transform.rotation = levelData.aiPositions[i].transform.rotation;
                levelData.aiCars[i].gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator LoadingSequence()
    {

        indicatorCanvas.SetActive(false);
        // 2. Prepare the mission (car position, etc.)
        PrepareMission();

        // 3. Show loading screen
        if (loadingScreenPanel != null) loadingScreenPanel.SetActive(true);

        // Wait for loading duration (real time, unaffected by time scale)
        yield return new WaitForSecondsRealtime(loadingDuration);

        // 4. Hide loading screen
        if (loadingScreenPanel != null) loadingScreenPanel.SetActive(false);

        // After all loading, ensure pause state is reset
        isPaused = false;
        if (levelPausePanel != null) levelPausePanel.SetActive(false);
        Time.timeScale = 1f;

        // 4.1. Disable RCC Canvas and Camera
        if (rccCanvas != null) rccCanvas.SetActive(false);
        //if (rccCamera != null) rccCamera.SetActive(false);
        if (rccCamera != null) rccCamera.GetComponent<RCC_Camera>().cameraMode = RCC_Camera.CameraMode.CINEMATIC;
        cinematicCamera.Play(cinematicCamera.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
        lineDrawer.gameObject.SetActive(false);
        
        yield return new WaitForSecondsRealtime(2f);

        // 6. Re-enable RCC Canvas and Camera
        if (rccCanvas != null) rccCanvas.SetActive(true);

        if(missionType == MissionType.Race)
        {
            activeCar.GetComponent<RCC_CarControllerV3>().canControl = false;
        }

        //if (rccCamera != null) rccCamera.SetActive(true);
        if (rccCamera != null) rccCamera.GetComponent<RCC_Camera>().cameraMode = RCC_Camera.CameraMode.TPS;
        //if(missionType == MissionType.Parking)
        //{
        //    lineDrawer.gameObject.SetActive(true);
        //}
        levelTimerText.gameObject.SetActive(true);
        isLevelActive = true;   // will be set true after loading sequence
        remainingTime = levelData.timeLimit;
        // 7. Show "GO!" text
        if (goText != null)
        {
            goText.SetActive(true);
            yield return new WaitForSecondsRealtime(goTextDisplayTime);
            goText.SetActive(false);
        }

        if (missionType == MissionType.Race)
        {
            for (int i = 0; i < levelData.aiPositions.Length; i++)
            {
                activeCar.GetComponent<RCC_CarControllerV3>().canControl = true;
                levelData.aiCars[i].GetComponent<RCC_CarControllerV3>().canControl = true;
                levelData.aiCars[i].GetComponent<RCC_AICarController_Simple>().isActive = true;
            }
        }

        // 8. Start level timer
        StartLevelTimer();

        // 8. Mission is now fully active
        Debug.Log("Mission started, controls enabled.");
    }

    private void StartLevelTimer()
    {
        if (levelData != null)
            remainingTime = levelData.timeLimit;
        else
            remainingTime = 60f;   // fallback

        isLevelActive = true;
        UpdateTimerDisplay();      // initial display
        Debug.Log($"Level timer started: {remainingTime} seconds");
    }

    private void UpdateTimerDisplay()
    {
        if (levelTimerText == null) return;

        int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(remainingTime));
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        if (hours > 0)
            levelTimerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        else
            levelTimerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
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
        if (levelCompleted) return;
        // If paused, resume before completing
        if (isPaused) ResumeGame();
        levelCompleted = true;
        isLevelActive = false;

        // Stop any running timers
        if (fillBar != null) fillBar.SetActive(false);
        if (levelTimerText != null) levelTimerText.gameObject.SetActive(false);

        // Calculate reward (no bonus yet)
        if(missionType == MissionType.Parking)
        {
            currentBaseReward = parkingMissions[currentMissionIndex].reward;
        }
        else if (missionType == MissionType.JumpTrail)
        {
            currentBaseReward = jumpTrailMissions[currentMissionIndex].reward;
            //activeCar.GetComponent<Rigidbody>().isKinematic = true;
        }
        else if (missionType == MissionType.MegaRamp)
        {
            currentBaseReward = megaRampMissions[currentMissionIndex].reward;
        }
        else if (missionType == MissionType.Race)
        {
            currentBaseReward = raceMissions[currentMissionIndex].reward;
        }

        int reward = currentBaseReward;
        ShowFinishPanel(true, reward);
    }

    public void LevelFailed()
    {
        if (levelCompleted) return;
        if (isPaused) ResumeGame();
        levelCompleted = true;
        isLevelActive = false;

        // Stop timers
        if (fillBar != null) fillBar.SetActive(false);
        if (levelTimerText != null) levelTimerText.gameObject.SetActive(false);

        // No reward on failure
        ShowFinishPanel(false, 0);
    }

    private void ShowFinishPanel(bool success, int baseReward)
    {
        if (isFinishPanelActive) return;
        isFinishPanelActive = true;

        // Pause game time
        //Time.timeScale = 0f;

        // Disable car controls and UI
        if (rccCanvas != null) rccCanvas.SetActive(false);
        //if (rccCamera != null) rccCamera.SetActive(false);
        if (rccCamera != null) rccCamera.GetComponent<RCC_Camera>().cameraMode = RCC_Camera.CameraMode.CINEMATIC;
        cinematicCamera.Play(cinematicCamera.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
        if (lineDrawer != null) lineDrawer.gameObject.SetActive(false);

        // Setup panel content
        levelFinishPanel.SetActive(true);

        if (success)
        {
            watchVideoX3.gameObject.SetActive(true);
            restartBtn.gameObject.SetActive(false);
            if (levelData.winVFX)
                levelData.winVFX.SetActive(true);
            finishTitleText.text = "FINISH";
            finishTitleText.color = winColor;

            if(missionType == MissionType.Parking && currentMissionIndex + 1 < parkingMissions.Length)
            {
                PlayerPrefs.SetInt($"Current{missionType}Mission",
                    PlayerPrefs.GetInt($"Current{missionType}Mission", 0) + 1);
            }
            else if (missionType == MissionType.JumpTrail && currentMissionIndex + 1 < jumpTrailMissions.Length)
            {
                PlayerPrefs.SetInt($"Current{missionType}Mission",
                    PlayerPrefs.GetInt($"Current{missionType}Mission", 0) + 1);
            }
            else if (missionType == MissionType.MegaRamp && currentMissionIndex + 1 < megaRampMissions.Length)
            {
                PlayerPrefs.SetInt($"Current{missionType}Mission",
                    PlayerPrefs.GetInt($"Current{missionType}Mission", 0) + 1);
            }
            else if (missionType == MissionType.Race && currentMissionIndex + 1 < raceMissions.Length)
            {
                PlayerPrefs.SetInt($"Current{missionType}Mission",
                    PlayerPrefs.GetInt($"Current{missionType}Mission", 0) + 1);
            }



            // Get current coins
            int currentCoins = PlayerPrefs.GetInt("Coins", 0);
            currentBaseReward = baseReward;
            currentBonusReward = 0;

            finishRewardText.text = currentBaseReward.ToString();
            finishBonusRewardText.text = "0";
            finishTotalRewardText.text = currentBaseReward.ToString();

            // Add base reward to coins (will be updated if video watched)
            int newTotal = currentCoins + currentBaseReward;
            PlayerPrefs.SetInt("Coins", newTotal);
            PlayerPrefs.Save();
        }
        else
        {
            watchVideoX3.gameObject.SetActive(false);
            restartBtn.gameObject.SetActive(true);
            //if (rccCamera != null) rccCamera.SetActive(true);
            if (rccCamera != null) rccCamera.GetComponent<RCC_Camera>().cameraMode = RCC_Camera.CameraMode.CINEMATIC;
            cinematicCamera.Play(cinematicCamera.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
            finishTitleText.text = "FAIL";
            finishTitleText.color = failColor;
            finishRewardText.text = "0";
            finishBonusRewardText.text = "0";
            finishTotalRewardText.text = "0";
        }

        if(missionType == MissionType.Parking)
        {
            finishlevelText.text = parkingMissions[currentMissionIndex].missionName;
        }
        else if(missionType == MissionType.JumpTrail)
        {
            finishlevelText.text = jumpTrailMissions[currentMissionIndex].missionName;
            //activeCar.GetComponent<Rigidbody>().isKinematic = false;
        }
        else if(missionType == MissionType.MegaRamp)
        {
            finishlevelText.text = megaRampMissions[currentMissionIndex].missionName;
        }
        else if(missionType == MissionType.Race)
        {
            finishlevelText.text = raceMissions[currentMissionIndex].missionName;
        }

        // Setup button listeners (remove previous to avoid duplicates)
        watchVideoX3.onClick.RemoveAllListeners();
        garageBtn.onClick.RemoveAllListeners();
        missOutBtn.onClick.RemoveAllListeners();
        nextMissionBtn.onClick.RemoveAllListeners();
        restartBtn.onClick.RemoveAllListeners();

        watchVideoX3.onClick.AddListener(OnWatchVideoClicked);
        garageBtn.onClick.AddListener(OnGarageClicked);
        missOutBtn.onClick.AddListener(OnMissOutClicked);
        nextMissionBtn.onClick.AddListener(OnNextMissionClicked);
        restartBtn.onClick.AddListener(OnRestartClicked);

        // Disable watch button if already used or failure
        watchVideoX3.interactable = success;
    }

    //public void LevelComplete()
    //{
    //    // Deactivate mission objects
    //    //if (missionPrefab != null) missionPrefab.SetActive(false);
    //    if (lineDrawer != null) lineDrawer.gameObject.SetActive(false);
    //    //if (levelData != null)
    //    //{
    //    //    if (levelData.successCamera != null) levelData.successCamera.SetActive(false);
    //    //    if (levelData.winVFX != null) levelData.winVFX.SetActive(false);
    //    //}

    //    //levelData = null;
    //    //missionPrefab = null;
    //    //activeCar = null;
    //    lineDrawer = null;
    //    playerStartCamera = null;

    //    // Reset timers
    //    levelCompleted = false;
    //    isLevelActive = false;
    //    remainingTime = 0f;
    //    if (fillBar != null) fillBar.SetActive(false);
    //    if (levelTimerText != null) levelTimerText.gameObject.SetActive(false);

    //    levelData.winVFX.SetActive(true);
    //    levelData.successCamera.SetActive(true);
    //    if (rccCanvas != null) rccCanvas.SetActive(false);
    //    if (rccCamera != null) rccCamera.SetActive(false);

    //    // Schedule next mission
    //    //CancelInvoke();
    //    //Invoke(nameof(ShowMission), timeToShowMission);
    //}

    public void OnCarFullyInside()
    {
        Debug.LogError("Car fully inside trigger detected in MissionManager.");
        if (levelCompleted) return;
        // Show the fill image – it will be updated via ParkingFinish's own timer
        if (fillBar != null)
        {
            fillBar.SetActive(true);
            fillImage.fillAmount = 1f;
        }
        Debug.Log("Car fully inside! Parking countdown started (managed by ParkingFinish).");
    }

    public void OnCarExited()
    {
        if (levelCompleted) return;
        // Hide fill image when car leaves
        if (fillBar != null)
            fillBar.SetActive(false);
        Debug.Log("Car left full area, countdown reset.");
    }

    //public void OnParkingSuccess()
    //{
    //    if (levelCompleted) return;
    //    levelCompleted = true;
    //    isLevelActive = false;          // stop the countdown
    //    if (fillBar != null) fillBar.gameObject.SetActive(false);
    //    Debug.Log("Parking success reported by ParkingFinish.");
    //}

    public void OnParkingSuccess()
    {
        if (levelCompleted) return;
        LevelComplete();   // This now shows the finish panel instead of immediate cleanup
    }

    private IEnumerator DelayedLevelComplete(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        LevelComplete();
    }

    //private void LevelFailed()
    //{
    //    if (levelCompleted) return;
    //    levelCompleted = true;
    //    isLevelActive = false;
    //    Debug.Log("LEVEL FAILED – Time ran out!");

    //    // Optional: disable car controls, show fail popup, then reschedule mission
    //    // For now, just clean up and show a new mission after a delay
    //    StartCoroutine(DelayedLevelFailure());
    //}

    private IEnumerator DelayedLevelFailure()
    {
        yield return new WaitForSecondsRealtime(2f);
        // Clean up the current mission
        if (missionPrefab != null) missionPrefab.SetActive(false);
        if (lineDrawer != null) lineDrawer.gameObject.SetActive(false);
        // Reset timers and UI
        levelCompleted = false;
        isLevelActive = false;
        if (fillBar != null) fillBar.SetActive(false);
        // Schedule next mission
        CancelInvoke();
        Invoke(nameof(ShowMission), timeToShowMission);
    }

    private void OnWatchVideoClicked()
    {
        // Simulate video watch – add bonus reward (2x extra = triple total)
        currentBonusReward = currentBaseReward * 2;
        int totalReward = currentBaseReward + currentBonusReward;

        // Update UI
        finishBonusRewardText.text = currentBonusReward.ToString();
        finishTotalRewardText.text = totalReward.ToString();

        // Update PlayerPrefs Coins: we already added base, now add bonus
        int currentCoins = PlayerPrefs.GetInt("Coins", 0);
        currentCoins += currentBonusReward;
        PlayerPrefs.SetInt("Coins", currentCoins);
        PlayerPrefs.Save();

        // Disable button after use
        watchVideoX3.interactable = false;

        // Optional: log video watched event
        Debug.Log("Watch video – bonus reward added!");
    }

    private void OnGarageClicked()
    {
        // Resume time
        Time.timeScale = 1f;
        loadingScreenPanel.SetActive(true);
        // Load garage scene (index 0 as per GameManager)
        SceneManager.LoadScene(0);
    }

    private void OnMissOutClicked()
    {
        // Close panel, clean up, and show new mission popup after delay
        CloseFinishPanelAndReset();
        GameManager.Instance.DestroyandInstantiateCar(GameManager.Instance.startPos);
        CancelInvoke(nameof(ShowMission));
        Invoke(nameof(ShowMission), timeToShowMission);
    }

    private void OnNextMissionClicked()
    {
        // Move to next mission
        int nextIndex = currentMissionIndex + 1;
        if (nextIndex >= parkingMissions.Length)
        {
            // No more missions – go to garage or loop? Here we go to garage.
            OnGarageClicked();
            return;
        }

        // Save next mission index
        currentMissionIndex = nextIndex;
        PlayerPrefs.SetInt($"Current{MissionType.Parking}Mission", currentMissionIndex);
        PlayerPrefs.Save();

        // Close panel and start next mission directly (or show popup)
        CloseFinishPanelAndReset();
        // Option A: start next mission immediately (skip popup)
        StartCoroutine(StartNextMissionAfterCleanup());
    }

    private void OnRestartClicked()
    {
        //loadingFinishPanel.SetActive(false);
        CloseFinishPanelAndReset();
        StartCoroutine(LoadingSequence());
    }

    private void CloseFinishPanelAndReset()
    {
        if (isPaused) ResumeGame();
        isFinishPanelActive = false;
        levelFinishPanel.SetActive(false);
        Time.timeScale = 1f;
        
        // Clean up current mission objects
        if (missionPrefab != null) missionPrefab.SetActive(false);
        if (lineDrawer != null) lineDrawer.gameObject.SetActive(false);
        if (levelData != null)
        {
            if (levelData.winVFX != null) levelData.winVFX.SetActive(false);
            //if (levelData.successCamera != null) levelData.successCamera.SetActive(false);
        }

        // Re-enable RCC UI and camera (if needed for next mission)
        if (rccCanvas != null) rccCanvas.SetActive(true);
        if (rccCamera != null) rccCamera.SetActive(true);
        if (rccCamera != null) rccCamera.GetComponent<RCC_Camera>().cameraMode = RCC_Camera.CameraMode.TPS;
        // Reset flags
        levelCompleted = false;
        isLevelActive = false;
        indicatorCanvas.SetActive(true);
        Destroy(missionPrefab);

    }

    private IEnumerator StartNextMissionAfterCleanup()
    {
        yield return null; // Wait one frame for clean-up
                           // Start the loading sequence for the new mission
        StartCoroutine(LoadingSequence());
    }

    public void ShowMissionPanel(MissionType missionType2)
    {
        CancelInvoke(nameof(ShowMission));
        missionType = missionType2;
        // Stop any pending auto‑accept if the popup is shown again early
        if (autoAcceptCoroutine != null)
        {
            StopCoroutine(autoAcceptCoroutine);
            autoAcceptCoroutine = null;
        }

        if (missionType == MissionType.Parking)
        {
            Debug.LogError($"Current{missionType}Mission " +
                PlayerPrefs.GetInt($"Current{missionType}Mission"));
            currentMissionIndex = PlayerPrefs.GetInt($"Current{missionType}Mission", 0);
            missionTitleText.text = parkingMissions[currentMissionIndex].missionName;
            missionDescriptionText.text = parkingMissions[currentMissionIndex].description;
            missionRewardText.text = $"Reward: {parkingMissions[currentMissionIndex].reward} coins";
            missionPopUp.SetActive(true);

            // NEW: start the auto‑accept countdown
            autoAcceptCoroutine = StartCoroutine(AutoAcceptCountdown());
        }
        else if (missionType == MissionType.JumpTrail)
        {
            Debug.LogError($"Current{missionType}Mission " +
                PlayerPrefs.GetInt($"Current{missionType}Mission"));
            currentMissionIndex = PlayerPrefs.GetInt($"Current{missionType}Mission", 0);
            missionTitleText.text = jumpTrailMissions[currentMissionIndex].missionName;
            missionDescriptionText.text = jumpTrailMissions[currentMissionIndex].description;
            missionRewardText.text = $"Reward: {jumpTrailMissions[currentMissionIndex].reward} coins";
            missionPopUp.SetActive(true);

            // NEW: start the auto‑accept countdown
            autoAcceptCoroutine = StartCoroutine(AutoAcceptCountdown());
        }
        else if (missionType == MissionType.MegaRamp)
        {
            Debug.LogError($"Current{missionType}Mission " +
                PlayerPrefs.GetInt($"Current{missionType}Mission"));
            currentMissionIndex = PlayerPrefs.GetInt($"Current{missionType}Mission", 0);
            missionTitleText.text = megaRampMissions[currentMissionIndex].missionName;
            missionDescriptionText.text = megaRampMissions[currentMissionIndex].description;
            missionRewardText.text = $"Reward: {megaRampMissions[currentMissionIndex].reward} coins";
            missionPopUp.SetActive(true);

            // NEW: start the auto‑accept countdown
            autoAcceptCoroutine = StartCoroutine(AutoAcceptCountdown());
        }
        else if (missionType == MissionType.Race)
        {
            Debug.LogError($"Current{missionType}Mission " +
                PlayerPrefs.GetInt($"Current{missionType}Mission"));
            currentMissionIndex = PlayerPrefs.GetInt($"Current{missionType}Mission", 0);
            missionTitleText.text = raceMissions[currentMissionIndex].missionName;
            missionDescriptionText.text = raceMissions[currentMissionIndex].description;
            missionRewardText.text = $"Reward: {raceMissions[currentMissionIndex].reward} coins";
            missionPopUp.SetActive(true);

            // NEW: start the auto‑accept countdown
            autoAcceptCoroutine = StartCoroutine(AutoAcceptCountdown());
        }
        StartCoroutine(SetTimeScaleSmooth(0.3f, 0f, 0.5f));
    }

    // --- Audio Settings ---
    private void LoadAudioSettings()
    {
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float soundVol = PlayerPrefs.GetFloat("SoundVolume", 0.5f);

        if (musicSlider != null) musicSlider.value = musicVol;
        if (soundSlider != null) soundSlider.value = soundVol;

        // Apply to SoundtrackManager (music) and AudioListener (master sound)
        if (SoundtrackManager.Instance != null)
            SoundtrackManager.Instance.SetMusicVolume(musicVol);
        AudioListener.volume = soundVol;
    }

    private void OnMusicSliderChanged(float value)
    {
        if (SoundtrackManager.Instance != null)
            SoundtrackManager.Instance.SetMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    private void OnSoundSliderChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("SoundVolume", value);
        PlayerPrefs.Save();
    }

    // Ensure you also have the PauseGame, ResumeGame, GoHome, etc.
    // and all other methods that were already present.

    private void OnDestroy()
    {
        // Remove listeners to avoid memory leaks
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        if (soundSlider != null) soundSlider.onValueChanged.RemoveListener(OnSoundSliderChanged);
        if (acceptMission != null) acceptMission.onClick.RemoveListener(OnAcceptMission);
        if (declineMission != null) declineMission.onClick.RemoveListener(OnDeclineMission);
        if (pauseBtn != null) pauseBtn.onClick.RemoveListener(PauseGame);
        if (resumeBtn != null) resumeBtn.onClick.RemoveListener(ResumeGame);
        if (homeBtn != null) homeBtn.onClick.RemoveListener(GoHome);
    }
}

public enum MissionType
{
    Parking,
    JumpTrail,
    MegaRamp,
    Race,
}
