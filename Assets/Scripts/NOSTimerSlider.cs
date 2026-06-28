using UnityEngine;
using UnityEngine.UI;

public class NOSTimerSlider : MonoBehaviour
{
    Image slider;
    RCC_CarControllerV3 car;

    void Start()
    {
        slider = GetComponent<Image>();
    }

    void Update()
    {
        if (RCC_SceneManager.Instance.activePlayerVehicle != null)
        {
            car = RCC_SceneManager.Instance.activePlayerVehicle;
            if (car.useNOS && car.nosActive)
                slider.fillAmount = car.nosTimer / car.nosDurationPerCharge;
            else
                slider.fillAmount = 1f;
        }
    }
}