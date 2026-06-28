using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DailyReward : MonoBehaviour
{
    public static DailyReward instance;
    public int secondsToWait = 10;
    public bool canGetReward = true;
    [Space]
    public GameObject[] highlighted;
    public GameObject[] claimed;
    public Button[] buttons;
    public Button rewardButton;
    public Text displayText;
    public GameObject readytoclaim;
    private ulong lastRewarded;
    private ulong difference;
    private ulong milisec;
    private float milisecToWait;
    private float secondsLeft;

    public GameObject DailyRewardPanel;
    public bool DailyRewardShown;
    public int carUnlockIndex;
    public Image nextRewardImg; 
    public Sprite[] rewardIcons;         // assign 7 sprites (index 0..6) in Inspector
    //public Sprite allClaimedSprite;      // optional: sprite shown when all rewards claimed

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("FirstDay") == 0)
        {
            PlayerPrefs.SetString("LastRewarded", "0");
            PlayerPrefs.SetInt("FirstDay", 1);
        }
        else
        {
            MyFun();
        }

        string lastSavedTime = PlayerPrefs.GetString("LastRewarded");
        lastRewarded = ulong.Parse(lastSavedTime);

        if (!CanGetReward())
        {
            canGetReward = false;
            if (PlayerPrefs.GetInt("Dailyreward") == 0)
            {
                DailyRewardPanel.SetActive(false);
            }
            else
            {
                //DailyRewardPanel.SetActive(true);
                //DailyRewardPanel.GetComponent<AnimationHandler>().PlayAnimation(AnimationHandler.AnimationType.FadeIn);
                PlayerPrefs.SetInt("Dailyreward", 0);
            }
        }

        else if (CanGetReward())
        {
            canGetReward = true;
            //DailyRewardPanel.SetActive(true);
            //DailyRewardPanel.GetComponent<AnimationHandler>().PlayAnimation(AnimationHandler.AnimationType.FadeIn);

        }
        //UpdateNextRewardImage();
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt("Day") >= 6)
        {
            displayText.text = "All Claimed!";
            UpdateHighlight();

            // Disable the reward image when all claimed
            if (nextRewardImg != null)
                nextRewardImg.enabled = false;

            return;
        }

        canGetReward = CanGetReward();


        if (canGetReward)
        {
            TryShowDailyRewardPanelOnce();
            MyFun();
            rewardButton.interactable = true;
            buttons[PlayerPrefs.GetInt("Day")].interactable = true;
            readytoclaim.SetActive(true);
            displayText.text = "Ready To Claim";
        }


        else if (!canGetReward)
        {
            rewardButton.interactable = false;
            readytoclaim.SetActive(false);

            string timerText = "";

            timerText += ((int)secondsLeft / 3600).ToString("00") + "h ";
            secondsLeft -= ((int)secondsLeft / 3600) * 3600;

            timerText += ((int)secondsLeft / 60).ToString("00") + "m ";

            timerText += (secondsLeft % 60).ToString("00") + "s";

            displayText.text = timerText;
        }
        UpdateHighlight();
    }

    void MyFun()
    {
        for (int i = 0; i < claimed.Length; i++)
        {
            claimed[i].SetActive(false);
        }

        for (int i = 0; i < PlayerPrefs.GetInt("Day"); i++)
        {

            if (i <= 6)
            {
                claimed[i]?.SetActive(true);
            }
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
    }

    public void GetReward()
    {
        // Clear highlight immediately after claiming
        for (int i = 0; i < highlighted.Length; i++)
        {
            highlighted[i].SetActive(false);
        }

        MyFun();

        Invoke("offreward", 1f);
        rewardButton.interactable = false;
        readytoclaim.SetActive(false);
        claimed[PlayerPrefs.GetInt("Day")].SetActive(true);
        buttons[PlayerPrefs.GetInt("Day")].interactable = false;

        if (canGetReward)
        {
            if (PlayerPrefs.GetInt("Day") == 0)
            {
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 100);
            }

            if (PlayerPrefs.GetInt("Day") == 1)
            {
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 300);
            }

            if (PlayerPrefs.GetInt("Day") == 2)
            {
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 500);
            }

            if (PlayerPrefs.GetInt("Day") == 3)
            {
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 500);
            }

            if (PlayerPrefs.GetInt("Day") == 4)
            {
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 1000);
            }

            if (PlayerPrefs.GetInt("Day") == 5)
            {
                PlayerPrefs.SetInt($"IsCarUnlocked_{carUnlockIndex}", 1);
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 1000);
            }

            if (PlayerPrefs.GetInt("Day") == 6)
            {
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 2000);
            }

            lastRewarded = (ulong)DateTime.Now.Ticks;
            PlayerPrefs.SetString("LastRewarded", lastRewarded.ToString());

            canGetReward = false;
            if (PlayerPrefs.GetInt("Day") < 6)
            {
                PlayerPrefs.SetInt("Day", PlayerPrefs.GetInt("Day") + 1);
                //UpdateNextRewardImage();
            }
            //PlayerPrefs.SetInt("Day", PlayerPrefs.GetInt("Day") + 1);
            //if (PlayerPrefs.GetInt("Day") > 6)
            //{
            //    PlayerPrefs.SetInt("Day", 0);
            //}

        }

        PlayerPrefs.SetInt("DailyRewardShown", 0);
    }
    private void offreward()
    {
        DailyRewardPanel.SetActive(false);
    }
    private bool CanGetReward()
    {
        //Getting the difference between the current time and the <LastRewarded> time.
        difference = ((ulong)DateTime.Now.Ticks - lastRewarded);
        milisec = difference / TimeSpan.TicksPerMillisecond;

        //Since the input wait time is in seconds, we have to multiply by 1000 to get it in miliseconds.
        milisecToWait = secondsToWait * 1000;
        secondsLeft = (float)(milisecToWait - milisec) / 1000f;
        //Debug.Log(difference + " & " + milisec + " & " + milisecToWait + " & " + secondsLeft);
        //Check if we can get the reward.
        if (secondsLeft < 0)
            return true;

        else return false;
    }

    void TryShowDailyRewardPanelOnce()
    {
        if (canGetReward && PlayerPrefs.GetInt("DailyRewardShown", 0) == 0)
        {
            DailyRewardPanel.SetActive(true);
            PlayerPrefs.SetInt("DailyRewardShown", 1);
        }
    }

    void UpdateHighlight()
    {
        // Turn all highlights OFF first
        for (int i = 0; i < highlighted.Length; i++)
        {
            highlighted[i].SetActive(false);
        }

        int day = PlayerPrefs.GetInt("Day");

        // Safety checks
        if (day >= highlighted.Length)
            return;

        // Highlight ONLY if reward is ready
        if (canGetReward)
        {
            highlighted[day].SetActive(true);
        }
    }

    private void UpdateNextRewardImage()
    {
        int day = PlayerPrefs.GetInt("Day");

        if (day < rewardIcons.Length)   // day 0..6 → reward still available
        {
            nextRewardImg.sprite = rewardIcons[day];
            nextRewardImg.enabled = true;
        }
        else                            // day >= 7 → all claimed
        {
            //if (allClaimedSprite != null)
            //{
            //    nextRewardImg.sprite = allClaimedSprite;
            //    nextRewardImg.enabled = true;
            //}
            //else
            //{
            //}
                nextRewardImg.enabled = false; // hide image when done
        }
    }


}