using UnityEngine;
using UnityEngine.UI;

public class NOSChargeDisplay : MonoBehaviour
{
    Text text;
    RCC_CarControllerV3 car;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        if (RCC_SceneManager.Instance.activePlayerVehicle != null)
        {
            car = RCC_SceneManager.Instance.activePlayerVehicle;
            if (car.useNOS)
                text.text = car.nosCharges.ToString();
            else
                text.text = "0";
        }
    }
}