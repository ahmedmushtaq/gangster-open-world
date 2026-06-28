using UnityEngine;

public class MissionPopUp : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag of the player's car root object")]
    public string carTag = "Player";
    public MissionType missionType;

    void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;
        if (root.CompareTag(carTag))
        {
            OnCarEntered();
        }
    }

    private void OnCarEntered()
    {
        Invoke(nameof(ShowPopUp), 0f);
    }

    public void ShowPopUp()
    {
        Debug.LogError($"Showing pop-up for mission type: {missionType}");
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.ShowMissionPanel(missionType);
        }
        else
            Debug.LogError("MissionManager.Instance is null!");
    }
}
