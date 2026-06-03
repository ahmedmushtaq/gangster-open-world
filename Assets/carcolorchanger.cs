using UnityEngine;

/// <summary>
/// Cycles through car body colors when the vehicle passes through a "portal" trigger.
/// </summary>
public class carcolorchanger : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material[] carMeshes;

    [Header("Body Renderers")]
    [SerializeField] private GameObject carBody;
    [SerializeField] private GameObject carBody2;
    [SerializeField] private GameObject carBody3;

    [SerializeField] private int selectedMeshIndex = 0;

    /// <summary>
    /// Current selected material index.
    /// </summary>
    public int SelectedMeshIndex
    {
        get => selectedMeshIndex;
        set => selectedMeshIndex = value;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("portal"))
            return;

        if (carMeshes == null || carMeshes.Length == 0)
        {
            Debug.LogWarning("No car materials assigned to cycle through.");
            return;
        }

        ApplyMaterialToRenderer(carBody);
        ApplyMaterialToRenderer(carBody2);
        ApplyMaterialToRenderer(carBody3);

        selectedMeshIndex = (selectedMeshIndex + 1) % carMeshes.Length;
    }

    private void ApplyMaterialToRenderer(GameObject bodyPart)
    {
        if (bodyPart == null)
            return;

        if (bodyPart.TryGetComponent(out Renderer renderer))
        {
            renderer.material = carMeshes[selectedMeshIndex];
        }
    }
}
