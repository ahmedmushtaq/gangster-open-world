using UnityEngine;

public class LevelFail : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Transform root = collision.gameObject.transform.root;
        if (root.gameObject.CompareTag("Player"))
        {
            MissionManager.Instance.LevelFailed();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform root = other.gameObject.transform.root;
        if (root.gameObject.CompareTag("Player"))
        {
            MissionManager.Instance.LevelFailed();
        }
    }
}
