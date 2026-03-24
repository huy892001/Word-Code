using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InfinityGame.DesignPattern.Singleton
{
    /// <summary>
    /// Base Singleton class for MonoBehaviour.
    /// Usage: public class GameManager : Singleton<GameManager> { ... }
    /// </summary>
    public abstract class Singleton<T> : SerializedMonoBehaviour where T : Singleton<T>
    {
        private static T _instance;

        [Header("Singleton Settings")]
        [Tooltip("If true, this object will not be destroyed when loading a new scene.")]
        [SerializeField]
        protected bool _isDontDestroyOnLoad = false;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        Debug.LogWarning($"[Singleton] Instance of {typeof(T).Name} not found in the scene.");
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
                if (_isDontDestroyOnLoad)
                {
                    if (transform.parent != null) transform.SetParent(null);
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T).Name} detected. Destroying new one.");
                Destroy(gameObject);
            }
        }
    }
}