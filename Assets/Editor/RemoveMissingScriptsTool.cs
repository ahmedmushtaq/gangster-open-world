using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class RemoveMissingScriptsTool
{
    // ---------------------------------------------------------------------
    // Menu items
    // ---------------------------------------------------------------------

    [MenuItem("Tools/Remove Missing Scripts/From Selected (Including Children)")]
    private static void RemoveFromSelected()
    {
        if (Selection.gameObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select at least one GameObject.", "OK");
            return;
        }

        if (EditorUtility.DisplayDialog("Remove Missing Scripts",
            "This will remove all missing script components from the selected GameObjects and all their children.\n\n" +
            "Continue?", "Yes", "Cancel"))
        {
            ProcessGameObjects(Selection.gameObjects);
        }
    }

    [MenuItem("Tools/Remove Missing Scripts/From All GameObjects in Scene")]
    private static void RemoveFromAll()
    {
        if (EditorUtility.DisplayDialog("Remove Missing Scripts (Entire Scene)",
            "This will remove all missing script components from EVERY GameObject in the current scene " +
            "(including inactive and children).\n\nContinue?", "Yes", "Cancel"))
        {
            // Get all root GameObjects in the active scene
            GameObject[] rootObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();
            ProcessGameObjects(rootObjects);
        }
    }

    // ---------------------------------------------------------------------
    // Core processing
    // ---------------------------------------------------------------------

    private static void ProcessGameObjects(GameObject[] rootObjects)
    {
        int totalRemoved = 0;
        int totalRoots = rootObjects.Length;

        for (int i = 0; i < totalRoots; i++)
        {
            GameObject root = rootObjects[i];
            string progressMessage = $"Processing {root.name} ({i + 1}/{totalRoots})";
            EditorUtility.DisplayProgressBar("Removing Missing Scripts", progressMessage, (float)i / totalRoots);

            // Get all transforms in the hierarchy (including inactive)
            Transform[] allTransforms = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allTransforms)
            {
                GameObject go = t.gameObject;

                // Record the GameObject before removing anything for Undo
                Undo.RegisterCompleteObjectUndo(go, "Remove Missing Scripts");

                // Remove any missing MonoBehaviours
                int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                if (removed > 0)
                {
                    totalRemoved += removed;
                    EditorUtility.SetDirty(go);
                }
            }
        }

        EditorUtility.ClearProgressBar();

        // Mark the scene as dirty so changes are saved
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log($"✅ Removed {totalRemoved} missing script component(s) from {totalRoots} root object(s).");
        EditorUtility.DisplayDialog("Complete", $"Removed {totalRemoved} missing script components.", "OK");
    }
}