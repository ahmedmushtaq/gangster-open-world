using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("RCC V3/Customization/RCCV3 Paint Manager")]
public class RCCV3_PaintManager : MonoBehaviour
{
    private RCCV3_Paintable[] paintables;
    private Color currentColor = Color.white;
    private List<Color> defaultColors = new List<Color>();

    private void Awake()
    {
        // Find all paintable parts in children (including inactive)
        paintables = GetComponentsInChildren<RCCV3_Paintable>(true);
        // Store default colors for reset
        foreach (var p in paintables)
        {
            if (p.paintMaterial != null)
                defaultColors.Add(p.paintMaterial.GetColor(p.propertyName));
        }
    }

    private void Start()
    {
        // Load saved color for this car (optional)
        string hex = PlayerPrefs.GetString("CarColor_" + gameObject.name, "");
        if (!string.IsNullOrEmpty(hex))
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(hex, out color))
                Paint(color);
        }
    }

    public void Paint(Color newColor, bool save = true)
    {
        currentColor = newColor;
        foreach (var p in paintables)
        {
            if (p != null)
                p.ApplyColor(newColor);
        }
        if (save)
            SaveColor();
    }

    public void Restore()
    {
        for (int i = 0; i < paintables.Length; i++)
        {
            if (paintables[i] != null && i < defaultColors.Count)
                paintables[i].ApplyColor(defaultColors[i]);
        }
    }

    private void SaveColor()
    {
        string hex = "#" + ColorUtility.ToHtmlStringRGB(currentColor);
        PlayerPrefs.SetString("CarColor_" + gameObject.name, hex);
        PlayerPrefs.Save();
    }
}