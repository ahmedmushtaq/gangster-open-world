using UnityEngine;
using UnityEngine.Events;

public class JumpFinish : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag of the player's car root object")]
    public string carTag = "Player";
    public bool activated = false;
    public GameObject finalPos;
    private GameObject car;

    void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;
        if (root.CompareTag(carTag) && activated == false)
        {
            car = root.gameObject;
            OnCarParked();
            activated = true;
        }
    }

    private void OnCarParked()
    {
        Invoke(nameof(LevelComplete), 0.5f);
    }

    public void LevelComplete()
    {
        Debug.LogError("CAR FULLY PARKED!");
        if (MissionManager.Instance != null && GameManager.Instance != null)
        {
            GameManager.Instance.DestroyandInstantiateCar(finalPos);
            MissionManager.Instance.LevelComplete();
            GetComponent<BoxCollider>().enabled = false;
            //enabled = false;
            activated = false;
        }
        else
            Debug.LogError("MissionManager.Instance is null!");
    }
}