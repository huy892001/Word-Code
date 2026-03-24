using System.Collections.Generic;
using InfinityGame.Extensions;
using InfinityGame.DesignPattern.Singleton;
using UnityEngine;

namespace InfinityGame.DesignPattern.ObjectPooling
{
    /// <summary>
    /// Manages object pooling for performance optimization.
    /// </summary>
    public class ObjectPool : Singleton<ObjectPool>
    {
        [SerializeField] private Dictionary<ItemPoolType, GameObject> _pool =
            new Dictionary<ItemPoolType, GameObject>();

        private PoolManager _poolManager;

        private void OnValidate()
        {
            _isDontDestroyOnLoad = true;
        }

        protected override void Awake()
        {
            _poolManager = gameObject.GetOrAddComponent<PoolManager>();
            base.Awake();
        }

        private GameObject GetObject(ItemPoolType objectType)
        {
            return objectType == ItemPoolType.None ? null : _pool.GetValueOrDefault(objectType);
        }

        /// <summary>
        /// Spawns an object.
        /// </summary>
        public T Spawn<T>(GameObject prefab, Transform parent = null) where T : Component
        {
            var obj = _poolManager.Spawn(prefab, parent);

            if (obj == null) return null;

            if (typeof(T) == typeof(GameObject))
            {
                return (T)(object)obj;
            }

            return obj.GetComponent<T>();
        }

        /// <summary>
        /// Spawns an object with location.
        /// </summary>
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            return _poolManager.Spawn(prefab, position, rotation, parent);
        }

        /// <summary>
        /// Spawns an array of GameObjects from the pool.
        /// </summary>
        public GameObject[] SpawnArray(GameObject prefab, int amount, Transform parent = null)
        {
            GameObject[] result = new GameObject[amount];
            for (int i = 0; i < amount; i++)
            {
                result[i] = _poolManager.Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
            }

            return result;
        }

        /// <summary>
        /// Spawns an object from the pool with default location.
        /// </summary>
        public T Spawn<T>(ItemPoolType objectType, Transform parent = null)
        {
            var obj = _poolManager.Spawn(GetObject(objectType), parent);

            if (obj == null) return default(T);

            if (typeof(T) == typeof(GameObject))
            {
                return (T)(object)obj;
            }

            return obj.GetComponent<T>();
        }

        /// <summary>
        /// Spawns an object from the pool with designated location.
        /// </summary>
        public T Spawn<T>(ItemPoolType objectType, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var obj = _poolManager.Spawn(GetObject(objectType), position, rotation, parent);

            if (obj == null) return default(T);

            if (typeof(T) == typeof(GameObject))
            {
                return (T)(object)obj;
            }

            return obj.GetComponent<T>();
        }


        /// <summary>
        /// Spawns some objects from the pool.
        /// </summary>
        public void SpawnWithAmount(ItemPoolType objectType, int amount, Transform parent = null)
        {
            for (int i = 0; i < amount; i++)
            {
                _poolManager.Spawn(GetObject(objectType), parent);
            }
        }

        /// <summary>
        /// Spawns some objects from the pool and return array.
        /// </summary>
        public T[] SpawnArray<T>(ItemPoolType objectType, int amount, Transform parent = null)
        {
            T[] result = new T[amount];
            var prefab = GetObject(objectType);

            for (int i = 0; i < amount; i++)
            {
                var obj = _poolManager.Spawn(prefab, parent);

                if (typeof(T) == typeof(GameObject))
                {
                    result[i] = (T)(object)obj;
                }
                else
                {
                    var component = obj.GetComponent<T>();
                    if (component != null)
                    {
                        result[i] = component;
                    }
                    else
                    {
                        Debug.LogError($"[SpawnArray] Component {typeof(T).Name} not found on {obj.name}");
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        public void Despawn(GameObject obj)
        {
            _poolManager.Despawn(obj);
        }

        /// <summary>
        /// Returns all object off parent to the pool.
        /// </summary>
        public void DespawnAll(Transform parent)
        {
            _poolManager.DespawnAll(parent);
        }
    }
}