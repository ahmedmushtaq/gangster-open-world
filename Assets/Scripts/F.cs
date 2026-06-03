using UnityEngine;

namespace RVP
{
    /// <summary>
    /// Static utility class with extension methods and helper functions.
    /// </summary>
    public static class F
    {
        /// <summary>
        /// Returns the number with the greatest absolute value from the provided values.
        /// </summary>
        public static float MaxAbs(params float[] nums)
        {
            if (nums == null || nums.Length == 0)
                return 0f;

            float result = 0f;
            foreach (float num in nums)
            {
                float abs = Mathf.Abs(num);
                if (abs > Mathf.Abs(result))
                {
                    result = num;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the topmost parent of a Transform that has a component of type <typeparamref name="T"/>.
        /// </summary>
        public static T GetTopmostParentComponent<T>(this Transform transform) where T : Component
        {
            T result = null;
            Transform current = transform;

            while (current.parent != null)
            {
                if (current.parent.TryGetComponent(out T component))
                {
                    result = component;
                }

                current = current.parent;
            }

            return result;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Returns whether the given object is part of a prefab asset.
        /// Intended for use with selected objects in the inspector.
        /// </summary>
        public static bool IsPrefab(Object componentOrGameObject)
        {
            if (componentOrGameObject == null)
                return false;

            var type = UnityEditor.PrefabUtility.GetPrefabAssetType(componentOrGameObject);
            return type != UnityEditor.PrefabAssetType.NotAPrefab
                && type != UnityEditor.PrefabAssetType.MissingAsset;
        }
#endif
    }
}
