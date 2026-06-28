using UnityEngine;

/// <summary>
/// Detects when the vehicle touches the ground (tagged "floor") using trigger colliders.
/// </summary>
public class carcollisiondetector : MonoBehaviour
{
    private static carcollisiondetector _instance;
    public static carcollisiondetector Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<carcollisiondetector>();
            return _instance;
        }
        private set => _instance = value;
    }

    [SerializeField] private bool isGrounded = true;

    /// <summary>
    /// Whether the vehicle is currently touching the ground.
    /// </summary>
    public bool IsGrounded
    {
        get => isGrounded;
        private set => isGrounded = value;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"Multiple {nameof(carcollisiondetector)} instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("floor"))
        {
            IsGrounded = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("floor"))
        {
            IsGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("floor"))
        {
            IsGrounded = false;
        }
    }
}
