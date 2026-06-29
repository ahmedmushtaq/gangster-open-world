using UnityEngine;

[AddComponentMenu("RCC V3/Customization/RCCV3 Paintable")]
public class RCCV3_Paintable : MonoBehaviour
{
    [Tooltip("The material to be painted.")]
    public Material paintMaterial;

    [Tooltip("Shader property name (e.g., '_Color' for Standard, '_BaseColor' for URP).")]
    public string propertyName = "_Color";

    // Called by the manager to update this part
    public void ApplyColor(Color color)
    {
        if (paintMaterial == null) return;

        // If the material is shared, we need to instantiate it to avoid affecting others
        Material instance = Instantiate(paintMaterial);
        instance.SetColor(propertyName, color);

        // Find the renderer that uses this material and replace it
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] mats = renderer.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] == paintMaterial)
                {
                    mats[i] = instance;
                    renderer.materials = mats;
                    break;
                }
            }
        }
    }
}