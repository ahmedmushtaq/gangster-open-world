using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag of the player's car root object")]
    public string carTag = "Player";
    public bool activated = false;
    private GameObject car;
    public GameObject Next;

    private void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;
        car = root.gameObject;
        if (root.CompareTag("Player"))
        {
            if (Next != null)
            {
                Next.SetActive(true);
            }
            else
            {
                OnCarParked();
                activated = true;
            }
            gameObject.SetActive(false);
        }
    }

    private void OnCarParked()
    {
        MakeCarBreak();
        Invoke(nameof(LevelComplete), 1f);
    }

    public void LevelComplete()
    {
        Debug.LogError("CAR FULLY PARKED!");
        if (MissionManager.Instance != null)
        {
            // Get the car controller
            MissionManager.Instance.LevelComplete();
            //GetComponent<BoxCollider>().enabled = false;
            //enabled = false;
            activated = false;
        }
        else
            Debug.LogError("MissionManager.Instance is null!");
    }

    public void MakeCarBreak()
    {
        RCC_CarControllerV3 carController = car.GetComponent<RCC_CarControllerV3>();
        if (carController != null)
        {
            // ---- MAKE BRAKES EXTREMELY HARD ----
            // Store original values so they can be restored if needed
            float originalBrakeTorque = carController.brakeTorque;
            bool originalABS = carController.ABS;

            // 1. Boost brake torque to a huge value (60,000 N·m)
            carController.brakeTorque = 60000f;

            // 2. Optionally turn off ABS so wheels can lock (even more stopping power)
            carController.ABS = false;

            // Disable player input
            carController.externalController = true;

            // Apply full brakes + right turn
            carController.brakeInput = 1f;
            carController.steerInput = 0.5f;   // right turn
            carController.gasInput = 0f;
            carController.handbrakeInput = 0f;

            // (Optional) Also add a direct deceleration impulse for extra bite
            carController.rigid.AddForce(-carController.transform.forward * 8000f, ForceMode.Impulse);

            // (Optional) Restore original values after a few seconds if you want
            Invoke(nameof(RestoreCarSettings), 2f);
        }
    }

    private void RestoreCarSettings()
    {
        RCC_CarControllerV3 carController = car.GetComponent<RCC_CarControllerV3>();
        if (carController != null)
        {
            carController.brakeTorque = 2000f;   // or whatever your default is
            carController.ABS = true;
        }
    }
}
