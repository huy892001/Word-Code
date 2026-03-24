using UnityEngine;

namespace InfinityGame.Extensions
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Gets the component of type T. If it doesn't exist, adds it.
        /// Usage: var rb = gameObject.GetOrAddComponent<Rigidbody>();
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            // TryGetComponent is faster than GetComponent (Unity 2019.2+)
            if (gameObject.TryGetComponent<T>(out var component))
            {
                return component;
            }

            return gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Helper to call GetOrAddComponent on a Component (Transform, MonoBehavior, etc.)
        /// </summary>
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetOrAddComponent<T>();
        }
    }
}