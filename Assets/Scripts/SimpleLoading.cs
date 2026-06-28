using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MultiTechStudio.SnakeEscape
{
    public class SimpleLoading : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image valueBar;
        [SerializeField] private Image[] loadingBG;

        [Header("Loading Settings")]
        [SerializeField] private float minLoadingTime = 1.5f;

        [Header("Background Cycling")]
        [SerializeField] private float bgDisplayDuration = 12f;     // How long each bg stays fully visible
        [SerializeField] private float bgFadeDuration = 2f;        // Fade‑out duration for old bg
        [SerializeField] private float newImageFadeInDuration = 0.5f; // NEW: fade‑in time for the new bg

        private Coroutine bgCoroutine;
        private int currentBGIndex = -1;

        private void Awake()
        {
            AutoAssignReferences();
        }

        private void AutoAssignReferences()
        {
            if (valueBar == null)
            {
                Transform valueBarTransform = transform.Find("Content/ProgressBar/valueBar");
                if (valueBarTransform == null)
                    valueBarTransform = transform.Find("ProgressBar/valueBar");
                if (valueBarTransform == null)
                    valueBarTransform = transform.Find("valueBar");
                if (valueBarTransform != null)
                    valueBar = valueBarTransform.GetComponent<Image>();
            }

            if (valueBar != null)
            {
                valueBar.type = Image.Type.Filled;
                valueBar.fillAmount = 0f;
            }
        }

        private void Start()
        {
            Show();
        }

        private void OnEnable()
        {
            Show();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(LoadSceneCoroutine());

            if (loadingBG != null && loadingBG.Length > 1)
            {
                if (bgCoroutine != null)
                    StopCoroutine(bgCoroutine);
                bgCoroutine = StartCoroutine(BackgroundCycleCoroutine());
            }
        }

        private IEnumerator LoadSceneCoroutine()
        {
            if (valueBar != null)
                valueBar.fillAmount = 0f;

            float loadingStartTime = Time.time;

            

            //while (Time.deltaTime < 0.9f)
            //{
            //    float progress = Time.deltaTime;
            //    if (valueBar != null)
            //        valueBar.fillAmount = progress;
            //    yield return null;
            //}

            float elapsedTime = Time.time - loadingStartTime;
            if (elapsedTime < minLoadingTime)
            {
                float remainingTime = minLoadingTime - elapsedTime;
                float startProgress = valueBar != null ? valueBar.fillAmount : 0f;
                bool adShown = false;
                while (elapsedTime < minLoadingTime)
                {
                    elapsedTime = Time.time - loadingStartTime;
                    float timeProgress = elapsedTime / minLoadingTime;
                    float progress = Mathf.Lerp(startProgress, 1f, timeProgress);

                    if (valueBar != null)
                        valueBar.fillAmount = progress;

                    if (elapsedTime > 6 && !adShown)
                    {
                        //AdsManager_AdmobMediation.Instance?.ShowAppOpenAd();
                        adShown = true;
                    }

                    yield return null;
                }
            }

            if (valueBar != null)
                valueBar.fillAmount = 1f;

            yield return null;
        }

        private IEnumerator BackgroundCycleCoroutine()
        {
            // Initially hide all backgrounds
            SetAllBackgroundsAlpha(0f);

            // Pick and show the first background
            currentBGIndex = Random.Range(0, loadingBG.Length);
            SetBackgroundAlpha(currentBGIndex, 1f);

            while (true)
            {
                // Wait while the current bg is fully visible
                yield return new WaitForSeconds(bgDisplayDuration);

                // Choose a new background (different from current)
                int newIndex = currentBGIndex;
                while (newIndex == currentBGIndex)
                    newIndex = Random.Range(0, loadingBG.Length);

                // --- Fade in the new image first ---
                // Set new image alpha to 0 and ensure it's active
                SetBackgroundAlpha(newIndex, 0f);

                float elapsed = 0f;
                while (elapsed < newImageFadeInDuration)
                {
                    elapsed += Time.deltaTime;
                    float alpha = Mathf.Lerp(0f, 1f, elapsed / newImageFadeInDuration);
                    SetBackgroundAlpha(newIndex, alpha);
                    yield return null;
                }
                SetBackgroundAlpha(newIndex, 1f); // ensure full opacity

                // --- Now fade out the old image ---
                elapsed = 0f;
                while (elapsed < bgFadeDuration)
                {
                    elapsed += Time.deltaTime;
                    float alpha = Mathf.Lerp(1f, 0f, elapsed / bgFadeDuration);
                    SetBackgroundAlpha(currentBGIndex, alpha);
                    yield return null;
                }
                SetBackgroundAlpha(currentBGIndex, 0f); // ensure fully transparent

                // Update current index
                currentBGIndex = newIndex;
            }
        }

        private void SetAllBackgroundsAlpha(float alpha)
        {
            foreach (Image img in loadingBG)
                if (img != null)
                    SetImageAlpha(img, alpha);
        }

        private void SetBackgroundAlpha(int index, float alpha)
        {
            if (index < 0 || index >= loadingBG.Length || loadingBG[index] == null)
                return;
            SetImageAlpha(loadingBG[index], alpha);
        }

        private void SetImageAlpha(Image img, float alpha)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }

        public void Init()
        {
            if (valueBar != null)
                valueBar.fillAmount = 0f;
        }

        public Image ValueBar => valueBar;

        private void OnDisable()
        {
            if (bgCoroutine != null)
            {
                StopCoroutine(bgCoroutine);
                bgCoroutine = null;
            }
        }
    }
}